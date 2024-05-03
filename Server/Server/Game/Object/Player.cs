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
            Hp = 2;
        }
        
        public override void OnDamaged(GameObject attacker, int damage)
        {
            // 같은 종족끼리는 공격할 수 없다.
            if (attacker.PlayerType == PlayerType)
                return;
            
            base.OnDamaged(attacker, damage);
        }

        public override void OnDead(GameObject attacker)
        {
            switch (attacker.PlayerType)
            {
                case PlayerType.Rock:
                    PlayerType = PlayerType.Rock;
                    break;
                case PlayerType.Scissors:
                    PlayerType = PlayerType.Scissors;
                    break;
                case PlayerType.Paper:
                    PlayerType = PlayerType.Paper;
                    break;
            }
            
            base.OnDead(attacker);
        }
    }
}