using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class CreatureController : BaseController
{
    Vector2 _moveDir = Vector2.zero;
    public Vector2 MoveDir
    {
        get => _moveDir;
        set { _moveDir = value.normalized; }
    }

    protected override bool Init()
    {
        base.Init();

        return true;
    }
    
    #region State Pattern
    private CreatureState _creatureState = CreatureState.Idle;
    public virtual CreatureState CreatureState
    {
        get => _creatureState;
        set
        {
            _creatureState = value;
            UpdateAnimation();
        }
    }

    protected Animator _animator;
    public virtual void UpdateAnimation()
    {
        
    }

    public override void UpdateController()
    {
        base.UpdateController();

        switch (CreatureState)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    protected virtual void UpdateIdle()
    {
        
    }
    protected virtual void UpdateSkill()
    {
        
    }
    protected virtual void UpdateMoving()
    {
        // Vector3 moveDir = CellPos - transform.position;
        //
        // // 도착 여부 체크
        // float dist = moveDir.magnitude;
        // if (dist < Speed * Time.deltaTime)
        // {
        //     transform.position = CellPos;
        // }
        // else
        // {
        //     transform.position += moveDir.normalized * Speed * Time.deltaTime;
        //     State = CreatureState.Moving;
        // }
        
        
        
        Vector3 dir = MoveDir * Speed * Time.deltaTime;
        transform.position += new Vector3(dir.x, 0, dir.y);
        
        Debug.Log($"MoveDir {MoveDir}");
        Debug.Log($"dir {dir}");
        
        // if (_moveDir != Vector2.zero)
        // {
        //     _indicator.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180 / Mathf.PI);
        // }
        
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    protected virtual void UpdateDead()
    {
        
    }

    #endregion

    public virtual void OnDamaged(BaseController attacker, int damage)
    {
        if (Hp <= 0)
            return;
        
        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }
    }

    protected virtual void OnDead()
    {
        
    }
}
