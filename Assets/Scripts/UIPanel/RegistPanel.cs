using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;

public class RegistPanel : BasePanel {

    private InputField usernameIF;
    private InputField passwordIF;
    private InputField repasswordIF;
    private RegistRequest registRequest;

    private void Start()
    {
        usernameIF = transform.Find("UsernameLabel/UsernameInput").GetComponent<InputField>();
        passwordIF = transform.Find("PasswordLabel/PasswordInput").GetComponent<InputField>();
        repasswordIF = transform.Find("RePasswordLabel/RePasswordInput").GetComponent<InputField>();
        registRequest = GetComponent<RegistRequest>();

        transform.Find("RegistButton").GetComponent<Button>().onClick.AddListener(OnRegsitButtonClick);
        transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseButtonClick);
    }

    public override void OnEnter()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.5f);
        transform.localPosition = new Vector3(800, 0, 0);
        transform.DOLocalMove(Vector3.zero, 0.5f);
    }

    // 进行注册 并将消息发送到服务器端
    private void OnRegsitButtonClick()
    {
        PlayClickSound();
        string msg = "";
        if (string.IsNullOrEmpty(usernameIF.text))
            msg += "Username can't be empty. ";
        if (string.IsNullOrEmpty(passwordIF.text))
            msg += "Password can't be empty. ";
        if (passwordIF.text != repasswordIF.text)
            msg += "Passwords don't match. ";
        if(msg != "")
        {
            uiManager.ShowMessage(msg);
            return;
        }

        registRequest.SendRequest(usernameIF.text, passwordIF.text);
    }
    
    public void OnRegistResponse(ReturnCode returnCode)
    {
        if(returnCode == ReturnCode.Success)
        {
            uiManager.ShowMessageSync("Success regist!");
        }
        else
        {
            uiManager.ShowMessageSync("Username already exist.");
        }
    }

    private void OnCloseButtonClick()
    {
        PlayClickSound();
        transform.DOScale(0, 0.5f);
        Tweener tweener = transform.DOLocalMove(new Vector3(800, 0, 0), 0.5f);
        tweener.OnComplete(() => uiManager.PopPanel());
    }
    public override void OnExit()
    {
        base.OnExit();
        gameObject.SetActive(false);
    }
}
