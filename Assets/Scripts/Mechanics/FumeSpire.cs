using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FumeSpire : MonoBehaviour
{
    public GameObject basic; // base model
    public GameObject destroyed;    // destroyed model, both should be already on the fumespire object itself
    public float explosionRadius;
    public float explosionDamage;
    public float explosionForce;
    public LayerMask whatIsTarget;
    public ParticleSystem ps;

    public void Awake(){
        basic.SetActive(true);
        destroyed.SetActive(false);
    }

    public void Explode(){
        basic.SetActive(false);
        destroyed.SetActive(true);
        Collider[] targets = Physics.OverlapSphere(this.transform.position, explosionRadius, whatIsTarget);
        Rigidbody targetRB;
        foreach(Collider target in targets){
            target.SendMessage("DAMAGE", explosionDamage, SendMessageOptions.DontRequireReceiver);
            if(target.TryGetComponent<Rigidbody>(out targetRB)){
                targetRB.AddExplosionForce(explosionForce, this.transform.position, explosionRadius, 0f, ForceMode.Impulse);
            }
        }
    }
}
