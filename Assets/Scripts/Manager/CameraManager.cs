using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : BaseManager {

    private GameObject cameraGo;
    private Animator cameraAnim;
    private FollowTarget followTarget;
    private Vector3 originalPosition;
    private Vector3 originalRotation;

    public CameraManager(GameFacade facade) : base (facade) { }

    public override void OnInit()
    {
        cameraGo = Camera.main.gameObject;
        cameraAnim = cameraGo.GetComponent<Animator>();
        followTarget = cameraGo.GetComponent<FollowTarget>();
        
    }

    //public override void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        FollowTarget(null);
    //    }
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        WalkthoughScene();
    //    }
    //}

    // 切换为跟随角色
    public void FollowRole()
    {
        followTarget.target = facade.GetCurrentRole().transform;
        cameraAnim.enabled = false;
        originalPosition = cameraGo.transform.position;
        originalRotation = cameraGo.transform.eulerAngles;

        Quaternion targetQuaternion = Quaternion.LookRotation(followTarget.target.position - cameraGo.transform.position);
        cameraGo.transform.DORotateQuaternion(targetQuaternion, 1f).OnComplete(delegate
        {
            followTarget.enabled = true;
        });   
    }

    // 切换为场景漫游
    public void WalkthoughScene()
    {
        followTarget.enabled = false;
        cameraGo.transform.DOMove(originalPosition, 1f);
        cameraGo.transform.DORotate(originalRotation, 1f).OnComplete(delegate()
        {
            cameraAnim.enabled = true;
        });
    }
}
