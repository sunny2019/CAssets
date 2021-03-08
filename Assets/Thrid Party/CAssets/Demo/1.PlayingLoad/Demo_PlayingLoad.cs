using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace CAssets
{
    public class Demo_PlayingLoad : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(this);
        }



        [Button("加载场景")]
        public void LoadScene()
        {
            CAssets.Instance.Load("Assets/Thrid Party/CAssets/DemoAssets/Scenes/LoadedScene.unity", LoadSceneSuccess, null, null);
        }

        [Button("加载场景1")]
        private void LoadScene1()
        {
            CAssets.Instance.Load("Assets/Thrid Party/CAssets/DemoAssets/Scenes/LoadedScene 1.unity", LoadSceneSuccess, null, null);
        }
        [Button("赋值场景1的图片")]
        public void SetSpite()
        {
            GameObject.FindObjectOfType<Image>().sprite = spite;
        }

        [Button("Resources.UnloadUnusedAssets()")]
        private void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }

        [Button("AssetBundle.UnloadAllAssetBundles(false)")]
        private void UnloadAllAssetBundlesFalse()
        {
            AssetBundle.UnloadAllAssetBundles(false);
        }

        [Button("AssetBundle.UnloadAllAssetBundles(true)")]
        private void UnloadUnusedAssetsTrue()
        {
            AssetBundle.UnloadAllAssetBundles(true);
        }

        


        private void LoadSceneSuccess(string arg1, object arg2)
        {
            if (arg1 == "Assets/Thrid Party/CAssets/DemoAssets/Scenes/LoadedScene.unity")
            {
                Destroy(GameObject.Find("Ch08_nonPBR"));
                CAssets.Instance.Load("Assets/Thrid Party/CAssets/DemoAssets/Models/Man/Prefabs/Ch08_nonPBR.prefab", (name, obj) => { Instantiate((GameObject) obj); }, null, null);
            }

            if (arg1 == "Assets/Thrid Party/CAssets/DemoAssets/Scenes/LoadedScene 1.unity")
            {
                CAssets.Instance.Load("Assets/Thrid Party/CAssets/DemoAssets/Sprites/p1.jpg", (name, obj) =>
                {
                    //GameObject.FindObjectOfType<Image>().sprite = Texture2DToSprite((Texture2D) obj);
                    spite = AssetHelper.Texture2DToSprite((Texture2D) obj);
                }, null, null);
            }
        }

        public Sprite spite;




    }
}