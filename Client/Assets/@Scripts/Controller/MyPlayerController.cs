using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class MyPlayerController : CreatureController
{
    private bool _moveKeyPressed = false;
    private bool _attackKeyPressed = false;
    private Coroutine _coSkillCoolTime;
    private Coroutine _coPorjectileSkillCoolTime;
    private LineRenderer _lineRenderer;
    public LineRenderer LineRenderer { get => _lineRenderer; }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        Managers.Game.onMoveDirChanged += HandleOnMoveDirChanged;
        Managers.Game.onMovePointerUp += HandleOnMovePointerUp;
        Managers.Game.onAttackPointerUp += HandleOnAttackPointerUp;

        _lineRenderer = transform.Find("AttackLineIndicator").GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.enabled = false;

        return true;
    }
    
    private void OnDestroy()
    {
        if (Managers.Game != null)
        {
            Managers.Game.onMoveDirChanged -= HandleOnMoveDirChanged;
            Managers.Game.onMovePointerUp -= HandleOnMovePointerUp;
            Managers.Game.onAttackPointerUp -= HandleOnAttackPointerUp;
            Managers.Game.CoolTimeValue = 0;
        }
    }
    
    public void PlayAttackIndicator(Color color, Vector3 from, Vector3 to)
    {
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, from);
        _lineRenderer.SetPosition(1, to);
    }
    
    public void StopAttackIndicator()
    {
        _lineRenderer.enabled = false;
    }
    
    void HandleOnMoveDirChanged(Vector2 dir)
    {
        if (State == CreatureState.Stun)
            return;
        
        State = CreatureState.Moving;
        MoveDir = dir;
        _moveKeyPressed = true;
    }
    
    void HandleOnMovePointerUp()
    {            
        _moveKeyPressed = false;
    }
    
    void HandleOnAttackPointerUp()
    {
        if (State == CreatureState.Stun)
            return;
        
        _attackKeyPressed = true;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
        
        // 스킬 상태로 갈지 확인
        if (_coSkillCoolTime == null && _attackKeyPressed)
        {
            _attackKeyPressed = false;
            State = CreatureState.Skill;
            
            CheckUpdatedFlag();

            // 공격 패킷 보내기
            {
                C_Skill skill = new C_Skill() { Info = new SkillInfo() };
            
                switch (Managers.Game.SkillType)
                {
                    case SkillType.SkillAuto:
                        skill.Info.SkillId = 1;
                        _coSkillCoolTime = StartCoroutine("CoStartPunch", 1.0f);
                        break;
                    case SkillType.SkillProjectile:
                        if (_coPorjectileSkillCoolTime == null)
                        {
                            skill.Info.SkillId = 2;
                            _coSkillCoolTime = StartCoroutine("CoStartProjectile", 1.0f);
                        }
                        else
                        {
                            State = CreatureState.Idle;
                            return;
                        }
                        break;
                }
                Managers.Network.Send(skill);
            }
        }
    }
    
    IEnumerator CoStartPunch(float time)
    {
        _rangeSkill = false;
        yield return new WaitForSeconds(time);
        State = CreatureState.Idle;
        _coSkillCoolTime = null;
        CheckUpdatedFlag();
    }
    
    IEnumerator CoStartProjectile(float time)
    {
        _rangeSkill = true;
        _coPorjectileSkillCoolTime = StartCoroutine("CoStartProjectileCoolTime", 5.0f);
        yield return new WaitForSeconds(time);
        State = CreatureState.Idle;
        _coSkillCoolTime = null;
        CheckUpdatedFlag();
    }
    
    IEnumerator CoStartProjectileCoolTime(float time)
    {
        yield return new WaitForSeconds(time);
        _coPorjectileSkillCoolTime = null;
        Managers.Game.CoolTimeValue = 0;
    }

    public override void UpdateController()
    {
        if (_coPorjectileSkillCoolTime != null)
        {
            Managers.Game.CoolTimeValue += 20 * Time.deltaTime;
        }
        
        base.UpdateController();
    }

    protected override void UpdateMoving()
    {
        if (_moveKeyPressed == false && _attackKeyPressed == false)
        {
            State = CreatureState.Idle;
            CheckUpdatedFlag();
            return;
        }
        
        Vector3 dir = MoveDir * Speed * Time.deltaTime;
        Vector3 destPose = new Vector3(dir.x, 0, dir.y);
        CellPos = destPose + transform.position;
        
        base.UpdateMoving();
        
        CheckUpdatedFlag();
    }
    
    private void CheckUpdatedFlag()
    {
        if (_updated)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;
            Managers.Network.Send(movePacket);
            _updated = false;
        }
    }

    public override int Hp
    {
        get { return Stat.Hp; }
        set
        {
            Stat.Hp = value;
            Managers.Game.Hp = value;
        }
    }
}
