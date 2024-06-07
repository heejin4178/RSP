using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;
using Google.Protobuf.Protocol;

public class NetworkManager
{
	ServerSession _session = new ServerSession();

	public void Send(IMessage packet)
	{
		_session.Send(packet);
	}

	public bool Init()
	{
		// 이중 접속을 방지함
		if (_session.SessionSocket != null)
			return false;
		
		IPHostEntry hostEntry = Dns.GetHostEntry("rspgame.net");
		IPAddress ipAddr = hostEntry.AddressList[0]; // 첫 번째 IP 주소 사용
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

		Connector connector = new Connector();

		connector.Connect(endPoint,
			() => { return _session; },
			1);
		
		C_EnterGame enterPacket = new C_EnterGame();
		enterPacket.Name = Managers.Game.NickName; // 닉네임 패킷에 포함
		Managers.Network.Send(enterPacket);
		
		return true;
	}

	public void Update()
	{
		if (_session == null)
			return;
		
		List<PacketMessage> list = PacketQueue.Instance.PopAll();
		foreach (PacketMessage packet in list)
		{
			Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
			if (handler != null)
				handler.Invoke(_session, packet.Message);
		}	
	}

	public void Disconnect()
	{
		if (_session == null)
			return;
		
		_session.Disconnect();
	}

	public bool CheckSessionConnected()
	{
		// 이중 접속을 방지함
		if (_session.SessionSocket != null)
			return true;

		return false;
	}

}
