using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI层级
/// </summary>
public enum E_UI_Layer
{
    BottomPanelLayer,
    levelOneLayer,
    levelTwoLayer,
    levelThreeLayer,
}

namespace MyGameFrameWork
{
    public class UIManager : MyGameFrameWork.Singleton<UIManager>
    {
        private Camera mUICamera;

        private Transform mUIRoot;

        private LoadPrefabPathConfig loadPrefabPathConfig;

        private ABNameConfig aBNameConfig;

        private Dictionary<string, BasePanel> mAllPanelDic = new Dictionary<string, BasePanel>();

        private List<BasePanel> mAllPanelList = new List<BasePanel>();

        private List<BasePanel> mVisiblePanelList = new List<BasePanel>();

        private Queue<BasePanel> mPanelStack = new Queue<BasePanel>();

        private bool mStartPopStackWndStatus = false;

        /// <summary>
        /// level越大层级越高
        /// </summary>
        private Transform bottomPanelLayer;
        private Transform levelOneLayer;//sortingLayer 1-100
        private Transform levelTwoLayer;//sortingLayer 101-200
        private Transform levelThreeLayer;//sortingLayer 201-300

        public void Initialize()
        {
            mUICamera = Camera.main;
            mUIRoot = GameObject.Find("UIRoot").transform;
            bottomPanelLayer = new GameObject("BottomPanelLayer").transform;
            bottomPanelLayer.SetParent(mUIRoot);
            levelOneLayer = new GameObject("levelOneLayer").transform;
            levelOneLayer.SetParent(mUIRoot);
            levelTwoLayer = new GameObject("levelTwoLayer").transform;
            levelTwoLayer.SetParent(mUIRoot);
            levelThreeLayer = new GameObject("levelThreeLayer").transform;
            levelThreeLayer.SetParent(mUIRoot);
            loadPrefabPathConfig = Resources.Load<LoadPrefabPathConfig>("ScriptableObject/LoadPrefabPathConfig");
            aBNameConfig = Resources.Load<ABNameConfig>("ScriptableObject/ABNameConfig");
            //在手机上不会触发调用
#if UNITY_EDITOR
            loadPrefabPathConfig.GeneratorPanelConfig();
            aBNameConfig.GeneratorABConfig();
#endif
        }

        /// <summary>
        /// 预加载，只加载物体，不调用生命周期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void PreLoadPanel<T>() where T : BasePanel, new()
        {
            System.Type type = typeof(T);
            string wndName = type.Name;
            T PanelBase = new T();
            //克隆界面，初始化界面信息
            //1.生成对应的窗口预制体
            GameObject nWnd = TempLoadPanel(wndName);
            //2.初始出对应管理类
            if (nWnd != null)
            {
                PanelBase.gameObject = nWnd;
                PanelBase.transform = nWnd.transform;
                PanelBase.canvas = nWnd.GetComponent<Canvas>();
                PanelBase.canvas.worldCamera = mUICamera;
                PanelBase.name = nWnd.name;
                PanelBase.Init();
                PanelBase.SetActive(false);
                RectTransform rectTrans = nWnd.GetComponent<RectTransform>();
                rectTrans.anchorMax = Vector2.one;
                rectTrans.offsetMax = Vector2.zero;
                rectTrans.offsetMin = Vector2.zero;
                mAllPanelDic.Add(wndName, PanelBase);
                mAllPanelList.Add(PanelBase);
            }
            Debug.Log("预加载窗口 窗口名字：" + wndName);
        }
        /// <summary>
        /// 弹出一个窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T PopUpPanel<T>(E_UI_Layer e_UI_Layer) where T : BasePanel, new()
        {
            System.Type type = typeof(T);
            string wndName = type.Name;
            BasePanel wnd = GetPanel(wndName);
            if (wnd != null)
            {
                return ShowPanel(wndName) as T;
            }

            T t = new T();
            return InitializePanel(t, wndName, e_UI_Layer) as T;
        }

