using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;

public class RoomPanel : BasePanel {

    private Text localPlayerUsername;
    private Text localPlayerTotalCount;
    private Text localPlayerWinCount;
    private Text enemyPlayerUsername;
    private Text enemyPlayerTotalCount;
    private Text enemyPlayerWinCount;
    private Transform bluePanel;
    private Transform redPanel;
    private Transform startButton;
    private Transform exitButton;
    private UserData ud;
    private UserData ud1;
    private UserData ud2;
    private QuitRoomRequest quitRoomRequest;
    private StartGameRequest startGameRequest;
    private bool isPopPanel = false;

    private void Start()
    {
        localPlayerUsername = transform.Find("BluePanel/Username").GetComponent<Text>();
        localPlayerTotalCount = transform.Find("BluePanel/TotalCount").GetComponent<Text>();
        localPlayerWinCount = transform.Find("BluePanel/WinCount").GetComponent<Text>();
        enemyPlayerUsername = transform.Find("RedPanel/Username").GetComponent<Text>();
        enemyPlayerTotalCount = transform.Find("RedPanel/TotalCount").GetComponent<Text>();
        enemyPlayerWinCount = transform.Find("RedPanel/WinCount").GetComponent<Text>();

        bluePanel = transform.Find("BluePanel");
        redPanel = transform.Find("RedPanel");
        startButton = transform.Find("StartButton");
        exitButton = transform.Find("ExitButton");

        transform.Find("StartButton").GetComponent<Button>().onClick.AddListener(OnStartButtonClick);
        transform.Find("ExitButton").GetComponent<Button>().onClick.AddListener(OnExitButtonClick);

        quitRoomRequest = GetComponent<QuitRoomRequest>();
        startGameRequest = GetComponent<StartGameRequest>();

        EnterAnim();
    }

    public override void OnEnter()
    {
        if(bluePanel != null)
            EnterAnim();
    }

    public override void OnExit()
    {
        base.OnExit();
        HideAnim();
    }

    public override void OnPause()
    {
        base.OnPause();
        HideAnim();
    }

    public override void OnResume()
    {
        base.OnResume();
        EnterAnim();
    }

    private void Update()
    {
        if(ud != null)
        {
            SetLocalPlayerRes(ud.Username, ud.TotalCount.ToString(), ud.WinCount.ToString());
            ClearEnemyPlayerRes();
            ud = null;
        }
        if(ud1 != null) // 更新房间信息
        {
            SetLocalPlayerRes(ud1.Username, ud1.TotalCount.ToString(), ud1.WinCount.ToString());
            if (ud2 != null)
                SetEnemyPlayerRes(ud2.Username, ud2.TotalCount.ToString(), ud2.WinCount.ToString());
            else
                ClearEnemyPlayerRes();
            ud1 = null;
            ud2 = null;
        }
        if (isPopPanel)
        {
            uiManager.PopPanel();
            isPopPanel = false;
        }
    }

    public void SetLocalPlayerResSync()
    {
        ud = facade.GetUserData();
    }

    public void SetAllPlayerResSync(UserData ud1, UserData ud2)
    {
        this.ud1 = ud1;
        this.ud2 = ud2;
    }

    // 蓝色方
    public void SetLocalPlayerRes(string username, string totalCount, string winCount)
    {
        this.localPlayerUsername.text = username;
        this.localPlayerTotalCount.text = "Total : " + totalCount;
        this.localPlayerWinCount.text = "Win : " + winCount;
    }

    // 红色方
    private void SetEnemyPlayerRes(string username, string totalCount, string winCount)
    {
        this.enemyPlayerUsername.text = username;
        this.enemyPlayerTotalCount.text = "Total : " + totalCount;
        this.enemyPlayerWinCount.text = "Win : " + winCount;
    }

    public void ClearEnemyPlayerRes()
    {
        this.enemyPlayerUsername.text = "";
        this.enemyPlayerTotalCount.text = "Waiting...";
        this.enemyPlayerWinCount.text = "";
    }

    private void OnStartButtonClick()
    {
        PlayClickSound();
        startGameRequest.SendRequest();
    }

    private void OnExitButtonClick()
    {
        PlayClickSound();
        quitRoomRequest.SendRequest();
    }

    public void OnExitResponse()
    {
        isPopPanel = true;
    }

    public void OnStartResponse(ReturnCode returnCode)
    {
        if(returnCode == ReturnCode.Fail)
        {
            uiManager.ShowMessageSync("You are not owner.");
        }
        else
        {
            // 开始游戏
            uiManager.PushPanelSync(UIPanelType.Game);
            facade.EnterPlayingSync();
        }
    }

    private void EnterAnim()
    {
        gameObject.SetActive(true);
        bluePanel.localPosition = new Vector3(-1000, 0, 0);
        bluePanel.DOLocalMoveX(-173, 0.5f);
        redPanel.localPosition = new Vector3(1000, 0, 0);
        redPanel.DOLocalMoveX(188, 0.5f);
        startButton.localScale = Vector3.zero;
        startButton.DOScale(1, 0.5f);
        exitButton.localScale = Vector3.zero;
        exitButton.DOScale(1, 0.5f);
    }

    private void HideAnim()
    {
        bluePanel.DOLocalMoveX(-1000, 0.5f);
        redPanel.DOLocalMoveX(1000, 0.5f);
        startButton.DOScale(0, 0.5f);
        exitButton.DOScale(0, 0.5f).OnComplete(() => gameObject.SetActive(false));
    }
}
