using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_NickNameText : UI_Base
{
    public string NickName { get; set; }
    enum Texts
    {
        NickName,
    }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindText(typeof(Texts));
        SetInfo();
        
        return true;
    }
    
    public void SetInfo()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        GetText((int)Texts.NickName).text = NickName;
    }
    
    private void Update()
    {
        Transform parent = transform.parent;
        transform.position = parent.position + Vector3.up * 4.5f;
        transform.rotation = Camera.main.transform.rotation;
    }
}