        /// <summary>
        /// 堆栈系统弹出
        /// </summary>
        /// <param name="Panel"></param>
        /// <returns></returns>
        private BasePanel PopUpPanel(BasePanel Panel, E_UI_Layer e_UI_Layer)
        {
            System.Type type = Panel.GetType();
            string wndName = type.Name;
            BasePanel wnd = GetPanel(wndName);
            if (wnd != null)
            {
                return ShowPanel(wndName);
            }
            return InitializePanel(Panel, wndName, e_UI_Layer);
        }
        private BasePanel InitializePanel(BasePanel PanelBase, string wndName, E_UI_Layer e_UI_Layer)
        {
            //1.生成对应的窗口预制体
            GameObject nWnd = TempLoadPanel(wndName);
            //2.初始出对应管理类
            if (nWnd != null)
            {
                Transform father = bottomPanelLayer;
                switch (e_UI_Layer)
                {
                    case E_UI_Layer.BottomPanelLayer:
                        father = bottomPanelLayer;
                        break;
                    case E_UI_Layer.levelOneLayer:
                        father = levelOneLayer;
                        break;
                    case E_UI_Layer.levelTwoLayer:
                        father = levelTwoLayer;
                        break;
                    case E_UI_Layer.levelThreeLayer:
                        father = levelThreeLayer;
                        break;
                }
                PanelBase.gameObject = nWnd;
                PanelBase.transform = nWnd.transform;
                PanelBase.canvas = nWnd.GetComponent<Canvas>();
                PanelBase.canvas.worldCamera = mUICamera;
                PanelBase._Layer = e_UI_Layer;
                PanelBase.transform.SetParent(father);
                PanelBase.transform.SetAsLastSibling();
                PanelBase.name = nWnd.name;
                PanelBase.Init();
                PanelBase.SetActive(true);
                PanelBase.ShowMe();
                RectTransform rectTrans = nWnd.GetComponent<RectTransform>();
                rectTrans.anchorMax = Vector2.one;
                rectTrans.offsetMax = Vector2.zero;
                rectTrans.offsetMin = Vector2.zero;
                mAllPanelDic.Add(wndName, PanelBase);
                mAllPanelList.Add(PanelBase);
                mVisiblePanelList.Add(PanelBase);
                SetPanelMaskVisible();
                return PanelBase;
            }
            Debug.LogError("没有加载到对应的窗口 窗口名字：" + wndName);
            return null;
        }
        private BasePanel GetPanel(string winName)
        {
            if (mAllPanelDic.ContainsKey(winName))
            {
                return mAllPanelDic[winName];
            }
            return null;
        }
        /// <summary>
        /// 获取已经弹出的弹窗
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetPanel<T>() where T : BasePanel
        {
            System.Type type = typeof(T);
            foreach (var item in mVisiblePanelList)
            {
                if (item.name == type.Name)
                {
                    return (T)item;
                }
            }
            Debug.LogError("该窗口没有获取到：" + type.Name);
            return null;
        }

        private BasePanel ShowPanel(string winName)
        {
            BasePanel Panel = null;
            if (mAllPanelDic.ContainsKey(winName))
            {
                Panel = mAllPanelDic[winName];
                if (Panel.gameObject != null && Panel.isActive == false)
                {
                    mVisiblePanelList.Add(Panel);
                    Panel.transform.SetAsLastSibling();
                    Panel.SetActive(true);
                    SetPanelMaskVisible();
                    Panel.ShowMe();
                }
                return Panel;
            }
            else
                Debug.LogError(winName + " 窗口不存在");
            return null;
        }

        public void HidePanel(string wndName)
        {
            BasePanel Panel = GetPanel(wndName);
            HidePanel(Panel);
        }
        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void HidePanel<T>() where T : BasePanel
        {
            HidePanel(typeof(T).Name);
        }
        public void HidePanel(BasePanel Panel)
        {
            if (Panel != null && Panel.isActive)
            {
                mVisiblePanelList.Remove(Panel);
                Panel.SetActive(false);//隐藏弹窗物体
                SetPanelMaskVisible();
                Panel.HideMe();
            }
            //在出栈的情况下，上一个界面隐藏时，自动打开栈种的下一个界面
            PopNextStackPanel(Panel);
        }
        private void DestroyPanel(string wndName)
        {
            BasePanel Panel = GetPanel(wndName);
            DestoryPanel(Panel);
        }
        public void DestroyWinodw<T>() where T : BasePanel
        {
            DestroyPanel(typeof(T).Name);
        }
        private void DestoryPanel(BasePanel Panel)
        {
            if (Panel != null)
            {
                if (mAllPanelDic.ContainsKey(Panel.name))
                {
                    mAllPanelDic.Remove(Panel.name);
                    mAllPanelList.Remove(Panel);
                    mVisiblePanelList.Remove(Panel);
                }
                Panel.SetActive(false);
                SetPanelMaskVisible();
                Panel.HideMe();
                Panel.OnDestroy();
                GameObject.Destroy(Panel.gameObject);
                //在出栈的情况下，上一个界面销毁时，自动打开栈种的下一个界面
                PopNextStackPanel(Panel);
            }
        }
        /// <summary>
        /// 处过滤窗口外 其他窗口全部销毁
        /// </summary>
        /// <param name="filterlist"></param>
        public void DestroyAllPanel(List<string> filterlist = null)
        {
            for (int i = mAllPanelList.Count - 1; i >= 0; i--)
            {
                BasePanel Panel = mAllPanelList[i];
                if (Panel == null || (filterlist != null && filterlist.Contains(Panel.name)))
                {
                    continue;
                }
                DestroyPanel(Panel.name);
            }
            Resources.UnloadUnusedAssets();
        }
        /// <summary>
        /// 设置单遮罩
        /// </summary>
        private void SetPanelMaskVisible()
        {
            if (!UISetting.Instance.SINGMASK_SYSTEM)
            {
                return;
            }
            BasePanel MaxOrderPanel = null;//最大渲染层级的窗口
            int maxOrder = 0;//最大渲染层级
            int maxIndex = 0;//最大排序下标 在相同父节点下的位置下标
                             //1.关闭所有窗口的Mask 设置为不可见
                             //2.从所有可见窗口中找到一个层级最大的窗口，把Mask设置为可见
            for (int i = 0; i < mVisiblePanelList.Count; i++)
            {
                BasePanel PanelBase = mVisiblePanelList[i];
                if (PanelBase != null && PanelBase.gameObject != null)
                {
                    PanelBase.SetMaskVisible(false);
                    if (MaxOrderPanel == null)
                    {
                        MaxOrderPanel = PanelBase;
                        maxOrder = PanelBase.canvas.sortingOrder;
                        maxIndex = PanelBase.transform.GetSiblingIndex();
                    }
                    else
                    {
                        //找到最大渲染层级的窗口，拿到它
                        if (maxOrder < PanelBase.canvas.sortingOrder)
                        {
                            MaxOrderPanel = PanelBase;
                            maxOrder = PanelBase.canvas.sortingOrder;
                        }
                        //如果两个窗口的渲染层级相同，就找到同节点下最靠下一个物体，优先渲染Mask
                        else if (maxOrder == PanelBase.canvas.sortingOrder && maxIndex < PanelBase.transform.GetSiblingIndex())
                        {
                            MaxOrderPanel = PanelBase;
                            maxIndex = PanelBase.transform.GetSiblingIndex();
                        }
                    }
                }
            }
            if (MaxOrderPanel != null)
            {
                MaxOrderPanel.SetMaskVisible(true);
            }
        }

