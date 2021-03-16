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


        /// <summary>
        /// 获取某个目录下所有除filter和文件夹路径以外的资源路径
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<string> GetAllAssetPathByPath(string[] paths)
        {
            List<string> guids = AssetDatabase.FindAssets("", paths).ToList();
            List<string> assetPaths = new List<string>();
            string assetPathStr = "";
            string metaPathStr = "";
            string metaContentStr = "";
            string[] depens;
            //剔除掉文件夹
            for (int i = 0; i < guids.Count; i++)
            {
                assetPathStr = AssetDatabase.GUIDToAssetPath(guids[i]);
                //添加自己
                assetPaths.Add(assetPathStr);
                //添加依赖文件
                depens = AssetDatabase.GetDependencies(assetPathStr);
                assetPaths.AddRange(depens);
            }
            return assetPaths;
        }


        /// <summary>
        /// 剔除文件夹
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static List<string> RemoveFolder(List<string> paths)
        {
            List<string> assetPaths = new List<string>();
            string metaPathStr = "";
            string metaContentStr = "";
            for (int i = 0; i < paths.Count; i++)
            {
                metaPathStr = AssetDatabase.GetTextMetaFilePathFromAssetPath(paths[i]);
                metaContentStr = File.ReadAllText(metaPathStr).ToLower();
                if (!metaContentStr.Contains("folderasset: yes"))
                {
                    //添加自己
                    assetPaths.Add(paths[i]);
                }
            }

            return assetPaths;
        }

        /// <summary>
        /// 剔除不需要的文件类型
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static List<string> RemoveNotNeededType(List<string> paths)
        {
            List<string> assetPaths = new List<string>();
            string extensionStr = "";
            for (int i = 0; i < paths.Count; i++)
            {
                extensionStr = Path.GetExtension(paths[i]);
                if (!CAssetsSetting.ABAssetOutFilter.Contains(extensionStr))
                {
                    assetPaths.Add(paths[i]);
                }
            }

            return assetPaths;
        }

        /// <summary>
        /// 剔除不需要的文件类型
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static List<string> RemoveNotNeededFile(List<string> paths)
        {
            List<string> assetPaths = new List<string>();
            string fileName = "";
            for (int i = 0; i < paths.Count; i++)
            {
                fileName = Path.GetFileName(paths[i]);
                if (!CAssetsSetting.ABAssetOutFileFilter.Contains(fileName))
                {
                    assetPaths.Add(paths[i]);
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
            List<string> pureAssetPaths = new List<string>();
            pureAssetPaths.AddRange(allBuildAssets);

            int sceneIndex = 0;
            string pathFloder = "";
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
                        pathFloder = (Directory.GetParent(allBuildAssets[i]) + "/" + Path.GetFileNameWithoutExtension(allBuildAssets[i]) + "/").Replace('\\', '/');

                        dic_BuildABAssets.Add(allBuildAssets[i], CAssetsSetting.ScenePrefix + sceneIndex+".unity3d");

                        for (int j = 0; j < allBuildAssets.Count; j++)
                        {
                            if (allBuildAssets[j].StartsWith(pathFloder))
                            {
                                //场景包括烘焙资源已添加进来
                                dic_BuildABAssets.Add(allBuildAssets[j], CAssetsSetting.ScenePrefix + sceneIndex+".unity3d");
                            }
                        }

                        sceneIndex++;
                    }
                }
            }

            int assetIndex = 0;
            for (int i = 0; i < allBuildAssets.Count; i++)
            {
                if (!dic_BuildABAssets.ContainsKey(allBuildAssets[i]))
                {
                    dic_BuildABAssets.Add(allBuildAssets[i], CAssetsSetting.AssetPrefix + assetIndex);
                    assetIndex++;
                }
            }

            Dictionary<string, List<string>> dic_AssetBundle = new Dictionary<string, List<string>>();
            foreach (var v in dic_BuildABAssets)
            {
                if (!dic_AssetBundle.ContainsKey(v.Value))
                {
                    dic_AssetBundle.Add(v.Value, new List<string>());
                }

                dic_AssetBundle[v.Value].Add(v.Key);
            }

            ABBuilds = new List<AssetBundleBuild>();
            foreach (var v in dic_AssetBundle)
            {
                AssetBundleBuild temp = new AssetBundleBuild();
                temp.assetBundleName = v.Key;
                temp.assetNames = v.Value.ToArray();
                ABBuilds.Add(temp);
            }


            return dic_BuildABAssets;
        }


        public static List<AssetBundleBuild> GetAssetAB(List<AssetBundleBuild> list_Build)
        {
            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            for (int i = 0; i < list_Build.Count; i++)
            {
                if (list_Build[i].assetBundleName.StartsWith(CAssetsSetting.AssetPrefix))
                {
                    builds.Add(list_Build[i]);
                }
            }
            return builds;
        }

        public static Dictionary<string, string> GetSceneAB(List<AssetBundleBuild> list_Build)
        {
            Dictionary<string, string> dic_SceneBuilds = new Dictionary<string, string>();
            for (int i = 0; i < list_Build.Count; i++)
            {
                if (list_Build[i].assetBundleName.StartsWith(CAssetsSetting.ScenePrefix))
                {
                    for (int j = 0; j < list_Build[i].assetNames.Length; j++)
                    {
                        if (list_Build[i].assetNames[j].EndsWith(".unity"))
                        {
                            dic_SceneBuilds.Add(list_Build[i].assetBundleName, list_Build[i].assetNames[j]);
                        }
                    }
                }
            }

            return dic_SceneBuilds;
        }
    }
}