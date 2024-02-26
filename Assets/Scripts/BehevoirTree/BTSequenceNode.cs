using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    /// <summary>
    /// 顺序节点 按顺序执行他的子节点，直到一个子节点失败 
    /// </summary>
    public class BTSequenceNode : BTBaseNode
    {
        public BTSequenceNode() : base() { }
        public override E_NodeState Excute()
        {
            BTBaseNode childNode;
            if (childList.Count != 0)
            {
                childNode = childList[NowIndex];
                switch (childNode.Excute())
                {
                    case E_NodeState.Success:
                        ++NowIndex;
                        if (NowIndex == childList.Count)
                        {
                            NowIndex = 0;
                            return E_NodeState.Success;
                        }
                        break;
                    case E_NodeState.Failed:
                        NowIndex = 0;
                        return E_NodeState.Failed;
                    case E_NodeState.Running:
                        return E_NodeState.Running;
                    default:
                        break;
                }
            }
            return E_NodeState.Failed;
        }
    }
}
