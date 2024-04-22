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
        
        _animator = GetComponent<Animator>();

        return true;
    }
    
    #region State Pattern
    protected Animator _animator;

    public override void UpdateController()
    {
        base.UpdateController();
        
        if (_animator == null)
            return;

        switch (State)
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
        _animator.Play("IDLE");
    }
    protected virtual void UpdateSkill()
    {
        _animator.Play("ATTACK");
    }
    protected virtual void UpdateMoving()
    {
        _animator.Play("RUN");
        
        Vector3 moveDir = CellPos - transform.position;
        Vector3 dir = moveDir.normalized * Speed * Time.deltaTime;
        Vector3 destPose = new Vector3(dir.x, 0, dir.z);
        
        transform.position += destPose;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);

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
    
    protected virtual void CheckUpdatedFlag()
    {
        
    }
}
