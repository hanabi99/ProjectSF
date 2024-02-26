using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGameFrameWork;
using UnityEngine.UI;

public class LoginWIndow : BasePanel
{
    private Button Login;
    public override void Init()
    {
        base.Init();
        Login = GetControl<Button>("[Button]Login");
        AddButtonClickListener(Login, () => { UIManager.GetInstance().PopUpPanel<HallWindow>(E_UI_Layer.BottomPanelLayer); UIManager.GetInstance().HidePanel(this); });
    }
    public override void ShowMe()
    {
        base.ShowMe();
        Debug.Log("ShowLoginWIndow");
    }
}
