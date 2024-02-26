using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGameFrameWork;
using UnityEngine.UI;

public class ChatWIndow : BasePanel
{
    private Button close;

    public override void Init()
    {
        base.Init();
        close = GetControl<Button>("[Button]Close");
        AddButtonClickListener(close, () => { UIManager.GetInstance().HidePanel(this); });
    }
}
