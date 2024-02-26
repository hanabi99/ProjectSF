using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT {
    public class BTParallelSelectorNode : BTBaseNode
    {
        public override E_NodeState Excute()
        {
            List<E_NodeState> states = new List<E_NodeState>();

            for (int i = 0; i < childList.Count; i++)
            {
               E_NodeState state =  childList[i].Excute();
                switch (state)
                {
                    case E_NodeState.Success:
                        states.Add(state);
                        break;
                    case E_NodeState.Failed:
                        return E_NodeState.Failed;
                    case E_NodeState.Running:
                        states.Add(state);
                        break;
                }          
            }
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i] == E_NodeState.Running)
                {           
                    return E_NodeState.Running;
                }
            }

            return E_NodeState.Success;
        }
    }
}
