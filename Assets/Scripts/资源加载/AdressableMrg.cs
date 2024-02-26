using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace MyGameFrameWork { 
public class AsyncOperationHandleInfo{

    public AsyncOperationHandle handle;

    public uint refenceCount;
    public AsyncOperationHandleInfo(AsyncOperationHandle handle)
    {
        this.handle = handle;
        refenceCount += 1;
    }
}
    public class AdressableMrg : Singleton<AdressableMrg>
    {
        /// <summary>
        /// 存储异步加载返回值
        /// </summary>
        public Dictionary<string, AsyncOperationHandleInfo> resDic = new Dictionary<string, AsyncOperationHandleInfo>();

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void LoadAssetAsync<T>(string name, Action<AsyncOperationHandle<T>> callback)
        {
            //由于存在同名 不同类型资源的区分加载
            //通过名字加类型名拼接
            string keyname = name + "_" + typeof(T).Name;
            AsyncOperationHandle<T> handle;

            //如果加载过资源
            if (resDic.ContainsKey(keyname))
            {
                handle = resDic[keyname].handle.Convert<T>();

                resDic[keyname].refenceCount++;

                //如果连续whlie循环加载 他加载结束了
                if (handle.IsDone)//如果异步加载结束 那肯定是成功了的
                {
                    callback(handle);
                }
                else//加载没结束
                {
                    handle.Completed += (obj) =>
                    {
                        if (obj.Status == AsyncOperationStatus.Succeeded)
                        {
                            callback(obj);
                        }
                    };
                }
                return;
            }
            //直到进行异步加载 并且记录
            handle = Addressables.LoadAssetAsync<T>(name);
            handle.Completed += (obj) =>
            {
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    callback(obj);
                }
                else
                {
                    Debug.LogWarning(keyname + "资源加载失败了");
                    if (resDic.ContainsKey(keyname))
                        resDic.Remove(keyname);
                }

            };
            AsyncOperationHandleInfo info = new AsyncOperationHandleInfo(handle);
            resDic.Add(keyname, info);
        }

        /// <summary>
        /// 加载多个满足条件资源 或者 指定条件资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mergeMode"></param>
        /// <param name="callback"></param>
        /// <param name="keys"></param>
        public void LoadAssetsAsync<T>(Addressables.MergeMode mergeMode, Action<T> callback, params string[] keys)
        {
            List<string> list = new List<string>(keys);
            string Keyname = "";
            foreach (var key in keys)
            {
                Keyname += key + "_";
            }
            Keyname += typeof(T).Name;

            AsyncOperationHandle<IList<T>> handle;

            if (resDic.ContainsKey(Keyname))
            {
                handle = resDic[Keyname].handle.Convert<IList<T>>();

                resDic[Keyname].refenceCount++;
                if (handle.IsDone)
                {
                    foreach (var item in handle.Result)
                    {
                        callback(item);
                    }
                }
                else
                {
                    handle.Completed += (obj) =>
                    {
                        if (obj.Status == AsyncOperationStatus.Succeeded)
                        {
                            foreach (var item in handle.Result)
                            {
                                callback(item);
                            }
                        }
                    };
                }
                return;
            }


            handle = Addressables.LoadAssetsAsync<T>(list, callback, mergeMode);
            handle.Completed += (obj) =>
            {
                if (obj.Status == AsyncOperationStatus.Failed)
                {
                    Debug.Log("资源加载失败" + Keyname);
                    if (resDic.ContainsKey(Keyname))
                    {
                        resDic.Remove(Keyname);
                    }
                }
            };
            AsyncOperationHandleInfo info = new AsyncOperationHandleInfo(handle);
            resDic.Add(Keyname, info);

        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        public void Release<T>(string name)
        {
            string keyName = name + "_" + typeof(T).Name;
            if (resDic.ContainsKey(keyName))
            {
                //手动计数减一
                resDic[keyName].refenceCount--;
                if (resDic[keyName].refenceCount == 0)
                {
                    AsyncOperationHandle<T> handle = resDic[keyName].handle.Convert<T>();
                    Addressables.Release(handle);
                    resDic.Remove(keyName);
                }
            }
        }

        /// <summary>
        /// 释放指定条件资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        public void Release<T>(params string[] keys)
        {
            List<string> list = new List<string>(keys);
            string Keyname = "";
            foreach (var key in keys)
            {
                Keyname += key + "_";
            }
            Keyname += typeof(T).Name;

            if (resDic.ContainsKey(Keyname))
            { //手动计数减一
                resDic[Keyname].refenceCount--;
                if (resDic[Keyname].refenceCount == 0)
                {
                    AsyncOperationHandle<IList<T>> handel = resDic[Keyname].handle.Convert<IList<T>>();
                    Addressables.Release(handel);
                    resDic.Remove(Keyname);
                }
            }
        }
        /// <summary>
        /// 清空资源
        /// </summary>
        public void ClearRes()
        {
            foreach (var item in resDic.Values)
            {
                Addressables.Release(item.handle);
            }
            resDic.Clear();
            AssetBundle.UnloadAllAssetBundles(true);
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

    }

}
