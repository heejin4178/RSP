using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using UnityEngine;

class PacketHandler
{ 
	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		
		Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
		
		GameObjectType objectType = GetObjectTypeById(enterGamePacket.Player.ObjectId);
		if (objectType == GameObjectType.Player)
		{
			Managers.Game.PlayerCount = 0;
			Managers.Game.PlayerCount++;
		}
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
			
			GameObjectType objectType = GetObjectTypeById(obj.ObjectId);
			if (objectType == GameObjectType.Player)
				Managers.Game.PlayerCount++;
		}
	}
	
	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		
		foreach (int id in despawnPacket.ObjectIds)
		{
			Managers.Object.Remove(id);
			GameObjectType objectType = GetObjectTypeById(id);
			if (objectType == GameObjectType.Player)
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
		// 여기서 2분 타이머를 실행한다.
		Managers.UI.GetSceneUI<UI_GameScene>().StartGameTimer();
	}
	
	public static void S_StopGameHandler(PacketSession session, IMessage packet)
	{
		// 여기서 시간 멈추고 게임 결과 UI 보여준다.
		// Time.timeScale = 0; // 혹시 모르니 서버에서도 게임이 종료되었다고 알려주고, 게임이 중단되지 않았다면 게임을 중단.
	}
	
	public static void S_StunHandler(PacketSession session, IMessage packet)
	{
		S_Stun stunPacket = packet as S_Stun;

		GameObject go = Managers.Object.FindById(stunPacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc != null)
		{
			cc.State = CreatureState.Stun;
		}
	}
	
	
	
	private static GameObjectType GetObjectTypeById(int id)
	{
		int type = (id >> 24) & 0x7F;
		return (GameObjectType)type;
	}
}
