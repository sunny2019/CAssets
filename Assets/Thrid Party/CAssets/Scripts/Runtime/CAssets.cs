using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace CAssets
{
    public class CAssets : MonoSingleton<CAssets>
    {
        public bool m_Init = false;
        public Dictionary<string, AssetBundle> dic_loadedAssetBundle = new Dictionary<string, AssetBundle>();
        private IResourceManager m_ResourceManager;
        private AssetBundleManifest _manifest;
        private Dictionary<string, string> _config;
        
        private void Awake()
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            CAssetsConfig CAssetsConfig = Resources.Load<CAssetsConfig>("CAssetsConfig");
            _config = CAssetsConfig.ABAssetsConfig;
#if UNITY_EDITOR
            if (CAssetsConfig.SimMode == true)
            {
                m_ResourceManager = new EditorResourcesManager();
                m_Init = true;
            }
            else
            {
                m_ResourceManager = new ResourcesManager();
                _config = Resources.Load<CAssetsConfig>("CAssetsConfig").ABAssetsConfig;
                StartCoroutine(GetManifest());
            }
#else
        m_ResourceManager = new ResourcesManager();
        _config = Resources.Load<CAssetsConfig>("CAssetsConfig").ABAssetsConfig;
        StartCoroutine(GetManifest());
#endif
        }


        private IEnumerator GetManifest()
        {
            UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(CAssetsSetting.BuildABPath + "CAssets");
            yield return webRequest.SendWebRequest();
            if (webRequest.isHttpError)
            {
                Debug.Log("获取Manifest失败，尝试重新获取：" + webRequest.error);
                StartCoroutine(GetManifest());
            }


            while (!webRequest.isDone)
            {
                yield return null;
            }

            if (!webRequest.isHttpError && webRequest.isDone)
            {
                m_Init = true;
                AssetBundle loaded = DownloadHandlerAssetBundle.GetContent(webRequest);
                _manifest = loaded.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
        }

        public AssetBundle GetAssetBundleInCache(string buildName)
        {
            if (dic_loadedAssetBundle.ContainsKey(buildName))
            {
                return dic_loadedAssetBundle[buildName];
            }
            else
            {
                return null;
            }
        }

        public void AddAssetBundleInCache(string buildName, AssetBundle bundle)
        {
            if (dic_loadedAssetBundle.ContainsKey(buildName))
                dic_loadedAssetBundle[buildName] = bundle;
            else
            {
                dic_loadedAssetBundle.Add(buildName, bundle);
            }
        }

        public string GetABName(string assetPath)
        {
            if (_config.ContainsKey(assetPath))
            {
                return _config[assetPath];
            }
            else
            {
                Debug.LogError("CAssetsConfig不包含：" + assetPath);
                return null;
            }
        }

        public string[] GetAllDependenciesABNamesByABName(string abName)
        {
            return _manifest.GetAllDependencies(abName);
        }


        public void Load(string assetName, Action<string, object> successLoadAssetCallback, Action<float> loadingSceneCallBack, Action<string, string> failureLoadAssetCallback)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("Asset name is invalid.");
                return;
            }


            if (!assetName.StartsWith("Assets/", StringComparison.Ordinal))
            {
                Debug.LogError($"Asset name '{assetName}' is invalid.");
                return;
            }

            m_ResourceManager.Load(assetName, successLoadAssetCallback, loadingSceneCallBack, failureLoadAssetCallback);
        }
        
        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="abNames"></param>
        /// <param name="processCallBak"></param>
        /// <param name="finshedCallBack"></param>
        public void StartPreloadAllAssetAB( Action<float> processCallBak, Action finshedCallBack)
        {
            if (m_ResourceManager != null && _config != null) 
            {
                m_ResourceManager.StartPreLoadAllAssetAB(_config.Values.ToList(), processCallBak, finshedCallBack);
            }
        }
    }
}