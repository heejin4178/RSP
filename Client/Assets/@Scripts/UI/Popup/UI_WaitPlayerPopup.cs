using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

public class UI_WaitPlayerPopup : UI_Base
{
    enum GameObjects
    {
        ContentObject,
        ResultRewardScrollContentObject,
        ResultGoldObject,
        ResultKillObject,
    }

    enum Texts
    {
        PlayerCountText,
    }

    enum Buttons
    {
        CancelButton,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CancelButton).gameObject.BindEvent(OnClickCancelButton);

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

        // 정보 취합
        GetText((int)Texts.PlayerCountText).text = $"{_playerCountText}/12";
    }

    void OnClickCancelButton()
    {
        Managers.Network.Disconnect();
        Managers.Game.PlayerCount = 0;
        Managers.UI.CloseSceneUI();
        Managers.Scene.LoadScene(Define.Scene.Login);
    }


    private int _playerCountText;

    public void SetPlayerCount(int playerCount)
    {
        _playerCountText = playerCount;
        SetInfo();
    }
}
