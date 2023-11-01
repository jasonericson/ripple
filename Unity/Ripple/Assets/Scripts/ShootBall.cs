using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.Labs.SuperScience;
using Oculus.Interaction.Input;

public class ShootBall : MonoBehaviour
{
    public GameObject ballRef;
    public float throwBoost = 1.5f;

    private GameObject heldBall;
    private ControllerInput input;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<ControllerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if (input.GripButtonDown() && !heldBall)
        {
            // if grip button is pressed, spawn ball in hand
            var fingerBone = input.skeleton.Bones[8];
            if (fingerBone.Transform)
            {
                heldBall = Instantiate(ballRef, fingerBone.Transform.position, Quaternion.identity, this.transform);
            }
            else
            {
                heldBall = Instantiate(ballRef, this.transform.position, Quaternion.identity, this.transform);
            }
            var ballRigidbody = heldBall.GetComponent<Rigidbody>();
            ballRigidbody.isKinematic = true;
            ballRigidbody.useGravity = false;
        }
        else if (input.GripButtonUp() && heldBall)
        {
            // if grip button is released, let go of ball
            heldBall.transform.SetParent(null);

            var ballRigidbody = heldBall.GetComponent<Rigidbody>();
            ballRigidbody.isKinematic = false;
            ballRigidbody.useGravity = true;
            ballRigidbody.velocity = input.GetVelocity() * throwBoost;

            heldBall = null;
        }
    }
}