        /// <summary>
        /// 实例化界面
        /// </summary>
        /// <param name="wndName"></param>
        /// <returns></returns>
        private GameObject TempLoadPanel(string wndName)
        {
            //自己搞时要改成AB或者Adreesable
            GameObject Panel = GameObject.Instantiate<GameObject>(ABManager.GetInstance().LoadRes<GameObject>(aBNameConfig.GetABName(wndName), wndName), mUIRoot);
            //Panel.transform.SetParent(mUIRoot);
            Panel.transform.localScale = Vector3.one;
            Panel.transform.localPosition = Vector3.zero;
            Panel.transform.rotation = Quaternion.identity;
            Panel.name = wndName;
            return Panel;
        }

        /// <summary>
        /// 进栈一个界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="popCallBack"></param>
        public void PushPanelToStack<T>(E_UI_Layer e_UI_Layer,Action<BasePanel> popCallBack = null) where T : BasePanel, new()
        {
            BasePanel wndBase = null;
            try
            {
                wndBase = mAllPanelDic[typeof(T).Name];
            }
            catch (Exception e)
            {
                wndBase = new T();
                wndBase._Layer = e_UI_Layer;
            }
            //T wndBase = new T();
            wndBase.PopStackListener = popCallBack;
            mPanelStack.Enqueue(wndBase);
        }
        /// <summary>
        /// 弹出堆栈中第一个弹窗
        /// </summary>
        public void StartPopFirstStackPanel()
        {
            if (mStartPopStackWndStatus == true) return;//进入弹栈流程了不会在去显示Panel 而是只是压栈
            mStartPopStackWndStatus = true;//可以显示Panel
            PopStackPanel();
        }

        /// <summary>
        /// 弹出堆栈中的下一个窗口
        /// </summary>
        /// <param name="PanelBase"></param>
        private void PopNextStackPanel(BasePanel PanelBase)
        {
            if (PanelBase != null && mStartPopStackWndStatus && PanelBase.PopStack)
            {
                PanelBase.PopStack = false;
                PopStackPanel();
            }
        }
        /// <summary>
        /// 压入并且弹出堆栈弹窗
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="popCallBack"></param>
        public void PushAndPopStackPanel<T>(E_UI_Layer e_UI_Layer,Action<BasePanel> popCallBack = null) where T : BasePanel, new()
        {
            PushPanelToStack<T>(e_UI_Layer,popCallBack);
            StartPopFirstStackPanel();
        }

        /// <summary>
        /// 弹出堆栈弹窗
        /// </summary>
        /// <returns></returns>
        private bool PopStackPanel()
        {
            if (mPanelStack.Count > 0)
            {
                BasePanel Panel = mPanelStack.Dequeue();
                BasePanel popPanel = PopUpPanel(Panel,Panel._Layer);
                popPanel.PopStackListener = Panel.PopStackListener;
                popPanel.PopStack = true;
                popPanel.PopStackListener?.Invoke(popPanel);
                popPanel.PopStackListener = null;
                return true;
            }
            else
            {
                mStartPopStackWndStatus = false;
                return false;
            }
        }
        public void ClearStackPanels()
        {
            mPanelStack.Clear();
        }
    }
}
