using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : CreatureController
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
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

    // public override void UseSkill(int skillId)
    // {
    //     if (skillId == 1)
    //     {
    //         _coSkill = StartCoroutine("CoStartPunch");
    //     }
    //     else if (skillId == 2)
    //     {
    //         _coSkill = StartCoroutine("CoStartShootArrow");
    //     }
    // }
    //
    // IEnumerator CoStartPunch()
    // {
    //     // 대기 시간
    //     _rangeSkill = false;
    //     State = CreatureState.Skill;
    //     Debug.Log($"Player State {State}!!");
    //     yield return new WaitForSeconds(1.5f);
    //     State = CreatureState.Idle;
    //     _coSkill = null;
    //     CheckUpdatedFlag();
    // }
    //
    // IEnumerator CoStartShootArrow()
    // {
    //     // 대기 시간
    //     _rangeSkill = true;
    //     State = CreatureState.Skill;
    //     yield return new WaitForSeconds(2.3f);
    //     State = CreatureState.Idle;
    //     _coSkill = null;
    //     CheckUpdatedFlag();
    // }
}
