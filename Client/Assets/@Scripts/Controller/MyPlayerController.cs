using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class MyPlayerController : CreatureController
{
    private bool _moveKeyPressed = false;
    private bool _attackKeyPressed = false;
    private Coroutine _coSkillCoolTime;
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
        _attackKeyPressed = true;
    }

    protected override void UpdateIdle()
    {
        if (State == CreatureState.Stun)
            return;
        
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
                        break;
                    case SkillType.SkillProjectile:
                        skill.Info.SkillId = 2;
                        break;
                }
                Managers.Network.Send(skill);
            }

            _coSkillCoolTime = StartCoroutine("CoStartPunch", 1.0f);
        }
    }
    
    IEnumerator CoStartPunch(float time)
    {
        _rangeSkill = false;
        // State = CreatureState.Skill;
        yield return new WaitForSeconds(time);
        State = CreatureState.Idle;
        _coSkillCoolTime = null;
        CheckUpdatedFlag();
    }

    protected override void UpdateSkill()
    {
        if (State == CreatureState.Stun)
            return;
        
        base.UpdateSkill();
    }

    protected override void UpdateMoving()
    {
        if (State == CreatureState.Stun)
            return;
        
        if (_moveKeyPressed == false && _attackKeyPressed == false)
        {
            State = CreatureState.Idle;
            CheckUpdatedFlag();
            return;
        }
        
        Vector3 dir = MoveDir * Speed * Time.deltaTime;
        Vector3 destPose = new Vector3(dir.x, 0, dir.y);
        CellPos = destPose + transform.position;

        // Debug.DrawRay(transform.position + Vector3.up * 1.5f, destPose.normalized, Color.green);
        // 벽이나 건물을 통과하지 못하게 함.
        if (Physics.Raycast(transform.position + Vector3.up * 1.5f, destPose, 2.0f, LayerMask.GetMask("Block")))
            return;
        
        base.UpdateMoving();
        
        CheckUpdatedFlag();
    }
}
