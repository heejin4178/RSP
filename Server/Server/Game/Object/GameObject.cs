using System;
using System.Numerics;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        
        public ClientSession Session { get; set; }

        public PlayerType PlayerType
        {
            get => Info.PlayerType;
            set
            {
                Info.PlayerType = value;
            }
        }

        public int Id
        {
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }
        public GameRoom Room { get; set; }
        public ObjectInfo Info { get; set; } = new ObjectInfo();
        public PositionInfo PosInfo { get; set; } = new PositionInfo();
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
        
        public GameObject Chaser { get; set; }

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
        
        public virtual void OnHitProjectile(GameObject attacker)
        {
            if (Room == null)
                return;

            State = CreatureState.Stun;

            S_Stun stunPacket = new S_Stun();
            stunPacket.ObjectId = Id;
            Room.Broadcast(stunPacket);
        }

        public virtual void OnDead(GameObject attacker)
        {
            if (Room == null)
                return;

            Chaser = null;
            
            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Room.Broadcast(diePacket);

            GameRoom room = Room;
            room.Push(room.DeSpawnGame, Id);
            
            Stat.Hp = Stat.MaxHp;
            PosInfo.State = CreatureState.Idle;
            Info.PlayerType = attacker.PlayerType;
            Info.PosInfo = PosInfo;
            
            room.Push(room.SpawnGame, this);
        }
    }
}