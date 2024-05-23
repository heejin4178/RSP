using System.Drawing;
using Utils;

public class UI_GameResultPopup : UI_Base
{
    enum Texts
    {
        ResultStageValueText,
        ResultKillValueText,
        ResultDeathValueText,
        ConfirmButtonText,
    }

    enum Buttons
    {
        StatisticsButton,
        ConfirmButton,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        
        GetButton((int)Buttons.ConfirmButton).gameObject.BindEvent(OnClickConfirmButton);

        SetInfo();
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
        
        GetText((int)Texts.ResultStageValueText).text = SetWinnerText(Managers.Game.Winner);
        GetText((int)Texts.ResultStageValueText).color = SetWinnerColor(Managers.Game.Winner);
        GetText((int)Texts.ResultKillValueText).text = Managers.Game.KillCount.ToString();
        GetText((int)Texts.ResultDeathValueText).text = Managers.Game.DeathCount.ToString();
        GetText((int)Texts.ConfirmButtonText).text = "OK";
    }

    public string SetWinnerText(int winner)
    {
        switch (winner)
        {
            case 0:
                return "Draw!";
            case 1:
                return "Rock Win!";
            case 2:
                return "Scissors Win!";
            case 3:
                return "Paper Win!";
        }

        return null;
    }
    
    public UnityEngine.Color SetWinnerColor(int winner)
    {
        switch (winner)
        {
            case 0:
                return UnityEngine.Color.green;
            case 1:
                return UnityEngine.Color.black;
            case 2:
                return UnityEngine.Color.yellow;
            case 3:
                return UnityEngine.Color.white;
        }

        return UnityEngine.Color.green;
    }

    void OnClickConfirmButton()
    {
        // 킬&데스 카운트 초기화
        Managers.Game.KillCount = 0;
        Managers.Game.DeathCount = 0;
        Managers.Game.Winner = 0;
        Managers.Game.NickName = null;

        // 게임 결과 팝업 닫기
        Managers.UI.ClosePopup();
        
        // 로그인 씬으로 이동
        Managers.Scene.LoadScene(Define.Scene.Login);
	}
}
