using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    public GameObject weapon;
    public CharacterController control;
    public MouseLook mLook;
    public PlayerStats stats;

    public void OnPrimaryFire(InputValue inputValue){
        if(inputValue.isPressed){
            weapon.SendMessage("Shoot", false, SendMessageOptions.DontRequireReceiver);
            //print("shot");
        } 
    }

    public void OnSecondaryFire(InputValue inputValue){
        if(inputValue.isPressed){
            weapon.SendMessage("Shoot", true, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void OnJump(InputValue value){
        control.InvokeJumpExternal();
    }

    public void OnMovement(InputValue value){
        control.inputDir = value.Get<Vector2>();
    }

    public void OnLook(InputValue value){
        mLook.mDelta = value.Get<Vector2>();
    }

    public void OnJet(InputValue value){
        control.Jet();
    } 
}
