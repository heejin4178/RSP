using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class CreatureController : BaseController
{
    public override StatInfo Stat
    {
        get { return base.Stat; }
        set
        {
            base.Stat = value; 
        }
    }

    public int Hp
    {
        get { return base.Hp; }
        set
        {
            base.Hp = value;
        }
    }

    protected override bool Init()
    {
        base.Init();

        // Skills = gameObject.GetOrAddComponent<SkillBook>();

        return true;
    }

    public virtual void OnDamaged(BaseController attacker, int damage)
    {
        if (Hp <= 0)
            return;
        
        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }
    }

    protected virtual void OnDead()
    {
        
    }
}
