using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController
{
    private CreatureController _owner;

    private Vector3 _moveDir;

    private float speed = 10.0f;

    private float _lifeTime = 10.0f;
    
    // public ProjectileController() : base(Define.SkillType.None)
    // {
    //     
    // }
    //
    // public override bool Init()
    // {
    //     base.Init();
    //     
    //     StartDestroy(_lifeTime);
    //
    //     return true;
    // }
    //
    // public void SetInfo(int templateId, CreatureController owner, Vector3 moveDir)
    // {
    //     if (Managers.Data.SkillDic.TryGetValue(templateId, out Data.SkillData data) == false)
    //     {
    //         Debug.Log("ProjecteController SetInfo Faile");
    //         return;
    //     }
    //
    //     _owner = owner;
    //     _moveDir = moveDir;
    //     SkillData = data;
    // }
    //
    // public override void UpdateController()
    // {
    //     base.UpdateController();
    //
    //     transform.position += _moveDir * speed * Time.deltaTime;
    // }
    //
    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     MonsterController mc = collision.gameObject.GetComponent<MonsterController>();
    //     if (mc.IsValid() == false)
    //         return;
    //     if (this.IsValid() == false)
    //         return;
    //     
    //     mc.OnDamaged(_owner, SkillData.damage);
    //     
    //     StopDestroy();
    //     
    //     Managers.Object.Despawn(this);
    // }
}
