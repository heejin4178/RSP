using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager
{
    #region 재화
    public int Gold { get; set; }

    private int _gem = 0;
    public event Action<int> OnGemCountChanged;
    public int Gem
    {
        get => _gem;
        set
        {
            _gem = value;
            OnGemCountChanged?.Invoke(value);
        }
    }
    #endregion

    #region 전투
    private int _killCount;
    public event Action<int> OnKillCountChanged;

    public int KillCount
    {
        get => _killCount;
        set
        {
            _killCount = value;
            OnKillCountChanged?.Invoke(value);
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
    public event Action onPointerUp;
    
    public bool MoveKeyPressed
    {
        get => _moveKeyPressed;
        set
        {
            _moveKeyPressed = value;
            onPointerUp.Invoke();
        }
    }
    
    #endregion

}
