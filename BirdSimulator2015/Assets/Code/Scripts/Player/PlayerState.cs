using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerState : MonoBehaviour
{
    public enum BirdState
    {
        Hovering,
        Gliding,
        Easing,
        EasingAndTurningLeft,
        EasingAndTurningRight,
        SwoopingUpAfterEasing,
        Decelerating,
        TurningLeft,
        TurningRight,
        Diving,
        DivingAndTurningLeft,
        DivingAndTurningRight,
        Descending,
        Ascending,
        AscendingAndTurningLeft,
        AscendingAndTurningRight,
        AboutFacing,
        FlappingForward,
        Landing,
        Grounded,

        QuickAscending,
        RollingLeft,
        RollingRight,

        LiftingOff,

        // cosmetics
        RandomFlapping,

        DashingForward,
        
        HoveringAndTurningLeft,
        HoveringAndTurningRight,
    }

    private Animator animator;
    private Dictionary<int, BirdState> hash = new Dictionary<int, BirdState>();
    private BirdState state;

    const float MAX_FORWARD_VELOCITY = 25f;
    const float MAX_DOWNWARD_VELOCITY = 40f;
    const float DOWNWARD_ACCELERATION = 1f;
    const float MAX_UPWARD_VELOCITY = 5f;
    const float MAX_FORWARD_VELOCITY_WHEN_ASCENDING = 25f;
    const float DIVE_RATE = 50f;
    const float TURN_RATE_INITIAL = 20f;
    const float TURN_RATE_MAX = 80f;
    const float TURN_ACCELERATION = 10f;
    const float TURN_ACCELERATION_WHILE_DIVING = 5f;
    const float TURN_RATE_WHEN_DIVING = 200f;
    const float EASE_RATE = 100f;
    const float MOMENTUM_LOSS_RATE = 50f;
    const float TURN_SHARPNESS = 1.4f;
    const float DECELERATION_RATE = 200f;

    const float TILT_LIMIT = 70f;

    // we need this so we don't slowly descend (usually when gliding forward and turning at low speeds)
    // for some dumb reason, the y component of the velocity vector becomes -0.5f, so the player will slowly
    // descend and start hitting stuff. this is really annoying, so we compensate.
    const float LIFT_OFFSET = 0.5f;


    private float currentMaxSpeed = 0f;
    private float currentTurnSpeed = TURN_RATE_INITIAL;
    private Vector3 targetVelocity = Vector3.zero;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private float rotationZ = 0f; // of child transform
    private float momentum = 0f;

    // collision
    private RaycastHit hit;
    private bool flipped = false;
    private int tilting = 0;

    // collision trigger (landing)
    public Vector3 LandPos = Vector3.zero;
    public bool CanLand = false;

    void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    void Start()
    {
        foreach (BirdState state in (BirdState[])System.Enum.GetValues(typeof(BirdState)))
        {
            hash.Add(Animator.StringToHash("Base Layer." + state.ToString()), state);
        }
    }

    void FixedUpdate()
    {
        state = hash[animator.GetCurrentAnimatorStateInfo(0).nameHash];

        Debug.DrawRay(this.transform.position, this.rigidbody.velocity * 5f, Color.magenta);
        Debug.DrawRay(this.transform.position, this.transform.up * 1f, Color.blue);
        Debug.DrawRay(this.transform.position, this.transform.forward * 1f, Color.green);
        Debug.DrawRay(this.transform.position, Vector3.down * 1f, Color.red);

        Vector3 from = this.transform.position;
        Vector3 direction = this.transform.forward;

        if (Physics.Raycast(from, direction, out hit, 1f))
        {
            if (state != BirdState.AboutFacing && state != BirdState.FlappingForward)
            {
                Debug.DrawRay(hit.point, hit.normal, Color.red, 5f);
                Debug.Log(Vector3.Angle(hit.normal, -direction));

                if (Vector3.Angle(hit.normal, -direction) < 35f)
                {
                    animator.SetTrigger("t_AboutFace");
                }
            }
        }

        Debug.DrawRay(from, (this.transform.forward + this.transform.right) * 0.8f, Color.black);
        Debug.DrawRay(from, (this.transform.forward - this.transform.right) * 0.8f, Color.black);
        if (Physics.Raycast(from, this.transform.forward + this.transform.right, out hit, 1f))
        {
            tilting = 1; // right
            tiltTowards(-TILT_LIMIT); // tilt away (opposite)

            if (Physics.Raycast(from, this.transform.forward + this.transform.right, out hit, 0.7f))
            {
                rotationY = Mathf.Lerp(rotationY, rotationY - 20f, Time.deltaTime);
                this.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationY, 0);
            }

        }
        else if (Physics.Raycast(from, this.transform.forward - this.transform.right, out hit, 1f))
        {
            tilting = -1; // left
            tiltTowards(TILT_LIMIT);

            if (Physics.Raycast(from, this.transform.forward - this.transform.right, out hit, 0.7f))
            {
                rotationY = Mathf.Lerp(rotationY, rotationY + 20f, Time.deltaTime);
                this.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationY, 0);
            }
        }
        else
        {
            tilting = 0; // none
        }
    }

    void Update()
    {
        // prolly make this better or something
        animator.ResetTrigger("t_RandomFlap");
        if (Random.Range(1, 500) == 2)
        {
            animator.SetTrigger("t_RandomFlap");
        }

        // update body rotation
        rotationX = transform.localEulerAngles.x;
        rotationY = transform.localEulerAngles.y;

        // update model rotation (tilt)
        Vector3 rot = this.transform.GetChild(0).transform.localEulerAngles;
        rot.z = rotationZ;
        this.transform.GetChild(0).transform.localEulerAngles = rot;

        // decelerate over time, maybe remove this?
        currentMaxSpeed -= Time.deltaTime * 0.1f;
        currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, 0, MAX_FORWARD_VELOCITY);

        switch (state)
        {
            case BirdState.Hovering:
                ease();
                tiltTowards(0);
                break;

            case BirdState.Gliding:
                // reset some stuff
                flipped = false;
                momentum = 0f;
                currentTurnSpeed = TURN_RATE_INITIAL;

                // make sure we're not slamming against a wall, since the gliding state applies there also
                if (tilting == 0)
                    tiltTowards(0);

                this.transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
                targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * LIFT_OFFSET;
                break;

            case BirdState.Easing:
                // TODO diddle with momentum numbers
                currentMaxSpeed += momentum * 2f * Time.deltaTime;
                currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, 0, MAX_FORWARD_VELOCITY);
                ease();
                tiltTowards(0);
                targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY + Vector3.up * MAX_FORWARD_VELOCITY;
                break;

            case BirdState.EasingAndTurningLeft:
                rotationY -= currentTurnSpeed * Time.deltaTime;
                currentTurnSpeed += TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed;
                currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);

                currentMaxSpeed += momentum * Time.deltaTime;
                currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, 0, MAX_FORWARD_VELOCITY);
                ease();
                tiltTowards(-TILT_LIMIT);

                targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY + Vector3.up * MAX_FORWARD_VELOCITY - this.transform.right * currentMaxSpeed;
                break;

            case BirdState.EasingAndTurningRight:
                rotationY += currentTurnSpeed * Time.deltaTime;
                currentTurnSpeed += TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed;
                currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);

                currentMaxSpeed += momentum * Time.deltaTime;
                currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, 0, MAX_FORWARD_VELOCITY);
                ease();
                tiltTowards(TILT_LIMIT);

                targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY + Vector3.up * MAX_FORWARD_VELOCITY + this.transform.right * currentMaxSpeed;
                break;

            case BirdState.SwoopingUpAfterEasing:
                targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY;
                if (momentum > 0)
                {
                    // TODO make this better
                    //rotation = transform.localEulerAngles.x - TURN_RATE_INITIAL * Time.deltaTime;	
                    /////////if (rotation 
                    //transform.localEulerAngles = new Vector3(rotation, rotationY, 0);

                    //targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY + Vector3.up * MAX_FORWARD_VELOCITY;
                    //momentum -= MOMENTUM_LOSS_RATE * Time.deltaTime;
                }
                break;

            case BirdState.Descending:
                rotationX = transform.localEulerAngles.x + DIVE_RATE * Time.deltaTime;
                //rotationX = Mathf.Clamp(rotationX, 1f, Mathf.Abs(Input.GetAxisRaw("JoystickAxisY")) * 85f);
                rotationX = Mathf.Clamp(rotationX, 1f, 85f);
                transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);

                targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY_WHEN_ASCENDING + Vector3.down * MAX_UPWARD_VELOCITY;
                break;

            case BirdState.Diving:
                addMomentum();
                dive();
                tiltTowards(0);

                targetVelocity = this.transform.forward + Vector3.down * MAX_DOWNWARD_VELOCITY;
                break;

            case BirdState.DivingAndTurningLeft:
                addMomentum();
                 tiltTowards(0);

                rotationY -= currentTurnSpeed * Time.deltaTime;
                currentTurnSpeed += TURN_ACCELERATION_WHILE_DIVING * Time.deltaTime;
                Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
                dive();

                targetVelocity = this.transform.forward + Vector3.down * MAX_DOWNWARD_VELOCITY;
                break;

            case BirdState.DivingAndTurningRight:
                addMomentum();
                tiltTowards(0);

                rotationY += currentTurnSpeed * Time.deltaTime;
                currentTurnSpeed += TURN_ACCELERATION_WHILE_DIVING * Time.deltaTime;
                Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
                dive();

                targetVelocity = this.transform.forward + Vector3.down * MAX_DOWNWARD_VELOCITY;
                break;

            case BirdState.Decelerating:
                ease();
                tiltTowards(0);
                currentMaxSpeed = 0;
                targetVelocity = Vector3.zero;
                this.rigidbody.velocity = this.rigidbody.velocity * 0.4f;
                break;

            case BirdState.TurningLeft:
                if (tilting != -1)
                {
                    // OLD too jittery and shit
                    //this.transform.Rotate(this.transform.up, -TURN_RATE * Time.deltaTime, Space.Self);

                    rotationY -= currentTurnSpeed * Time.deltaTime * TURN_SHARPNESS;
                    currentTurnSpeed += Mathf.Pow(Mathf.Abs(Input.GetAxisRaw("JoystickAxisX")), 2) * TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed;
                    currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);

                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationY, 0);

                    tiltTowards(-TILT_LIMIT);

                    // TODO fuck around with this modifier to make turning feel sharp at high speeds
                    targetVelocity = this.transform.forward * currentMaxSpeed - this.transform.right * currentMaxSpeed + Vector3.up * LIFT_OFFSET;
                }
                else { targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * LIFT_OFFSET; }
                break;

            case BirdState.TurningRight:
                if (tilting != 1)
                {
                    // OLD too jittery and shit
                    //this.transform.Rotate(this.transform.up, TURN_RATE * Time.deltaTime, Space.Self);

                    rotationY += currentTurnSpeed * Time.deltaTime * TURN_SHARPNESS;
                    currentTurnSpeed += Mathf.Pow(Mathf.Abs(Input.GetAxisRaw("JoystickAxisX")), 2) * TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed;
                    currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);

                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationY, 0);

                    tiltTowards(TILT_LIMIT);

                    // TODO fuck around with this modifier to make turning feel sharp at high speeds
                    targetVelocity = this.transform.forward * currentMaxSpeed + this.transform.right * currentMaxSpeed + Vector3.up * LIFT_OFFSET;
                }
                else { targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * LIFT_OFFSET; }
                break;

            case BirdState.Ascending:
                tiltTowards(0);

                rotationX = transform.localEulerAngles.x - DIVE_RATE * Time.deltaTime;
                rotationX = Mathf.Clamp(rotationX, 1f, 85f);
                transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);

                targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * MAX_UPWARD_VELOCITY;
                break;

            case BirdState.AscendingAndTurningLeft:
                tiltTowards(-TILT_LIMIT);

                rotationY -= currentTurnSpeed * Time.deltaTime;
                currentTurnSpeed += TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed;
                currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationY, 0);

                // OLD removed shitty turning
                //this.transform.Rotate(this.transform.up, -TURN_RATE_INITIAL * Time.deltaTime, Space.Self);

                // TODO fuck around with this modifier to make turning feel sharp at high speeds
                targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * MAX_UPWARD_VELOCITY - this.transform.right * currentMaxSpeed;

                // OLD removed shitty velocity	
                //targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY;
                break;

            case BirdState.AscendingAndTurningRight:
                tiltTowards(TILT_LIMIT);

                rotationY += currentTurnSpeed * Time.deltaTime;
                currentTurnSpeed += TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed;
                currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationY, 0);

                // OLD removed shitty turning
                //this.transform.Rotate(this.transform.up, TURN_RATE_INITIAL * Time.deltaTime, Space.Self);

                // TODO fuck around with this modifier to make turning feel sharp at high speeds
                targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * MAX_UPWARD_VELOCITY + this.transform.right * currentMaxSpeed;

                // OLD removed shitty velocity	
                //targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY;
                break;

            case BirdState.AboutFacing:
                tiltTowards(0);
                momentum = 0f;
                currentMaxSpeed = 0f;

                this.rigidbody.velocity = Vector3.zero;
                targetVelocity = Vector3.zero + Vector3.up * LIFT_OFFSET;
                break;

            case BirdState.FlappingForward:
			    if (!flipped)
			    {
                    // flip once
                    flipped = true;
				    this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y + 180f, this.transform.localEulerAngles.z);
			    }

			    //Debug.DrawRay (this.transform.position, hit.normal * 5f, Color.cyan, 2f);
			    currentMaxSpeed = 10f;
			    // instant or velocity over time?
			    targetVelocity = hit.normal * currentMaxSpeed;
			    //this.rigidbody.velocity = hit.normal * currentMaxSpeed;
                break;

            case BirdState.QuickAscending:
				tiltTowards(0);
				this.rigidbody.velocity += Vector3.up * 0.5f;
                break;

            case BirdState.RollingLeft:
                tiltTowards(TILT_LIMIT / 2);
                this.rigidbody.velocity -= this.transform.right;
                break;

            case BirdState.RollingRight:
                tiltTowards(-TILT_LIMIT / 2);
                this.rigidbody.velocity += this.transform.right;
                break;

            case BirdState.Landing:
                tiltTowards(0);
                momentum = 0f;
                currentMaxSpeed = 0f;

                this.rigidbody.velocity = Vector3.zero;
                targetVelocity = Vector3.zero + Vector3.up * LIFT_OFFSET;

                // move this body to the center of the landing zone
                this.rigidbody.MovePosition(this.rigidbody.position + (LandPos - this.transform.position) * Time.deltaTime);

                if (Vector3.Distance(this.transform.position, LandPos) < 0.05f)
                {
                    animator.SetBool("b_Grounded", true);
                }

                break;

            case BirdState.Grounded:
                tiltTowards(0);
                momentum = 0f;
                currentMaxSpeed = 0f;

                this.rigidbody.velocity = Vector3.zero;
                targetVelocity = Vector3.zero + Vector3.up * LIFT_OFFSET;
                break;

            case BirdState.LiftingOff:
                tiltTowards(0);
                targetVelocity = Vector3.up * 5f;
                break;

            case BirdState.DashingForward:
                tiltTowards(0);
                this.rigidbody.velocity += this.transform.forward * 0.5f; // instant impulse
                currentMaxSpeed += Time.deltaTime * 10f;
                break;

			case BirdState.HoveringAndTurningLeft:
				rotationY -= currentTurnSpeed * Time.deltaTime * TURN_SHARPNESS;
				currentTurnSpeed += TURN_ACCELERATION * Time.deltaTime;
				currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
				
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationY, 0);
				
				tiltTowards(-TILT_LIMIT * 0.2f);
				break;
				
			case BirdState.HoveringAndTurningRight:
				rotationY += currentTurnSpeed * Time.deltaTime * TURN_SHARPNESS;
				currentTurnSpeed += TURN_ACCELERATION * Time.deltaTime;
				currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
				
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationY, 0);
				
				tiltTowards(TILT_LIMIT * 0.2f);
				break;

        }

        // update the actual body
        this.rigidbody.velocity = Vector3.Lerp(this.rigidbody.velocity, targetVelocity, Time.deltaTime);

        // update the animator
        animator.SetFloat("Rotation", this.transform.localEulerAngles.x);
        animator.SetFloat("Momentum", momentum);
        animator.SetFloat("Velocity", this.transform.rigidbody.velocity.magnitude);
    }

    void dive()
    {
        rotationX = transform.localEulerAngles.x + DIVE_RATE * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, 1f, 85f);
        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
    }

    void ease()
    {
        rotationX = transform.localEulerAngles.x - EASE_RATE * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, 1f, 85f);
        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
    }

    const float TILT_EASE_RATE = 5f;
    void tiltTowards(float f)
    {
        rotationZ = Mathf.Lerp(rotationZ, f, Time.deltaTime * TILT_EASE_RATE);
    }

    const float MOMENTUM_GAIN = 5f;
    void addMomentum()
    {
        momentum += MOMENTUM_GAIN * Time.deltaTime;
    }

    public BirdState GetState()
    {
        return state;
    }
}
