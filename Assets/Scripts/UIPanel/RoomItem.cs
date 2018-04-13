using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour {

    public Text username;
    public Text totalCount;
    public Text winCount;
    public Button joinButton;

    private int ID;
    private RoomListPanel panel;

    void Start()
    {
        if(joinButton != null)
        {
            joinButton.onClick.AddListener(OnJoinClick);
        }
    }

    public void SetRoomInfo(int id, string username, int tot, int win, RoomListPanel panel)
    {
        SetRoomInfo(id, username, tot.ToString(), win.ToString(), panel);
    }

    public void SetRoomInfo(int id, string username, string tot, string win, RoomListPanel panel)
    {
        this.panel = panel;
        this.ID = id;
        this.username.text = username;
        this.totalCount.text = "Total\n" + tot;
        this.winCount.text = "Win\n" + win;
    }

    private void OnJoinClick()
    {
        panel.OnJoinButtonClick(ID);
    }

    public void DestroySelf()
    {
        GameObject.Destroy(this.gameObject);
    }
}
