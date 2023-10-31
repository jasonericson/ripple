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
        var ball = collision.collider.GetComponent<Ball>();
        if (!ball)
            return;

        // if ball collides, splash! (use impulse magnitude for size/strength)
        var impulseMag = collision.impulse.magnitude;
        var normalized = Mathf.Clamp01((impulseMag - 4.0f) / 4.0f);
        rippler.DoRippleFromWorldPos(collision.transform.position, normalized, normalized);

        Destroy(ball.gameObject);
    }
}
