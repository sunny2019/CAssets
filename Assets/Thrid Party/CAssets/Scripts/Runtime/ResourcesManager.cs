using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CAssets
{
    public class ResourcesManager : IResourceManager
    {
        private CAssetsDownLoader _downLoader;

        public void StartPreLoadAllAssetAB(List<string> abNames, Action<float> processCallBack, Action finshedCallBack)
        {
            if (_downLoader == null)
                _downLoader = CAssets.Instance.gameObject.AddComponent<CAssetsDownLoader>();
            _downLoader.StartPreLoadAllAssetAB(abNames,processCallBack,finshedCallBack);
        }

        public void Load(string name, Action<string, object> successCallbacks, Action<float> loadingCallBack, Action<string, string> failureCallback)
        {
            if (_downLoader == null)
                _downLoader = CAssets.Instance.gameObject.AddComponent<CAssetsDownLoader>();

            CAssetsDownLoadTask task = new CAssetsDownLoadTask();
            task.assetPath = name;
            task.assetABName = CAssets.Instance.GetABName(name);
            task.successLoadSceneCallback = successCallbacks;
            task.loadingSceneCallBack = loadingCallBack;
            task.failureLoadSceneCallback = failureCallback;
            _downLoader.AddLoadAB(task);
        }
    }
}