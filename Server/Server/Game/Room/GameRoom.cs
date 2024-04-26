using System;
using System.Collections.Generic;
using System.Numerics;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game
{
    public class GameRoom : JobSerializer
    { 
        public int RoomId { get; set; }
        
        private Dictionary<int, Player> _players = new Dictionary<int, Player>();
        private Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();

        // public Map Map { get; private set; } = new Map();

        public void Init(int mapId)
        {
            // Map.LoadMap(mapId, "../../../../../Common/MapData");
            
            // TEMP
            // Push(EnterGame, monster);
        }

        // 누군가 주기적으로 호출해줘야 한다.
        public void Update()
        {
            // foreach (Monster monster in _monsters.Values)
            // {
            //     monster.Update();
            // }
            
            foreach (Projectile projectile in _projectiles.Values)
            {
                projectile.Update();
            }
            
            Flush();
        }

        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);
            
            if (type == GameObjectType.Player)
            {
                Player player = gameObject as Player;
                _players.Add(gameObject.Id, player);
                player.Room = this;
                
                ApplyMove(player, new Vector3(player.CellPos.X, 0, player.CellPos.Z), player.PosInfo.Rotation);
                
                // 본인한테 정보 전송
                {
                    // 나에게 나에 대한 정보를 보냄
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = player.Info;
                    player.Session.Send(enterPacket);
                
                    // 나에게 다른 사람들의 정보를 보냄
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (Player p in _players.Values)
                    {
                        if (player != p)
                            spawnPacket.Objects.Add(p.Info);
                    }
                    
                    // foreach (Monster m in _monsters.Values)
                    //     spawnPacket.Objects.Add(m.Info);

                    foreach (Projectile p in _projectiles.Values)
                        spawnPacket.Objects.Add(p.Info);
                    
                    player.Session.Send(spawnPacket);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                // Monster monster = gameObject as Monster;
                // _monsters.Add(gameObject.Id, monster);
                // monster.Room = this;
                //
                // Map.ApplyMove(monster, new Vector2Int(monster.CellPos.x, monster.CellPos.y));
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = gameObject as Projectile;
                _projectiles.Add(gameObject.Id, projectile);
                projectile.Room = this;
            }
            
            // 타인에게 정보 전송
            {
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(gameObject.Info);
                foreach (Player p in _players.Values)
                {
                    if (p.Id != gameObject.Id)
                        p.Session.Send(spawnPacket);
                }
            }
        }
        
        public void LeaveGame(int objectId)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

            if (type == GameObjectType.Player)
            {
                Player player = null;
                if (_players.Remove(objectId, out player) == false)
                    return;
            
                // Map.ApplyLeave(player);
                player.Room = null;
            
                // 본인한테 정보 전송
                {
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                // Monster monster = null;
                // if (_monsters.Remove(objectId, out monster) == false)
                //     return;
                //
                // Map.ApplyLeave(monster);
                // monster.Room = null;
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = null;
                if (_projectiles.Remove(objectId, out projectile) == false)
                    return;

                projectile.Room = null;
            }
            
            // 타인에게 정보 전송
            {
                S_Despawn despawnPacket = new S_Despawn();
                despawnPacket.ObjectIds.Add(objectId);
                foreach (Player p in _players.Values)
                {
                    if (p.Id != objectId)
                        p.Session.Send(despawnPacket);
                }
            }
        }

        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
                return;
            
            // TODO : 검증
            PositionInfo movePosInfo = movePacket.PosInfo;
            ObjectInfo info = player.Info;
            
            // // 다른 좌표로 이동할 경우, 갈 수 있는지 체크
            // if (movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosZ != info.PosInfo.PosZ)
            // {
            //     // if (Map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosZ)) == false)
            //     //     return;
            // }

            info.PosInfo.State = movePosInfo.State;
            ApplyMove(player, new Vector3(movePosInfo.PosX, 0, movePosInfo.PosZ), movePosInfo.Rotation);

            // 다른 플레이어한테도 알려준다.
            S_Move resMovePacket = new S_Move();
            resMovePacket.ObjectId = player.Info.ObjectId;
            resMovePacket.PosInfo = movePacket.PosInfo;

            Broadcast(resMovePacket);
        }
        
        public void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null)
                return;

            ObjectInfo info = player.Info;
            if (info.PosInfo.State != CreatureState.Idle)
                return;
            
            // TODO : 스킬 사용 가능 여부 체크
            
            info.PosInfo.State = CreatureState.Skill;

            S_Skill skill = new S_Skill() { Info = new SkillInfo() };
            skill.ObjectId = info.ObjectId;
            skill.Info.SkillId = skillPacket.Info.SkillId;
            Broadcast(skill);

            Skill skillData = null;
            if (DataManager.SkillDict.TryGetValue(skillPacket.Info.SkillId, out skillData) == false)
                return;

            switch (skillData.SkillType)
            {
                case SkillType.SkillAuto:
                {
                    // 데미지 판정
                    Vector3 skillSize = new Vector3(2.0f, 0, 4.0f);
                    
                    // 쿼터니언을 생성합니다. 여기서는 y값만 사용하여 회전을 표현합니다.
                    Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0, info.PosInfo.Rotation * MathF.PI / 180f, 0);
                    // 생성한 쿼터니언의 forward 속성을 이용하여 캐릭터의 바라보는 방향 벡터를 얻습니다.
                    Vector3 characterForward = Vector3.Transform(Vector3.UnitZ, rotation);
                    // 방향 벡터의 x, y 값을 서로 바꿔줍니다.
                    (characterForward.X, characterForward.Y) = (-characterForward.Y, characterForward.X);
                    
                    Vector3 skillPos = player.CellPos + characterForward;
                    
                    foreach (var p in _players.Values)
                    {
                        if (p == player)
                            continue;
                        
                        // 플레이어의 위치가 skillPos 범위 안에 있는지 확인
                        if (IsPlayerInSkillRange(p.CellPos, skillPos, skillSize))
                        {
                            Console.WriteLine("Hit GameObject!");
                        }
                    }
                }
                break;
                
                case SkillType.SkillProjectile:
                {
                    // Arrow
                    // Arrow arrow = ObjectManager.Instance.Add<Arrow>();
                    // if (arrow == null)
                    //     return;

                    // arrow.Owner = player;
                    // arrow.Data = skillData;
                    // arrow.PosInfo.State = CreatureState.Moving;
                    // arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                    // arrow.PosInfo.PosX = player.PosInfo.PosX;
                    // arrow.PosInfo.PosZ = player.PosInfo.PosZ;
                    // arrow.Speed = skillData.Projectile.speed;
                    // Push(EnterGame, arrow);
                }
                break;
            }
        }

        public Player FindPlayer(Func<GameObject, bool> condition)
        {
            foreach (Player player in _players.Values)
            {
                if (condition.Invoke(player))
                    return player;
            }

            return null;
        }
        
        public void Broadcast(IMessage packet)
        {
            foreach (Player p in _players.Values)
            {
                p.Session.Send(packet);
            }
        }
        
        bool IsPlayerInSkillRange(Vector3 playerPos, Vector3 skillPos, Vector3 skillSize)
        {
            // 플레이어의 위치가 skillPos 범위 안에 있는지 확인
            if (playerPos.X >= skillPos.X - skillSize.X / 2 &&
                playerPos.X <= skillPos.X + skillSize.X / 2 &&
                playerPos.Z >= skillPos.Z - skillSize.Z / 2 &&
                playerPos.Z <= skillPos.Z + skillSize.Z / 2)
            {
                return true;
            }
            return false;
        }

        public bool ApplyMove(GameObject gameObject, Vector3 dest, float rotation)
        {
            if (gameObject.Room == null)
                return false;
		
            PositionInfo posInfo = gameObject.PosInfo;
            
            // TODO : 맵의 최대 크기를 알아내 이상하게 이동하는 패킷은 이동하지 못하게 해야함.
            // if (CanGo(dest, true) == false)
            //     return false;
		
            // 실제 좌표 이동
            posInfo.PosX = dest.X;
            posInfo.PosZ = dest.Z;
            posInfo.Rotation = rotation;
		
            return true;
        }
    }
}