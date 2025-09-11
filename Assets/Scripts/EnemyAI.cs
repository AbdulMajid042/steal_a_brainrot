using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Patrol, Chase, Attack }
    public State currentState = State.Idle;   // Start in Idle

    [Header("AI Settings")]
    public float sightRange = 12f;
    public float attackRange = 2f;
    public float patrolRadius = 10f;
    public float idleDuration = 3f;
    public float attackCooldown = 1.2f;
    public float turnSpeed = 10f;

    private float patrolTimer = 0f;
    public float minPatrolTime = 3f; // set in Inspector

    [Header("Movement Speeds")]
    public float walkSpeed = 2f;   // used for wandering
    public float chaseSpeed = 4f;  // used for chasing

    private float idleTimer = 0f;
    private float lastAttackTime = 0f;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;

    private Vector3 patrolTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj) player = playerObj.transform;

        ChangeState(State.Idle); // Start idle
    }

    void Update()
    {
        float distanceToPlayer = player ? Vector3.Distance(transform.position, player.position) : Mathf.Infinity;

        switch (currentState)
        {
            case State.Idle: Idle(); break;
            case State.Patrol: Patrol(distanceToPlayer); break;
            case State.Chase: ChasePlayer(distanceToPlayer); break;
            case State.Attack: AttackPlayer(distanceToPlayer); break;
        }
        if(agent)
        {
            Debug.Log($"Velocity: {agent.velocity.magnitude:F2}, hasPath: {agent.hasPath}, pathPending: {agent.pathPending}, pathStatus: {agent.pathStatus}");
            if(animator)
            {
                Debug.Log($"isWalking animator param: {animator.GetBool("isWalking")}");
            }
        }
    }

    // ---------------- STATES ----------------
    void Idle()
    {
        if (animator) animator.SetBool("isWalking", false);
        agent.isStopped = true;

        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration)
        {
            idleTimer = 0f;
            SetNewPatrolTarget();
            ChangeState(State.Patrol);   // after 3s  wander
        }
    }

    void Patrol(float distanceToPlayer)
    {
        if (animator) animator.SetBool("isWalking", true);

        agent.speed = walkSpeed;   //  Ensure agent uses walk speed in patrol
        agent.isStopped = false;   //  Just in case it was stopped in Attack

        patrolTimer += Time.deltaTime;
        agent.SetDestination(patrolTarget);

        // Only stop patrolling if reached destination AND patrolled for enough time
        if (!agent.pathPending && agent.remainingDistance < 0.5f && patrolTimer >= minPatrolTime)
        {
            patrolTimer = 0f;
            ChangeState(State.Idle);
        }

        if (distanceToPlayer <= sightRange)
        {
            patrolTimer = 0f;
            ChangeState(State.Chase);
        }
    }

    void ChasePlayer(float distanceToPlayer)
    {
        if (animator) animator.SetBool("isWalking", true);

        agent.speed = chaseSpeed;  // <- chase speed
        agent.SetDestination(player.position);

        if (distanceToPlayer <= attackRange)
            ChangeState(State.Attack);
        else if (distanceToPlayer > sightRange * 1.5f)
            ChangeState(State.Idle); // lost sight  idle
    }

    void AttackPlayer(float distanceToPlayer)
    {
        agent.isStopped = true;
        agent.ResetPath();
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(player.position - transform.position),
            Time.deltaTime * turnSpeed
        );

        if (animator) animator.SetBool("isWalking", false);

        if (Time.time - lastAttackTime > attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack");
            Debug.Log("Enemy attacked with melee weapon!");
        }

        if (distanceToPlayer > attackRange)
            ChangeState(State.Chase);
    }

    // ---------------- HELPERS ----------------
    void ChangeState(State newState)
    {
        currentState = newState;
    }

    void SetNewPatrolTarget()
    {
        Vector3 randomDir = Random.insideUnitSphere * patrolRadius;
        randomDir += transform.position;

        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
        }
        else
        {
            patrolTarget = transform.position; // fallback
        }
    }

    // ---------------- DEBUG DRAW ----------------
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}
