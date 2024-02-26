using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGameFrameWork;
using UnityEngine.UI;

public class HallWindow : BasePanel
{
    public override void ShowMe()
    {
        base.ShowMe();
        UIManager.GetInstance().PushPanelToStack<UserInfoWIndow>(E_UI_Layer.levelTwoLayer);
        UIManager.GetInstance().PushPanelToStack<SettingWIndow>(E_UI_Layer.levelTwoLayer);
        UIManager.GetInstance().PushPanelToStack<ChatWIndow>(E_UI_Layer.levelOneLayer);

        UIManager.GetInstance().StartPopFirstStackPanel();
    }
}
