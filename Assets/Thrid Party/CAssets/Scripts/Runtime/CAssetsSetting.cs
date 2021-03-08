using System.Collections.Generic;
using UnityEngine;

namespace CAssets
{
    public static class CAssetsSetting
    {


        /// <summary>
        /// 项目主文件夹
        /// </summary>
        public static string ProjectPath = "Assets/Thrid Party/CAssets/DemoAssets";


        /// <summary>
        /// 打包AB路径
        /// </summary>
        public static string BuildABPath = Application.streamingAssetsPath + "/CAssets/";



        /// <summary>
        /// AB包中排除的资源后缀
        /// </summary>
        public static List<string> ABAssetOutFilter = new List<string>()
        {
            ".asmdef",
            ".meta",
            ".cs",
        };


        /// <summary>
        /// AB包场景前缀
        /// </summary>
        public const string ScenePrefix = "scene";

        /// <summary>
        /// AB包资源前缀
        /// </summary>
        public const string AssetPrefix = "asset";


    }
}