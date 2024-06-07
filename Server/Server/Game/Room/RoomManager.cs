using System.Collections.Generic;

namespace Server.Game
{
    // RoomManager 클래스는 게임 룸을 관리하는 싱글톤 클래스입니다.
    public class RoomManager
    {
        public static RoomManager Instance { get; } = new RoomManager();
        private object _lock = new object();
        private Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        private int _roomId = 1;

        /// <summary>
        /// 새로운 게임 방을 추가하고 초기화하는 메서드입니다.
        /// </summary>
        /// <returns>추가된 게임 방 객체</returns>
        public GameRoom Add()
        {
            // 새로운 GameRoom 객체를 생성합니다.
            GameRoom gameRoom = new GameRoom();

            // GameRoom 초기화 작업을 진행합니다.
            gameRoom.Init();
            
            lock (_lock)
            {
                // 새로운 GameRoom 객체에 방 ID를 할당합니다.
                gameRoom.RoomId = _roomId;

                // _rooms 딕셔너리에 새로운 GameRoom 객체를 추가합니다.
                _rooms.Add(_roomId, gameRoom);

                // 다음 방 ID를 위해 _roomId를 증가시킵니다.
                _roomId++;
            }
            
            return gameRoom;
        }

        /// <summary>
        /// 특정 방 ID를 가진 게임 방을 제거하는 메서드입니다.
        /// </summary>
        /// <param name="roomId">제거할 게임 방의 ID</param>
        /// <returns>제거 성공 여부</returns>
        public bool Remove(int roomId)
        {
            lock (_lock)
            {
                return _rooms.Remove(roomId);
            }
        }

        /// <summary>
        /// 특정 방 ID를 가진 게임 방을 찾는 메서드입니다.
        /// </summary>
        /// <param name="roomId">찾을 게임 방의 ID</param>
        /// <returns>찾은 게임 방 객체</returns>
        public GameRoom Find(int roomId)
        {
            lock (_lock)
            {
                // 지정된 roomId를 가진 GameRoom 객체를 찾습니다.
                GameRoom room = null;
                if (_rooms.TryGetValue(roomId, out room))
                    return room;

                return null;
            }
        }

        /// <summary>
        /// 플레이 가능한 게임 방을 찾는 메서드입니다.
        /// </summary>
        /// <returns>플레이 가능한 게임 방 객체</returns>
        public GameRoom FindCanPlayRoom()
        {
            lock (_lock)
            {
                foreach (var room in _rooms.Values)
                {
                    // 게임이 시작되지 않았고, 최대 플레이어 수 미만인 방을 찾습니다.
                    if (room.PlayingGame == false && room.PlayersCount < 12)
                        return room;
                }
                
                return null;
            }
        }
    }
}
