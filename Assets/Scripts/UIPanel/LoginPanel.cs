using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;

public class LoginPanel : BasePanel
{
    private Button closeButton;
    private InputField usernameIF;
    private InputField passwordIF;
    private LoginRequest loginRequest;

    void Start()
    {
        usernameIF = transform.Find("UsernameLabel/UsernameInput").GetComponent<InputField>();
        passwordIF = transform.Find("PasswordLabel/PasswordInput").GetComponent<InputField>();

        closeButton = transform.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(OnCloseButtonClick);
        transform.Find("LoginButton").GetComponent<Button>().onClick.AddListener(OnLoginButtonClick);
        transform.Find("RegistButton").GetComponent<Button>().onClick.AddListener(OnRegistButtonClick);

        loginRequest = GetComponent<LoginRequest>();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        EnterAnim();       
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

    private void OnLoginButtonClick()
    {
        PlayClickSound();
        string msg = "";
        if (string.IsNullOrEmpty(usernameIF.text))
        {
            msg += "Username can't be empty. ";
        }
        if (string.IsNullOrEmpty(passwordIF.text))
        {
            msg += "Password can't be empty. ";
        }
        if(msg != "")
        {
            uiManager.ShowMessage(msg);
            return;
        }
        // 发送到服务器端进行验证
        loginRequest.SendRequest(usernameIF.text, passwordIF.text);
    }

    private void OnRegistButtonClick()
    {
        PlayClickSound();
        uiManager.PushPanel(UIPanelType.Regist);
    }

    private void OnCloseButtonClick()
    {
        PlayClickSound();
        uiManager.PopPanel();
    }

    public override void OnExit()
    {
        HideAnim();
    }

    // 处理登录返回的信息 success或fail
    public void OnLoginResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Success)
        {
            uiManager.PushPanelSync(UIPanelType.RoomList);
        }
        else
        {
            uiManager.ShowMessageSync("Wrong username or password.");
        }
    }

    private void EnterAnim()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.5f);
        transform.localPosition = new Vector3(800, 0, 0);
        transform.DOLocalMove(Vector3.zero, 0.5f);
    }

    private void HideAnim()
    {
        transform.DOScale(0, 0.5f);
        transform.DOLocalMoveX(1000, 0.5f).OnComplete(() => gameObject.SetActive(false));
    }
}
