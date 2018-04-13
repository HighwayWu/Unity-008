using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

// 用于同步
public class MoveRequest : BaseRequest {

    private Transform localPlayerTransform;
    private PlayerMove localPlayerMove;
    private int syncRate = 30;
    private Transform remotePlayerTransform;
    private Animator remotePlayerAnim;
    private bool isSyncRemotePlayer = false;
    private Vector3 pos;
    private Vector3 rotation;
    private float forward;

    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Move;
        base.Awake();
    }

    private void Start()
    {
        InvokeRepeating("SyncLocalPlayer", 1f, 1f / syncRate);
    }

    private void Update()
    {
        if (isSyncRemotePlayer)
        {
            SyncRemotePlayer();
            isSyncRemotePlayer = false;
        }
    }

    public MoveRequest SetLocalPlayer(Transform localPlayerTransform, PlayerMove localPlayerMove)
    {
        this.localPlayerTransform = localPlayerTransform;
        this.localPlayerMove = localPlayerMove;
        return this;
    }

    public MoveRequest SetRemotePlayer(Transform remotePlayerTransform)
    {
        this.remotePlayerTransform = remotePlayerTransform;
        this.remotePlayerAnim = remotePlayerTransform.GetComponent<Animator>();
        return this;
    }

    private void SyncLocalPlayer()
    {
        SendRequest(localPlayerTransform.position.x, localPlayerTransform.position.y, localPlayerTransform.position.z,
            localPlayerTransform.eulerAngles.x, localPlayerTransform.eulerAngles.y, localPlayerTransform.eulerAngles.z, localPlayerMove.forward);
    }

    // 同步远程角色
    private void SyncRemotePlayer()
    {
        this.remotePlayerTransform.position = pos;
        this.remotePlayerTransform.eulerAngles = rotation;
        this.remotePlayerAnim.SetFloat("Forward", forward);
    }

    private void SendRequest(float x, float y, float z, float rotationX, float rotationY, float rotationZ,float foward)
    {
        string data = string.Format("{0},{1},{2}|{3},{4},{5}|{6}", x, y, z, rotationX, rotationY, rotationZ, foward);
        base.SendRequest(data);
    }

    // 解析数据 并进行同步
    public override void OnResponse(string data)
    {
        string[] strs = data.Split('|');
        pos = UnityTools.ParseVector3(strs[0]);
        rotation = UnityTools.ParseVector3(strs[1]);
        forward = float.Parse(strs[2]);
        isSyncRemotePlayer = true;
    }
}
