using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class ObjectManager
    {
        public static ObjectManager Instance { get; } = new ObjectManager();

        private object _lock = new object();
        private Dictionary<int, Player> _players = new Dictionary<int, Player>(); // 다른 플레이어를 찾아서 무언갈 하고 싶을때 사용
        
        // [UNUSED(1) TYPE(7)] [ID(24)]
        private int _counter = 1;
        
        public T Add<T>() where T : GameObject, new ()
        {
            T gameObject = new T();

            lock (_lock)
            {
                gameObject.Id = GenerateId(gameObject.ObjectType);

                if (gameObject.ObjectType == GameObjectType.Player)
                {
                    // 가위, 바위, 보 중 랜덤으로 선택됨
                    Random random = new Random();
                    gameObject.PlayerType = (PlayerType)random.Next(1, 4);

                    _players.Add(gameObject.Id, gameObject as Player);
                }
            }
            
            
            // foreach (var player in _players.Values)
            // {
            //     switch (player.PlayerType)
            //     {
            //         case PlayerType.Rock:
            //             RockNum++;
            //             break;
            //         case PlayerType.Scissors:
            //             ScissorsNum++;
            //             break;
            //         case PlayerType.Paper:
            //             PaperNum++;
            //             break;
            //     }
            // }

            return gameObject;
        }

        int GenerateId(GameObjectType type)
        {
            lock (_lock)
            {
                return ((int)type << 24) | (_counter++);
            }
        }

        public static GameObjectType GetObjectTypeById(int id)
        {
            int type = (id >> 24) & 0x7F;
            return (GameObjectType)type;
        }

        public bool Remove(int objectId)
        {
            GameObjectType objectType = GetObjectTypeById(objectId);
            
            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                    return _players.Remove(objectId);
            }

            return false;
        }

        public Player Find(int objectId)
        {
            GameObjectType objectType = GetObjectTypeById(objectId);

            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                {
                    Player player = null;
                    if (_players.TryGetValue(objectId, out player))
                        return player;
                }
            }
            
            return null;
        }
    }
}