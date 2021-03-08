#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CAssets
{
    public class EditorResourcesManager : IResourceManager
    {
        private CAssetsDownLoader _downLoader;

        public void StartPreLoadAllAssetAB(List<string> abNames, Action<float> processCallBack, Action finshedCallBack)
        {
            processCallBack?.Invoke(1);
            finshedCallBack?.Invoke();
        }

        public void Load(string name, Action<string, object> successCallbacks, Action<float> loadingCallBack, Action<string, string> failureCallback)
        {
            if (_downLoader == null)
                _downLoader = CAssets.Instance.gameObject.AddComponent<CAssetsDownLoader>();

            CAssetsDownLoadTask task = new CAssetsDownLoadTask();
            task.assetPath = name;
            task.assetABName = String.Empty;
            task.successLoadSceneCallback = successCallbacks;
            task.loadingSceneCallBack = loadingCallBack;
            task.failureLoadSceneCallback = failureCallback;
            _downLoader.AddLoadAB(task);
        }


    }
}
#endif