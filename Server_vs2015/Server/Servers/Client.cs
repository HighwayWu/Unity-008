using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Common;
using MySql.Data.MySqlClient;
using GameServer.Tool;
using GameServer.Model;
using GameServer.DAO;

namespace GameServer.Servers
{
    class Client
    {
        private Socket clientSocket;
        private Server server;
        private Message msg = new Message();
        private MySqlConnection mysqlConn;
        private Room room;
        private User user;
        private Result res;
        private ResultDAO resultDAO = new ResultDAO();

        public int HP { get; set; }
        
        public Client() { }

        public Client(Socket clientSocket, Server server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
            mysqlConn = ConnHelper.Connect();
        }

        // 开始接收客户端的消息
        public void Start()
        {
            if (clientSocket == null || !clientSocket.Connected)
                return;
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, null);
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            try {
                if (clientSocket == null || !clientSocket.Connected)
                    return;
                int count = clientSocket.EndReceive(ar);
                if (count == 0)
                {
                    Close();
                }
                msg.ReadMessage(count, OnProcessMessage);
                Start();

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Close();
            }
        }

        // 处理接收到的消息
        private void OnProcessMessage(RequestCode requestCode, ActionCode actionCode, string data)
        {
            server.HandleRequest(requestCode, actionCode, data, this);
        }

        private void Close()
        {
            ConnHelper.CloseConnection(mysqlConn);
            if(clientSocket != null)
                clientSocket.Close();
            if (room != null)
                room.QuitRoom(this);
            server.RemoveClient(this);
        }

        // 发送响应给客户端
        public void Send(ActionCode actionCode, string data)
        {
            try
            {
                byte[] bytes = Message.PackData(actionCode, data);
                clientSocket.Send(bytes);
            }
            catch(Exception e)
            {
                Console.WriteLine("无法发送消息: " + e);
            }
        }

        public MySqlConnection MySqlConn
        {
            get
            {
                return mysqlConn;
            }
        }

        public void SetUserData(User user, Result result)
        {
            this.user = user;
            this.res = result;
        }

        public string GetUserData()
        {
            return user.ID + "," + user.Username + "," + res.TotalCount + "," + res.WinCount;
        }

        public int GetUserID()
        {
            return user.ID;
        }

        public Room Room
        {
            get { return room; }
            set { room = value; }
        }

        public bool IsHouseOwner()
        {
            return room.IsHouseOwner(this);
        }

        public bool TakeDamage(int damage)
        {
            HP -= damage;
            HP = Math.Max(HP, 0);
            if (HP <= 0)
                return true;
            else
                return false;
        }

        public bool IsDie()
        {
            return HP <= 0;
        }

        public void UpdateResult(bool isVictory)
        {
            UpdateResultToDB(isVictory);
            UpdateResultToClient();
        }

        private void UpdateResultToDB(bool isVictory)
        {
            res.TotalCount++;
            if (isVictory)
                res.WinCount++;
            resultDAO.UpdateOrAddResult(mysqlConn, res);
        }

        private void UpdateResultToClient()
        {
            Send(ActionCode.UpdateResult, string.Format("{0},{1}", res.TotalCount, res.WinCount));
        }
    }
}
