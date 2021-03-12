using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace CAssets
{
    public class CAssetsDownLoader : SerializedMonoBehaviour
    {
        private Queue<CAssetsDownLoadTask> lodingAbTasks = new Queue<CAssetsDownLoadTask>();

        private Coroutine currentLoad;

        private bool m_SimMode = false;


        private void Start()
        {
            CAssetsConfig CAssetsConfig = Resources.Load<CAssetsConfig>("CAssetsConfig");
#if UNITY_EDITOR
            m_SimMode = CAssetsConfig.SimMode;
#endif

            StartCoroutine(UnLoadUnusedAsset(CAssetsConfig.UnloadFrequency));
        }

        private IEnumerator UnLoadUnusedAsset(float UnloadFrequency)
        {
            while (true)
            {
                yield return new WaitForSeconds(UnloadFrequency);
                if (currentLoad == null)
                {
                    AssetBundle.UnloadAllAssetBundles(false);
                    Resources.UnloadUnusedAssets();
                }
            }
        }

        private void Update()
        {
            if (CAssets.Instance.m_Init && currentLoad == null && lodingAbTasks.Count != 0)
            {
#if UNITY_EDITOR
                if (m_SimMode == true)
                {
                    currentLoad = StartCoroutine(LoadAssetAB_Editor(lodingAbTasks.Dequeue()));
                }
                else
                {
                    currentLoad = StartCoroutine(LoadAssetAB(lodingAbTasks.Dequeue()));
                }
#else
            currentLoad = StartCoroutine(LoadAssetAB(lodingAbTasks.Dequeue()));
#endif
            }
        }

#if UNITY_EDITOR
        public IEnumerator LoadAssetAB_Editor(CAssetsDownLoadTask task)
        {
            //依赖和主资源AB包全部加载成功
            if (task.assetPath.EndsWith(".unity") == true)
            {
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(task.assetPath);

                yield return new WaitUntil(() =>
                {
                    task.loadingSceneCallBack?.Invoke(asyncOperation.progress);
                    if (asyncOperation.isDone)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                task.successLoadSceneCallback?.Invoke(task.assetPath, SceneManager.GetActiveScene());
            }
            else
            {
                task.successLoadSceneCallback.Invoke(task.assetPath, AssetDatabase.LoadAssetAtPath(task.assetPath, typeof(object)));
            }

            currentLoad = null;
        }
#endif

        public void AddLoadAB(CAssetsDownLoadTask task)
        {
            lodingAbTasks.Enqueue(task);
        }

        public IEnumerator LoadAssetAB(CAssetsDownLoadTask task)
        {
            bool isLoading = true;
            UnityWebRequest webRequest;
            AssetBundle mainAB = null;
            string errorLog = null;
            string[] assetDepenABNames = CAssets.Instance.GetAllDependenciesABNamesByABName(task.assetABName);
            //加载依赖
            for (int i = 0; i < assetDepenABNames.Length; i++)
            {
                if (CAssets.Instance.GetAssetBundleInCache(assetDepenABNames[i]) == null)
                {
                    webRequest = UnityWebRequestAssetBundle.GetAssetBundle(CAssetsSetting.BuildABPath + assetDepenABNames[i]);
                    yield return webRequest.SendWebRequest();
                    if (webRequest.isHttpError)
                    {
                        errorLog = "依赖资源：" + assetDepenABNames[i] + "\t下载失败：" + webRequest.error;
                        Debug.LogError(errorLog);
                        isLoading = false;
                        break;
                    }

                    while (!webRequest.isDone)
                    {
                        yield return null;
                    }

                    if (!webRequest.isHttpError && webRequest.isDone)
                    {
                        AssetBundle loaded = DownloadHandlerAssetBundle.GetContent(webRequest);
                        CAssets.Instance.AddAssetBundleInCache(assetDepenABNames[i], loaded);
                        task.loadingSceneCallBack?.Invoke(i + 1 / assetDepenABNames.Length + 1 + 1);
                    }
                }
                else
                {
                    task.loadingSceneCallBack?.Invoke(i + 1 / assetDepenABNames.Length + 1 + 1);
                    continue;
                }
            }

            //加载主资源
            if (isLoading == true)
            {
                if (CAssets.Instance.GetAssetBundleInCache(task.assetABName) == null)
                {
                    webRequest = UnityWebRequestAssetBundle.GetAssetBundle(CAssetsSetting.BuildABPath + task.assetABName);
                    yield return webRequest.SendWebRequest();
                    if (webRequest.isHttpError)
                    {
                        errorLog = "主资源：" + task.assetABName + "\t下载失败：" + webRequest.error;
                        Debug.LogError(errorLog);
                        isLoading = false;
                    }

                    if (!webRequest.isDone)
                    {
                        yield return null;
                    }

                    if (!webRequest.isHttpError && webRequest.isDone)
                    {
                        mainAB = DownloadHandlerAssetBundle.GetContent(webRequest);
                        CAssets.Instance.AddAssetBundleInCache(task.assetABName, mainAB);
                        task.loadingSceneCallBack?.Invoke(0.9f);
                    }
                }
                else
                {
                    mainAB = CAssets.Instance.GetAssetBundleInCache(task.assetABName);
                    task.loadingSceneCallBack?.Invoke(0.9f);
                }
            }

            if (isLoading)
            {
                //依赖和主资源AB包全部加载成功
                if (task.assetPath.EndsWith(".unity") == true)
                {
                    AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(task.assetPath);

                    yield return new WaitUntil(() =>
                    {
                        task.loadingSceneCallBack?.Invoke((0.9f + asyncOperation.progress / 10f) / 1f);
                        if (asyncOperation.isDone)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });
                    task.successLoadSceneCallback?.Invoke(task.assetPath, SceneManager.GetActiveScene());
                }
                else
                {
                    task.successLoadSceneCallback.Invoke(task.assetPath, mainAB.LoadAsset(task.assetPath));
                }
            }
            else
            {
                //未加载成功
                task.failureLoadSceneCallback?.Invoke(task.assetPath, "依赖及主资源加载失败：" + errorLog);
            }

            currentLoad = null;
        }


        /// <summary>
        /// 开始进行预加载所有资源
        /// </summary>
        /// <param name="abNames"></param>
        /// <param name="processCallBack"></param>
        /// <param name="finshedCallBack"></param>
        public void StartPreLoadAllAssetAB(List<string> abNames, Action<float> processCallBack, Action finshedCallBack)
        {
            StartCoroutine(PreLoadAllAssetAB(abNames,processCallBack,finshedCallBack));
        }
        /// <summary>
        /// 直接预加载所有资源
        /// </summary>
        /// <param name="abNames"></param>
        /// <param name="processCallBack"></param>
        /// <param name="finshedCallBack"></param>
        /// <returns></returns>
        public IEnumerator PreLoadAllAssetAB(List<string> abNames,Action<float> processCallBack,Action finshedCallBack)
        {
            UnityWebRequest webRequest;
            AssetBundle ab = null;

            for (int i = 0; i < abNames.Count; i++)
            {
                if (CAssets.Instance.GetAssetBundleInCache(abNames[i]) == null)
                {
                    ab = null;
                    webRequest = UnityWebRequestAssetBundle.GetAssetBundle(CAssetsSetting.BuildABPath + abNames[i]);
                    yield return webRequest.SendWebRequest();
                    if (webRequest.isHttpError)
                    {
                        Debug.Log($"{abNames[i]}下载失败，正在尝试重新下载");
                        i--;
                        continue;
                    }

                    if (!webRequest.isDone)
                    {
                        yield return null;
                    }

                    if (!webRequest.isHttpError && webRequest.isDone)
                    {

                        ab = DownloadHandlerAssetBundle.GetContent(webRequest);
                        CAssets.Instance.AddAssetBundleInCache(abNames[i], ab);
                        processCallBack?.Invoke((float) (i + 1) / (float) abNames.Count);
                        AssetBundle.UnloadAllAssetBundles(false);
                    }
                }
            }
            finshedCallBack?.Invoke();
           
        }
    }


    public class CAssetsDownLoadTask
    {
        /// <summary>
        /// 资源名或场景名
        /// </summary>
        public string assetPath;

        /// <summary>
        /// 资源所在AB包名称
        /// </summary>
        public string assetABName;

        /// <summary>
        /// 加载成功回调
        /// </summary>
        public Action<string, object> successLoadSceneCallback;

        /// <summary>
        /// 进度回调
        /// </summary>
        public Action<float> loadingSceneCallBack;

        /// <summary>
        /// 失败回调
        /// </summary>
        public Action<string, string> failureLoadSceneCallback;
    }
}