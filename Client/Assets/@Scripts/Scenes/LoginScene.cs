using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class LoginScene : BaseScene
{
    private Button PlayButton;
    
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

        
        // Canvas를 찾음
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();

        // Canvas 하위에서 Button 컴포넌트를 가진 게임 오브젝트를 찾음
        PlayButton = canvas.GetComponentInChildren<Button>();
        PlayButton.onClick.AddListener(OnClickPlayBtn);
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Q))
    //     {
    //         Managers.Scene.LoadScene(Define.Scene.Game);
    //     }
    // }

    public override void Clear()
    {
        Debug.Log($"Login Scene Clear!!");
    }

    private void OnClickPlayBtn()
    {
        Managers.Scene.LoadScene(Define.Scene.Game);
    }
}
