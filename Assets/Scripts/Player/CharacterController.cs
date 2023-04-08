using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float jumpBufferTime;
    public bool jumpBuffer;
    public float airMultiplier;
    public bool readyToJump;
    public float gravityValue;
    public float jetStrength;
    private bool jetting;

    [Header("Ground check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    public RaycastHit slopeHit;
    public bool exitingSlope;

    public Transform orientation;
    [SerializeField] private PlayerStats stats;
    public Vector2 inputDir;

    [SerializeField] Vector3 currentVelocity;
    Vector3 moveDirection;
    Rigidbody rb;

    public void Start(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
    }

    public void Update(){
        currentVelocity = rb.velocity;
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        SpeedControl();

        if(grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    public void FixedUpdate(){
        moveDirection = orientation.forward * inputDir.y + orientation.right * inputDir.x;

        if(jumpBuffer && grounded && readyToJump){
            exitingSlope = true;
            readyToJump = false;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            jumpBuffer = false;
        }

        if(jetting && stats.oxygen > 0){
            rb.AddForce(transform.up * jetStrength, ForceMode.Force);
        }

        if(OnSlope() && !exitingSlope){
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 15f, ForceMode.Force);
            if(rb.velocity.y != 0){
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    public void SpeedControl(){
        if(OnSlope() && !exitingSlope){
            if(rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else{
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            Vector3 jetVel = new Vector3(0f, rb.velocity.y, 0f);
        
            if(flatVel.magnitude > moveSpeed){
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
            if(jetVel.y > moveSpeed){
                Vector3 limitedJetVel = jetVel.normalized * moveSpeed * 0.75f;
                rb.velocity = new Vector3(rb.velocity.x, limitedJetVel.y, rb.velocity.z);
            }
        }
    }

    public bool OnSlope(){
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f)){
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection(){
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    public void Jump(){
        jumpBuffer = true;
        Invoke(nameof(JumpBufferReset), jumpBufferTime);
    }
    public void JumpBufferReset(){
        jumpBuffer = false;
    }
    public void ResetJump(){
        readyToJump = true;
        exitingSlope = false;
    }

    public void InvokeJumpExternal(){
        if(readyToJump){
            Jump();
            Invoke("ResetJump", jumpCooldown);
        }
    }

    // boomer shooter only, not necessarily universal

    public void Jet(){
        jetting = !jetting;

        if(jetting){
            stats.OxyReduceInvoke();
        }
        else if (!jetting){
            stats.CancelOxyReduceInvoke();
        }
        print("jetting");
    }
}