using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class CreatureController : BaseController
{
    protected bool _rangeSkill = false;
    private Coroutine _coHitCoolTime;

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
            case CreatureState.Hit:
                UpdateHit();
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
        
        // 공격하는 방향 적용
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, Rotation, 0); // y 축만 회전하도록 설정
        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, 10 * Time.deltaTime);
    }
    protected virtual void UpdateMoving()
    {
        _animator.Play("RUN");
        
        Vector3 moveDir = CellPos - transform.position;
        Vector3 dir = moveDir.normalized * Speed * Time.deltaTime;
        Vector3 destPose = new Vector3(dir.x, 0, dir.z);
        
        transform.position += destPose;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);

        Rotation = transform.rotation.eulerAngles.y;

        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    protected virtual void UpdateDead()
    {
        
    }
    protected virtual void UpdateHit()
    {
        _coHitCoolTime = StartCoroutine("CoStartHitReact", 0.3);
    }
    
    IEnumerator CoStartHitReact(float time)
    {
        yield return new WaitForSeconds(time);
        _animator.Play("REACT");
        yield return new WaitForSeconds(time);
        State = CreatureState.Idle;
        _coHitCoolTime = null;
        CheckUpdatedFlag();
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
        if (_updated)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;
            Managers.Network.Send(movePacket);
            _updated = false;
        }
    }
}
