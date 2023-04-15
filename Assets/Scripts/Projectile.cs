using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /*  this just controls the behaviour and settings of a projectile
        the model and visual side is done by hand outside of this script*/

    public LayerMask whatIsTarget;
    public float damage;
    public float speed;
    public float rotationSpeed; // only affects the object if followtarget is enabled
    public bool explosive;
    public float explosiveRange;
    public bool useGravity;
    //public bool followTarget;
    //private Transform targetToFollow;
    private Rigidbody rb;

    public void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.useGravity = useGravity;
    }

    public void Update(){
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    public void Explode(){
        print("boooooooom!");
    }

    void OnCollisionEnter(Collision other)
    {
        if((whatIsTarget & (1 << other.gameObject.layer)) != 0){
            SendMessage("DAMAGE", -damage, SendMessageOptions.DontRequireReceiver);
        }
        if(explosive){
            Explode();
        }
        // other.contacts[0].point for effects spawning
        Destroy(this.gameObject);
    }
}
