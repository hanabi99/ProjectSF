using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGameFrameWork
{
    public abstract class CommandBase : MonoBehaviour
    {
        /// <summary>
        /// Ö´ÐÐÃüÁî
        /// </summary>
        public abstract void Excute();


        /// <summary>
        /// ³·ÏúÃüÁî
        /// </summary>
        public abstract void Undo();
    }
}
