using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MyGameFrameWork
{
    public enum EventCenterEventType
    {
       RedDotChangeUI
    }
    public interface IEventCallBack
    {
        bool ContainsFunction(UnityAction<EventArgs> myEvent, UnityAction<EventArgs> function);
    }

    public class EventArgs
    {
        public EventCenterEventType eventType;

        public object[] ArgsArray { get; set; }
        public object Args
        {
            get
            {
                return ArgsArray[0];
            }
        }

        public object Args1
        {
            get
            {
                return ArgsArray[1];
            }
        }
        public object Args2
        {
            get
            {
                return ArgsArray[2];
            }
        }
        public EventArgs(EventCenterEventType et, params object[] dataParam)
        {
            eventType = et;
            ArgsArray = dataParam;
        }
        public EventArgs(EventCenterEventType et) { eventType = et; }

    }

    public class EventInfo : IEventCallBack
    {
        public UnityAction<EventArgs> actions;

        public EventInfo(UnityAction<EventArgs> action)
        {
            if (!ContainsFunction(actions, action))
            {
                actions += action;
            }
        }
        public bool ContainsFunction(UnityAction<EventArgs> myEvent, UnityAction<EventArgs> function)
        {
            if (myEvent != null && function != null)
            {
                foreach (UnityAction<EventArgs> handler in myEvent.GetInvocationList())
                {
                    if (handler == function)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
    public class EventCenter : Singleton<EventCenter>
    {
        //字典事件
        Dictionary<EventCenterEventType, IEventCallBack> mDictEvent = new Dictionary<EventCenterEventType, IEventCallBack>();

        /// <summary>
        /// 添加事件监听
        /// </summary>
        public void AddEventListener(EventCenterEventType eventType, UnityAction<EventArgs> callBack)
        {
            if (mDictEvent.ContainsKey(eventType))
            {
                //如果没有相同的函数引用 则+=
                if (!(mDictEvent[eventType] as EventInfo).ContainsFunction((mDictEvent[eventType] as EventInfo).actions, callBack))
                {
                    (mDictEvent[eventType] as EventInfo).actions += callBack;
                }
            }
            else
            {
                mDictEvent.Add(eventType, new EventInfo(callBack));
            }
        }
        /// <summary>
        /// 删除事件监听泛型
        /// </summary>
        public void RemoveEventListener(EventCenterEventType eventType, UnityAction<EventArgs> callBack)
        {
            if (!mDictEvent.ContainsKey(eventType))
            {
                return;
            }
            if (mDictEvent.ContainsKey(eventType))
            {
                if ((mDictEvent[eventType] as EventInfo).ContainsFunction((mDictEvent[eventType] as EventInfo).actions, callBack))
                {
                    (mDictEvent[eventType] as EventInfo).actions -= callBack;
                }
                // 如果回调都被移除了 那么key也从字典移除
                if ((mDictEvent[eventType] as EventInfo).actions.GetInvocationList().Length == 0)
                {
                    mDictEvent.Remove(eventType);
                }
            }
        }

        /// <summary>
        /// 分发事件
        /// </summary>
        /// <param name="et"></param>
        /// <param name="dataParam"></param>
        public void EventTrigger(EventCenterEventType et, params object[] dataParam)
        {
            NotifyEvent(new EventArgs(et, dataParam));
        }

        /// <summary>
        /// 相应事件
        /// </summary>
        /// <param name="et"></param>
        void NotifyEvent(EventArgs et)
        {
            if (et.eventType >= 0)
            {
                if (mDictEvent.ContainsKey(et.eventType))
                {
                    if ((mDictEvent[et.eventType] as EventInfo).actions != null)
                    {
                        (mDictEvent[et.eventType] as EventInfo).actions?.Invoke(et);
                    }
                }
            }
        }

        /// <summary>
        /// 清除某个类型的事件所有引用函数
        /// </summary>
        /// <param name="eventType"></param>
        public void ClearEvent(EventCenterEventType eventType)
        {
            if (mDictEvent.ContainsKey(eventType))
            {
                (mDictEvent[eventType] as EventInfo).actions = null;
            }
        }
        /// <summary>
        /// 清空事件中心
        /// 主要用在 场景切换时
        /// </summary>
        public void Clear()
        {
            mDictEvent.Clear();
        }
    }
}
