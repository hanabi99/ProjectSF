using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace MyGameFrameWork
{
    public class ABManager : Singleton<ABManager>
    {
        //private Dictionary<string, AssetBundleObject> _readyABList = new Dictionary<string, AssetBundleObject>(); //预备加载的列表
        //private Dictionary<string, AssetBundleObject> _loadingABList = new Dictionary<string, AssetBundleObject>(); //正在加载的列表
        private Dictionary<string, AssetBundleObject> _loadedABList = new Dictionary<string, AssetBundleObject>(); //加载完成的列表
        private Dictionary<string, AssetBundleObject> _unloadABList = new Dictionary<string, AssetBundleObject>(); //准备卸载的列表

        private Dictionary<string, string[]> _dependeciesDataList = new Dictionary<string, string[]>();//AB包依赖项字典

       // private const int MAX_LOADING_COUNT = 10; //同时加载的最大数量

        //主包 
        private AssetBundle mainAB = null;
        private AssetBundleManifest manifest = null;

        private string PathURL
        {
            get { return "Assets/ArtRes/AB/PC" + "/"; }
        }

        private string MainABName
        {
            get
            {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
                return "PC";
#endif
            }
        }
        /// <summary>
        /// 加载Manifest
        /// </summary>
        public void LoadMainfest()
        {
            _dependeciesDataList.Clear();

            //加载AB包
            if (mainAB == null)
            {
                mainAB = AssetBundle.LoadFromFile(PathURL + MainABName);
            }

            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            foreach (string assetName in manifest.GetAllAssetBundles())
            {
                string abname = assetName.Replace(".ab", "");
                string[] dps = manifest.GetAllDependencies(assetName);
                for (int i = 0; i < dps.Length; i++)
                    dps[i] = dps[i].Replace(".ab", "");
                _dependeciesDataList.Add(abname, dps);
            }

            mainAB.Unload(true);
            mainAB = null;
            foreach (var item in _dependeciesDataList)
            {
                for (int i = 0; i < item.Value.Length; i++)
                {
                    Debug.Log(item.Key + ": " + item.Value[i]);
                }
            }
            Debug.Log("AssetBundleLoadMgr dependsCount=" + _dependeciesDataList.Count);
        }



        /// <summary>
        /// 递归添加引用计数 将AB包的直接/间接依赖全部+1
        /// </summary>
        /// <param name="abObj"></param>
        private void AddDependsRef(AssetBundleObject abObj)
        {
            abObj._refCount++;
            if(_dependeciesDataList[abObj.abName].Length > 0 && abObj._depends.Count == 0)
            {
                for (int i = 0; i < _dependeciesDataList[abObj.abName].Length; i++)
                {
                    abObj._depends.Add(_loadedABList[_dependeciesDataList[abObj.abName][i]]);
                }
            }
            foreach (var dpObj in abObj._depends)
            {
                dpObj._refCount++;
            }
        }

        /// <summary>
        /// 同步加载AB包
        /// </summary>
        /// <param name="abname"></param>
        public AssetBundleObject LoadAB(string abname)
        {
            AssetBundleObject abObj = null;
            //判断字典里是否有该AB包
            if (_loadedABList.ContainsKey(abname))
            {
                abObj = _loadedABList[abname];
                AddDependsRef(abObj);
                return abObj;
            }
            //创建ABObject对象
            abObj = new AssetBundleObject();
            abObj.abName = abname;
            abObj._refCount = 1;
            abObj._ab = AssetBundle.LoadFromFile(PathURL + abname);

            string[] dependeciesData = null;

            if (_dependeciesDataList.ContainsKey(abname))
            {
                dependeciesData = _dependeciesDataList[abname];
            }

            if (dependeciesData != null && dependeciesData.Length > 0)
            {
                abObj._dependLoadingCount = dependeciesData.Length;

                foreach (var dpAssetName in dependeciesData)
                {
                    AssetBundleObject abObjTemp;
                    if (_loadedABList.ContainsKey(dpAssetName))
                    {
                        abObjTemp = _loadedABList[dpAssetName];
                        abObjTemp._refCount++;
                    }
                    else
                    {
                        abObjTemp = new AssetBundleObject();
                        abObjTemp.abName = dpAssetName;
                        abObjTemp._ab = AssetBundle.LoadFromFile(PathURL + dpAssetName);
                        abObjTemp._refCount = 1;
                        _loadedABList.Add(dpAssetName, abObjTemp);
                    }
                    abObj._depends.Add(abObjTemp);
                }
            }
            _loadedABList.Add(abObj.abName, abObj);

            return abObj;
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="abname"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        public Object LoadRes(string abname, string resName)
        {
            LoadAB(abname);
            Object obj = _loadedABList[abname]._ab.LoadAsset(resName);
            return obj;
        }
        /// <summary>
        /// 同步加载 typeof
        /// </summary>
        /// <param name="abname"></param>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Object LoadRes(string abname, string resName, System.Type type)
        {
            LoadAB(abname);
            Object obj = _loadedABList[abname]._ab.LoadAsset(resName, type);
            return obj;

        }

        /// <summary>
        /// 同步加载 泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abname"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        public T LoadRes<T>(string abname, string resName) where T : Object
        {
            LoadAB(abname);
            T obj = _loadedABList[abname]._ab.LoadAsset<T>(resName);
            return obj;
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="abname"></param>
        /// <param name="resname"></param>
        /// <param name="callback"></param>
        public void LoadResAsync(string abname, string resname, UnityAction<Object> callback = null)
        {
            StartCoroutine(ReallyLoadResAsync(abname, resname, callback));
        }
        private IEnumerator ReallyLoadResAsync(string abname, string resname, UnityAction<Object> callback)
        {
            LoadAB(abname);
            AssetBundleRequest abr = _loadedABList[abname]._ab.LoadAssetAsync(resname);
            yield return abr;

            if (abr.asset is GameObject)
                callback?.Invoke(abr.asset);
            else
                callback?.Invoke(abr.asset);
        }

        /// <summary>
        /// 异步加载重载,type
        /// </summary>
        /// <param name="abname"></param>
        /// <param name="resname"></param>
        /// <param name="callback"></param>
        /// <param name="type"></param>
        public void LoadResAsync(string abname, string resname, System.Type type, UnityAction<Object> callback = null)
        {
            StartCoroutine(ReallyLoadResAsync(abname, resname, callback, type));
        }
        private IEnumerator ReallyLoadResAsync(string abname, string resname, UnityAction<Object> callback, System.Type type)
        {
            LoadAB(abname);
            AssetBundleRequest abr = _loadedABList[abname]._ab.LoadAssetAsync(resname, type);
            yield return abr;

            if (abr.asset is GameObject)
                callback?.Invoke(Instantiate(abr.asset));
            else
                callback?.Invoke(abr.asset);
        }

        /// <summary>
        ///泛型异步加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abname"></param>
        /// <param name="resname"></param>
        /// <param name="callback"></param>
        public void LoadResAsync<T>(string abname, string resname, UnityAction<T> callback = null) where T : Object
        {
            StartCoroutine(ReallyLoadResAsync<T>(abname, resname, callback));
        }
        private IEnumerator ReallyLoadResAsync<T>(string abname, string resname, UnityAction<T> callback) where T : Object
        {
            LoadAB(abname);
            AssetBundleRequest abr = _loadedABList[abname]._ab.LoadAssetAsync<T>(resname);
            yield return abr;

            if (abr.asset is GameObject)
                callback?.Invoke(abr.asset as T);
            else
                callback?.Invoke(abr.asset as T);

        }
        /// <summary>
        /// Debug打印引用计数
        /// </summary>
        public void DebugRefCount()
        {
            foreach (var item in _loadedABList)
            {
                Debug.Log(item.Key + ": " + item.Value._refCount);
            }
        }

        /// <summary>
        /// 单个包卸载
        /// </summary>
        /// <param name="abname"></param>
        public void UnLoad(string abname)
        {
            if (_loadedABList.ContainsKey(abname))
            {
                _loadedABList[abname]._refCount--;
                for (int i = 0; i < _loadedABList[abname]._depends.Count; i++)
                {
                    _loadedABList[abname]._depends[i]._refCount--;
                    if (_loadedABList[abname]._depends[i]._refCount == 0)
                    {
                        _unloadABList.Add(_loadedABList[abname]._depends[i].abName, _loadedABList[abname]._depends[i]);
                        _loadedABList.Remove(_loadedABList[abname]._depends[i].abName);
                    }
                }
                if (_loadedABList[abname]._refCount == 0)
                {
                    _unloadABList.Add(_loadedABList[abname].abName, _loadedABList[abname]);
                    _loadedABList.Remove(abname);
                }
                UnLoad();
            }
            else
            {
                Debug.LogError("该AB包未加载，无法卸载");
            }
        }
        /// <summary>
        /// 真正的卸载AB包
        /// </summary>
        public void UnLoad()
        {
            if(_unloadABList.Count > 0)
            {
                foreach (AssetBundleObject item in _unloadABList.Values)
                {
                    item._ab.Unload(true);
                }
            }
            _unloadABList.Clear();
        }
        public void ClearAb()
        {
            AssetBundle.UnloadAllAssetBundles(false);
            _loadedABList.Clear();
            _dependeciesDataList.Clear();
            mainAB = null;
            manifest = null;
        }

        public void OnDestroy()
        {
            GC.Collect();
        }
    }

    public delegate void AssetBundleLoadCallBack(AssetBundle ab);
    public class AssetBundleObject
    {
        public string abName;

        public AssetBundle _ab; //加载到的ab

        public int _refCount; //引用计数

        //public List<AssetBundleLoadCallBack> _callFunList = new List<AssetBundleLoadCallBack>(); //回调函数

        //public AssetBundleCreateRequest _request; //异步加载请求

        public int _dependLoadingCount; //依赖计数

        public List<AssetBundleObject> _depends = new List<AssetBundleObject>(); //依赖项
    }

}
