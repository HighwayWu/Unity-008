using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class PlayerManager : BaseManager {

    private UserData userData;
    private Dictionary<RoleType, RoleData> roleDataDict = new Dictionary<RoleType, RoleData>();
    private Transform playerPositions;
    private RoleType currentRoleType;
    private GameObject currentRole; // 当前控制的角色
    private GameObject playerSyncRequest;
    private GameObject remoteRole;
    private ShootRequest shootRequest;
    private AttackRequest attackRequest;

    public PlayerManager(GameFacade facade) : base (facade) { }

    public UserData UserData {
        set { userData = value; }
        get { return userData; }
    }

    public override void OnInit()
    {
        playerPositions = GameObject.Find("PlayerPositions").transform;
        InitRoleDataDict();
    }

    private void InitRoleDataDict()
    {
        roleDataDict.Add(RoleType.Blue, new RoleData(RoleType.Blue, "Hunter_BLUE", "Arrow_BLUE", "Explosion_BLUE", playerPositions.Find("Position1")));
        roleDataDict.Add(RoleType.Red, new RoleData(RoleType.Red, "Hunter_RED", "Arrow_RED", "Explosion_RED", playerPositions.Find("Position2")));
    }

    // 生成角色
    public void SpawnRoles()
    {
        foreach(RoleData rd in roleDataDict.Values)
        {
            GameObject go = GameObject.Instantiate(rd.RolePrefab, rd.SpawnPosition, Quaternion.identity);
            go.tag = "Player";
            if (rd.RoleType == currentRoleType)
            {
                currentRole = go;
                currentRole.GetComponent<PlayerInfo>().isLocal = true;
            }
            else
                remoteRole = go;
        }
    }

    public void SetCurrentRoleType(RoleType rt)
    {
        currentRoleType = rt;
    }

    public GameObject GetCurrentRole()
    {
        return currentRole;
    }

    public RoleData GetRoleData(RoleType rt)
    {
        RoleData rd = null;
        roleDataDict.TryGetValue(rt, out rd);
        return rd;
    }

    // 添加控制脚本 初始角色是不能移动攻击
    public void AddControlScript()
    {
        currentRole.AddComponent<PlayerMove>();
        PlayerAttack playerAttack = currentRole.AddComponent<PlayerAttack>();
        RoleType rt = currentRole.GetComponent<PlayerInfo>().roleType;
        RoleData rd = GetRoleData(rt);
        playerAttack.arrowPrefab = rd.ArrowPrefab;
        playerAttack.SetPlayerManager(this);
    }

    // 同步角色请求
    public void CreateSyncRequest()
    {
        playerSyncRequest = new GameObject("PlayerSyncRequest");
        // 添加同步脚本
        playerSyncRequest.AddComponent<MoveRequest>().SetLocalPlayer(currentRole.transform, currentRole.GetComponent<PlayerMove>())
            .SetRemotePlayer(remoteRole.transform);
        shootRequest = playerSyncRequest.AddComponent<ShootRequest>();
        shootRequest.playerManager = this;
        attackRequest = playerSyncRequest.AddComponent<AttackRequest>();
    }

    public void Shoot(GameObject arrowPrefab, Vector3 pos, Quaternion rotation)
    {
        facade.PlayNormalSound(AudioManager.Sound_Timer);
        // 本地创建箭
        GameObject.Instantiate(arrowPrefab, pos, rotation).GetComponent<Arrow>().isLocal = true;
        // 通知其他客户端生成箭
        shootRequest.SendRequest(arrowPrefab.GetComponent<Arrow>().roleType, pos, rotation.eulerAngles);
    }

    public void RemoteShoot(RoleType rt, Vector3 pos, Vector3 rotation)
    {
        // 将其他客户端产生的箭实例化
        GameObject arrowPrefab = GetRoleData(rt).ArrowPrefab;
        Transform transform = GameObject.Instantiate(arrowPrefab).GetComponent<Transform>();
        transform.position = pos;
        transform.eulerAngles = rotation;
    }

    public void SendAttack(int damage)
    {
        attackRequest.SendRequest(damage);
    }

    public void GameOver()
    {
        GameObject.Destroy(currentRole);
        GameObject.Destroy(playerSyncRequest);
        GameObject.Destroy(remoteRole);
        shootRequest = null;
        attackRequest = null;
    }

    public void UpdateResult(int totalCount, int winCount)
    {
        userData.TotalCount = totalCount;
        userData.WinCount = winCount;
    }
}
