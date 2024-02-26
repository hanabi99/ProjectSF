using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BT
{
    /// <summary>
    /// 动作节点 (执行具体逻辑)此动作节点 是将逻辑推给 外部执行
    /// </summary>
    public class BTActionNode : BTBaseNode
    {
        public Func<E_NodeState> action;

        public E_NodeState nodeState;
        public BTActionNode(Func<E_NodeState> action)
        {
            this.action = action;
        }

        public override E_NodeState Excute()
        {
            if (action == null)
            {
                nodeState = E_NodeState.Failed;
                return nodeState;
            }

            switch (action.Invoke())
            {
                case E_NodeState.Failed:
                    nodeState = E_NodeState.Failed;
                    return E_NodeState.Failed;
                case E_NodeState.Running:
                    nodeState = E_NodeState.Running;
                    return E_NodeState.Running;
            }
            nodeState = E_NodeState.Success;
            return nodeState;

        }
    }
}
