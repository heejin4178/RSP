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

		bool myPlayer = false;

		foreach (ObjectInfo obj in spawnPacket.Objects)
		{
			GameObjectType objectType = GetObjectTypeById(obj.ObjectId);
			if (objectType == GameObjectType.Player)
				Managers.Game.PlayerCount++;
			
			if (obj.ObjectId == Managers.Object.MyPlayer.Id)
				myPlayer = true;

			Managers.Object.Add(obj, myPlayer);
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
    // 패킷을 S_Move로 캐스팅하여 이동 패킷 객체를 가져옵니다.
    S_Move movePacket = packet as S_Move;

    // 이동 대상의 게임 오브젝트를 찾습니다.
    GameObject go = Managers.Object.FindById(movePacket.ObjectId);
    if (go == null)
        return;

    // 이동 대상이 자신의 플레이어인 경우 처리를 종료합니다.
    if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
        return;

    // 이동 대상의 BaseController 컴포넌트를 가져옵니다.
    BaseController bc = go.GetComponent<BaseController>();
    if (bc == null)
        return;

    // 이동 패킷에서 받은 위치 정보를 이동 대상의 위치 정보로 설정합니다.
    bc.PosInfo = movePacket.PosInfo;
}

public static void S_SkillHandler(PacketSession session, IMessage packet)
{
    // 패킷을 S_Skill로 캐스팅하여 스킬 패킷 객체를 가져옵니다.
    S_Skill skillPacket = packet as S_Skill;

    // 스킬을 사용하는 대상의 게임 오브젝트를 찾습니다.
    GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
    if (go == null)
        return;

    // 스킬을 사용하는 대상이 자신의 플레이어인 경우 처리를 종료합니다.
    if (Managers.Object.MyPlayer.Id == skillPacket.ObjectId)
        return;

    // 스킬을 사용하는 대상의 PlayerController 컴포넌트를 가져옵니다.
    PlayerController pc = go.GetComponent<PlayerController>();
    if (pc != null)
    {
        // 대상의 상태를 스킬 상태로 변경합니다.
        pc.State = CreatureState.Skill;
    }
}

public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
{
    // 패킷을 S_ChangeHp로 캐스팅하여 체력 변화 패킷 객체를 가져옵니다.
    S_ChangeHp changePacket = packet as S_ChangeHp;

    // 체력 변화 대상의 게임 오브젝트를 찾습니다.
    GameObject go = Managers.Object.FindById(changePacket.ObjectId);
    if (go == null)
        return; // 대상이 존재하지 않으면 처리 종료합니다.

    // 체력 변화 대상의 CreatureController 컴포넌트를 가져옵니다.
    CreatureController cc = go.GetComponent<CreatureController>();
    if (cc != null)
    {
        // 대상의 상태를 피격 상태로 변경하고 체력을 업데이트합니다.
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

		if (diePacket.AttackerId == Managers.Object.MyPlayer.Id)
			Managers.Game.KillCount++;
		
		if (diePacket.ObjectId == Managers.Object.MyPlayer.Id)
			Managers.Game.DeathCount++;
		
		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc != null)
			cc.Hp = 0;
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
		S_StopGame stopGamePacket = packet as S_StopGame;

		// 여기서 시간 멈추고 게임 결과 UI 보여준다.
		Managers.Game.Winner = stopGamePacket.Winner;
		Debug.Log($"stopGamePacket {Managers.Game.Winner}");
		Managers.UI.ShowPopup<UI_GameResultPopup>();
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
