using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BT
{
    /// <summary>
    /// 反向修饰节点
    /// </summary>
    public class BTDecoratorNot : BTBaseNode
    {
        public Func<bool> action;

        public BTDecoratorNot(Func<bool> action)
        {
            this.action = action;
        }

        public override E_NodeState Excute()
        {
            if (action == null)
            {
                return E_NodeState.Failed;
            }

            return action.Invoke() ? E_NodeState.Failed : E_NodeState.Success;
        }
    }
}
