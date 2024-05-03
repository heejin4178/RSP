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
		C_LeaveGame leavePacket = packet as C_LeaveGame;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;
		
		// 여기서 다시 룸을 찾을때 이미 게임이 진행 중인 룸은 제외하고 찾아준다.
		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.EnterGame, player);
	}
}
