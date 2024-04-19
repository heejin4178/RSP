using System;
using Google.Protobuf;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class Player : GameObject
    { 
        public ClientSession Session { get; set; }

        public Player()
        {
            ObjectType = GameObjectType.Player;
        }
        
        public override void OnDamaged(GameObject attacker, int damage)
        {
            base.OnDamaged(attacker, damage);
        }

        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);
        }
    }
}