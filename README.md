# CAssets  
这是一个专门为Webgl资源加载开发的小工具(当然无需更改也可用于其他平台)，打包资源目录中的所有资源会进行筛选，并获取依赖文件，然后对所有文件进行AssetBundle打包。运行时使用统一的接口对所有资源进行加载。CAssets主要针对加快项目启动速度开发，并没有资源版本对比环节，更依赖于Webgl对Web请求的缓存来进行加速，对于使用到其他无缓存机制的项目本地平台实现资源热更需要进行修改。  
## 使用流程：  
初始的项目启动场景中可放置简介内容等等，待在启动场景中预加载所有资源结束，再跳转主场景，打开主界面（参见Demo2Preload）。  
1. 配置静态脚本CAssets/Scripts/Runtime/CAssetsSetting.cs  
这个脚本中配置了需要打包的资源路径、目标路径、资源后缀过滤、AssetBundle前缀。  
2. 配置脚本化资源CAssets/Resources/CAssetsConfig.asset  
这个资源中需要配置卸载频率、是否模拟模式运行等。  
3. 点击标题栏Tools/BuildAB/Build，即可再目标路径中生成AssetBundle。  
4. 想要模拟真机运行取消脚本化资源的模拟选项即可。  
5. 每次更新资源后需要重新生成AssetBundle，点击标题栏Tools/BuildAB/Clear即可。  
6. 另有两个Demo，包含了运行时加载和预加载两个Demo。  
## 主要接口  
1. 加载资源接口（包括场景）：CAssets.Instance.Load(string assetName, Action<string, object> successLoadAssetCallback, Action<float> loadingSceneCallBack, Action<string, string> failureLoadAssetCallback)  
2. 预加载接口：CAssets.Instance.StartPreloadAllAssetAB( Action<float> processCallBak, Action finshedCallBack)  
