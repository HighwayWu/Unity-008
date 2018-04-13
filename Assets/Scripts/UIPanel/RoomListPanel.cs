using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;

public class RoomListPanel : BasePanel {

    private RectTransform battleRes;
    private RectTransform roomList;
    private VerticalLayoutGroup roomLayout;
    private GameObject roomItemPrefabs;
    private ListRoomRequest listRoomRequest;
    private List<UserData> udList = null;
    private CreateRequest createRequest;
    private JoinRoomRequest joinRoomRequest;
    private UserData ud1 = null;
    private UserData ud2 = null;

    void Start()
    {
        battleRes = transform.Find("BattleRes").GetComponent<RectTransform>();
        roomList = transform.Find("RoomList").GetComponent<RectTransform>();
        roomLayout = transform.Find("RoomList/ScrollRect/Layout").GetComponent<VerticalLayoutGroup>();
        roomItemPrefabs = Resources.Load("UIPanel/RoomItem") as GameObject;
        transform.Find("RoomList/CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseButtonClick);
        transform.Find("RoomList/CreateButton").GetComponent<Button>().onClick.AddListener(OnCreateRoomButtonClick);
        transform.Find("RoomList/RefreshButton").GetComponent<Button>().onClick.AddListener(OnRefreshButtonClick);
        listRoomRequest = GetComponent<ListRoomRequest>();
        createRequest = GetComponent<CreateRequest>();
        joinRoomRequest = GetComponent<JoinRoomRequest>();
        EnterAnim();
    }

    public override void OnEnter()
    {
        SetBattleRes();
        if (battleRes != null)       
            EnterAnim();
        if (listRoomRequest == null)
            listRoomRequest = GetComponent<ListRoomRequest>();
        listRoomRequest.SendRequest();
    }

    public override void OnExit()
    {
        HideAnim();
    }

    public override void OnPause()
    {
        HideAnim();
    }

    public override void OnResume()
    {
        EnterAnim();
        listRoomRequest.SendRequest();
    }

    private void Update()
    {
        if(udList != null)
        {
            LoadRoomItem(udList);
            udList = null;
        }
        if(ud1 != null && ud2 != null)
        {
            BasePanel panel = uiManager.PushPanel(UIPanelType.Room);
            (panel as RoomPanel).SetAllPlayerResSync(ud1, ud2);
            ud1 = null;
            ud2 = null;
        }
    }

    private void OnCloseButtonClick()
    {
        PlayClickSound();
        uiManager.PopPanel();
    }

    private void OnCreateRoomButtonClick()
    {
        PlayClickSound();
        BasePanel panel = uiManager.PushPanel(UIPanelType.Room);
        createRequest.SetPanel(panel);
        createRequest.SendRequest();
    }

    private void OnRefreshButtonClick()
    {
        PlayClickSound();
        listRoomRequest.SendRequest();
    }

    private void EnterAnim()
    {
        gameObject.SetActive(true);

        battleRes.localPosition = new Vector3(-1000, 0);
        battleRes.DOLocalMoveX(-307, 0.5f);

        roomList.localPosition = new Vector3(1000, 0);
        roomList.DOLocalMoveX(181, 0.5f);
    }

    private void HideAnim()
    {
        battleRes.DOLocalMoveX(-1000, 0.5f);
        roomList.DOLocalMoveX(1000, 0.5f).OnComplete(() => gameObject.SetActive(false));
    }

    private void SetBattleRes()
    {
        UserData ud = facade.GetUserData();
        transform.Find("BattleRes/Username").GetComponent<Text>().text = ud.Username;
        transform.Find("BattleRes/TotalCount").GetComponent<Text>().text = "Total : " + ud.TotalCount.ToString();
        transform.Find("BattleRes/WinCount").GetComponent<Text>().text = "Win : " + ud.WinCount.ToString();
    }

    public void LoadRoomItemSync(List<UserData> udList)
    {
        this.udList = udList;
    }

    private void LoadRoomItem(List<UserData> udList)
    {
        RoomItem[] riArray = roomLayout.GetComponentsInChildren<RoomItem>();
        foreach(RoomItem ri in riArray)
        {
            ri.DestroySelf();
        }

        int count = udList.Count;
        for (int i = 0; i < count; i++) 
        {
            GameObject roomItem = GameObject.Instantiate(roomItemPrefabs);
            roomItem.transform.SetParent(roomLayout.transform);
            UserData ud = udList[i];
            roomItem.GetComponent<RoomItem>().SetRoomInfo(ud.ID, ud.Username, ud.TotalCount, ud.WinCount, this);
        }
        int roomCount = GetComponentsInChildren<RoomItem>().Length;
        Vector2 size = roomLayout.GetComponent<RectTransform>().sizeDelta;
        roomLayout.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x,
            roomCount * (roomItemPrefabs.GetComponent<RectTransform>().sizeDelta.y + roomLayout.spacing));
    }

    public void OnJoinButtonClick(int id)
    {
        PlayClickSound();
        joinRoomRequest.SendRequest(id);
    }

    public void OnJoinResponse(ReturnCode returnCode, UserData ud1, UserData ud2)
    {
        switch (returnCode)
        {
            case ReturnCode.NotFound:
                uiManager.ShowMessageSync("Room not exist.");
                break;
            case ReturnCode.Fail:
                uiManager.ShowMessageSync("This room is full.");
                break;
            case ReturnCode.Success:
                this.ud1 = ud1;
                this.ud2 = ud2;
                break;
        }
    }

    public void OnUpdateResultResponse(int totalCount, int winCount)
    {
        facade.UpdateResult(totalCount, winCount);
        SetBattleRes();
    }
}
