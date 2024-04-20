using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public int Id { get; set; }

    private StatInfo _stat = new StatInfo { Speed = 30.0f };

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
    public Vector3 CellPos
    {
        get
        {
            return new Vector3(PosInfo.PosX, 0, PosInfo.PosZ);
        }
        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosZ == value.z)
                return;
            
            PosInfo.PosX = value.x;
            PosInfo.PosZ = value.z;
        }
    }
    
    public virtual CreatureState State
    {
        get => PosInfo.State;
        set
        {
            if (PosInfo.State == value)
                return;

            PosInfo.State = value;
        }
    }
    
    private PositionInfo _PositionInfo = new PositionInfo();

    public PositionInfo PosInfo
    {
        get { return _PositionInfo; }
        set
        {
            if (_PositionInfo.Equals(value))
                return;
            
            State = value.State;
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

    public virtual void UpdateController()
    {
        
    }
}
