using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class DataBase 
    {
        public Dictionary<string, object> _dataContext = new Dictionary<string, object>();


        public void Setdata(string key, object value)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext[key] = value;
            }
            else
            {
                _dataContext.Add(key, value);
            }
        }

        public object GetData(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                return _dataContext[key];
            }
            Debug.LogWarning("没有此数据，无法获取");
            return null;
        }
        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);

                return true;
            }
            Debug.LogWarning("没有此数据，无法清除");
            return false;
        }
    }
}

