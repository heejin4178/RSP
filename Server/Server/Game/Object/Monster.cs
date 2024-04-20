// using System;
// using System.Collections.Generic;
// using Google.Protobuf.Protocol;
// using Microsoft.VisualBasic.CompilerServices;
// using Server.Data;
//
// namespace Server.Game
// {
//     public class Monster : GameObject
//     {
//         public Monster()
//         {
//             ObjectType = GameObjectType.Monster;
//             
//             // TEMP
//             Stat.Level = 1;
//             Stat.Hp = 100;
//             Stat.MaxHp = 100;
//             Stat.Speed = 5.0f;
//
//             State = CreatureState.Idle;
//         }
//         
//         // FSM (Finite State Machine)
//         public override void Update()
//         {
//             switch (State)
//             {
//                 case CreatureState.Idle:
//                     UpdateIdle();
//                     break;
//                 case CreatureState.Moving:
//                     UpdateMoving();
//                     break;
//                 case CreatureState.Skill:
//                     UpdateSkill();
//                     break;
//                 case CreatureState.Dead:
//                     UpdateDead();
//                     break;
//             }
//         }
//
//         private Player _target;
//         private int _searchCellDist = 10;
//         private int _chaseCellDist = 15;
//         private long _nextSearchTick = 0;
//         protected virtual void UpdateIdle()
//         {
//             if (_nextSearchTick > Environment.TickCount64)
//                 return;
//             _nextSearchTick = Environment.TickCount64 + 1000;
//
//             // // 몬스터와 플레이어 사이의 거리를 측정하고 범위안에 있으면 true를 리턴함.
//             // Player target = Room.FindPlayer(p =>
//             // {
//             //     Vector2Int dir = p.CellPos - CellPos;
//             //     return dir.cellDistFromZero < _searchCellDist;
//             // });
//             
//             // if (target == null)
//             //     return;
//             //
//             // _target = target;
//             // State = CreatureState.Moving;
//         }
//
//
//         private int _skillRange = 1;
//         private long _nextMoveTick = 0;
//         protected virtual void UpdateMoving()
//         {
//             if (_nextMoveTick > Environment.TickCount64)
//                 return;
//             int moveTick = (int)(1000 / Speed);
//             _nextMoveTick = Environment.TickCount64 + moveTick;
//
//             // 몬스터가 쫓는 플레이어가 나가거나, 같은 룸에 없다면 멈춤
//             if (_target == null || _target.Room != Room)
//             {
//                 _target = null;
//                 State = CreatureState.Idle;
//                 BroadcastMove();
//                 return;
//             }
//
//             Vector2Int dir = _target.CellPos - CellPos;
//             int dist = dir.cellDistFromZero;
//             if (dist == 0 || dist > _chaseCellDist)
//             {
//                 _target = null;
//                 State = CreatureState.Idle;
//                 BroadcastMove();
//                 return;
//             }
//
//             List<Vector2Int> path = Room.Map.FindPath(CellPos, _target.CellPos, checkObjects: false);
//             if (path.Count < 2 || path.Count > _chaseCellDist)
//             {
//                 _target = null;
//                 State = CreatureState.Idle;
//                 BroadcastMove();
//                 return;
//             }
//             
//             // 스킬로 넘어갈지 체크
//             if (dist <= _skillRange && (dir.x == 0 || dir.y == 0)) // 대각선 공격을 막음
//             {
//                 _coolTick = 0;
//                 State = CreatureState.Skill;
//                 return;
//             }
//             
//             // 여기까지 오면 몬스터를 이동함.
//             Dir = GetDirFromVec(path[1] - CellPos);
//             Room.Map.ApplyMove(this, path[1]);
//             BroadcastMove();
//         }
//
//         void BroadcastMove()
//         {
//             // 다른 플레이어한테도 알려준다
//             S_Move movePacket = new S_Move();
//             movePacket.ObjectId = Id;
//             movePacket.PosInfo = PosInfo;
//             Room.Broadcast(movePacket);
//         }
//
//         private long _coolTick = 0;
//         protected virtual void UpdateSkill()
//         {
//             if (_coolTick == 0)
//             {
//                 // 유효한 타겟인지
//                 if (_target == null || _target.Room != Room || _target.Hp == 0)
//                 {
//                     _target = null;
//                     State = CreatureState.Moving;
//                     BroadcastMove();
//                     return;
//                 }
//
//                 // 스킬이 아직 사용 가능한지
//                 Vector2Int dir = _target.CellPos - CellPos;
//                 int dist = dir.cellDistFromZero;
//                 bool canUseSkill = dist <= _skillRange && (dir.x == 0 || dir.y == 0);
//                 if (canUseSkill == false)
//                 {
//                     State = CreatureState.Moving;
//                     BroadcastMove();
//                     return;
//                 }
//                 
//                 // 타게팅 방향 주시
//                 MoveDir lookDir = GetDirFromVec(dir);
//                 if (Dir != lookDir)
//                 {
//                     Dir = lookDir;
//                     BroadcastMove();
//                 }
//
//                 Skill skillData = null;
//                 DataManager.SkillDict.TryGetValue(1, out skillData);
//
//                 // 데미지 판정
//                 _target.OnDamaged(this, skillData.damage + Stat.Attack);
//
//                 // 스킬 사용 모두에게 알림
//                 S_Skill skill = new S_Skill() { Info = new SkillInfo() };
//                 skill.ObjectId = Id;
//                 skill.Info.SkillId = skillData.id;
//                 Room.Broadcast(skill);
//
//                 // 스킬 쿨타임 적용
//                 int coolTick = (int)(1000 * skillData.cooldown);
//                 _coolTick = Environment.TickCount64 + coolTick;
//             }
//             
//             if (_coolTick > Environment.TickCount64)
//                 return;
//
//             _coolTick = 0;
//         }
//         
//         protected virtual void UpdateDead()
//         {
//             
//         }
//     }
// }