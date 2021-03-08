using System;
using System.Collections;
using System.Collections.Generic;

namespace CAssets
{
    /// <summary>
    /// 资源管理器接口。
    /// </summary>
    public interface IResourceManager
    {
        void StartPreLoadAllAssetAB(List<string> abNames, Action<float> processCallBack, Action finshedCallBack);
        void Load(string name, Action<string, object> successCallbacks, Action<float> loadingCallBack, Action<string, string> failureCallback);


    }
}