using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;

public class Message {

    private byte[] data = new byte[1024];
    private int startIndex = 0; // 已经在数组里存取了多少个数据

    public byte[] Data
    {
        get
        {
            return data;
        }
    }

    public int StartIndex
    {
        get
        {
            return startIndex;
        }
    }

    public int RemainSize
    {
        get
        {
            return data.Length - startIndex;
        }
    }

    // 解析数据
    public void ReadMessage(int newDataAmount, Action<ActionCode, string> processDataCallBack)
    {
        startIndex += newDataAmount;
        while (true)
        {
            if (startIndex <= 4)
                return;
            int count = BitConverter.ToInt32(data, 0);
            if ((startIndex - 4) >= count)
            {

                ActionCode actionCode = (ActionCode)BitConverter.ToInt32(data, 4);
                string s = Encoding.UTF8.GetString(data, 8, count - 4);
                processDataCallBack(actionCode, s);
                Array.Copy(data, 4 + count, data, 0, startIndex - 4 - count);
                startIndex -= (4 + count);
            }
            else
            {
                break;
            }
        }
    }

    public static byte[] PackData(RequestCode requestCode, ActionCode actionCode, string data)
    {
        byte[] requestCodeBytes = BitConverter.GetBytes((int)requestCode);
        byte[] actionCodeBytes = BitConverter.GetBytes((int)actionCode);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        int dataAmount = requestCodeBytes.Length + actionCodeBytes.Length + dataBytes.Length;
        byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);

        return dataAmountBytes.Concat(requestCodeBytes).ToArray()
            .Concat(actionCodeBytes).ToArray()
            .Concat(dataBytes).ToArray();
    }
}
