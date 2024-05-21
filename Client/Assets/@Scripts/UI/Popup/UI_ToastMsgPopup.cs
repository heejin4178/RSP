using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class UI_ToastMsgPopup : UI_Base
{
    enum GameObjects
    {
        ContentObject
    }

    enum Texts
    {
        ToastMsgText,
    }

    enum Buttons
    {
        ConfirmButton,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        
        GetButton((int)Buttons.ConfirmButton).gameObject.BindEvent(OnClickConfirmButton);

        RefreshUI();
        return true;
    }

    public void SetInfo()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        if (_init == false)
            return;
        
        GetText((int)Texts.ToastMsgText).text = "Enter Your NickName!";
    }

    void OnClickConfirmButton()
    {
        Debug.Log("OnClickConfirmButton");
        
        // 게임 결과 팝업 닫기
        Managers.UI.ClosePopup();
    }
}
