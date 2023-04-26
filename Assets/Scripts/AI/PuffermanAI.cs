using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class PuffermanAI : ActorParent
{
    public float damage; // cqb: 1.0x; mid: 2.0x; far: 1.6x
    //ranges pertaining to checks for decisions
    public float closeRange;
    public float midRange;
    public float farRange;
    public float preAttackWindUp; // placeholder until i do it in animation data
    public float postAttackCooldown;
    public bool isAttacking;
    public bool canAttack; // governed by inter attack cd
    public bool attackIntent;
    
    public GameObject midProjectile;
    public GameObject farProjectile;

    [Header("Behaviour Settings")]
    public float reactionTime = 0.25f; // base
    public float interAttackCooldown = 0.5f; // possibly gonna be higher
    public float intentDecay = 1.0f;    // how much time should pass before cancelling the current behaviour if not completed   
    public float repositionRange = 10.0f; //range is probably okay as is

    public enum NextAction{
        close,
        mid,
        far,
        assault,
        reposition
    }
    public NextAction attackToDo;
    public UnityEvent ContinueBehaviour;

    protected override void Start(){
        base.Start();
        isAttacking = false;
        canAttack = true;
        attackIntent = false;
        BeginBehaviourRoutine();
    }

    protected override void Update()
    {
        
    }

    private Coroutine br;
    /*  wait until AI can react to player
        check what attack it can perform at current range from player
        if it can attack, use decided state to start an attack routine
        else reposition to a random point*/
    public IEnumerator BehaviourRoutine(){
        //print("behaviour routine starting");
        yield return new WaitForSeconds(reactionTime);

        //print("checking attack decision");
        NextAction tempAction = CheckAttack();

        if(canAttack){
            switch(tempAction){
                case NextAction.close:
                    //print("starting close attack");
                    att = StartCoroutine(nameof(CloseAttack));
                    break;

                case NextAction.mid:
                    //print("starting mid attack");
                    att = StartCoroutine(nameof(MidAttack));
                    break;

                case NextAction.far:
                    //print("starting far attack");
                    att = StartCoroutine(nameof(FarAttack));
                    break;

                case NextAction.assault:
                    //print("starting to get into range");
                    GetIntoRange();
                    att = StartCoroutine(nameof(CheckIfInRange));
                    break;

                case NextAction.reposition: //backup incase it returns some weird stuff
                    mitd = StartCoroutine(nameof(MovementIntentTimeDecay));
                    RepositionToRandomSpot();
                    break;
            }
        }
        else if(!canAttack){
            //print("repositioning after decision");
            mitd = StartCoroutine(nameof(MovementIntentTimeDecay));
            RepositionToRandomSpot();
        }
    }
    public void BeginBehaviourRoutine(){
        if(this.isActive)
            br = StartCoroutine(nameof(BehaviourRoutine));
        else{
            this.StopAllCoroutines();
        }
    }


    // checks the distance to player while ignoring the y coordinate, called only when needed
    public float CheckRangeToTarget(){
        //print("checking range to target");
        Vector3 dist1 = this.transform.position;
        dist1.y = 0;
        Vector3 dist2 = target.position;
        dist2.y = 0;
        
        float distance = Vector3.Distance(dist1, dist2);
        return distance;
    }

    public void ResetInterAttackCooldown(){
        //print("interattack cooldown has reset");
        canAttack = true;
    }

    /*  checks how to attack according to range to target and target state
        targets state must be known before any attacks are undertaken
        and if it fails to be gotten then the attack routine is cancelled*/
    private Coroutine att;
    public NextAction CheckAttack(){
       // print("checking attack");
        float atRangeOf = CheckRangeToTarget();
        CharacterController targetCC;

        if(!target.TryGetComponent<CharacterController>(out targetCC)){ // probably wont ever happen
            //print("target character controller not found");
            return NextAction.reposition;
        }

        if((midRange < atRangeOf && atRangeOf <= farRange) || targetCC.jetting){
            //print("far attack decided");
            return NextAction.far;
        }
        else if(atRangeOf <= closeRange){
            //print("close attack decided");
            return NextAction.close;
        }
        else if((closeRange < atRangeOf && atRangeOf <= midRange) && !targetCC.jetting){
            //print("mid attack decided");
            return NextAction.mid;
        }
        else{
            //print("assault decided");
            return NextAction.assault;
        }
    }

    #region actual attacking behaviours
    public IEnumerator CloseAttack(){
        canAttack = false;
        StandInPlace();
        yield return new WaitForSeconds(preAttackWindUp);

        Collider[] hits;
        hits = Physics.OverlapSphere(this.transform.position, closeRange/2, whatIsTarget, QueryTriggerInteraction.Ignore);
        foreach(Collider hit in hits){
            hit.SendMessage("DAMAGE", -damage, SendMessageOptions.DontRequireReceiver);
        }
        yield return new WaitForSeconds(postAttackCooldown);  
        ContinueBehaviour.Invoke();
        Invoke(nameof(ResetInterAttackCooldown), interAttackCooldown);
    }
    public IEnumerator MidAttack(){
        canAttack = false;
        StandInPlace();
        yield return new WaitForSeconds(preAttackWindUp);

        var tempGO = Instantiate(midProjectile, this.transform.position + Vector3.up * 2f, Quaternion.LookRotation(target.position - this.transform.position));
        var tempProj = tempGO.GetComponent<Projectile>();

        tempProj.targetToFollow = this.target;
        tempProj.damage = this.damage * 2f;

        yield return new WaitForSeconds(postAttackCooldown);
        ContinueBehaviour.Invoke();
        Invoke(nameof(ResetInterAttackCooldown), interAttackCooldown);
    }
    public IEnumerator FarAttack(){
        canAttack = false;
        StandInPlace();

        yield return new WaitForSeconds(preAttackWindUp);

        //print("spawning projectile");

        var tempGO = Instantiate(farProjectile, this.transform.position + Vector3.up * 2f, Quaternion.LookRotation(target.position - this.transform.position));
        var tempProj = tempGO.GetComponent<Projectile>();

        tempProj.targetToFollow = this.target;
        tempProj.damage = this.damage * 1.6f;

        yield return new WaitForSeconds(postAttackCooldown);

        ContinueBehaviour.Invoke();
        Invoke(nameof(ResetInterAttackCooldown), interAttackCooldown);
    }
    #endregion

    public void RepositionToRandomSpot(){
        Vector3 randomDirection = Random.insideUnitSphere * repositionRange;

        randomDirection += this.transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, repositionRange, NavMesh.AllAreas);

        navMeshAgent.SetDestination(hit.position);
    }


    public void GetIntoRange(){
        //sample position towards player position
        Vector3 directedPos = target.position + Random.insideUnitSphere * repositionRange;

        NavMeshHit hit;
        NavMesh.SamplePosition(directedPos, out hit, repositionRange, NavMesh.AllAreas);

        navMeshAgent.SetDestination(hit.position);
        //Invoke("CancelActions", intentDecay);
    }
    //both of the checks below should be used as timed coroutines
    private Coroutine rangeCoroutine;
    public IEnumerator CheckIfInRange(){
        //print("checking if in range of movement");
        float atRangeOf;
        bool reachedGoal = false;

        while(!reachedGoal){
            yield return new WaitForSeconds(0.1f);
            atRangeOf = CheckRangeToTarget();

            if(atRangeOf < farRange){
                //print("Got into range!");
                ContinueBehaviour.Invoke();
                StandInPlace();
                reachedGoal = true;
                yield return false;
            }
            else if(navMeshAgent.remainingDistance < 2.0f){
                //print("At player object somehow");
                ContinueBehaviour.Invoke();
                reachedGoal = true;
                yield return false;
            }
        }
        //print("CHECK RANGE COMPLETE");
    }

    public void StandInPlace(){
        navMeshAgent.SetDestination(this.transform.position);
    }
    private Coroutine mitd; //the one below
    public IEnumerator MovementIntentTimeDecay(){
        //print("starting intent decay");
        yield return new WaitForSeconds(intentDecay);
        ContinueBehaviour.Invoke();
        StandInPlace();
    }
}
