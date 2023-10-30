using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ShootBall : MonoBehaviour
{
    public GameObject ballRef;

    private InputDevice rightController;
    private bool triggerLastPressed = false;

    // Start is called before the first frame update
    void Start()
    {
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

        rightController.TryGetFeatureValue(CommonUsages.triggerButton, out var trigger);
        if (trigger)
        {
            if (!triggerLastPressed)
            {
                var ball = Instantiate(ballRef, this.transform.position, Quaternion.identity);
                ball.GetComponent<Rigidbody>().AddForce(this.transform.forward * 1000.0f);
            }
        }

        triggerLastPressed = trigger;
    }
}
