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
            
            base.OnDamaged(attacker, damage);
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
                case CreatureState.Stun:
                    UpdateStun();
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
            GameObject target = Room.FindPlayer(FindTargetCondition1, FindTargetCondition2);

            if (target == null)
            {
                State = CreatureState.Idle;
                Console.WriteLine("target null");
                return;
            }
            
            Random random = new Random();
            _skillId = random.Next(1, 3);
            // _skillId = 2;
            
            // 타겟을 설정하고, 타겟의 추격자에 AI 플레이어를 설정해준다.
            _target = target;
            _target.Chaser = this;
            State = CreatureState.Moving;
        }
        
        private bool FindTargetCondition1(GameObject player)
        {
            // 본인은 제외함.
            if (player.Id == Id)
                return false;
            
            switch (PlayerType)
            {
                case PlayerType.Rock:
                    if (player.PlayerType == PlayerType.Scissors)
                        return true;
                    break;
                case PlayerType.Scissors:
                    if (player.PlayerType == PlayerType.Paper)
                        return true;
                    break;
                case PlayerType.Paper:
                    if (player.PlayerType == PlayerType.Rock)
                        return true;
                    break;
            }

            return false;
        }

        private bool FindTargetCondition2(GameObject player)
        {
            if (Vector3.Distance(player.CellPos, CellPos) < _searchCellDist)
                return true;

            return false;
        }

        private float _meleeSkillRange = 1.2f;
        private float _proSkillRange = 10.0f;
        private int _skillId;
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
                return;
            }
        
            float dist = Vector3.Distance(_target.CellPos, CellPos);

            switch (_skillId)
            {
                case 1:
                    // 스킬로 넘어갈지 체크
                    if (dist <= _meleeSkillRange)
                    {
                        _coolTick = 0;
                        State = CreatureState.Skill;
                        return;
                    }
                    break;
                case 2:
                    // 스킬로 넘어갈지 체크
                    if (dist <= _proSkillRange)
                    {
                        State = CreatureState.Skill;
                        return;
                    }
                    break;
            }

            Vector3 moveDirection = _target.CellPos - CellPos;
            float distance = (float)Math.Sqrt(moveDirection.X * moveDirection.X + moveDirection.Y * moveDirection.Y + moveDirection.Z * moveDirection.Z);
            moveDirection /= distance;
            
            // 이동할 거리를 방향 벡터에 곱하여 이동량을 계산합니다.
            Vector3 moveAmount = moveDirection * 1f;

            // 현재 위치에 이동량을 더합니다.
            CellPos += moveAmount;
            
            Vector3 forward = new Vector3(moveDirection.X, 0, moveDirection.Z);
            if (forward.Length() > 0)
            {
                // Y축 중심으로 회전 계산
                float targetAngle = (float)Math.Atan2(forward.X, forward.Z) * (180f / (float)Math.PI);
                Info.PosInfo.Rotation = targetAngle;
            }

            // 여기까지 오면 AI 플레이어를 이동함.
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
                if (_target == null || _target.Room != Room || _target.PlayerType == PlayerType)
                {
                    _target = null;
                    State = CreatureState.Idle;
                    BroadcastMove();
                    return;
                }
                
                Skill skillData = null;
                DataManager.SkillDict.TryGetValue(_skillId, out skillData);

                // 스킬 사용 모두에게 알림
                S_Skill skill = new S_Skill() { Info = new SkillInfo() };
                skill.ObjectId = Id;
                skill.Info.SkillId = skillData.id;
                Room.Push(Room.HandleS_Skill, this, _target, skill);
        
                // 스킬 쿨타임 적용
                int coolTick = (int)(1000 * skillData.cooldown);
                _coolTick = Environment.TickCount64 + coolTick;
            }

            if (_coolTick > Environment.TickCount64)
            {
                if (_coolTick - Environment.TickCount64 <= 4000 && _skillId == 2)
                {
                    _skillId = 1;
                    State = CreatureState.Moving;
                    BroadcastMove();
                }
                return;
            }
        
            _coolTick = 0;
        }

        private long _stunCoolTick = 0;
        protected virtual void UpdateStun()
        {
            if (_stunCoolTick == 0)
            {
                // 스킬 쿨타임 적용
                int coolTick = (int)(1000 * 5f);
                _stunCoolTick = Environment.TickCount64 + coolTick;
            }

            if (_stunCoolTick > Environment.TickCount64)
            {
                if (_target != null)
                {
                    State = CreatureState.Moving;
                    return;
                }
            }
            
            _stunCoolTick = 0;
        }
        
        protected virtual void UpdateDead()
        {
            Console.WriteLine("Finish!");
        }
    }
}