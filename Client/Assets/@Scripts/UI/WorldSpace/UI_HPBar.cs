using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_HPBar : UI_Base
{
    private int _hp = 3;
    enum Images
    {
        Cell1,
        Cell2,
        Cell3,
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        SetInfo();

        Managers.Game.OnHpChanged -= SetHp;
        Managers.Game.OnHpChanged += SetHp;
        
        return true;
    }

    private void OnDestroy()
    {
        if (Managers.Game != null)
            Managers.Game.OnHpChanged -= SetHp;
    }

    public void SetInfo()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        switch (_hp)
        {
            case 0:
                GetImage((int)Images.Cell1).color = Color.gray;
                break;
            case 1:
                GetImage((int)Images.Cell2).color = Color.gray;
                break;
            case 2:
                GetImage((int)Images.Cell3).color = Color.gray;
                break;
            case 3:
                GetImage((int)Images.Cell1).color = Color.green;
                GetImage((int)Images.Cell2).color = Color.green;
                GetImage((int)Images.Cell3).color = Color.green;
                break;
        }
    }

    private void SetHp(int hp)
    {
        _hp = hp;
        SetInfo();
    }
    
    private void Update()
    {
        Transform parent = transform.parent;
        transform.position = parent.position + Vector3.up * 3.5f;
        transform.rotation = Camera.main.transform.rotation;
    }
}
