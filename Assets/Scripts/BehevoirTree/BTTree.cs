using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public abstract class BTTree : MonoBehaviour
    {
        protected BTBaseNode _root;

        protected DataBase _database = new DataBase();

        public virtual void Start()
        {
            _root = SetUpTree();        
        }

        private void Update()
        {
            if(_root != null)
            {
                _root.Excute();
            }
        }

        public abstract BTBaseNode SetUpTree();

    }
}

