using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
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
			
		if (room == null)
		{
			room = RoomManager.Instance.Add();
			Program.TickRoom(room, 50);
		}

		room.Push(room.EnterGame, player);
		room.RunTimer = true;
		room.WaitTime = 1;
		room.WaitPlayerTimer();
		
		// if (room.RunTimer == false)
		// {
		// 	room.RunTimer = true;
		// 	room.WaitPlayerTimer();
		// }
	}
}
