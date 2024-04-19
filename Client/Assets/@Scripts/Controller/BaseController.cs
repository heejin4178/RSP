using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public int Id { get; set; }

    private StatInfo _stat = new StatInfo();
    
    public virtual StatInfo Stat
    {
        get { return _stat; }
        set
        {
            if (_stat.Equals(value))
                return;

            _stat.Hp = value.Hp;
            _stat.MaxHp = value.MaxHp;
            _stat.Speed = value.Speed;
        }
    }
    
    public float Speed
    {
        get { return _stat.Speed; }
        set { Stat.Speed = value; }
    }
    
    public virtual int Hp
    {
        get { return _stat.Hp; }
        set
        {
            Stat.Hp = value;
        }
    }
    
    private void Start()
    {
        Init();
    }

    private bool _init = false;
    protected virtual bool Init()
    {
        if (_init)
            return false;

        _init = true;
        return true;
    }
    
    void Update()
    {
        UpdateController();
    }

    protected virtual void UpdateController()
    {
        
    }
}
