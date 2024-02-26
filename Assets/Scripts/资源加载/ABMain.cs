using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGameFrameWork;
using System;

public class ABMain : MonoBehaviour
{ 
    void Start()
    {
        ABManager.GetInstance().LoadMainfest();
        ABManager.GetInstance().LoadRes<GameObject>("model", "Cube");
        ABManager.GetInstance().LoadAB("psb");
        ABManager.GetInstance().LoadRes<GameObject>("model", "Sphere");
        ABManager.GetInstance().LoadAB("m");
        ABManager.GetInstance().DebugRefCount();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ABManager.GetInstance().UnLoad("m");
            ABManager.GetInstance().DebugRefCount();
        }
    }
}
