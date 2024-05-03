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
        
        if (Managers.Network.Init() == false)
        {
            C_EnterGame enter = new C_EnterGame();
            Managers.Network.Send(enter);
        }
            
        // 여기서 재접속 패킷을 보냄

        // GameObject player = Managers.Resource.Instantiate("Creature/Player");
        // player.name = "Player";
        // Managers.Object.Add(player);
        //
        // for (int i = 0; i < 5; i++)
        // {
        //     GameObject monster = Managers.Resource.Instantiate("Creature/Monster");
        //     player.name = $"Monster_{i + 1}";
        //     
        //     // 랜덤 위치 스폰
        //     Vector3Int pos = new Vector3Int()
        //     {
        //         x = Random.Range(-8, 8),
        //         y = Random.Range(-5, 5)
        //     };
        //
        //     MonsterController mc = monster.GetComponent<MonsterController>();
        //     mc.CellPos = pos;
        //
        //     Managers.Object.Add(monster);
        // }
    }
    
    void StartLoaded()
    {
        Managers.UI.ShowSceneUI<UI_WaitPlayerPopup>();
        
        var moveJoystick = Managers.Resource.Instantiate("UI_MoveJoystick.prefab");
        moveJoystick.name = "@UI_MoveJoystick";
        
        var attackJoystick = Managers.Resource.Instantiate("UI_AttackJoystick.prefab");
        attackJoystick.name = "@UI_AttackJoystick";
        
        var map = Managers.Resource.Instantiate("Map.prefab");
        map.name = "@Map";
        
        // foreach (var playerData in Managers.Data.PlayerDic.Values)
        // {
        //     Debug.Log($"Lvl : {playerData.level}, Hp{playerData.maxHp}");
        // }
        
        Managers.Game.OnPlayerCountChanged -= HandleOnPlayerCountChanged;
        Managers.Game.OnPlayerCountChanged += HandleOnPlayerCountChanged;
    }
    
    public void HandleOnPlayerCountChanged(int playerCount)
    {
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
