using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Player : MonoBehaviour, IDamageable, IInputable
{
    #region Components
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private Transform mainCam;
    [SerializeField] private Transform feet;
    [SerializeField] private Transform orientation;
    #endregion

    [SerializeField] public AnimationCurve jumpCurve;
    [SerializeField] public AnimationCurve dashCurve;
    [field: SerializeField] public float maxHealth {get; set;} = 10f;
    public float curHealth {get; set;}
    [SerializeField] public float wSpeed;
    [SerializeField] public float rSpeed;
    [SerializeField] public float aSpeed;
    [SerializeField] public float rbDrag;
    [SerializeField] private float rotSpeed;
    [SerializeField] private LayerMask walkable;
    [HideInInspector] public Vector3 inp;

    #region StateMachine variables
    public StateMachine machine {get; set;}
    public Idle idle {get; set;}
    public Walk walk {get; set;}
    public Run run {get; set;}
    public Jump jump {get; set;}
    public Dash dash {get; set;}
    public InAir inAir {get; set;}
    #endregion
    
    #region Private variables
    const  float forceMult = 5000f;
    #endregion
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        curHealth = maxHealth;
        playerRb.drag = rbDrag;
        //Setup StateMachine
        machine = new StateMachine();
        idle = new Idle(this, machine);
        walk = new Walk(this, machine);
        run = new Run(this, machine);
        jump = new Jump(this, machine);
        dash = new Dash(this, machine);
        inAir = new InAir(this, machine);
        machine.SetState(idle);
    }

    void Update()
    {
        CamToFocused();
        State nCurState = machine.curState;
        if(nCurState != inAir && !OnGround())
        {
            if(nCurState != jump && nCurState != dash)
                machine.ChangeState(inAir);
        }
        //machine.globalState.FrameUpdate();
        machine.curState.FrameUpdate();
        stateText.text = new string($"State: {machine.curState.GetType().ToString()}");
    }

    void FixedUpdate()
    {
        //machine.globalState.FixedFrameUpdate();
        machine.curState.FixedFrameUpdate();
    }

    public void Move(float speed)
    {
        //Vector3 dir =  inp * speed * Time.fixedDeltaTime;
        //dir = new Vector3(dir.x, playerRb.velocity.y, dir.z);
        MoveInDir(speed, inp);
    }


    public void MoveInDir(float speed, Vector3 dir)
    {
        if(playerRb.velocity.sqrMagnitude < speed * speed)
        {
            playerRb.AddForce(dir * speed * forceMult * Time.fixedDeltaTime, ForceMode.Force);
        }
    }

    public void CamToFocused()
    {
        Vector3 lookDir = transform.position - new Vector3(mainCam.position.x, transform.position.y,mainCam.position.z);
        orientation.forward = lookDir.normalized;
        inp = new Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal")).normalized;
        inp = (inp.x * orientation.forward + inp.z * orientation.right).normalized;
        transform.forward = Vector3.Slerp(transform.forward, inp.normalized, rotSpeed * Time.fixedDeltaTime);
    }

    public void CamToFree()
    {
        transform.forward += inp;
    }

    public bool OnGround()
    {
        if(Physics.CheckSphere(feet.position, 0.1f, walkable))
            return true;
        else
            return false;
    }
    
    public void Damage(float amount) => curHealth -= amount;
    
    #region Input interface
    public bool IsMoving()
    {
        if(inp.magnitude > 0.01f)
            return true;
        else
            return false;
    }
    public bool IsShiftClicked()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
            return true;
        else
            return false;
    }

    public bool IsShiftHold()
    {
        if(Input.GetKey(KeyCode.LeftShift))
            return true;
        else
            return false;
    }

    public bool IsSpacePressed()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            return true;
        else
            return false;
    }
    #endregion
}
