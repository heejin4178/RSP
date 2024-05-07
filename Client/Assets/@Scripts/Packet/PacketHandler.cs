﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using UnityEngine;

class PacketHandler
{ 
	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		
		Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
		Managers.Game.PlayerCount++;
	}
	
	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		Managers.Object.Clear();
		Managers.Game.PlayerCount--;
	}
	
	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;

		foreach (ObjectInfo obj in spawnPacket.Objects)
		{
			Managers.Object.Add(obj, myPlayer: false);
			Managers.Game.PlayerCount++;
		}
	}
	
	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		
		foreach (int id in despawnPacket.ObjectIds)
		{
			Managers.Object.Remove(id);
			Managers.Game.PlayerCount--;
		}
	}
	
	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;

		GameObject go = Managers.Object.FindById(movePacket.ObjectId);
		if (go == null)
			return;
		
		if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
			return;

		BaseController bc = go.GetComponent<BaseController>();
		if (bc == null)
			return;

		bc.PosInfo = movePacket.PosInfo;
	}
	
	public static void S_SkillHandler(PacketSession session, IMessage packet)
	{
		S_Skill skillPacket = packet as S_Skill;

		GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
		if (go == null)
			return;
		
		if (Managers.Object.MyPlayer.Id == skillPacket.ObjectId)
			return;

		PlayerController pc = go.GetComponent<PlayerController>();
		if (pc != null)
		{
			pc.State = CreatureState.Skill;
		}
	}
	
	public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
	{
		S_ChangeHp changePacket = packet as S_ChangeHp;

		GameObject go = Managers.Object.FindById(changePacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc != null)
		{
			cc.State = CreatureState.Hit;
			cc.Hp = changePacket.Hp;
		}
	}
	
	public static void S_DieHandler(PacketSession session, IMessage packet)
	{
		S_Die diePacket = packet as S_Die;

		GameObject go = Managers.Object.FindById(diePacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc != null)
		{
			cc.Hp = 0;
			// cc.OnDead();
		}
	}
	
	public static void S_ReadyGameHandler(PacketSession session, IMessage packet)
	{
		Managers.UI.CloseSceneUI();
		Managers.UI.ShowSceneUI<UI_GameScene>();
	}
	
	public static void S_StartGameHandler(PacketSession session, IMessage packet)
	{
		
	}
	
	public static void S_StopGameHandler(PacketSession session, IMessage packet)
	{
		
	}
}
