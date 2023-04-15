using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabAI : ActorParent
{
    public float damage;
    public float range;
    public float preAttackWindUp;
    public float postAttackCooldown;
    public bool isAttacking;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if(Vector3.Distance(this.transform.position, target.position) < range && !isAttacking && isActive){
            isAttacking = true;
            navMeshAgent.isStopped = true;
            Invoke(nameof(Attack), preAttackWindUp);
            //print("invoked attack");
        }
    }

    public void Attack(){
        print("starting attack");
        if(Vector3.Distance(this.transform.position, target.position) < range){
            target.SendMessage("DAMAGE", -damage, SendMessageOptions.DontRequireReceiver);
            //print("should be ONE TIME");
        }
        Invoke(nameof(ResetAttack), 1/postAttackCooldown);
    }

    public void ResetAttack(){
        navMeshAgent.isStopped = false;
        isAttacking = false;
        //print("attack should be reset now");
    }
}
