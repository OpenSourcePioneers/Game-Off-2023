using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour, IDamageable
{
    #region Class variables
    [SerializeField] protected Player player;
    [SerializeField] public LayerMask playerMask;
    [SerializeField] private LayerMask walkable;
    [SerializeField] public TwoD_Grid grid;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] public float speed;
    [SerializeField] public float chaseRad;
    [SerializeField] private float rotSpeed;

    [HideInInspector] public Vector3 vec;
    [HideInInspector] public LayerMask obstacle;
    [HideInInspector] public bool lockedAtTarget;
    [HideInInspector] public bool finishedPath = false;
    [HideInInspector] public float disToPlayer;
    [HideInInspector] public float defSpeed;
    [HideInInspector] public int attackInd;
    #endregion

    #region  Private variables
    Coroutine waitCoroutine;
    protected Rigidbody enemyRb;
    List<float> dRadius = new List<float>();
    List<Color> dColor = new List<Color>();
    Vector3[] path = new Vector3[0];
    bool gettingPath = true;
    bool canMove;
    public bool canTransition = true;
    protected bool changedVec;
    public bool WalkState = true;
    float size;
    int pathInd;
    #endregion

    #region State variables
    public EnemyMachine machine {get; set;}
    public EnemyAttack attack {get; set;}
    public EnemyChase chase {get; set;}
    public EnemyCombat combat {get; set;}
    public EnemyWander wander {get; set;}
    #endregion

    #region Scriptable Object variables
    [SerializeField] private List<AttackSOBase> attackSOBase;
    [SerializeField] private CombatSOBase combatSOBase;
    [SerializeField] private ChaseSOBase chaseSOBase;
    [SerializeField] private WanderSOBase wanderSOBase;
    public List<AttackSOBase> instAttackBase {get; set;} = new List<AttackSOBase>();
    public ChaseSOBase instChaseBase {get; set;}
    public CombatSOBase instCombatBase {get; set;}
    public WanderSOBase instWanderBase {get; set;}
    #endregion
    
    #region IDamagable
    [field: SerializeField] public float maxHealth {get; set;} = 10f;
    public float curHealth {get; set;}
    public void Damage(float amount, Vector3 target, bool customConcussion = false)
    {
        //ReduceHealth
        curHealth -= amount;
        //Add KnockBack
        Vector3 dir = (target - transform.position).normalized;
        enemyRb.AddForce(dir * (amount + Universe.knockback) * Time.deltaTime, ForceMode.Impulse);
        //Apply concussion based on amount of damage
        if(!customConcussion)
            Concussion(0.1f + amount/15);
        //Destroy if no Health
        if(curHealth < 0)
            Destroy(this.gameObject);
    }

    public void Concussion(float amount)
    {
        StartCoroutine(StopTransitionFor(amount));
    }

    private IEnumerator StopTransitionFor(float amount)
    {
        canTransition = false;
        yield return new WaitForSeconds(amount);
        canTransition = true;
    }
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        AssignComponents();
    }

    void Start()
    {
        InitializeStates();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCall();
    }

    void FixedUpdate()
    {
        FixedUpdateCall();
    }

    #region Movement
    void CheckPath(Vector3[] _path, bool pathFound)
    {
        if(pathFound)
        {
            finishedPath = false;
            pathInd = 0;
            path = _path;
            gettingPath = false;
            //CheckForWayPoints();
        }
    }

    public void Move(bool _canMove = false)
    {
        if(_canMove == false)
            _canMove = canMove;
        Vector3 dir = new Vector3(vec.x, 0f, vec.z);
        //Movement
        if(enemyRb.velocity.sqrMagnitude < speed * speed && _canMove)
        {
            enemyRb.AddForce(dir.normalized * speed * Universe.forceMult * 
                Time.fixedDeltaTime, ForceMode.Force);
        }
    }

    public void CheckForWayPoints()
    {
        if(finishedPath || path == null || gettingPath)
        {
            canMove = false;
            return;
        }
        canMove = true;
        vec = path[pathInd] - transform.position;
        if(!lockedAtTarget)
            RotateToTarget(path[pathInd]);
        if(TwoD_Grid.NodeFromWorldPosition(path[pathInd]) == TwoD_Grid.NodeFromWorldPosition(transform.position))
        {
            pathInd++;
            changedVec = true;
            if(pathInd >= path.Length)
                finishedPath = true;
        }
        else
            changedVec = false;
    }

    public void PushPathRequest(Vector3 target)
    {
        Vector3 dVec = target - transform.position;
        Ray ray = new Ray(transform.position, dVec.normalized);
        gettingPath = true;
        PathManager.RequestPath(transform.position, target, CheckPath);
    }
    #endregion

    #region Utility
    public void RotateToTarget(Vector3 target, bool direct = false)
    {
        target -= transform.position;
        if(!direct)
            target = new Vector3(target.x, 0f, target.z);
        transform.forward = Vector3.Slerp(transform.forward, target, rotSpeed * Time.deltaTime);
    }

    public bool AimAtPlayer(float time,ref float curTime, bool direct = false, float size = 0.1f)
    {
        Ray ray = new Ray(new Vector3(transform.position.x, player.transform.position.y, 
            transform.position.z), transform.forward.normalized);
        if(Physics.SphereCast(ray, size, chaseRad * 2, playerMask))
        {
            curTime += Time.deltaTime;
            if(curTime > time)
            {
                lockedAtTarget = false;
                return true;
            }
        }
        else
        {
            //Rotate to look at player
            RotateToTarget(player.transform.position, direct);

            lockedAtTarget = true;
            curTime = 0f;
        }
        return false;
    }

    public bool GroundCheck()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(ray, size + 0.2f, walkable))
            return true;
        return false;
    }

    public void CallAfterTime(float time, Action<bool> callback)
    {
        waitCoroutine = StartCoroutine(Wait(time, callback));
    }

    private IEnumerator Wait(float time, Action<bool> callback)
    {
        yield return new WaitForSeconds(time);
        callback(true);
        waitCoroutine = null;
    }

    public void ResetPath()
    {
        gettingPath = false;
        finishedPath = true;
        vec = transform.position;
        path = new Vector3[0];
        pathInd = 0;
        if(waitCoroutine != null)
            StopCoroutine(waitCoroutine);
    }
    #endregion

    #region Protected for specific AI
    protected void AssignComponents()
    {
        if(player == null)
            player = GameObject.Find("Player").GetComponent<Player>();
        foreach (AttackSOBase attack in attackSOBase)
        {
            instAttackBase.Add(Instantiate(attack));
        }
        instChaseBase = Instantiate(chaseSOBase);
        instCombatBase = Instantiate(combatSOBase);
        instWanderBase = Instantiate(wanderSOBase);
        machine = new EnemyMachine();
        attack = new EnemyAttack(this, machine);
        chase = new EnemyChase(this, machine);
        combat = new EnemyCombat(this, machine);
        wander = new EnemyWander(this, machine);

        enemyRb = GetComponent<Rigidbody>();
    }

    protected void InitializeStates()
    {
        curHealth = maxHealth;
        size = GetComponent<Collider>().bounds.extents.y;
        defSpeed = speed;
        disToPlayer = (player.transform.position - transform.position).magnitude;
        obstacle = grid.unwalkableMask;
        //Initialize States
        foreach (AttackSOBase instAttack in instAttackBase)
        {
            instAttack.Initialize(this, gameObject);
        }
        instChaseBase.Initialize(this, gameObject);
        instCombatBase.Initialize(this, gameObject);
        instWanderBase.Initialize(this, gameObject);
        machine.SetState(wander);

        DebugCircle(chaseRad, Color.red);
    }
    protected void AssignComponentsForBosses()
    {
        if(player == null)
            player = GameObject.Find("Player").GetComponent<Player>();
        foreach (AttackSOBase attack in attackSOBase)
        {
            instAttackBase.Add(Instantiate(attack));
        }
        instCombatBase = Instantiate(combatSOBase);
        machine = new EnemyMachine();
        attack = new EnemyAttack(this, machine);
        combat = new EnemyCombat(this, machine);

        enemyRb = GetComponent<Rigidbody>();
    }
    protected void InitializeStatesForBosses()
    {
        curHealth = maxHealth;
        size = GetComponent<Collider>().bounds.extents.y;
        defSpeed = speed;
        disToPlayer = (player.transform.position - transform.position).magnitude;
        obstacle = grid.unwalkableMask;
        //Initialize States
        foreach (AttackSOBase instAttack in instAttackBase)
        {
            instAttack.Initialize(this, gameObject);
        }
        instCombatBase.Initialize(this, gameObject, true);
        machine.SetState(combat);
    }

    protected void UpdateCall()
    {
        if(canTransition)
        {
            if(stateText != null)
                stateText.text = new string($"State: {machine.curState.GetType()}");
            disToPlayer = (player.transform.position - transform.position).magnitude;
            machine.curState.FrameUpdate();
        }
    }
    protected void FixedUpdateCall() 
    {
        if(canTransition)
        machine.curState.FixedFrameUpdate();
    }

    #endregion

    #region Debug
    public void DebugCircle(float radius, Color color)
    {
        dRadius.Add(radius);
        dColor.Add(color);
    }

    void OnDrawGizmos() 
    {
        if(path == null || !Application.isPlaying)
            return;
        for(int i = pathInd; i < path.Length; i++)
        {
            Gizmos.DrawCube(path[i], Vector3.one * 2 * grid.nodeRadius);
            if(i+1 < path.Length)
                Gizmos.DrawLine(path[i], path[i+1]);
        }
        if(pathInd < path.Length)
            Gizmos.DrawLine(transform.position, path[pathInd]);

        for (int i = 0; i < dRadius.Count; i++)
        {
            Gizmos.color = dColor[i];
            Gizmos.DrawWireSphere(transform.position, dRadius[i]);
        }
    }
    #endregion
}
