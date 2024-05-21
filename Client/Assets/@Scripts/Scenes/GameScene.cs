using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using Utils;

public class GameScene : BaseScene
{
    void Awake() // 하이어라키에서 비활성화 되더라고 Init이 실행되도록 함.
    {
        Init();
    }
    
    protected override void Init()
    {
        SceneType = Define.Scene.Game;
        Screen.SetResolution(1024, 768, false);
        StartLoaded();
        
        // 재접속 패킷을 보냄
        if (Managers.Network.Init() == false)
        {
            C_EnterGame enterPacket = new C_EnterGame();
            enterPacket.Name = Managers.Game.NickName; // 닉네임 패킷에 포함
            Managers.Network.Send(enterPacket);
        }
    }
    
    void StartLoaded()
    {
        Managers.UI.ShowSceneUI<UI_WaitPlayerPopup>();

        var map = Managers.Resource.Instantiate("Map.prefab");
        map.name = "@Map";
        
        Managers.Game.OnPlayerCountChanged -= HandleOnPlayerCountChanged;
        Managers.Game.OnPlayerCountChanged += HandleOnPlayerCountChanged;
    }
    
    public void HandleOnPlayerCountChanged(int playerCount)
    {
        Debug.Log($"playerCount : {playerCount}");
        if (Managers.UI.GetSceneUI<UI_WaitPlayerPopup>() != null)
            Managers.UI.GetSceneUI<UI_WaitPlayerPopup>().SetPlayerCount(playerCount);
    }
    
    public override void Clear()
    {
        
    }
    
    private void OnDestroy()
    {
        if (Managers.Game != null)
        {
            Managers.Game.OnPlayerCountChanged -= HandleOnPlayerCountChanged;
        }
    }
}
