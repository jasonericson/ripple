using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.Labs.SuperScience;

public enum ControllerHand
{
    Left,
    Right,
}

public class ControllerInput : MonoBehaviour
{
    [System.NonSerialized] public InputDevice controller;
    [System.NonSerialized] public OVRSkeleton skeleton;
    public ControllerHand hand;

    private OVRHand ovrHand;
    private PhysicsTracker handPhysTracker;
    private InputDeviceCharacteristics controllerCharacteristics;
    private bool gripButton;
    private bool gripButtonDown;
    private bool gripButtonUp;

    // Start is called before the first frame update
    void Start()
    {
        handPhysTracker = new PhysicsTracker();
        controllerCharacteristics = hand == ControllerHand.Left ? InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right;
        controllerCharacteristics |= InputDeviceCharacteristics.Controller;

        ovrHand = GetComponentInChildren<OVRHand>();
        skeleton = GetComponentInChildren<OVRSkeleton>();
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

        // use physics tracker for better throwing
        handPhysTracker.Update(this.transform.position, this.transform.rotation, Time.deltaTime);

        gripButtonUp = false;
        gripButtonDown = false;
        if (controller.isValid)
        {
            controller.TryGetFeatureValue(CommonUsages.gripButton, out var grip);
            if (grip)
            {
                if (!gripButton)
                    gripButtonDown = true;

                gripButton = true;
            }
            else
            {
                if (gripButton)
                    gripButtonUp = true;

                gripButton = false;
            }
        }
        else
        {
            var pinching = ovrHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
            if (pinching)
            {
                if (!gripButton)
                    gripButtonDown = true;

                gripButton = true;
            }
            else
            {
                if (gripButton)
                    gripButtonUp = true;

                gripButton = false;
            }
        }
    }

    public bool GripButtonDown()
    {
        return gripButtonDown;
    }

    public bool GripButtonUp()
    {
        return gripButtonUp;
    }

    public Vector3 GetVelocity()
    {
        return handPhysTracker.Velocity;
    }
}
