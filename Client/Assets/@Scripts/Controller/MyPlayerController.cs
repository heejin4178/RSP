using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class MyPlayerController : CreatureController
{
    private bool _moveKeyPressed = false;
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        Managers.Game.onMoveDirChanged += HandleOnMoveDirChanged;

        return true;
    }
    
    private void OnDestroy()
    {
        if (Managers.Game != null)
            Managers.Game.onMoveDirChanged -= HandleOnMoveDirChanged;
    }
    
    void HandleOnMoveDirChanged(Vector2 dir)
    {
        State = CreatureState.Moving;
        MoveDir = dir;
    }

    protected override void UpdateMoving()
    {
        Vector3 dir = MoveDir * Speed * Time.deltaTime;
        Vector3 destPose = new Vector3(dir.x, 0, dir.y);
        CellPos = destPose + transform.position;
        
        base.UpdateMoving();

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
    // protected override void MoveToNextPos()
    // { 
    //     if (_moveKeyPressed == false)
    //     {
    //         State = CreatureState.Idle;
    //         CheckUpdatedFlag();
    //         return;
    //     }
    //     
    //     Vector3Int destPose = CellPos;
    //         
    //     switch (Dir)
    //     {
    //         case MoveDir.Up:
    //             destPose += Vector3Int.up;
    //             break;
    //         case MoveDir.Down:
    //             destPose += Vector3Int.down;
    //             break;
    //         case MoveDir.Left:
    //             destPose += Vector3Int.left;
    //             break;
    //         case MoveDir.Right:
    //             destPose += Vector3Int.right;
    //             break;
    //     }
    //         
    //     if (Managers.Map.CanGo(destPose))
    //     {
    //         if (Managers.Object.FindCreature(destPose) == null)
    //         {
    //             CellPos = destPose;
    //         }
    //     }
    //     
    //     CheckUpdatedFlag();
    // }
    //
    // protected override void CheckUpdatedFlag()
    // {
    //     if (_updated)
    //     {
    //         C_Move movePacket = new C_Move();
    //         movePacket.PosInfo = PosInfo;
    //         Managers.Network.Send(movePacket);
    //         _updated = false;
    //     }
    // }
}
