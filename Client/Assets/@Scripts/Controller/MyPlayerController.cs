using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class MyPlayerController : CreatureController
{
    private bool _moveKeyPressed = false;
    private bool _attackKeyPressed = false;
    private Coroutine _coSkillCoolTime;
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        Managers.Game.onMoveDirChanged += HandleOnMoveDirChanged;
        Managers.Game.onMovePointerUp += HandleOnMovePointerUp;
        Managers.Game.onAttackPointerUp += HandleOnAttackPointerUp;

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
        base.UpdateIdle();
        
        // 스킬 상태로 갈지 확인
        if (_coSkillCoolTime == null && _attackKeyPressed)
        {
            _attackKeyPressed = false;
            State = CreatureState.Skill;

            // 공격 패킷 보내기
            C_Skill skill = new C_Skill() { Info = new SkillInfo() };
            skill.Info.SkillId = 1;
            Managers.Network.Send(skill);

            // rotation 갱신
            Rotation = transform.rotation.eulerAngles.y;
            CheckUpdatedFlag();

            _coSkillCoolTime = StartCoroutine("CoStartPunch", 1.5f);
        }
    }
    
    IEnumerator CoStartPunch(float time)
    {
        _rangeSkill = false;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(time);
        State = CreatureState.Idle;
        _coSkillCoolTime = null;
        CheckUpdatedFlag();
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
        // Rotation = destPose.normalized;

        // Debug.DrawRay(transform.position + Vector3.up * 1.5f, destPose.normalized, Color.green);
        // 벽이나 건물을 통과하지 못하게 함.
        if (Physics.Raycast(transform.position + Vector3.up * 1.5f, destPose, 2.0f, LayerMask.GetMask("Block")))
            return;
        
        base.UpdateMoving();
        
        CheckUpdatedFlag();
    }
}
