using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class LoginScene : BaseScene
{
    void Awake() // 하이어라키에서 비활성화 되더라고 Init이 실행되도록 함.
    {
        Init();
    }
    
    protected override void Init()
    {
        SceneType = Define.Scene.Login;
        
        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
        {
            Debug.Log($"{key} {count}/{totalCount}");
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Managers.Scene.LoadScene(Define.Scene.Game);
        }
    }

    public override void Clear()
    {
        Debug.Log($"Login Scene Clear!!");
    }
}
