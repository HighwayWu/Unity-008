using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    class GameController : BaseController
    {
        public GameController()
        {
            requestCode = RequestCode.Game;
        }

        public string StartGame(string data, Client client, Server server)
        {
            if (client.IsHouseOwner())
            {
                Room room = client.Room;
                room.BroadcastMessage(client, ActionCode.StartGame, ((int)ReturnCode.Success).ToString());
                room.StartTimer();
                return ((int)ReturnCode.Success).ToString();
            }
            else
            {
                return ((int)ReturnCode.Fail).ToString();
            }
        }

        // 同步移动
        public string Move(string data, Client client, Server serve)
        {
            Room room = client.Room;
            if(room != null)
                room.BroadcastMessage(client, ActionCode.Move, data); // 将房间同步的请求发给其余客户端
            return null; // 当前房间不需要响应
        }

        // 同步射箭
        public string Shoot(string data, Client client, Server serve)
        {
            Room room = client.Room;
            if (room != null)
                room.BroadcastMessage(client, ActionCode.Shoot, data);
            return null;
        }

        // 处理血量
        public string Attack(string data, Client client, Server server)
        {
            int damage = int.Parse(data);
            Room room = client.Room;
            if (room == null)
                return null;
            room.TakeDamage(damage, client);
            return null;
        }

        // 战斗时退出 销毁房间
        public string QuitBattle(string data, Client client, Server server)
        {
            Room room = client.Room;
            if (room != null)
            {
                room.BroadcastMessage(null, ActionCode.QuitBattle, "r");
                room.Close();
            }
            return null;
        }
    }
}
