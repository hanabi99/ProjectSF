using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BT
{
    /// <summary>
    /// 行为节点基类
    /// </summary>
    public abstract class BTBaseNode
    {
        protected List<BTBaseNode> childList = new List<BTBaseNode>();

        public BTBaseNode parent;

        //当前执行逻辑的子节点序号
        protected int NowIndex = 0;


        /// <summary>
        /// 添加子节点方法
        /// </summary>
        /// <param name="nodes"></param>
        public virtual void AddChild(params BTBaseNode[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].parent = this;
                childList.Add(nodes[i]);
            }
        }

        public virtual void RemoveChild(int index)
        {

        }

        /// <summary>
        /// 执行节点逻辑 (Evaluate)
        /// </summary>
        /// <returns></returns>
        public abstract E_NodeState Excute();

     
    }
}
    
