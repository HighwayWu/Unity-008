using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyForTime : MonoBehaviour {

    public float time = 1;

    void Start()
    {
        Destroy(this.gameObject, time);
    }

}
