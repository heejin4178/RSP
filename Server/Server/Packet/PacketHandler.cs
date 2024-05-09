using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using Server.Data;
using Server.Game;

class PacketHandler
{
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;

		// Console.WriteLine($"C_Move({movePacket.PosInfo.PosX}, {movePacket.PosInfo.PosZ})");

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleMove, player, movePacket);
	}
	
	public static void C_SkillHandler(PacketSession session, IMessage packet)
	{
		C_Skill skillPacket = packet as C_Skill;
		ClientSession clientSession = session as ClientSession;
		
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleSkill, player, skillPacket);
	}
	
	public static void C_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		C_LeaveGame leavePacket = packet as C_LeaveGame;
		ClientSession clientSession = session as ClientSession;
		
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;
		
		room.Push(room.LeaveGame, leavePacket.ObjectId);
	}
	
	public static void C_EnterGameHandler(PacketSession session, IMessage packet)
	{
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;
		
		// 아직 플레이전인 룸을 찾고, 없다면 새로운 룸을 생성한다.
		GameRoom room = RoomManager.Instance.FindCanPlayRoom();
			
		// 룸이 없다면 새로생성 & AI 플레이어 넣어줌
		if (room == null)
		{
			room = RoomManager.Instance.Add();
			Program.TickRoom(room, 50);
		}
		// 룸이 있고 내가 첫번째 입장 유저라면, AI 플레이어 넣어줌
		else if (room.PlayersCount == 0)
		{
			room.Push(room.Init);
		}
		
		Console.WriteLine($"FindRoom C_EnterGameHandler : {room.RoomId}");

		// clientSession.MyPlayer = ObjectManager.Instance.Add<Player>();
		// {
		// 	clientSession.MyPlayer.Info.Name = $"Player_{clientSession.MyPlayer.Info.ObjectId}";
		// 	clientSession.MyPlayer.State = CreatureState.Idle;
		//
		// 	StatInfo stat = null;
		// 	DataManager.StatDict.TryGetValue(1, out stat);
		// 	clientSession.MyPlayer.Stat.MergeFrom(stat);
		// }
		
		// room.Push(room.ReplacePlayer, player); // 플레이어와 종족이 같은 AI 플레이와 교체함.
		room.Push(room.EnterGame, player);
		
		if (room.RunTimer == false)
			room.Push(room.ResetWaitTime);

		// 내가 첫번째로 들어가는 거라면, 웨이팅 타이머 실행
		if (room.PlayersCount == 0)
			room.Push(room.WaitPlayerTimer);
	}
}
