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

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

    protected override void UpdateMoving()
    {
        base.UpdateMoving();
    }
}
