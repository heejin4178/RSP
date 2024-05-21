using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class LoginScene : BaseScene
{
    private Button PlayButton;
    private TMP_InputField InputNickNameField;
    
    void Awake() // 하이어라키에서 비활성화 되더라고 Init이 실행되도록 함.
    {
        Init();
    }
    
    protected override void Init()
    {
        SceneType = Define.Scene.Login;

        // Canvas를 찾음
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();

        // Canvas 하위에서 Button,InputField 컴포넌트를 가진 게임 오브젝트를 찾음
        PlayButton = canvas.GetComponentInChildren<Button>();
        InputNickNameField = canvas.GetComponentInChildren<TMP_InputField>();
        
        PlayButton.onClick.AddListener(OnClickPlayBtn);
        InputNickNameField.onValueChanged.AddListener(OnNickNameValueChanged);

        // 최초 접속시에만 어드레서블 로드함.
        if (Managers.Network.CheckSessionConnected() == false)
        {
            Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
            {
                Debug.Log($"{key} {count}/{totalCount}");
            });
        }
    }

    public override void Clear()
    {
        Debug.Log($"Login Scene Clear!!");
    }
    
    private void OnNickNameValueChanged(string arg)
    {
        Managers.Game.NickName = arg;
    }

    private void OnClickPlayBtn()
    {
        if (string.IsNullOrEmpty(Managers.Game.NickName))
        {
            Managers.UI.ShowPopup<UI_ToastMsgPopup>(); // 토스트메세지 팝업 활성화
        }
        else
        {
            Managers.Game.NickName = InputNickNameField.text; // 닉네임 설정
            Managers.Scene.LoadScene(Define.Scene.Game);
        }
    }
}
