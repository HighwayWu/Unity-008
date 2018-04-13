using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class RegistRequest : BaseRequest {

    private RegistPanel registPanel;

    public override void Awake()
    {
        requestCode = RequestCode.User;
        actionCode = ActionCode.Regist;
        registPanel = GetComponent<RegistPanel>();
        base.Awake();
    }

    // 组拼要发送的数据
    public void SendRequest(string username, string password)
    {
        string data = username + "," + password;
        base.SendRequest(data);
    }

    // 解析返回的数据 success或fail
    public override void OnResponse(string data)
    {
        ReturnCode returnCode = (ReturnCode)int.Parse(data);
        registPanel.OnRegistResponse(returnCode);
    }
}
