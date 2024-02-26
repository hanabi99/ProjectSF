using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
namespace MyGameFrameWork
{
    /// <summary>
    /// 1.可以提供给外部添加帧更新事件的方法
    /// 2.可以提供给外部添加 协程的方法
    /// </summary>
    public class MonoMgr : Singleton<MonoMgr>
    {
        private MonoController controller;

        private void Awake()
        {
            GameObject obj = new GameObject("MonoController");
            controller = obj.AddComponent<MonoController>();
        }

        /// <summary>
        /// 给外部提供的 添加帧更新事件的函数
        /// </summary>
        /// <param name="fun"></param>
        public void AddUpdateListener(UnityAction fun)
        {
            controller.AddUpdateListener(fun);
        }

        /// <summary>
        /// 提供给外部 用于移除帧更新事件函数
        /// </summary>
        /// <param name="fun"></param>
        public void RemoveUpdateListener(UnityAction fun)
        {
            controller.RemoveUpdateListener(fun);
        }
        IEnumerator enumerator()
        {
            yield return null;
            StartCoroutine(enumerator());
        }

       new  public Coroutine StartCoroutine(IEnumerator routine)
        {
            return controller.StartCoroutine(routine);
        }

        new public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
        {
            return controller.StartCoroutine(methodName, value);
        }

        new public Coroutine StartCoroutine(string methodName)
        {
            return controller.StartCoroutine(methodName);
        }
    }
}
