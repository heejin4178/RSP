using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game
{
    public class GameRoom : JobSerializer
    { 
        public int RoomId { get; set; }
        public bool PlayingGame { get; set; }
        public bool RunTimer { get; set; } = true;
        public int WaitTime { get; set; } = 1;
        public int GameTime { get; set; } = 1;

        public UInt16 RockNum 
        {
            get
            {
                UInt16 count = 0;
                foreach (var player in _allPlayers.Values)
                {
                    if (player.PlayerType == PlayerType.Rock)
                        count++;
                }

                return count;
            }
        }
        public UInt16 ScissorsNum 
        {
            get
            {
                UInt16 count = 0;
                foreach (var player in _allPlayers.Values)
                {
                    if (player.PlayerType == PlayerType.Scissors)
                        count++;
                }

                return count;
            }
        }
        public UInt16 PaperNum 
        {
            get
            {
                UInt16 count = 0;
                foreach (var player in _allPlayers.Values)
                {
                    if (player.PlayerType == PlayerType.Paper)
                        count++;
                }

                return count;
            }
        }
        public UInt16 PlayersCount { get => (ushort)_players.Count; }
        
        private Dictionary<int, Player> _players = new Dictionary<int, Player>();
        private Dictionary<int, AIPlayer> _aiPlayers = new Dictionary<int, AIPlayer>();
        private Dictionary<int, GameObject> _allPlayers = new Dictionary<int, GameObject>();
        private Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();

        #region Timer

        public void ResetWaitTime()
        {
            WaitTime = 1;
            RunTimer = true;
        }
        public void WaitPlayerTimer()
        {
            if (RunTimer == false || _players.Count <= 0)
            {
                ClearRoom();
                return;
            }

            Console.WriteLine($"WaitPlayerTimer : {WaitTime}");
            if (WaitTime < 5)
            {
                WaitTime++;
                PushAfter(1000, WaitPlayerTimer);
            }
            else
            {
                Push(ReadyGame); // 게임 준비 완료 패킷 전송
                PushAfter(1000, BeforeStartGameTimer);
                WaitTime = 1;
            }
        }
        
        public void BeforeStartGameTimer()
        {
            if (RunTimer == false || _players.Count <= 0)
            {
                ClearRoom();
                return;
            }
            
            Console.WriteLine($"BeforeStartGameTimer : {WaitTime}");
            if (WaitTime < 3)
            {
                WaitTime++;
                PushAfter(1000, BeforeStartGameTimer);
            }
            else
            {
                PlayingGame = true;
                Push(StartGame); // 게임 시작 패킷 전송
                Push(GameTimer); // 게임 타이머 실행
                WaitTime = 1;
            }
        }
        
        public void GameTimer()
        {
            if (RunTimer == false || _players.Count <= 0)
            {
                ClearRoom();
                return;
            }
            
            // Console.WriteLine($"GameTimer : {GameTime}");
            Console.WriteLine($"RockNum : {RockNum}, ScissorsNum : {ScissorsNum}, PaperNum : {PaperNum}");

            if (GameTime < 61)
            {
                GameTime++;
                PushAfter(1000, GameTimer);
            }
            else
            {
                Push(StopGame); // 게임 종료 패킷 전송
                GameTime = 1;
            }
        }
        #endregion
        
        
        #region RoomLogic
        public void Init()
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
                    _allPlayers.Add(aiPlayer.Id, aiPlayer);
                }
            }
        }
        
        public void ReplacePlayer(Player replacePlayer)
        {
            if (PlayersCount == 0) // 가장 첫번째 입장 유저라면
            {
                // 가위, 바위, 보 중 랜덤으로 선택됨
                Random random = new Random();
                replacePlayer.PlayerType = (PlayerType)random.Next(1, 4);
            }
            else // 사람 수가 가장 적은것으로 정하고, 사람 수가 같다면 랜덤으로 정한다.
            {
                // 종족별 인원수를 배열에 담습니다.
                int[] counts = { RockNum, ScissorsNum, PaperNum };

                // 종족 중 인원이 가장 적은 종족을 찾습니다.
                int minCount = counts.Min();

                // 최소값과 같은 변수들의 인덱스를 찾습니다.
                var minIndexes = Enumerable.Range(0, counts.Length)
                    .Where(i => counts[i] == minCount)
                    .ToList();

                // 모든 종족의 수가 같다면
                if (minIndexes.Count == counts.Length)
                {
                    // 가위, 바위, 보 중 랜덤으로 선택됨
                    Random random = new Random();
                    replacePlayer.PlayerType = (PlayerType)random.Next(1, 4);
                }
                else
                {
                    // 수가 가장 적은 종족을 선택합니다.
                    int selectedIndex = Array.IndexOf(counts, minCount);
                    replacePlayer.PlayerType = (PlayerType)selectedIndex + 1;
                }
            }

            foreach (var aiPlayer in _aiPlayers.Values)
            {
                if (aiPlayer.PlayerType == replacePlayer.PlayerType)
                {
                    replacePlayer.Info.PosInfo = aiPlayer.PosInfo;
                    replacePlayer.PosInfo = aiPlayer.PosInfo;
                    Console.WriteLine($"Remove!! : {aiPlayer.Id}, Count : {_aiPlayers.Count}");
                    LeaveGame(aiPlayer.Id); // AI 유저 내보내기
                    return;
                }
            }
            
        }

        
        // 누군가 주기적으로 호출해줘야 한다.
        public void Update()
        {
            if (PlayingGame == false)
            {
                Flush();
                return;
            }
            
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
        private AIPlayer CreatePlayer(PlayerType type)
        {
            AIPlayer aiPlayer = ObjectManager.Instance.Add<AIPlayer>();

            // 플레이어 정보 설정
            aiPlayer.Info.Name = $"AI-{type.ToString()}";
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
        
        private bool IsPointInsideRectangle(Vector3 point, Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight)
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
        
        public GameObject IsPointInsideCircle(GameObject player, Vector3 circleCenter, float radius)
        {
            GameObject target = null;
            
            foreach (var p in _allPlayers.Values)
            {
                if (p == player)
                    continue;
                
                if (p.PlayerType == player.PlayerType)
                    continue;
                
                // 원의 중심과 주어진 점 사이의 거리를 계산합니다.
                float distanceSquared = (p.CellPos.X - circleCenter.X) * (p.CellPos.X - circleCenter.X) +
                                        (p.CellPos.Z - circleCenter.Z) * (p.CellPos.Z - circleCenter.Z);

                // 거리가 반지름보다 작거나 같으면 원 안에 있는 것으로 판정합니다.
                bool check = distanceSquared <= radius * radius;

                // 플레이어의 위치가 skillPos 범위 안에 있는지 확인
                if (check)
                {
                    target = p;
                }
            }

            return target;
        }


        public bool ApplyMove(GameObject gameObject, Vector3 dest, float rotation)
        {
            if (gameObject.Room == null)
                return false;

            gameObject.Info.PosInfo = gameObject.PosInfo;
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
        
        private void ClearRoom()
        {
            WaitTime = 1;
            RunTimer = false;
            PlayingGame = false;
            
            _players.Clear();
            _aiPlayers.Clear();
            _allPlayers.Clear();
            _projectiles.Clear();
            
            RoomManager.Instance.Remove(RoomId);
        }
        
        /// <summary>
        /// 사람과 AI플레이어 구분하지 않고 그중에서 찾아줌.
        /// </summary>
        public GameObject FindPlayer(Func<GameObject, bool> condition1, Func<GameObject, bool> condition2)
        {
            foreach (GameObject player in _allPlayers.Values)
            {
                // 타겟이 될 플레이어를 찾는데 그 플레이에어 타켓이 붙어 있으면 안찾는다.
                if (player.Chaser != null)
                    continue;

                if (condition1.Invoke(player) == false)
                    continue;

                if (condition2.Invoke(player))
                    return player;
            }

            return null;
        }
        #endregion
        

        #region Packet
        public void ReadyGame()
        {
            S_ReadyGame readyPacket = new S_ReadyGame();
            Broadcast(readyPacket);
        }
        
        public void StartGame()
        {
            S_StartGame startPacket = new S_StartGame();
            Broadcast(startPacket);
        }
        
        public void StopGame()
        {
            S_StopGame stopPacket = new S_StopGame();
            stopPacket.Winner = FindWinner(); // 승리 종족 넣어주기
            Broadcast(stopPacket);
            
            S_LeaveGame leavePacket = new S_LeaveGame();
            Broadcast(leavePacket);
            
            ClearRoom();
        }

        private int FindWinner()
        {
            // 종족별 인원수를 배열에 담습니다.
            int[] counts = { RockNum, ScissorsNum, PaperNum };

            // 종족 중 인원이 가장 많은 종족을 찾습니다.
            int maxCount = counts.Max();
            
            // 세 종족의 인원 수가 모두 같은지 확인합니다.
            bool allEqual = counts.Distinct().Count() == 1;
            
            // 모든 종족의 수가 같다면
            if (allEqual)
                return 0; // draw!
            
            // 수가 가장 많은 종족을 선택합니다.
            return Array.IndexOf(counts, maxCount) + 1;
        }
        
        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null)
                return;
            
            if (_players.Count >= 12)
                return;

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);
            
            if (type == GameObjectType.Player)
            {
                WaitTime = 1; // 누군가 새로 들어오면 웨이팅 타임 초기화
                
                Player player = gameObject as Player;
                _players.Add(gameObject.Id, player);
                _allPlayers.Add(gameObject.Id, player);
                player.Room = this;
                
                ReplacePlayer(player);
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
                _allPlayers.Add(gameObject.Id, aiPlayer);
                aiPlayer.Room = this;
                
                ApplyMove(aiPlayer, new Vector3(aiPlayer.CellPos.X, 0, aiPlayer.CellPos.Z), aiPlayer.PosInfo.Rotation);
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = gameObject as Projectile;
                _projectiles.Add(gameObject.Id, projectile);
                projectile.Room = this;
                
                ApplyMove(projectile, new Vector3(projectile.CellPos.X, 0, projectile.CellPos.Z), projectile.PosInfo.Rotation);
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
            
            // 누군가 접속하면 타이머 작동 할 수 있게함
            RunTimer = true;
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
                
                // 룸에서 제거
                if (_allPlayers.Remove(objectId) == false)
                    return;
                
                // 오브젝트 매니저에서 제거
                if (ObjectManager.Instance.Remove(objectId) == false)
                    return;

                if (_players.Count <= 0)
                {
                    ClearRoom();
                    return;
                }
                
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
                
                // 룸에서 제거
                if (_allPlayers.Remove(objectId) == false)
                    return;
                
                // aiPlayer.Room = null;
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = null;
                if (_projectiles.Remove(objectId, out projectile) == false)
                    return;

                // projectile.Room = null;
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
        
        public void SpawnGame(GameObject gameObject)
        {
            S_Spawn spawnPacket = new S_Spawn();
            spawnPacket.Objects.Add(gameObject.Info);
            Broadcast(spawnPacket);
            
            // 한 종족이 12명이 되면 최종 승리하고 게임을 종료한다.
            if (RockNum == 12 || ScissorsNum == 12 || PaperNum == 12)
            {
                Push(StopGame); // 게임 종료 패킷 전송
                GameTime = 1;
            }
        }
        
        public void DeSpawnGame(int objectId)
        {
            S_Despawn despawnPacket = new S_Despawn();
            despawnPacket.ObjectIds.Add(objectId);
            Broadcast(despawnPacket);
        }

        public void HandleMove(GameObject player, C_Move movePacket)
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
        
        public void HandleS_Move(GameObject player, S_Move movePacket)
        {
            if (player == null)
                return;
            
            // TODO : 검증
            PositionInfo movePosInfo = movePacket.PosInfo;
            ObjectInfo info = player.Info;

            info.PosInfo = movePosInfo;
            player.PosInfo = movePosInfo;
            ApplyMove(player, new Vector3(movePosInfo.PosX, 0, movePosInfo.PosZ), movePosInfo.Rotation);
            Broadcast(movePacket);
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
                    
                    foreach (var p in _allPlayers.Values)
                    {
                        // 플레이어의 위치가 skillPos 범위 안에 있는지 확인
                        if (IsPointInsideRectangle(p.CellPos, topLeft, topRight, bottomLeft, bottomRight))
                        {
                            p.OnDamaged(player, 1);
                            // Console.WriteLine($"Hit Player!, CellPos : {p.CellPos}, SkillPos : {skillPos}, Rotation : {info.PosInfo.Rotation}");
                        }
                    }
                }
                break;
                
                case SkillType.SkillProjectile:
                {
                    // Hand Projectile
                    Hand hand = ObjectManager.Instance.Add<Hand>();
                    if (hand == null)
                        return;

                    hand.Owner = player;
                    hand.Data = skillData;
                    hand.PosInfo = player.PosInfo;
                    hand.State = CreatureState.Moving;
                    hand.Speed = skillData.Projectile.speed;
                    hand.PlayerType = player.PlayerType;
                    Push(EnterGame, hand);
                }
                break;
            }
        }
        
        public void HandleS_Skill(GameObject aiPlayer, GameObject target, S_Skill skillPacket)
        {
            if (aiPlayer == null)
                return;

            ObjectInfo info = aiPlayer.Info;
            
            info.PosInfo.State = CreatureState.Skill;
            
            Broadcast(skillPacket);

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
                    
                    Vector3 skillPos = aiPlayer.CellPos + characterForward;

                    // 캐릭터가 바라보는 방향 벡터와 오른쪽 방향 벡터를 계산
                    Vector3 right = Vector3.Cross(Vector3.UnitY, characterForward);

                    // 직사각형의 네 꼭지점 좌표 계산
                    Vector3 topLeft = skillPos + (-right * (width / 2)) + (characterForward * (height / 2));
                    Vector3 topRight = skillPos + (right * (width / 2)) + (characterForward * (height / 2));
                    Vector3 bottomLeft = skillPos + (-right * (width / 2)) + (-characterForward * (height / 2));
                    Vector3 bottomRight = skillPos + (right * (width / 2)) + (-characterForward * (height / 2));
                    
                    // Console.WriteLine($"topLeft : {topLeft}, topRight : {topRight}, bottomLeft : {bottomLeft}, bottomRight : {bottomRight}");
                    // Console.WriteLine("info.PosInfo.Rotation : " + info.PosInfo.Rotation);
                    
                    foreach (var p in _allPlayers.Values)
                    {
                        if (p.PlayerType == aiPlayer.PlayerType)
                            continue;
                        
                        // 플레이어의 위치가 skillPos 범위 안에 있는지 확인
                        if (IsPointInsideRectangle(p.CellPos, topLeft, topRight, bottomLeft, bottomRight))
                        {
                            p.OnDamaged(aiPlayer, skillData.damage);
                            // Console.WriteLine($"Hit AI Player!, CellPos : {p.CellPos}, SkillPos : {skillPos}, Rotation : {info.PosInfo.Rotation}");
                        }
                    }
                    // 플레이어의 위치가 skillPos 범위 안에 있는지 확인
                    if (IsPointInsideRectangle(target.CellPos, topLeft, topRight, bottomLeft, bottomRight) == false)
                        aiPlayer.Info.PosInfo.State = CreatureState.Moving;
                }
                break;
                
                case SkillType.SkillProjectile:
                {
                    // Hand Projectile
                    Hand hand = ObjectManager.Instance.Add<Hand>();
                    if (hand == null)
                        return;

                    hand.Owner = aiPlayer;
                    hand.Data = skillData;
                    hand.PosInfo = new PositionInfo() // 값 복사
                    { 
                        PosX = aiPlayer.CellPos.X, 
                        PosZ = aiPlayer.CellPos.Z, 
                        Rotation = aiPlayer.PosInfo.Rotation, 
                        State = CreatureState.Moving
                    };
                    hand.State = CreatureState.Moving;
                    hand.Speed = skillData.Projectile.speed;
                    hand.PlayerType = aiPlayer.PlayerType;
                    Push(EnterGame, hand);
                }
                break;
            }
        }
        
        public void Broadcast(IMessage packet)
        {
            foreach (Player p in _players.Values)
            {
                p.Session.Send(packet);
            }
        }
        #endregion
    }
}