using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public int Id { get; set; }

    private StatInfo _stat = new StatInfo { Speed = 8.0f };

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
    
    protected bool _updated = false;

    public Vector3 CellPos
    {
        get
        {
            return new Vector3(PosInfo.PosX, 0, PosInfo.PosZ);
        }
        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosZ == value.z && State == CreatureState.Moving)
            {
                State = CreatureState.Idle;
                return;
            }
            
            PosInfo.PosX = value.x;
            PosInfo.PosZ = value.z;
            _updated = true;
        }
    }
    
    public CreatureState State
    {
        get => PosInfo.State;
        set
        {
            if (PosInfo.State == value)
                return;

            PosInfo.State = value;
            _updated = true;
        }
    }
    
    public float Rotation
    {
        get => PosInfo.Rotation;
        set
        {
            if (PosInfo.Rotation == value)
                return;

            PosInfo.Rotation = value;
            _updated = true;
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
            
            CellPos = new Vector3(value.PosX, 0, value.PosZ);
            State = value.State;
            Rotation = value.Rotation;
        }
    }
    
    public void SyncPos()
    {
        transform.position = CellPos;
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
