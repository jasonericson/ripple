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

    private InputDevice controller;
    private bool triggerLastPressed = false;
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
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
            if (devices.Count > 0)
                controller = devices[0];
        }

        if (!controller.isValid)
            return;

        handPhysTracker.Update(this.transform.position, this.transform.rotation, Time.deltaTime);

        controller.TryGetFeatureValue(CommonUsages.deviceVelocity, out var controllerVelocity);

        controller.TryGetFeatureValue(CommonUsages.gripButton, out var grip);
        if (grip && !heldBall)
        {
            heldBall = Instantiate(ballRef, this.transform.position, Quaternion.identity, this.transform);
            var ballRigidbody = heldBall.GetComponent<Rigidbody>();
            ballRigidbody.isKinematic = true;
            ballRigidbody.useGravity = false;
        }
        else if (!grip && heldBall)
        {
            heldBall.transform.SetParent(null);

            var ballRigidbody = heldBall.GetComponent<Rigidbody>();
            ballRigidbody.isKinematic = false;
            ballRigidbody.useGravity = true;
            ballRigidbody.velocity = handPhysTracker.Velocity;

            heldBall = null;
        }

        
    }
}
