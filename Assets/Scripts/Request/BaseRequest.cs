using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class BaseRequest : MonoBehaviour {

    protected RequestCode requestCode = RequestCode.None;
    protected ActionCode actionCode = ActionCode.None;
    protected GameFacade _facade;

    protected GameFacade facade
    {
        get
        {
            if (_facade == null)
                _facade = GameFacade.Instance;
            return _facade;
        }
    }

    public virtual void Awake()
    {
        facade.AddRequest(actionCode, this);
        //_facade = GameFacade.Instance;
    }

    public virtual void SendRequest() { }

    public virtual void OnResponse(string data) { }

    public virtual void OnDestroy()
    {
        GameFacade.Instance.RemoveRequest(actionCode);
    }

    protected void SendRequest(string data)
    {
        if(facade != null)
            facade.SendRequest(requestCode, actionCode, data);
    }
}
