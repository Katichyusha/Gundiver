using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ActorParent : MonoBehaviour
{
    [SerializeField] protected Transform target;
    [SerializeField] protected LayerMask whatIsTarget;
    [SerializeField] protected Transform player;
    [SerializeField] protected Health hpScript;
    public bool isActive;

    [HideInInspector] protected NavMeshAgent navMeshAgent;
    private Rigidbody[] ragdollRigidbodies;
    private Task[] tasks;

    [Header("Stats")]
    public float moveSpeed; // enemies should have static moveSpeed cause why not

    protected virtual void Start() // should be called in child scripts with base.Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        this.GetComponent<Health>();
        isActive = true;
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        SetRigidBodyState(true);
        SetColliderState(true);
    }

    protected virtual void Update() // designed to be overridden
    {
    }

    public void Die(){
        isActive = false;
        this.CancelInvoke();
        Destroy(this.gameObject, 15f);
        GetComponentInChildren<Animator>().enabled = false;
        navMeshAgent.enabled = false;
        //navMeshAgent.isStopped = true;

        SetRigidBodyState(false);
        SetColliderState(true);

        //this.GetComponentInChildren<Rigidbody>().AddForce(-deathForceDir * hpScript.InformDamageBuffer() * 2f, ForceMode.Impulse);
        //this.GetComponentInChildren<Rigidbody>().AddTorque(deathForceDir * hpScript.InformDamageBuffer() * 2f, ForceMode.Impulse);
    }

    public virtual void DieGib(){
        isActive = false;
        Destroy(this.gameObject);
        GetComponentInChildren<Animator>().enabled = false;
        this.GetComponent<NavMeshAgent>().enabled = false;
    }

    public void Gib(){

    }

    void SetRigidBodyState(bool state){
        foreach(Rigidbody ribo in ragdollRigidbodies){
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
