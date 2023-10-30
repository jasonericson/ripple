using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.Labs.SuperScience;

public class ShootBall : MonoBehaviour
{
    public GameObject ballRef;

    private InputDevice rightController;
    private bool triggerLastPressed = false;
    private GameObject heldBall;
    private PhysicsTracker handPhysTracker;

    // Start is called before the first frame update
    void Start()
    {
        handPhysTracker = new PhysicsTracker();
    }

    // Update is called once per frame
    void Update()
    {
        if (!rightController.isValid)
        {
            var devices = new List<InputDevice>();
            var rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);
            if (devices.Count > 0)
                rightController = devices[0];
        }

        if (!rightController.isValid)
            return;

        handPhysTracker.Update(this.transform.position, this.transform.rotation, Time.deltaTime);

        rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out var controllerVelocity);
        Debug.LogFormat("Controller Velocity: {0}", handPhysTracker.Velocity.magnitude);

        rightController.TryGetFeatureValue(CommonUsages.gripButton, out var grip);
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
