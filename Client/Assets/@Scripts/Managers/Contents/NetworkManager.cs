using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;

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
		
		// DNS (Domain Name System)
		// IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
		IPAddress ipAddr = IPAddress.Parse("192.168.45.71");
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

		Connector connector = new Connector();

		connector.Connect(endPoint,
			() => { return _session; },
			1);

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
		_session.Disconnect();
	}

}
