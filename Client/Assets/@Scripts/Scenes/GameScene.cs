using System.Collections;
using System.Collections.Generic;
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
        base.Init();
        
        Managers.Network.Init();
        
        SceneType = Define.Scene.Game;

        StartLoaded();
        
        // Screen.SetResolution(1024, 768, false);

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
        // var player = Managers.Object.Spawn<PlayerController>(Vector3.zero);
        
        var joystick = Managers.Resource.Instantiate("UI_Joystick.prefab");
        joystick.name = "@UI_Joystick";
        
        var map = Managers.Resource.Instantiate("Map.prefab");
        map.name = "@Map";

        // Camera.main.GetComponent<CameraController>().Target = player.gameObject;
        
        // foreach (var playerData in Managers.Data.PlayerDic.Values)
        // {
        //     Debug.Log($"Lvl : {playerData.level}, Hp{playerData.maxHp}");
        // }

        // Managers.Game.OnGemCountChanged -= HandelOnGemCountChanged;
        // Managers.Game.OnGemCountChanged += HandelOnGemCountChanged;
        // Managers.Game.OnKillCountChanged -= HandleOnKillCountChanged;
        // Managers.Game.OnKillCountChanged += HandleOnKillCountChanged;
    }
    
    public override void Clear()
    {
        
    }
}
