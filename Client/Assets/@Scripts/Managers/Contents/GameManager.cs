using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager
{
    #region 공통
    public string NickName { get; set; }
    public int Winner { get; set; }

    
    private int _hp = 0;
    public event Action<int> OnHpChanged;
    public int Hp
    {
        get => _hp;
        set
        {
            _hp = value;
            OnHpChanged?.Invoke(value);
        }
    }
    #endregion

    #region 전투
    public int KillCount { get; set; }
    
    public int DeathCount { get; set; }
    
    private int _playerCount;
    public event Action<int> OnPlayerCountChanged;

    public int PlayerCount
    {
        get => _playerCount;
        set
        {
            _playerCount = value;
            OnPlayerCountChanged?.Invoke(value);
        }
    }

    private bool _attackKeyPressed = false;
    public event Action onAttackPointerUp;
    
    public bool AttackKeyPressed
    {
        get => _attackKeyPressed;
        set
        {
            _attackKeyPressed = value;
            onAttackPointerUp.Invoke();
        }
    }
    public SkillType SkillType { get; set; }

    private float _coolTimeValue;
    public event Action<float> OnCoolTimeValueChanged; 
    public float CoolTimeValue
    {
        get => _coolTimeValue;
        set
        {
            _coolTimeValue = value;
            OnCoolTimeValueChanged.Invoke(value);
        }
    }

    #endregion
    
    #region 이동
    private Vector2 _moveDir;
    public event Action<Vector2> onMoveDirChanged; 
    public Vector2 MoveDir
    {
        get => _moveDir;
        set
        {
            _moveDir = value;
            onMoveDirChanged.Invoke(_moveDir);
        }
    }
    
    private bool _moveKeyPressed = false;
    public event Action onMovePointerUp;
    
    public bool MoveKeyPressed
    {
        get => _moveKeyPressed;
        set
        {
            _moveKeyPressed = value;
            onMovePointerUp.Invoke();
        }
    }
    
    #endregion

}
