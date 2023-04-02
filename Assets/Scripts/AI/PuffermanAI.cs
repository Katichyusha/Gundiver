using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuffermanAI : ActorParent
{
    public float damage;
    public float shortRange; // 0 -> shortrange is for melee attack
    public float mediumRange;   // shortrange -> mediumrange is for medium range attack
    public float longRange; // shortrange -> longrange is for spear throw attack, anything above is repositioning
    public float preAttackWindUp;
    public float postAttackCooldown;
    public bool isAttacking;
    private float distFromTarget;

    protected override void Start(){
        base.Start();
        isAttacking = false;
    }

    protected override void Update(){
        if(!isAttacking && isActive)
            distFromTarget = Vector3.Distance(this.transform.position, target.transform.position);
            if(distFromTarget <= shortRange){
                isAttacking = true;
                navMeshAgent.isStopped = true;
                Invoke(nameof(ShortAttack), preAttackWindUp);
            }
            else if(shortRange < distFromTarget && distFromTarget < mediumRange){
                isAttacking = true;
                navMeshAgent.isStopped = true;
                Invoke(nameof(MediumAttack), preAttackWindUp);
            }
            else if(mediumRange < distFromTarget && distFromTarget < longRange){
                isAttacking = true;
                navMeshAgent.isStopped = true;
                Invoke(nameof(LongAttack), preAttackWindUp);
            }
            else{
                navMeshAgent.SetDestination(target.position);
            }
    }

    public void ShortAttack(){
        print("short range attack");
        Invoke(nameof(ResetAttack), postAttackCooldown);
    }

    public void MediumAttack(){
        print("medium range attack");
        Invoke(nameof(ResetAttack), postAttackCooldown);
    }

    public void LongAttack(){
        print("long range attack");
        Invoke(nameof(ResetAttack), postAttackCooldown);
    }

    public void ResetAttack(){
        navMeshAgent.isStopped = false;
        isAttacking = false;
    }
}
