using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.Labs.SuperScience;
using Oculus.Interaction.Input;

public class ShootBall : MonoBehaviour
{
    public GameObject ballRef;
    public bool isRight;
    public float throwBoost = 1.5f;

    private InputDevice controller;
    private GameObject heldBall;
    private PhysicsTracker handPhysTracker;
    private InputDeviceCharacteristics controllerCharacteristics;

    // Start is called before the first frame update
    void Start()
    {
        handPhysTracker = new PhysicsTracker();
        controllerCharacteristics = isRight ? InputDeviceCharacteristics.Right : InputDeviceCharacteristics.Left;
        controllerCharacteristics |= InputDeviceCharacteristics.Controller;
    }

    // Update is called once per frame
    void Update()
    {
        if (!controller.isValid)
        {
            // get controller reference, either left or right
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
            if (devices.Count > 0)
                controller = devices[0];
        }

        if (!controller.isValid)
            return;

        // use physics tracker for better throwing
        handPhysTracker.Update(this.transform.position, this.transform.rotation, Time.deltaTime);

        controller.TryGetFeatureValue(CommonUsages.gripButton, out var grip);
        if (grip && !heldBall)
        {
            // if grip button is pressed, spawn ball in hand
            heldBall = Instantiate(ballRef, this.transform.position, Quaternion.identity, this.transform);
            var ballRigidbody = heldBall.GetComponent<Rigidbody>();
            ballRigidbody.isKinematic = true;
            ballRigidbody.useGravity = false;
        }
        else if (!grip && heldBall)
        {
            // if grip button is released, let go of ball
            heldBall.transform.SetParent(null);

            var ballRigidbody = heldBall.GetComponent<Rigidbody>();
            ballRigidbody.isKinematic = false;
            ballRigidbody.useGravity = true;
            ballRigidbody.velocity = handPhysTracker.Velocity * throwBoost;

            heldBall = null;
        }

        
    }
}
