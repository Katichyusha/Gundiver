using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActorParent : MonoBehaviour
{
    [SerializeField] protected Transform target;
    [SerializeField] protected LayerMask whatIsTarget;
    [SerializeField] protected Transform player;
    [SerializeField] protected Health hpScript;
    public bool isActive;
    private Vector3 deathForceDir;
    private List<RaycastHit> knockbackHits;

    [HideInInspector] protected NavMeshAgent navMeshAgent;

    [Header("Stats")]
    public float moveSpeed; // enemies should have static moveSpeed cause why not

    protected virtual void Start() // should be called in child scripts with base.Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        this.GetComponent<Health>();
        isActive = true;
        SetRigidBodyState(true);
        SetColliderState(false);
    }

    protected virtual void Update() // designed to be overridden
    {
        if(isActive)
            navMeshAgent.SetDestination(target.position);
    }

    public virtual void Die(){
        this.CancelInvoke();
        isActive = false;
        Destroy(this.gameObject, 15f);
        GetComponentInChildren<Animator>().enabled = false;
        navMeshAgent.enabled = false;
        //navMeshAgent.isStopped = true;
        SetRigidBodyState(false);
        SetColliderState(true);
        this.GetComponentInChildren<Rigidbody>().AddForce(-deathForceDir * hpScript.InformDamageBuffer() * 2f, ForceMode.Impulse);
        this.GetComponentInChildren<Rigidbody>().AddTorque(deathForceDir * hpScript.InformDamageBuffer() * 2f, ForceMode.Impulse);
    }

    public virtual void DieGib(){
        isActive = false;
        Destroy(this.gameObject);
        GetComponentInChildren<Animator>().enabled = false;
        this.GetComponent<NavMeshAgent>().enabled = false;
    }

    public void Knockback(Vector3 direction){
        deathForceDir = direction;
    }

    public void Gib(){

    }

    void SetRigidBodyState(bool state){
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach(Rigidbody ribo in rigidbodies){
            ribo.isKinematic = state;
        }

        //GetComponent<Rigidbody>().isKinematic = !state;
    }

    void SetColliderState(bool state){
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach(Collider ribo in colliders){
            ribo.enabled = state;
        }

        GetComponent<Collider>().enabled = !state;
    }
}
