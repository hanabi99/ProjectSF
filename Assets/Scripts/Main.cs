using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGameFrameWork;
public class Main : MonoBehaviour
{
   
    void Start()
    {
        UIManager.GetInstance().Initialize();
        ABManager.GetInstance().LoadMainfest();
        UIManager.GetInstance().PopUpPanel<LoginWIndow>(E_UI_Layer.BottomPanelLayer);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UIManager.GetInstance().PopUpPanel<ChatWIndow>(E_UI_Layer.levelOneLayer);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UIManager.GetInstance().PopUpPanel<UserInfoWIndow>(E_UI_Layer.levelTwoLayer);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UIManager.GetInstance().PopUpPanel<SettingWIndow>(E_UI_Layer.levelTwoLayer);
        }

    }
}
