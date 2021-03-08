using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CAssets
{

    public class CAssetsBuild
    {
        [MenuItem("Tools/CAssets/Build")]
        public static void BuildAB()
        {
            //需要打包AB的资源
            List<string> buildABAssets = new List<string>();

            //获取项目主文件夹下的所有资源(剔除.cs脚本)
            List<string> ProjectPathAssets = CAssetsHelper.GetAllAssetPathByPath(new string[] {CAssetsSetting.ProjectPath});
            buildABAssets.AddRange(ProjectPathAssets);

            //去重
            buildABAssets = buildABAssets.StringListTrim();

            //保存资源路径和包名    生成打包builds
            List<AssetBundleBuild> list_Builds;
            Dictionary<string, string> dic_BuildABAssets = CAssetsHelper.GetDicAssetNameAndBundleName(buildABAssets, out list_Builds);
            //生成打包builds
            Debug.Log("资源数量：" + buildABAssets.Count + "\t字典索引数量：" + dic_BuildABAssets.Count + "\t打包配置数量：" + list_Builds.Count);
            if (buildABAssets.Count == dic_BuildABAssets.Count && dic_BuildABAssets.Count == list_Builds.Count)
            {
                // for (int i = 0; i < list_Builds.Count; i++)
                // {
                //     Debug.Log(list_Builds[i].assetBundleName + "\t" + list_Builds[i].assetNames.Length + "\t" + list_Builds[i].assetNames[0]);
                // }

                if (!Directory.Exists(CAssetsSetting.BuildABPath))
                    Directory.CreateDirectory(CAssetsSetting.BuildABPath);


#if UNITY_WEBGL
                BuildPipeline.BuildAssetBundles(CAssetsSetting.BuildABPath, list_Builds.ToArray(), BuildAssetBundleOptions.None, BuildTarget.WebGL);
#elif UNITY_STANDALONE
            BuildPipeline.BuildAssetBundles(CAssetsSetting.BuildABPath, list_Builds.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
#endif

                CAssetsConfig config = Resources.Load<CAssetsConfig>("CAssetsConfig");
                config.ABAssetsConfig = dic_BuildABAssets;

                AssetDatabase.Refresh();
            }
        }


        [MenuItem("Tools/CAssets/Clear")]
        public static void ClearABAssets()
        {
            CAssetsConfig config = Resources.Load<CAssetsConfig>("CAssetsConfig");
            if (config != null)
            {
                config.ABAssetsConfig.Clear();
            }

            if (Directory.Exists(CAssetsSetting.BuildABPath))
            {
                Directory.Delete(CAssetsSetting.BuildABPath, true);

                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                for (int j = 0; j < scenes.Length; j++)
                {
                    scenes[j].enabled = true;
                }

                EditorBuildSettings.scenes = scenes;
                Debug.Log("清除AB资源成功,已启用所有场景");
            }

            AssetDatabase.Refresh();

        }
    }
}