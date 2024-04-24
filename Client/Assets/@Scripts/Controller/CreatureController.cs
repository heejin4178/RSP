using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class CreatureController : BaseController
{
    protected Coroutine _coSkill;
    protected bool _rangeSkill = false;
    
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
        
        // 현재 회전 각도 가져오기
        Quaternion currentRotation = transform.rotation;

        // 목표 회전 각도 설정
        Quaternion targetRotation = Quaternion.Euler(0, Rotation, 0); // y 축만 회전하도록 설정

        // Quaternion.Slerp를 사용하여 부드러운 회전 적용
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
        if (_updated)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;
            Managers.Network.Send(movePacket);
            _updated = false;
        }
    }
    
    public virtual void UseSkill(int skillId)
    {
        if (skillId == 1)
        {
            _coSkill = StartCoroutine("CoStartPunch", 1.5f);
        }
        else if (skillId == 2)
        {
            _coSkill = StartCoroutine("CoStartShootArrow", 2.5f);
        }
    }
    
    IEnumerator CoStartPunch(float time)
    {
        // 대기 시간
        _rangeSkill = false;
        State = CreatureState.Skill;
        // Debug.Log($"Player State {State}!!");
        yield return new WaitForSeconds(time);
        State = CreatureState.Idle;
        _coSkill = null;
        CheckUpdatedFlag();
    }
    
    IEnumerator CoStartShootArrow(float time)
    {
        // 대기 시간
        _rangeSkill = true;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(time);
        State = CreatureState.Idle;
        _coSkill = null;
        CheckUpdatedFlag();
    }
}
