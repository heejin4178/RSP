using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class HandController : BaseController
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public override void UpdateController()
    {
        base.UpdateController();
        
        switch (State)
        {
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Hit:
                UpdateHit();
                break;
        }
    }
    
    private void UpdateMoving()
    {

        Vector3 moveDir = CellPos - transform.position;
        Vector3 dir = moveDir.normalized * Speed * Time.deltaTime;
        // Vector3 dir = -(transform.forward) * Speed * Time.deltaTime;
        Vector3 destPose = new Vector3(dir.x, 0, dir.z);
        
        Debug.Log($"CellPos : {CellPos}");
        Debug.Log($"transform.position : {transform.position}");
        
        // 벽이나 건물을 통과하지 못하게 함.
        if (Physics.Raycast(transform.position + Vector3.up, -(transform.forward), 0.1f, LayerMask.GetMask("Block")))
        {
            C_LeaveGame leave = new C_LeaveGame();
            leave.ObjectId = Id;
            Managers.Network.Send(leave);
            return;
        }
        
        transform.position += destPose;
        // CellPos += destPose;
        // CheckUpdatedFlag(); // 여기서 패킷을 보내면 너무 많이 보내서 안됨 -> 서버에서 보내는걸로 바꿔야함
    }

    private void UpdateHit()
    {
        
    }
    
    public override void SyncPos()
    {
        transform.position = new Vector3(CellPos.x, 1.5f, CellPos.z);
        transform.rotation = Quaternion.Euler(180, Rotation, 0); // y 축만 회전하도록 설정
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
