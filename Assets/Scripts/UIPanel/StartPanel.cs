using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartPanel : BasePanel
{
    private Button startButton;

    void Start()
    {
        startButton = transform.Find("StartButton").GetComponent<Button>();
        startButton.onClick.AddListener(OnStartClick);
    }

    public override void OnEnter()
    {
        base.OnEnter();       
    }

    private void OnStartClick()
    {
        PlayClickSound();
        uiManager.PushPanel(UIPanelType.Login);
    }

    public override void OnPause()
    {
        base.OnPause();
        startButton.transform.localScale = Vector3.one;
        startButton.transform.DOScale(0, 0.5f).OnComplete(() => startButton.gameObject.SetActive(false));
    }

    public override void OnResume()
    {
        base.OnResume();
        startButton.gameObject.SetActive(true);
        startButton.transform.localScale = Vector3.zero;
        startButton.transform.DOScale(1, 0.5f);
    }
}
