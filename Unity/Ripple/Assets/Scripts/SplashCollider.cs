using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashCollider : MonoBehaviour
{
    public Rippler rippler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        rippler.DoRippleFromWorldPos(collision.transform.position, 0.5f, 0.5f);
    }
}
