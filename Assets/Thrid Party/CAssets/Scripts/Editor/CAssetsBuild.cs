using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector.Editor.Validation;
using UnityEditor;
using UnityEngine;

namespace CAssets
{
    public class CAssetsBuild
    {
        [MenuItem("Tools/CAssets/Build")]
        public static void BuildAB()
        {
            Caching.ClearCache();
            //需要打包AB的资源
            List<string> buildABAssets = new List<string>();

            //获取项目主文件夹下的所有资源(剔除.cs脚本)
            List<string> ProjectPathAssets = CAssetsHelper.GetAllAssetPathByPath(new string[] {CAssetsSetting.ProjectPath});
            buildABAssets.AddRange(ProjectPathAssets);

            //去重
            buildABAssets = buildABAssets.StringListTrim();
            //去除文件夹
            buildABAssets = CAssetsHelper.RemoveFolder(buildABAssets);
            //去除不需要的文件类型
            buildABAssets = CAssetsHelper.RemoveNotNeededType(buildABAssets);
            //去除不需要的文件
            buildABAssets = CAssetsHelper.RemoveNotNeededFile(buildABAssets);


            //保存资源路径和包名    生成打包builds
            List<AssetBundleBuild> list_Builds;
            Dictionary<string, string> dic_BuildABAssets = CAssetsHelper.GetDicAssetNameAndBundleName(buildABAssets, out list_Builds);


            //生成打包builds
            Debug.Log("资源数量：" + buildABAssets.Count + "\t字典索引数量：" + dic_BuildABAssets.Count + "\t打包配置数量：" + list_Builds.Count);


            if (!Directory.Exists(CAssetsSetting.BuildABPath))
                Directory.CreateDirectory(CAssetsSetting.BuildABPath);


            BuildPipeline.BuildAssetBundles(CAssetsSetting.BuildABPath, CAssetsHelper.GetAssetAB(list_Builds).ToArray(), BuildAssetBundleOptions.CollectDependencies,
                EditorUserBuildSettings.activeBuildTarget);
            Dictionary<string, string> dic_Scene = CAssetsHelper.GetSceneAB(list_Builds);
            foreach (var v in dic_Scene)
            {
                BuildPipeline.BuildPlayer(new[] {v.Value}, CAssetsSetting.BuildABPath + v.Key, EditorUserBuildSettings.activeBuildTarget,
                    BuildOptions.BuildAdditionalStreamedScenes);
            }

            

            CAssetsConfig config = Resources.Load<CAssetsConfig>("CAssetsConfig");
            config.ABAssetsConfig = dic_BuildABAssets;

            AssetDatabase.Refresh();
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