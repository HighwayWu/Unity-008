using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using Common;

// 用于管理跟服务器端的socket连接, 在接收到服务器的消息之后发送给RequestManager
public class ClientManager : BaseManager {
    
    private const string IP = "127.0.0.1";
    private const int PORT = 6688;
    private Socket clientSocket;
    private Message msg = new Message();

    public ClientManager(GameFacade facade) : base(facade) { }

    public override void OnInit()
    {
        base.OnInit();

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            clientSocket.Connect(IP, PORT);
            Start();
        }
        catch(Exception e)
        {
            Debug.LogWarning("无法连接到服务器端, 请检查您的网络, " + e);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        try
        {
            clientSocket.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning("无法关闭跟服务器端的连接, " + e);
        }
    }

    private void Start()
    {
        clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, null);
    }

    private void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            if (clientSocket == null || !clientSocket.Connected)
                return;

            int count = clientSocket.EndReceive(ar);

            msg.ReadMessage(count, OnProcessDataCallBack);

            Start();
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    private void OnProcessDataCallBack(ActionCode actionCode, string data)
    {
        facade.HandleResponse(actionCode, data);
    }

    // 发送数据给服务器端
    public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data)
    {
        byte[] bytes = Message.PackData(requestCode, actionCode, data);
        clientSocket.Send(bytes);
    }
}
