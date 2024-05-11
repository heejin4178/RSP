using System;
using System.Numerics;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class Hand : Projectile
    {
        public GameObject Owner { get; set; }

        private long _nextMoveTick = 0;
        public override void Update()
        {
            if (Data == null || Data.Projectile == null || Owner == null || Room == null)
                return;
            
            if (_nextMoveTick >= Environment.TickCount64)
                return;
            
            long tick = (long)(1000 / Data.Projectile.speed);
            _nextMoveTick = Environment.TickCount64 + tick;

            // 쿼터니언을 생성합니다. 여기서는 y값만 사용하여 회전을 표현합니다.
            Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0, Info.PosInfo.Rotation * MathF.PI / 180f, 0);
            // 생성한 쿼터니언의 forward 속성을 이용하여 캐릭터의 바라보는 방향 벡터를 얻습니다.
            Vector3 characterForward = Vector3.Transform(Vector3.UnitZ, rotation);
            // 방향 벡터의 x, y 값을 서로 바꿔줍니다.
            (characterForward.X, characterForward.Y) = (-characterForward.Y, characterForward.X);
                    
            Vector3 skillPos = CellPos + characterForward;

            CellPos += characterForward;
            
            Console.WriteLine($"Hit Player!, CellPos : {CellPos}, SkillPos : {skillPos}, Rotation : {Info.PosInfo.Rotation}");
            
            
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Room.Broadcast(movePacket);
            Console.WriteLine("Move Hand");
            
            // Vector2Int destPos = GetFrontCellPos();
            // if (Room.Map.CanGo(destPos))
            // {
            //     CellPos = destPos;
            //
            //     S_Move movePacket = new S_Move();
            //     movePacket.ObjectId = Id;
            //     movePacket.PosInfo = PosInfo;
            //     Room.Broadcast(movePacket);
            //     Console.WriteLine("Move Arrow");
            // }
            // else
            // {
            //     GameObject target = Room.Map.Find(destPos);
            //     if (target != null)
            //     {
            //         // 피격판정
            //         target.OnDamaged(this, Data.damage + Owner.Stat.Attack);
            //     }
            //     
            //     // 소멸
            //     Room.Push(Room.LeaveGame, Id);
            // }
        }
    }
}