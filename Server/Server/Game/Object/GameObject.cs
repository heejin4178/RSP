using System;
using System.Numerics;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;

        public int Id
        {
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }
        public GameRoom Room { get; set; }
        public ObjectInfo Info { get; set; } = new ObjectInfo();
        public PositionInfo PosInfo { get; private set; } = new PositionInfo();
        public StatInfo Stat { get; private set; } = new StatInfo();

        public float Speed
        {
            get { return Stat.Speed; }
            set { Stat.Speed = value; }
        }

        public int Hp
        {
            get { return Stat.Hp; }
            set { Stat.Hp = Math.Clamp(value, 0 , Stat.MaxHp); }
        }

        public CreatureState State
        {
            get { return PosInfo.State; }
            set { PosInfo.State = value; }
        }

        public GameObject()
        {
            Info.PosInfo = PosInfo;
            Info.StatInfo = Stat;
        }

        public virtual void Update()
        {
            
        }
        
        public Vector3 CellPos
        {
            get
            {
                return new Vector3(PosInfo.PosX, 0, PosInfo.PosZ);
            }
            set
            {
                PosInfo.PosX = value.X;
                PosInfo.PosZ = value.Z;
            }
        }
        
        public Vector3 GetMeleeAttackCellPos()
        {
            Vector3 cellPos = CellPos;
            
            // 내가 있는 위치(좌표)에서, 내가 현재 바라보는 방향으로 스킬 범위만큼 더한 값
            Vector3 skillPos = new Vector3(0.1f, 0, 0.2f);

            return cellPos + skillPos;
        
            // switch (dir)
            // {
            //     case MoveDir.Up:
            //         cellPos += Vector2Int.up;
            //         break;
            //     case MoveDir.Down:
            //         cellPos += Vector2Int.down;
            //         break;
            //     case MoveDir.Left:
            //         cellPos += Vector2Int.left;
            //         break;
            //     case MoveDir.Right:
            //         cellPos += Vector2Int.right;
            //         break;
            // }
            //
            // return cellPos;
        }

        public virtual void OnDamaged(GameObject attacker, int damage)
        {
            if (Room == null)
                return;
            
            Stat.Hp = Math.Max(Stat.Hp - damage, 0);
            
            S_ChangeHp changePacket = new S_ChangeHp();
            changePacket.ObjectId = Id;
            changePacket.Hp = Stat.Hp;
            Room.Broadcast(changePacket);
            
            if (Stat.Hp <= 0)
            {
                Stat.Hp = 0;
                OnDead(attacker);
            }
        }

        public virtual void OnDead(GameObject attacker)
        {
            if (Room == null)
                return;
            
            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Room.Broadcast(diePacket);

            GameRoom room = Room;
            room.LeaveGame(Id);
            
            Stat.Hp = Stat.MaxHp;
            PosInfo.State = CreatureState.Idle;
            PosInfo.PosX = 0;
            PosInfo.PosZ = 0;
            
            room.EnterGame(this);
        }
    }
}