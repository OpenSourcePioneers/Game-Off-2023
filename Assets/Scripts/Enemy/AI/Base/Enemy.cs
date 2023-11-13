using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private Player player;
    [SerializeField] public LayerMask playerMask;
    [SerializeField] private TwoD_Grid grid;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private float speed;
    [SerializeField] public float chaseRad;
    [SerializeField] private float rotSpeed;
    [HideInInspector] public float disToPlayer;

    Rigidbody enemyRb;
    List<float> dRadius = new List<float>();
    List<Color> dColor = new List<Color>();
    Vector3[] path = new Vector3[0];
    Vector3 vec;
    bool gettingPath = true;
    [HideInInspector] public bool finishedPath = false;
    bool canMove;
    int pathInd;
    [HideInInspector] public LayerMask obstacle;

    #region State variables
    public EnemyMachine machine {get; set;}
    public EnemyAttack attack {get; set;}
    public EnemyChase chase {get; set;}
    public EnemyCombat combat {get; set;}
    public EnemyWander wander {get; set;}
    #endregion

    #region Scriptable Object variables
    [SerializeField] private AttackSOBase attackSOBase;
    [SerializeField] private ChaseSOBase chaseSOBase;
    [SerializeField] private CombatSOBase combatSOBase;
    [SerializeField] private WanderSOBase wanderSOBase;
    public AttackSOBase instAttackBase {get; set;}
    public ChaseSOBase instChaseBase {get; set;}
    public CombatSOBase instCombatBase {get; set;}
    public WanderSOBase instWanderBase {get; set;}

    #endregion
    
    #region IDamagable
    [field: SerializeField] public float maxHealth {get; set;} = 10f;
    public float curHealth {get; set;}
    public void Damage(float amount) => curHealth -= amount;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        instAttackBase = Instantiate(attackSOBase);
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

    void Start()
    {
        InitializeStates();
    }

    // Update is called once per frame
    void Update()
    {
        stateText.text = new string($"State: {machine.curState.GetType()}");
        disToPlayer = (player.transform.position - transform.position).magnitude;
        machine.curState.FrameUpdate();
    }

    void FixedUpdate()
    {
        machine.curState.FixedFrameUpdate();
    }

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

    public void Move()
    {
        Vector3 dir = new Vector3(vec.x, 0f, vec.z);
        //Movement
        if(enemyRb.velocity.sqrMagnitude < speed * speed && canMove)
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
        RotateToTarget(path[pathInd]);
        if(vec.sqrMagnitude < 1)
        {
            pathInd++;
            if(pathInd >= path.Length)
                finishedPath = true;
        }
    }

    public void PushPathRequest(Vector3 target)
    {
        gettingPath = true;
        PathManager.RequestPath(transform.position, target, CheckPath);
    }

    public void RotateToTarget(Vector3 target)
    {
        target = target - transform.position;
        target = new Vector3(target.x, 0f, target.z);
        transform.forward = Vector3.Slerp(transform.forward, target, rotSpeed * Time.deltaTime);
    }

    public void CallAfterTime(float time, Action<bool> callback)
    {
        StartCoroutine(Wait(time, callback));
    }

    private IEnumerator Wait(float time, Action<bool> callback)
    {
        yield return new WaitForSeconds(time);
        callback(true);
    }

    public void ResetPath()
    {
        gettingPath = false;
        finishedPath = true;
        path = new Vector3[0];
        pathInd = 0;
        StopCoroutine("Wait");
    }
    #region Protected for specific AI
    protected void InitializeStates()
    {
        curHealth = maxHealth;
        disToPlayer = (player.transform.position - transform.position).magnitude;
        obstacle = grid.unwalkableMask;
        instAttackBase.Initialize(this, gameObject);
        instChaseBase.Initialize(this, gameObject);
        instCombatBase.Initialize(this, gameObject);
        instWanderBase.Initialize(this, gameObject);
        machine.SetState(wander);

        DebugCircle(chaseRad, Color.red);
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
            Gizmos.DrawCube(path[i], Vector3.one * grid.nodeRadius);
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
