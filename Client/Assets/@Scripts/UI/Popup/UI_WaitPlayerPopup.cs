using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
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
        // 게임 룸 접속 종료 패킷 보내기
        C_LeaveGame leave = new C_LeaveGame();
        leave.ObjectId = Managers.Object.MyPlayer.Id;
        Managers.Network.Send(leave);
        
        // 기존 변수 초기화
        Managers.Game.PlayerCount = 0;
        Managers.Object.Clear();
        
        // 웨이팅 화면 닫기
        Managers.UI.CloseSceneUI();
        
        // 로그인 씬으로 이동
        Managers.Scene.LoadScene(Define.Scene.Login);
    }


    private int _playerCountText;

    public void SetPlayerCount(int playerCount)
    {
        _playerCountText = playerCount;
        SetInfo();
    }
}
