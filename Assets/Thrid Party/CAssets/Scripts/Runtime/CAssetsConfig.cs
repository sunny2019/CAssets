using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;

namespace CAssets
{
   [CreateAssetMenu(fileName = "CAssetsConfig", menuName = "Create CAssetsConfig", order = 1)]
   public class CAssetsConfig : SerializedScriptableObject
   {
      [LabelText("模拟模式")] public bool SimMode = false;

      [LabelText("资源卸载频率")] [Range(10f, 60f)]
      public float UnloadFrequency = 30f;

      [LabelText("已打包资源")] public Dictionary<string, string> ABAssetsConfig = new Dictionary<string, string>();

   }
}