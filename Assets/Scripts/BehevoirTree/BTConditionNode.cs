using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BT
{
    /// <summary>
    /// 条件节点 最终去进行一个条件判断,他与动作节点的区别就是 动作节点可能没有返回值
    /// </summary>
    public class BTConditionNode : BTBaseNode
    {

        public Func<bool> action;

        public BTConditionNode(Func<bool> action)
        {
            this.action = action;
        }

        public override E_NodeState Excute()
        {
            if (action == null)
            {
                return E_NodeState.Failed;
            }

            return action.Invoke() ? E_NodeState.Success : E_NodeState.Failed;
        }

    }
}
