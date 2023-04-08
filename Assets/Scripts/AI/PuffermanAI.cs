using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PuffermanAI : ActorParent
{
    public float damage;
    public float shortRange; // 0 -> shortrange is for melee attack
    public float mediumRange;   // shortrange -> mediumrange is for medium range attack
    public float longRange; // shortrange -> longrange is for spear throw attack, anything above is repositioning
    public float preAttackWindUp;
    public float postAttackCooldown;
    public bool isAttacking;

    [Header("State Specific")]
    public float staggerTime;
    public bool doingStateBehaviour;
    public float maxRepositionDistance;
    public LayerMask whatIsInLOS;

    public enum States{
        attacking,  //throwing one of the 3 attacks
        fleeing,    //if hp low enough to find random position anywhere close and away from player
        repositioning,  // find closest edge
        assaulting, //go directly to player until in medium range
        staggered   // stops all behaviour for stagger time
    }

    public States state;

    protected override void Start(){
        base.Start();
        isAttacking = false;
        state = States.staggered;
    }

    protected override void Update(){
        if(!doingStateBehaviour){
            doingStateBehaviour = true;
            switch(state){
                case States.staggered:
                    Stagger();
                    break;
                case States.attacking:
                    AttackCheck();
                    break;
                case States.repositioning:
                    Reposition();
                    break;
                case States.fleeing:
                    StateIncrement();
                    break;
                case States.assaulting:
                    StateIncrement();
                    break;
                default:
                    StateIncrement();
                    break;
            }
        }
    }

    public void StateIncrement(){
        States previous = state;
        switch(state){
            case States.staggered:
                state = States.repositioning;
                break;
            case States.fleeing:
                state = States.attacking;
                break;
            case States.repositioning:
                state = States.attacking;
                break;
            case States.assaulting:
                state = States.attacking;
                break;
            case States.attacking:
                state = States.repositioning;
                break;
            default:
                state = States.repositioning;
                break;
        }

        if(previous == States.repositioning && hpScript.currHP > hpScript.maxHP){
            state = States.assaulting;
        }
        else if(previous == States.repositioning && hpScript.currHP < hpScript.maxHP){
            state = States.fleeing;
        }

        Invoke(nameof(StateBehaviourReset), staggerTime);
    }

    public void StateBehaviourReset(){
        doingStateBehaviour = false;
    }

    #region attack state behaviour
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

    public void AttackCheck(){
            float targetDist = Vector3.Distance(target.transform.position, this.transform.position);
            if(!isAttacking)
                isAttacking = true;
                if(targetDist < shortRange){
                    ShortAttack();
                }
                else if(shortRange < targetDist && targetDist < mediumRange){
                    MediumAttack();
                }
                else if(mediumRange < targetDist && targetDist < longRange){
                    LongAttack();
                }
                else if(longRange < targetDist){
                    StateIncrement();
                }
        }
    #endregion

    public void Reposition(){
        if(NavMesh.SamplePosition(target.transform.position + Random.insideUnitSphere * maxRepositionDistance, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            navMeshAgent.SetDestination(hit.position);
        StateIncrement();
    }

    public void ResetAttack(){  // universal reset ig cba to change the names
        if(isActive){
            navMeshAgent.isStopped = false;
            isAttacking = false;
            StateIncrement();
        }       
    }

    public void Stagger(){
        navMeshAgent.isStopped = true;
        Invoke(nameof(ResetAttack), staggerTime);
    }
}
