using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class LoginRequest : BaseRequest {
    private LoginPanel loginPanel;

    public override void Awake()
    {
        requestCode = RequestCode.User;
        actionCode = ActionCode.Login;
        loginPanel = GetComponent<LoginPanel>();
        base.Awake();
    }

    // 组拼要发送的数据
    public void SendRequest(string username, string password)
    {
        string data = username + "," + password;
        base.SendRequest(data);
    }

    // 解析返回的数据 success或fail 并且设置userData
    public override void OnResponse(string data)
    {
        string[] str = data.Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(str[0]);
        loginPanel.OnLoginResponse(returnCode);

        if(returnCode == ReturnCode.Success)
        {
            string username = str[1];
            int totalCount = int.Parse(str[2]);
            int winCount = int.Parse(str[3]);
            UserData ud = new UserData(username, totalCount, winCount);
            facade.SetUserData(ud);
        }
    }
}
