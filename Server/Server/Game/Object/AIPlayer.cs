using System;
using System.Collections.Generic;
using System.Numerics;
using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game
{
    public class AIPlayer : GameObject
    {
        public AIPlayer()
        {
            ObjectType = GameObjectType.Aiplayer;

            // TEMP
            Stat.Level = 1;
            Stat.Hp = 2;
            Stat.MaxHp = 2;
            Stat.Speed = 10.0f;

            State = CreatureState.Idle;
        }
        
        public override void OnDamaged(GameObject attacker, int damage)
        {
            // 같은 종족끼리는 공격할 수 없다.
            if (attacker.PlayerType == PlayerType)
                return;
            
            // 나를 공격할 수 있는 종족이 아니라면 공격할 수 없도록 한다.
            switch (PlayerType)
            {
                case PlayerType.Rock:
                    if (attacker.PlayerType == PlayerType.Scissors)
                        return;
                    break;
                case PlayerType.Scissors:
                    if (attacker.PlayerType == PlayerType.Paper)
                        return;
                    break;
                case PlayerType.Paper:
                    if (attacker.PlayerType == PlayerType.Rock)
                        return;
                    break;
            }
            
            State = CreatureState.Hit;
            base.OnDamaged(attacker, damage);
        }

        public override void OnHitProjectile(GameObject attacker)
        {
            base.OnHitProjectile(attacker);
        }

        // FSM (Finite State Machine)
        public override void Update()
        {
            switch (State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Moving:
                    UpdateMoving();
                    break;
                case CreatureState.Skill:
                    UpdateSkill();
                    break;
                case CreatureState.Dead:
                    UpdateDead();
                    break;
            }
        }
        
        private GameObject _target;
        private int _searchCellDist = 30;
        private int _chaseCellDist = 30;
        private long _nextSearchTick = 0;
        protected virtual void UpdateIdle()
        {
            if (_nextSearchTick > Environment.TickCount64)
                return;
            _nextSearchTick = Environment.TickCount64 + 1000;
        
            // 몬스터와 플레이어 사이의 거리를 측정하고 범위안에 있으면 true.
            // 내가 공격 할 수 있는 종족일때만 true.
            GameObject target = Room.FindPlayer(p =>
            {
                // 본인은 제외함.
                if (p.Id == Id)
                    return false;
                
                bool checkDist = false;
                bool checkPlayerType = false;

                if (Vector3.Distance(p.CellPos, CellPos) < _searchCellDist)
                    checkDist = true;
                
                switch (PlayerType)
                {
                    case PlayerType.Rock:
                        if (p.PlayerType == PlayerType.Scissors)
                            checkPlayerType = true;
                        break;
                    case PlayerType.Scissors:
                        if (p.PlayerType == PlayerType.Paper)
                            checkPlayerType = true;
                        break;
                    case PlayerType.Paper:
                        if (p.PlayerType == PlayerType.Rock)
                            checkPlayerType = true;
                        break;
                }
                
                return checkDist && checkPlayerType;
            });
            
            if (target == null)
                return;
            
            // 타겟을 설정하고, 타겟의 추격자에 AI 플레이어를 설정해준다.
            _target = target;
            _target.Chaser = this;
            State = CreatureState.Moving;
        }
        
        
        private float _skillRange = 0.2f;
        private long _nextMoveTick = 0;
        protected virtual void UpdateMoving()
        {
            if (_nextMoveTick > Environment.TickCount64)
                return;
            int moveTick = (int)(1000 / Speed);
            _nextMoveTick = Environment.TickCount64 + moveTick;
        
            // 몬스터가 쫓는 플레이어가 나가거나, 같은 룸에 없다면 멈춤
            if (_target == null || _target.Room != Room)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                Console.WriteLine("Here1");
                return;
            }
        
            float dist = Vector3.Distance(_target.CellPos, CellPos);
            // if (dist == 0 || dist > _chaseCellDist)
            // {
            //     _target = null;
            //     State = CreatureState.Idle;
            //     BroadcastMove();
            //     Console.WriteLine("Here2");
            //     return;
            // }
            
            // 스킬로 넘어갈지 체크
            if (dist <= _skillRange) // 대각선 공격을 막음
            {
                _coolTick = 0;
                State = CreatureState.Skill;
                Console.WriteLine("Here4");
                return;
            }
            
            // Vector3 distVector3 = _target.CellPos - CellPos;
            
            Vector3 moveDirection = _target.CellPos - CellPos;
            float distance = (float)Math.Sqrt(moveDirection.X * moveDirection.X + moveDirection.Y * moveDirection.Y + moveDirection.Z * moveDirection.Z);
            moveDirection /= distance;
            
            // 이동할 거리를 방향 벡터에 곱하여 이동량을 계산합니다.
            Vector3 moveAmount = moveDirection * 1f;

            // 현재 위치에 이동량을 더합니다.
            CellPos += moveAmount;
            if (PlayerType == PlayerType.Paper)
            {
                Console.WriteLine($"MyId :{Id}, Find TargetId : {_target.Id}, MyPos :{CellPos}, TargetPos :{_target.CellPos}");
            }

            // 여기까지 오면 몬스터를 이동함.
            BroadcastMove();
        }
        
        void BroadcastMove()
        {
            // 다른 플레이어한테도 알려준다
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            
            Room.Push(Room.HandleS_Move, this, movePacket);
        }
        
        private long _coolTick = 0;
        protected virtual void UpdateSkill()
        {
            if (_coolTick == 0)
            {
                // 유효한 타겟인지
                if (_target == null || _target.Room != Room || _target.Hp == 0)
                {
                    _target = null;
                    State = CreatureState.Moving;
                    BroadcastMove();
                    return;
                }
        
                // 스킬이 아직 사용 가능한지
                // Vector2Int dir = _target.CellPos - CellPos;
                // int dist = dir.cellDistFromZero;
                // bool canUseSkill = dist <= _skillRange && (dir.x == 0 || dir.y == 0);
                // if (canUseSkill == false)
                // {
                //     State = CreatureState.Moving;
                //     BroadcastMove();
                //     return;
                // }
                
                Skill skillData = null;
                DataManager.SkillDict.TryGetValue(1, out skillData);
        
                // 데미지 판정
                _target.OnDamaged(this, Stat.Attack);
        
                // 스킬 사용 모두에게 알림
                S_Skill skill = new S_Skill() { Info = new SkillInfo() };
                skill.ObjectId = Id;
                skill.Info.SkillId = skillData.id;
                Room.Broadcast(skill);
        
                // 스킬 쿨타임 적용
                int coolTick = (int)(1000 * skillData.cooldown);
                _coolTick = Environment.TickCount64 + coolTick;
            }
            
            if (_coolTick > Environment.TickCount64)
                return;
        
            _coolTick = 0;
        }
        
        protected virtual void UpdateDead()
        {
            
        }
    }
}