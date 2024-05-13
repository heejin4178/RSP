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
        
        // 벽이나 건물을 통과하지 못하게 함.
        if (Physics.Raycast(transform.position + Vector3.up, -(transform.forward), 0.1f, LayerMask.GetMask("Block")))
        {
            C_LeaveGame leave = new C_LeaveGame();
            leave.ObjectId = Id;
            Managers.Network.Send(leave);
            return;
        }
        
        transform.position += destPose;
    }

    private void UpdateHit()
    {
        
    }
    
    public override void SyncPos()
    {
        transform.position = new Vector3(CellPos.x, 1.5f, CellPos.z);
        transform.rotation = Quaternion.Euler(180, Rotation, 0); // y 축만 회전하도록 설정
    }
}
