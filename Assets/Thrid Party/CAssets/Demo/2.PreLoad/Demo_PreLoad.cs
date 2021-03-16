using UnityEngine;
using UnityEngine.UI;

namespace CAssets
{
    public class Demo_PreLoad : MonoBehaviour
    {
        public GameObject panel_Preload;

        public Image img_Fill;

        public Button btn_EnterLoadedScene;
        
       

        // Start is called before the first frame update
        void Start()
        {
            panel_Preload.SetActive(true);
            panel_Preload.transform.SetAsLastSibling();
            CAssets.Instance.StartPreloadAllAssetAB((i) =>
            {
                Debug.Log($"加载进度：{i}");
                img_Fill.fillAmount = i;
            }, () =>
            {
                Destroy(panel_Preload);
                Debug.Log("加载完成");
                btn_EnterLoadedScene.gameObject.SetActive(true);
                btn_EnterLoadedScene.onClick.AddListener(() =>
                {
                    CAssets.Instance.Load("Assets/Thrid Party/CAssets/DemoAssets/Scenes/LoadedScene.unity", LoadSceneSuccess, null, null);
                });
            });
        }


        private void LoadSceneSuccess(string arg1, object arg2)
        {
            if (arg1 == "Assets/Thrid Party/CAssets/DemoAssets/Scenes/LoadedScene.unity")
            {
                Destroy(GameObject.Find("Ch08_nonPBR"));
                CAssets.Instance.Load("Assets/Thrid Party/CAssets/DemoAssets/Models/Man/Prefabs/Ch08_nonPBR.prefab",
                    (name, obj) => { Instantiate((GameObject) obj); }, null, null);
            }
        }
    }
}