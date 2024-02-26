using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        BinaryDataMgr.Instance.InitData();
        PlayerInfoContainer playerInfoContainer =  BinaryDataMgr.Instance.GetTable<PlayerInfoContainer>();
        print(playerInfoContainer.dataDic[1003].name);
    }
}
