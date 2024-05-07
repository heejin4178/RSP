using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Server.Data;
using Server.Game;
using ServerCore;
using Timer = System.Timers.Timer;

namespace Server
{
	class Program
	{
		static Listener _listener = new Listener();
		private static List<Timer> _timers = new List<Timer>();

		static void TickRoom(GameRoom room, int tick = 100)
		{
			var timer = new System.Timers.Timer();
			timer.Interval = tick;
			timer.Elapsed += ((s, e) => { room.Update(); }); // 위 interval이 지나면 함수를 실행해주겠다.
			timer.AutoReset = true;
			timer.Enabled = true;
			
			_timers.Add(timer);
		}

		static void Main(string[] args)
		{
			ConfigManager.LoadConfig();
			DataManager.LoadData();
			
			GameRoom room = RoomManager.Instance.Add(1);
			TickRoom(room, 50);
			
			// DNS (Domain Name System)
			IPAddress ipAddr = IPAddress.Parse("192.168.45.71");
			IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine("Listening...");

			while (true)
			{
				// JobTimer.Instance.Flush();
				Thread.Sleep(100);
			}
		}
	}
}
