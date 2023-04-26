using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Projectile : MonoBehaviour
{
    /*  this just controls the behaviour and settings of a projectile
        the model and visual side is done by hand outside of this script*/

    public LayerMask whatIsTarget;
    public float lifeTime;
    public float damage;
    public float speed;
    public float rotationSpeed; // only affects the object if followtarget is enabled
    public bool explosive;
    public float explosiveRange;
    public bool stickToTarget;
    public bool stuck;
    public Vector3 stuckPosition;
    public Quaternion stuckRotation;
    public float maxRangeToStickToWall;
    public bool followTarget;
    public Transform targetToFollow;
    private Rigidbody rb;
    private FixedJoint fj;

    public void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        stuck = false;
        
        if(explosive){
            Invoke(nameof(Explode), lifeTime);
        }
        else{
            Invoke(nameof(Expire), lifeTime);
        }
    }

    public void FixedUpdate(){
        if(!stuck)
            rb.velocity = transform.forward * speed;

        if(followTarget && !stuck){
            RotateProjectile();
        }
    }

    public void Explode(){
        print("boooooooom!");
        Expire();
    }

    public void Expire(){
        Destroy(this.gameObject);
    }

    public void RotateProjectile(){
        var headingTowards = targetToFollow.position - this.transform.position;

        var rotation = Quaternion.LookRotation(headingTowards);
        rb.MoveRotation(Quaternion.RotateTowards(this.transform.rotation, rotation, rotationSpeed * Time.deltaTime));
    }

    public IEnumerator StickToTarget(Rigidbody bodyToStickTo, Vector3 forceToAdd, Vector3 pointToAddAt){
        yield return new WaitForEndOfFrame();
        fj.connectedBody = bodyToStickTo;

        //add force after sticking

        bodyToStickTo.AddForceAtPosition(forceToAdd, pointToAddAt, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision other)
    {
        if((whatIsTarget & (1 << other.gameObject.layer)) != 0){
            if(explosive){
                Explode();
            }

            other.gameObject.SendMessageUpwards("DAMAGE", -damage, SendMessageOptions.DontRequireReceiver);

            if(stickToTarget){
                stuck = true;
                var speedTransfer = speed;
                rb.velocity = Vector3.zero;
                //this.transform.SetParent(other.transform, false);
                Physics.IgnoreCollision(this.GetComponent<Collider>(), other.collider, true);

                if(other.rigidbody != null){
                    
                    fj = gameObject.AddComponent<FixedJoint>();
                    var directionNorm = (transform.position - other.GetContact(0).point).normalized;
                    StartCoroutine(StickToTarget(other.rigidbody, directionNorm * speedTransfer, other.GetContact(0).point));
                    
                }
                else{
                    rb.isKinematic = true;
                }
                this.GetComponent<Rigidbody>().detectCollisions = false;
                CancelInvoke();
                Invoke(nameof(Expire), lifeTime);
            }
            else{
                Expire();
            }
            
        }
        // other.contacts[0].point for effects spawning
    }
}
