using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : CreatureController
{
    // [SerializeField] private Transform _indicator;
    // [SerializeField] private Transform _fireSocket;
    
    // public Transform Indicator { get => _indicator; }
    // public Vector3 FireSocket { get => _fireSocket.position; }
    // public Vector3 ShootDir { get => (_fireSocket.position - _indicator.position).normalized; }
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    private void OnDestroy()
    {
        // if (Managers.Game != null)
        //     Managers.Game.onMoveDirChanged -= HandleOnMoveDirChanged;
    }

    // void Update()
    // {
    //     MovePlayer();
    // }
    
    // void MovePlayer()
    // { 
    //     Vector3 dir = _moveDir * Speed * Time.deltaTime;
    //     transform.position += dir;
    //
    //     // if (_moveDir != Vector2.zero)
    //     // {
    //     //     _indicator.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180 / Mathf.PI);
    //     // }
    //     
    //     GetComponent<Rigidbody>().velocity = Vector3.zero;
    // }
    //
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     // MonsterController target = collision.gameObject.GetComponent<MonsterController>();
    //     if (target == null)
    //         return;
    // }
    //
    // public override void OnDamaged(BaseController attacker, int damage)
    // {
    //     base.OnDamaged(attacker, damage);
    //     
    //     Debug.Log($"OnDamaged : {Hp}");
    //     
    //     CreatureController cc = attacker as CreatureController;
    //     cc?.OnDamaged(this, 10);
    // }
}
