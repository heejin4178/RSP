using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : CreatureController
{ 
    private Coroutine _coSkill;

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

    // public void UseSkill(int skillId)
    // {
    //     if (skillId == 1)
    //     {
    //         _coSkill = StartCoroutine("CoStartPunch", 1.5f);
    //     }
    //     else if (skillId == 2)
    //     {
    //         _coSkill = StartCoroutine("CoStartShootArrow", 2.5f);
    //     }
    // }
    //
    // IEnumerator CoStartPunch(float time)
    // {
    //     // 대기 시간
    //     _rangeSkill = false;
    //     State = CreatureState.Skill;
    //     // Debug.Log($"Player State {State}!!");
    //     yield return new WaitForSeconds(time);
    //     State = CreatureState.Idle;
    //     _coSkill = null;
    // }
    //
    // IEnumerator CoStartShootArrow(float time)
    // {
    //     // 대기 시간
    //     _rangeSkill = true;
    //     State = CreatureState.Skill;
    //     yield return new WaitForSeconds(time);
    //     State = CreatureState.Idle;
    //     _coSkill = null;
    // }
}
