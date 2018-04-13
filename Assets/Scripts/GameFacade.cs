using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class GameFacade : MonoBehaviour {

    // 单例模式
    private static GameFacade _instance;
    public static GameFacade Instance { get {
            if (_instance == null)
            {
                GameObject obj = GameObject.Find("GameFacade");
                if (obj == null)
                    return null;
                _instance = obj.GetComponent<GameFacade>();
            }
            return _instance;
        } }

    private UIManager uiManager;
    private AudioManager audioManager;
    private PlayerManager playerManager;
    private CameraManager cameraManager;
    private RequestManager requestManager;
    private ClientManager clientManager;
    private bool isEnterPlaying = false;

    void Awake()
    {
        //if(_instance != null)
        //{
        //    Destroy(this.gameObject);
        //    return;
        //}
        //_instance = this;
        Screen.SetResolution(1280, 800, false);
    }

    void Start()
    {
        InitManager();
    }

    void Update()
    {
        UpdateManager();
        if (isEnterPlaying)
        {
            EnterPlaying();
            isEnterPlaying = false;
        }
    }

    void OnDestroy()
    {
        DestroyManager();
    }

    private void InitManager()
    {
        uiManager = new UIManager(this);
        audioManager = new AudioManager(this);
        playerManager = new PlayerManager(this);
        cameraManager = new CameraManager(this);
        requestManager = new RequestManager(this);
        clientManager = new ClientManager(this);

        uiManager.OnInit();
        audioManager.OnInit();
        playerManager.OnInit();
        cameraManager.OnInit();
        requestManager.OnInit();
        clientManager.OnInit();
    }

    private void DestroyManager()
    {
        uiManager.OnDestroy();
        audioManager.OnDestroy();
        playerManager.OnDestroy();
        cameraManager.OnDestroy();
        requestManager.OnDestroy();
        clientManager.OnDestroy();
    }

    private void UpdateManager()
    {
        uiManager.Update();
        audioManager.Update();
        playerManager.Update();
        cameraManager.Update();
        requestManager.Update();
        clientManager.Update();
    }

    public void AddRequest(ActionCode actionCode, BaseRequest request)
    {
        requestManager.AddRequest(actionCode, request);
    }

    public void RemoveRequest(ActionCode actionCode)
    {
        requestManager.RemoveRequest(actionCode);
    }

    public void HandleResponse(ActionCode actionCode, string data)
    {
        requestManager.HandleResponse(actionCode, data);
    }

    public void ShowMessage(string msg)
    {
        uiManager.ShowMessage(msg);
    }

    public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data)
    {
        clientManager.SendRequest(requestCode, actionCode, data);
    }

    public void PlayBgSound(string soundName)
    {
        audioManager.PlayBgSound(soundName);
    }

    public void PlayNormalSound(string soundName)
    {
        audioManager.PlayNormalSound(soundName);
    }

    public void SetUserData(UserData userData)
    {
        playerManager.UserData = userData;
    }

    public UserData GetUserData()
    {
        return playerManager.UserData;
    }

    public void SetCurrentRoleType(RoleType rt)
    {
        playerManager.SetCurrentRoleType(rt);
    }

    public GameObject GetCurrentRole()
    {
        return playerManager.GetCurrentRole();
    }

    public void EnterPlayingSync()
    {
        isEnterPlaying = true;
    }

    private void EnterPlaying()
    {
        playerManager.SpawnRoles();
        cameraManager.FollowRole();
    }

    public void StartPlaying()
    {
        playerManager.AddControlScript();
        playerManager.CreateSyncRequest();
    }

    public void SendAttack(int damage)
    {
        playerManager.SendAttack(damage);
    }

    public void GameOver()
    {
        cameraManager.WalkthoughScene();
        playerManager.GameOver();
    }

    public void UpdateResult(int totalCount, int winCount)
    {
        playerManager.UpdateResult(totalCount, winCount);
    }
}
