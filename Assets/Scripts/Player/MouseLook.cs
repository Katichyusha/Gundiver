using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 10f;

    public Transform orientation;
    public Transform supplementCam;

    public Vector2 targetRotation;
    public Vector2 mDelta;

    private void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update(){
        //Vector2 mDelta = Mouse.current.delta.ReadValue() * (sensitivity/10);
        targetRotation.y += mDelta.x * (sensitivity/10);
        targetRotation.x -= mDelta.y * (sensitivity/10);
        targetRotation.x = Mathf.Clamp(targetRotation.x, -90f, 90f);
        targetRotation.y = Mathf.Repeat(targetRotation.y, 360f);
        this.transform.localRotation = Quaternion.Euler(targetRotation.x, targetRotation.y, 0);
        this.transform.position = orientation.position + new Vector3(0, 0.5f, 0);
        orientation.localRotation = Quaternion.Euler(0, targetRotation.y, 0);
        supplementCam.localRotation = Quaternion.Euler(0, targetRotation.y, 0);
    }
}

