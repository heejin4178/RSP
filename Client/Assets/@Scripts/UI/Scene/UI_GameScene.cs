using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UI_GameScene : UI_Base
{
    // 여기에 필요한 UI
    // 3분 타이머
    // 현재 각 종족별 인원 수
    // 나의 킬수
    

    private TextMeshProUGUI _gameTimerDisplay;
    private TextMeshProUGUI _countDownDisplay;
    
    enum GameObjects
    {
        UI_MoveJoystick,
        UI_AttackJoystick
    }

    enum Texts
    {
        GameTimerText,
        CountDownText,
    }

    enum Buttons
    {
        CancelButton,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        
        // BindButton(typeof(Buttons));
        // GetButton((int)Buttons.CancelButton).gameObject.BindEvent(OnClickCancelButton);
        
        GetObject((int)GameObjects.UI_MoveJoystick).gameObject.SetActive(false);
        GetObject((int)GameObjects.UI_AttackJoystick).gameObject.SetActive(false);

        // 게임 시작전 3초 카운트다운
        CountDown();
        
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
    }

    
    private int _countDownTime = 3;
    
    private float time;
    private float curTime;
    int minute;
    int second;
    
    // public void SetCountDownTime(int time)
    // {
    //     _countDownTime = time;
    //     SetInfo();
    // }

    public void CountDown()
    {
        StartCoroutine("CountDownStart");
    }

    IEnumerator CountDownStart()
    {
        while (_countDownTime > 0)
        {
            GetText((int)Texts.CountDownText).text = $"{_countDownTime}";
            yield return new WaitForSeconds(1f);

            _countDownTime--;
        }

        GetText((int)Texts.CountDownText).text = "Game Start!";
        yield return new WaitForSeconds(1f);
        GetText((int)Texts.CountDownText).gameObject.SetActive(false);
    }
    
    public void StartGameTimer()
    {
        GetObject((int)GameObjects.UI_MoveJoystick).gameObject.SetActive(true);
        GetObject((int)GameObjects.UI_AttackJoystick).gameObject.SetActive(true);
        time = 60;
        StartCoroutine("StartTimer");
    }
    
    IEnumerator StartTimer()
    {
        curTime = time;
        while(curTime > 0)
        {
            curTime -= Time.deltaTime;
            minute = (int)curTime / 60;
            second = (int)curTime % 60;
            GetText((int)Texts.GameTimerText).text = minute.ToString("00") + ":" + second.ToString("00");
            yield return null;

            if(curTime <= 0)
            {
                Debug.Log("시간 종료");
                Time.timeScale = 0; // 클라 타이머가 종료되면 자체적으로 게임을 멈춤.
                curTime = 0;
                yield break;
            }
        }
    }
}
