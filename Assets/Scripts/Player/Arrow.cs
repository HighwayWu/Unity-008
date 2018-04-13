using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class Arrow : MonoBehaviour {
    public RoleType roleType;
    public int speed = 5;
    public GameObject explosionEffect;
    public bool isLocal = false; // 判断箭是否是本地的
    private Rigidbody rgd;

    void Start()
    {
        rgd = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rgd.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 是否射中玩家
        if (other.tag == "Player")
        {
            GameFacade.Instance.PlayNormalSound(AudioManager.Sound_ShootPerson);
            if (isLocal)
            {
                bool isPlayerLocal = other.GetComponent<PlayerInfo>().isLocal;
                if(isLocal != isPlayerLocal)
                {
                    GameFacade.Instance.SendAttack(Random.Range(10,20));
                }
            }
        }
        else
            GameFacade.Instance.PlayNormalSound(AudioManager.Sound_Miss);
        
        // 创建爆炸特效
        GameObject.Instantiate(explosionEffect, transform.position, transform.rotation);
        GameObject.Destroy(this.gameObject);
    }
}
