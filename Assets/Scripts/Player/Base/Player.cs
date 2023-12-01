using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Player : MonoBehaviour, IDamageable, IInputable
{
    #region Components
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Transform mainCam;
    [SerializeField] private Transform feet;
    [SerializeField] private Transform orientation;
    #endregion

    #region ClassVariables
    [SerializeField] public AnimationCurve jumpCurve;
    [SerializeField] public AnimationCurve dashCurve;
    [HideInInspector] public bool grounded;
    [SerializeField] public float wSpeed;
    [SerializeField] public float rSpeed;
    [SerializeField] public float aSpeed;
    [SerializeField] public float rbDrag;
    [SerializeField] private float rotSpeed;
    [SerializeField] public float dashDelay;
    [SerializeField] private float damage = 5f;
    [SerializeField] private LayerMask walkable;
    [HideInInspector] public Vector3 inp;
    [HideInInspector] public List<IDamageable> damaged = new List<IDamageable>();
    #endregion

    #region Private variables
    bool canTransition = true;
    bool stoppedEvent = true;
    float timeSinceLastAttack = 0f;
    #endregion

    #region StateMachine variables
    public PlayerMachine machine {get; set;}
    public Idle idle {get; set;}
    public Walk walk {get; set;}
    public Run run {get; set;}
    public Jump jump {get; set;}
    public Dash dash {get; set;}
    public AirCtrl airCtrl {get; set;}
    public Attack attack {get; set;}
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
        playerRb.AddForce(dir * (amount + Universe.knockback) * Time.deltaTime, ForceMode.Impulse);
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
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        curHealth = maxHealth;
        playerRb.drag = rbDrag;
        //Setup StateMachine
        machine = new PlayerMachine();
        idle = new Idle(this, machine);
        walk = new Walk(this, machine);
        run = new Run(this, machine);
        jump = new Jump(this, machine);
        dash = new Dash(this, machine);
        airCtrl = new AirCtrl(this, machine);
        attack = new Attack(this, machine);
        machine.SetState(idle);
    }

    void Update()
    {
        if(canTransition)
        {
            ConfigCam();
            machine.curState.FrameUpdate();

            //SFX
            if(IsMoving() && stoppedEvent)
            {
                AkSoundEngine.PostEvent("Play_PC_Footstep", this.gameObject);
                stoppedEvent = false;
            }
            else if(!IsMoving() && !stoppedEvent)
            {
                AkSoundEngine.PostEvent("Stop_PC_Footstep", this.gameObject);
                stoppedEvent = true;
            }

            //UI elements
            if(healthText != null)
                healthText.text = new string($"Health: {curHealth}");
        }
        timeSinceLastAttack += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if(canTransition)
            machine.curState.FixedFrameUpdate();
    }

    public void Move(float speed)
    {
        MoveInDir(speed, inp);
    }

    public void MoveInDir(float speed, Vector3 dir)
    {
        if(playerRb.velocity.sqrMagnitude < speed * speed)
        {
            playerRb.AddForce(dir * speed * Universe.forceMult * 
                Time.fixedDeltaTime, ForceMode.Force);
        }
    }

    public void ConfigCam()
    {
        Vector3 lookDir = transform.position - new Vector3(mainCam.position.x, transform.position.y,mainCam.position.z);
        orientation.forward = lookDir.normalized;
        inp = new Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal")).normalized;
        if(inp.sqrMagnitude == 0f)
            return;
        inp = (inp.x * orientation.forward + inp.z * orientation.right).normalized;
        transform.forward = Vector3.Slerp(transform.forward, inp.normalized, 
            rotSpeed * Time.deltaTime);
    }

    public bool OnGround()
    {
        if(Physics.CheckSphere(feet.position, 0.1f, walkable))
            return true;
        else
            return false;
    }

    void OnCollisionStay(Collision other)
    {
        IDamageable iDamageable;
        if(other.gameObject.TryGetComponent<IDamageable>(out iDamageable))
        {
            bool lMouse = false;
            if(canTransition)
                lMouse = IsLMouseClick();
            if(machine.curState == dash || lMouse)
            {
                foreach (IDamageable iDam in damaged)
                {
                    if(iDam == iDamageable)
                        return;
                }
                iDamageable.Damage(damage, other.collider.ClosestPoint(transform.position), true);
                iDamageable.Concussion(2f);
                timeSinceLastAttack = 0f;
                if(!lMouse)
                    damaged.Add(iDamageable);
            }
        }
    }

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

    public bool IsLMouseClick()
    {
        if(Input.GetMouseButtonDown(0) && timeSinceLastAttack > 0.5f)
        {
            Debug.Log("Working");
            return true;
        }    
        else
            return false;
    }

    public bool IsLMouseHold()
    {
        if(Input.GetMouseButton(0))
            return true;
        else
            return false;
    }

    public bool IsRMouseClick()
    {
        if(Input.GetMouseButtonDown(1))
            return true;
        else
            return false;
    }

    public bool IsRMouseHold()
    {
        if(Input.GetMouseButtonDown(1))
            return true;
        else
            return false;
    }
    #endregion
}
