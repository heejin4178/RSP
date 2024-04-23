using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class MyPlayerController : CreatureController
{
    private bool _moveKeyPressed = false;
    private bool _attackKeyPressed = false;
    private Coroutine _coSkillCooltime;
    
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
        if (_coSkillCooltime == null && _attackKeyPressed)
        {
            _attackKeyPressed = false;
            Debug.Log("Attack!");
            
            // TODO : 공격 패킷 보내기
            // C_Skill skill = new C_Skill() { Info = new SkillInfo() };
            // skill.Info.SkillId = 2;
            // Managers.Network.Send(skill);

            _coSkillCooltime = StartCoroutine("CoInputCooltime", 1.5f);
        }
    }
    
    IEnumerator CoInputCooltime(float time)
    {
        State = CreatureState.Skill;
        yield return new WaitForSeconds(time);
        State = CreatureState.Idle;
        _coSkillCooltime = null;
    }

    protected override void UpdateMoving()
    {
        if (_moveKeyPressed == false)
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
    
    protected override void CheckUpdatedFlag()
    {
        if (_updated)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;
            Managers.Network.Send(movePacket);
            _updated = false;
        }
    }

    public override void UpdateController()
    {
        // switch (State)
        // {
        //     case CreatureState.Idle:
        //         GetDirInput();
        //         break;
        //     case CreatureState.Moving:
        //         GetDirInput();
        //         break;
        // }
        base.UpdateController();
    }
    
    // protected override void UpdateIdle()
    // {
    //     // 이동 상태로 갈지 확인
    //     if (_moveKeyPressed)
    //     {
    //         State = CreatureState.Moving;
    //         return;
    //     }
    //     
    //     // 스킬 상태로 갈지 확인
    //     if (_coSkillCooltime == null && Input.GetKey(KeyCode.Space))
    //     {
    //         Debug.Log("Skill !");
    //         
    //         C_Skill skill = new C_Skill() { Info = new SkillInfo() };
    //         skill.Info.SkillId = 2;
    //         Managers.Network.Send(skill);
    //
    //         _coSkillCooltime = StartCoroutine("CoInputCooltime", 0.2f);
    //     }
    // }

    // private Coroutine _coSkillCooltime;
    //
    // IEnumerator CoInputCooltime(float time)
    // {
    //     yield return new WaitForSeconds(time);
    //     _coSkillCooltime = null;
    // }
    //
    // private void LateUpdate()
    // {
    //     Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    // }
    //
    // void GetDirInput()
    // {
    //     _moveKeyPressed = true;
    //     
    //     if (Input.GetKey(KeyCode.W))
    //     {
    //         Dir = MoveDir.Up;
    //     }
    //     else if (Input.GetKey(KeyCode.S))
    //     {
    //         Dir = MoveDir.Down;
    //     }
    //     else if (Input.GetKey(KeyCode.A))
    //     {
    //         Dir = MoveDir.Left;
    //     }
    //     else if (Input.GetKey(KeyCode.D))
    //     {
    //         Dir = MoveDir.Right;
    //     }
    //     else
    //     {
    //         _moveKeyPressed = false;
    //     }
    // }
    //
    
}
