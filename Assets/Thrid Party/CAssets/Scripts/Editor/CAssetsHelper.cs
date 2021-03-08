using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CAssets
{
    public static class CAssetsHelper
    {
        /// <summary>
        /// 去除字符串列表中重复项
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static List<string> StringListTrim(this List<string> origin)
        {
            List<string> listTemp = new List<string>();
            for (int i = 0; i < origin.Count; i++)
            {
                if (!listTemp.Contains(origin[i]) && origin[i].StartsWith("Assets"))
                {
                    listTemp.Add(origin[i]);
                }
            }

            return listTemp;
        }


        public delegate bool Filter(string suffix);

        /// <summary>
        /// 获取某个目录下所有除filter和文件夹路径以外的资源路径
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<string> GetAllAssetPathByPath(string[] paths)
        {
            List<string> guids = AssetDatabase.FindAssets("", paths).ToList();
            List<string> noFolderPaths = new List<string>();
            string assetPathStr = "";
            string metaPathStr = "";
            string metaContentStr = "";
            string[] depens;
            //剔除掉文件夹
            for (int i = 0; i < guids.Count; i++)
            {
                assetPathStr = AssetDatabase.GUIDToAssetPath(guids[i]);
                metaPathStr = AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPathStr);
                metaContentStr = File.ReadAllText(metaPathStr).ToLower();
                if (!metaContentStr.Contains("folderasset: yes"))
                {
                    //添加自己
                    noFolderPaths.Add(assetPathStr);
                    //添加依赖文件
                    depens = AssetDatabase.GetDependencies(assetPathStr);
                    noFolderPaths.AddRange(depens);
                }
            }

            //剔除不需要的类型文件
            List<string> assetPaths = new List<string>();
            string extensionStr = "";
            for (int i = 0; i < noFolderPaths.Count; i++)
            {
                extensionStr = Path.GetExtension(noFolderPaths[i]);
                if (!CAssetsSetting.ABAssetOutFilter.Contains(extensionStr))
                {
                    assetPaths.Add(noFolderPaths[i]);
                }
            }

            return assetPaths;
        }


        /// <summary>
        /// 获取字典索引和打包配置
        /// </summary>
        /// <param name="allBuildAssets"></param>
        /// <param name="ABBuilds"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDicAssetNameAndBundleName(List<string> allBuildAssets, out List<AssetBundleBuild> ABBuilds)
        {
            Dictionary<string, string> dic_BuildABAssets = new Dictionary<string, string>();
            int sceneIndex = 0;
            int assetIndex = 0;
            for (int i = 0; i < allBuildAssets.Count; i++)
            {
                if (!dic_BuildABAssets.ContainsKey(allBuildAssets[i]))
                {
                    if (allBuildAssets[i].EndsWith(".unity"))
                    {
                        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                        for (int j = 0; j < scenes.Length; j++)
                        {
                            if (scenes[j].path == allBuildAssets[i])
                            {
                                scenes[j].enabled = false;
                            }
                        }

                        EditorBuildSettings.scenes = scenes;
                        dic_BuildABAssets.Add(allBuildAssets[i], CAssetsSetting.ScenePrefix + sceneIndex);
                        sceneIndex++;
                    }
                    else
                    {
                        dic_BuildABAssets.Add(allBuildAssets[i], CAssetsSetting.AssetPrefix + assetIndex);
                        assetIndex++;
                    }
                }
            }

            ABBuilds = new List<AssetBundleBuild>();
            foreach (var v in dic_BuildABAssets)
            {
                AssetBundleBuild temp = new AssetBundleBuild();
                temp.assetBundleName = v.Value;
                temp.assetNames = new[] {v.Key};
                ABBuilds.Add(temp);
            }

            return dic_BuildABAssets;
        }
    }
}