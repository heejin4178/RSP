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
        public UInt16 RockNum { get; private set; }
        public UInt16 ScissorsNum { get; private set; }
        public UInt16 PaperNum { get; private set; }
        
        private Dictionary<int, Player> _players = new Dictionary<int, Player>();
        private Dictionary<int, AIPlayer> _aiPlayers = new Dictionary<int, AIPlayer>();
        private Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();

        public void Init(int mapId)
        {
            // 룸 하나가 만들어지면, 플레이어 타입 별로 4명씩 반복하여 추가
            foreach (PlayerType type in Enum.GetValues(typeof(PlayerType)))
            {
                if (type == PlayerType.NonePlayer)
                    continue;

                for (int i = 0; i < 4; i++)
                {
                    AIPlayer aiPlayer = CreatePlayer(type);
                    SetPlayerPose(aiPlayer, aiPlayer.PlayerType, i);
                    _aiPlayers.Add(aiPlayer.Id, aiPlayer);
                }
            }
        }
        // ai플레이어는 본인이 맞았을때 어떻게 다른사람들에게 패킷을 전달하는지?
        private AIPlayer CreatePlayer(PlayerType type)
        {
            AIPlayer aiPlayer = ObjectManager.Instance.Add<AIPlayer>();

            // 플레이어 정보 설정
            aiPlayer.Info.Name = $"AI_{type.ToString()}_{aiPlayer.Info.ObjectId}";
            aiPlayer.State = CreatureState.Idle;
            aiPlayer.PlayerType = type;
            aiPlayer.Room = this;

            // 스탯 설정
            StatInfo stat = null;
            DataManager.StatDict.TryGetValue(1, out stat);
            aiPlayer.Stat.MergeFrom(stat);

            return aiPlayer;
        }
        
        private void SetPlayerPose(AIPlayer player, PlayerType type, int playerNumber)
        {
            // 플레이어 타입에 기준 위치 설정
            switch (type)
            {
                case PlayerType.Paper:
                    player.Info.PosInfo.PosX = 0;
                    player.Info.PosInfo.PosZ = 5;
                    player.Info.PosInfo.Rotation = 180;
                    break;
                case PlayerType.Scissors:
                    player.Info.PosInfo.PosX = 10;
                    player.Info.PosInfo.PosZ = -7;
                    player.Info.PosInfo.Rotation = 270;
                    break;
                case PlayerType.Rock:
                    player.Info.PosInfo.PosX = -10;
                    player.Info.PosInfo.PosZ = -5;
                    player.Info.PosInfo.Rotation = 30;
                    break;
            }

            // 각 플레이어가 1부터 4까지의 포즈를 가지도록 설정
            switch (playerNumber)
            {
                case 0:
                    // 기존 포즈 유지
                    break;
                case 1:
                    player.Info.PosInfo.PosX += (type == PlayerType.Rock) ? -2 : 2;
                    break;
                case 2:
                    player.Info.PosInfo.PosZ += (type == PlayerType.Rock) ? -2 : 2;
                    break;
                case 3:
                    player.Info.PosInfo.PosX += (type == PlayerType.Rock) ? -2 : 2;
                    player.Info.PosInfo.PosZ += (type == PlayerType.Rock) ? -2 : 2;
                    break;
            }
        }
        
        public bool ReplacePlayer(Player replacePlayer)
        {
            foreach (var aiPlayer in _aiPlayers.Values)
            {
                if (aiPlayer.PlayerType == replacePlayer.PlayerType)
                {
                    replacePlayer.Info.PosInfo = aiPlayer.PosInfo;
                    return _aiPlayers.Remove(aiPlayer.Id);
                }
            }

            return false;
        }

        private void ClearRoom()
        {
            _players.Clear();
            _projectiles.Clear();
        }

        // 누군가 주기적으로 호출해줘야 한다.
        public void Update()
        {
            foreach (AIPlayer aiPlayer in _aiPlayers.Values)
            {
                aiPlayer.Update();
            }
            
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
                    // 나에게 나에 대한 정보를 전송
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = player.Info;
                    player.Session.Send(enterPacket);
                
                    // 스폰패킷에 현재 룸에 있는 모든 객체 정보를 담음
                    S_Spawn spawnPacket = new S_Spawn();
                    
                    foreach (Player p in _players.Values)
                        if (player != p)
                            spawnPacket.Objects.Add(p.Info);
                    
                    foreach (AIPlayer a in _aiPlayers.Values)
                        spawnPacket.Objects.Add(a.Info);

                    foreach (Projectile p in _projectiles.Values)
                        spawnPacket.Objects.Add(p.Info);
                    
                    // 나에게 다른 사람의 접속 정보를 전송
                    player.Session.Send(spawnPacket);
                }
            }
            else if (type == GameObjectType.Aiplayer)
            {
                AIPlayer aiPlayer = gameObject as AIPlayer;
                _aiPlayers.Add(gameObject.Id, aiPlayer);
                aiPlayer.Room = this;
                
                ApplyMove(aiPlayer, new Vector3(aiPlayer.CellPos.X, 0, aiPlayer.CellPos.Z), aiPlayer.PosInfo.Rotation);
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = gameObject as Projectile;
                _projectiles.Add(gameObject.Id, projectile);
                projectile.Room = this;
            }
            
            // 타인에게 내가 접속했다는 정보 전송
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
                
                // 룸에서 제거
                if (_players.Remove(objectId, out player) == false)
                    return;
                
                // 오브젝트 매니저에서 제거
                if (ObjectManager.Instance.Remove(objectId) == false)
                    return;

                if (_players.Count <= 0)
                    ClearRoom();
                
                // player.Room = null;
            
                // 본인한테 정보 전송
                {
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }
            }
            else if (type == GameObjectType.Aiplayer)
            {
                AIPlayer aiPlayer = null;
                if (_aiPlayers.Remove(objectId, out aiPlayer) == false)
                    return;
                
                // aiPlayer.Room = null;
                
                // // AI Player가 죽었다고 일반 플레이어들에게 패킷 전송
                // {
                //     S_LeaveGame leavePacket = new S_LeaveGame();
                //     aiPlayer.Room.Broadcast(leavePacket);
                // }
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

            info.PosInfo = movePosInfo;
            player.PosInfo = movePosInfo;
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
            // if (info.PosInfo.State != CreatureState.Idle)
            //     return;
            
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
                    float width = 1.5f; // 가로
                    float height = 2.5f; // 세로
                    
                    // 쿼터니언을 생성합니다. 여기서는 y값만 사용하여 회전을 표현합니다.
                    Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0, info.PosInfo.Rotation * MathF.PI / 180f, 0);
                    // 생성한 쿼터니언의 forward 속성을 이용하여 캐릭터의 바라보는 방향 벡터를 얻습니다.
                    Vector3 characterForward = Vector3.Transform(Vector3.UnitZ, rotation);
                    // 방향 벡터의 x, y 값을 서로 바꿔줍니다.
                    (characterForward.X, characterForward.Y) = (-characterForward.Y, characterForward.X);
                    
                    Vector3 skillPos = player.CellPos + characterForward;

                    // 캐릭터가 바라보는 방향 벡터와 오른쪽 방향 벡터를 계산
                    Vector3 right = Vector3.Cross(Vector3.UnitY, characterForward);

                    // 직사각형의 네 꼭지점 좌표 계산
                    Vector3 topLeft = skillPos + (-right * (width / 2)) + (characterForward * (height / 2));
                    Vector3 topRight = skillPos + (right * (width / 2)) + (characterForward * (height / 2));
                    Vector3 bottomLeft = skillPos + (-right * (width / 2)) + (-characterForward * (height / 2));
                    Vector3 bottomRight = skillPos + (right * (width / 2)) + (-characterForward * (height / 2));
                    
                    // Console.WriteLine($"topLeft : {topLeft}, topRight : {topRight}, bottomLeft : {bottomLeft}, bottomRight : {bottomRight}");
                    // Console.WriteLine("info.PosInfo.Rotation : " + info.PosInfo.Rotation);
                    foreach (var p in _players.Values)
                    {
                        if (p == player)
                            continue;
                        
                        // 플레이어의 위치가 skillPos 범위 안에 있는지 확인
                        if (IsPointInsideRectangle(p.CellPos, topLeft, topRight, bottomLeft, bottomRight))
                        {
                            p.OnDamaged(player, 1);
                            Console.WriteLine($"Hit Player!, CellPos : {p.CellPos}, SkillPos : {skillPos}, Rotation : {info.PosInfo.Rotation}");
                        }
                    }
                    
                    foreach (var p in _aiPlayers.Values)
                    {
                        // 플레이어의 위치가 skillPos 범위 안에 있는지 확인
                        if (IsPointInsideRectangle(p.CellPos, topLeft, topRight, bottomLeft, bottomRight))
                        {
                            p.OnDamaged(player, 1);
                            Console.WriteLine($"Hit AI Player!, CellPos : {p.CellPos}, SkillPos : {skillPos}, Rotation : {info.PosInfo.Rotation}");
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
        
        bool IsPointInsideRectangle(Vector3 point, Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight)
        {
            // 주어진 점의 x와 z 좌표를 가져옵니다.
            float x = point.X;
            float z = point.Z;

            // 직사각형의 x와 z 좌표 범위를 계산합니다.
            float minX = Math.Min(topLeft.X, Math.Min(topRight.X, Math.Min(bottomLeft.X, bottomRight.X)));
            float maxX = Math.Max(topLeft.X, Math.Max(topRight.X, Math.Max(bottomLeft.X, bottomRight.X)));
            float minZ = Math.Min(topLeft.Z, Math.Min(topRight.Z, Math.Min(bottomLeft.Z, bottomRight.Z)));
            float maxZ = Math.Max(topLeft.Z, Math.Max(topRight.Z, Math.Max(bottomLeft.Z, bottomRight.Z)));

            // 주어진 점이 직사각형의 x와 z 좌표 범위 내에 있는지 확인합니다.
            if (x >= minX && x <= maxX && z >= minZ && z <= maxZ)
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