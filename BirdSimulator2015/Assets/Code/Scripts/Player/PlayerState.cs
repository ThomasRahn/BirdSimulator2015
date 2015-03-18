using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerState : MonoBehaviour
{
    public enum BirdState
    {
        Hovering,
		HoveringAndTurningLeft,
		HoveringAndTurningRight,

		Gliding,
		RandomFlapping,
		
		Easing,
        EasingAndTurningLeft,
        EasingAndTurningRight,

        Decelerating,

        TurningLeft,
        TurningRight,

		Descending,
		DescendingAndTurningLeft,
		DescendingAndTurningRight,
		Diving,
        DivingAndTurningLeft,
        DivingAndTurningRight,

        Ascending,
        AscendingAndTurningLeft,
        AscendingAndTurningRight,

        AboutFacing,
        FlappingForward,
        Landing,
        Grounded,
		LiftingOff,

        RollingLeft,
        RollingRight,
		QuickAscending,
		DashingForward,

		Dying,
        Respawning,

	}
	
	private Animator animator;
    private Dictionary<int, BirdState> hash = new Dictionary<int, BirdState>();
    private BirdState state;

	const float MIN_FORWARD_VELOCITY = 15f;
    const float MAX_FORWARD_VELOCITY = 35f;
    const float MAX_DOWNWARD_VELOCITY = 80f;
    const float DOWNWARD_ACCELERATION = 5f;
    const float MAX_UPWARD_VELOCITY = 5f;
    const float MAX_FORWARD_VELOCITY_WHEN_ASCENDING = 25f;
    const float DESCENT_RATE = 50f;
    const float TURN_RATE_INITIAL = 20f;
    const float TURN_RATE_MAX = 80f;
    const float TURN_ACCELERATION = 10f;
    const float TURN_ACCELERATION_WHILE_DIVING = 5f;
    const float TURN_RATE_WHEN_DIVING = 200f;
	const float TURN_RATE_WHEN_IDLE = 80f;
	const float EASE_RATE = 100f;
    const float MOMENTUM_LOSS_RATE = 50f;
    const float TURN_SHARPNESS = 1.4f;
    const float DECELERATION_RATE = 500f;
    const float DIVE_STRAFE_RATE = 40f;

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
    public Transform LandTarget;

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
        state = hash[animator.GetCurrentAnimatorStateInfo(0).fullPathHash];

        Debug.DrawRay(this.transform.position, this.GetComponent<Rigidbody>().velocity * 5f, Color.magenta);
        Debug.DrawRay(this.transform.position, this.transform.up * 1f, Color.blue);
        Debug.DrawRay(this.transform.position, this.transform.forward * 1f, Color.green);
        Debug.DrawRay(this.transform.position, Vector3.down * 1f, Color.red);

        Vector3 from = this.transform.position;
        Vector3 direction = this.transform.forward;

		if (Physics.Raycast(from, this.GetComponent<Rigidbody>().velocity, out hit, 3f))
        {
            if (state != BirdState.AboutFacing && state != BirdState.FlappingForward)
            {
                //Debug.DrawRay(hit.point, hit.normal, Color.red, 5f);
                //Debug.Log(Vector3.Angle(hit.normal, -direction));

				if (Vector3.Angle(hit.normal, Vector3.up) < 10f)
				{
					// TODO land or swoop up
				}

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

	float intendedTurnSpeed;
    void Update()
    {
        // prolly make this better or something
        animator.ResetTrigger("t_RandomFlap");
        if (Random.Range(1, 50) == 2)
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

        // accelerate over time
        currentMaxSpeed += Time.deltaTime * 0.1f;
		currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, MIN_FORWARD_VELOCITY, MAX_FORWARD_VELOCITY);

        switch (state)
        {
            case BirdState.Hovering:
                ease();
                tiltTowards(0);
                rotationY += this.GetComponent<PlayerInput>().GetAxisHorizontal() * Time.deltaTime * TURN_RATE_WHEN_IDLE;
				break;

            case BirdState.Gliding:
                // reset some stuff
                flipped = false;
                momentum = 0f;
                currentTurnSpeed = TURN_RATE_INITIAL;

                // make sure we're not slamming against a wall, since the gliding state applies there also
                if (tilting == 0)
                    tiltTowards(0);

                targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * LIFT_OFFSET;
                break;

            case BirdState.Easing:
                // TODO diddle with momentum numbers
                currentMaxSpeed += momentum * 2f * Time.deltaTime;
                currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, 0, MAX_FORWARD_VELOCITY);
                ease();
                tiltTowards(0);
                targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY + Vector3.up * -this.GetComponent<Rigidbody>().velocity.y;
                break;

            case BirdState.EasingAndTurningLeft:
				/*
                rotationY -= currentTurnSpeed * Time.deltaTime;
                currentTurnSpeed += TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed;
                currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);

                currentMaxSpeed += momentum * Time.deltaTime;
                currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, 0, MAX_FORWARD_VELOCITY);
                ease();
                tiltTowards(-TILT_LIMIT);

                targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY + Vector3.up * MAX_FORWARD_VELOCITY - this.transform.right * currentMaxSpeed;
                */
                break;

            case BirdState.EasingAndTurningRight:
				/*
                rotationY += currentTurnSpeed * Time.deltaTime;
                currentTurnSpeed += TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed;
                currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);

                currentMaxSpeed += momentum * Time.deltaTime;
                currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, 0, MAX_FORWARD_VELOCITY);
                ease();
                tiltTowards(TILT_LIMIT);

                targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY + Vector3.up * MAX_FORWARD_VELOCITY + this.transform.right * currentMaxSpeed;
				*/
				break;

            case BirdState.Descending:
				tiltTowards(0);

				rotationX = transform.localEulerAngles.x + DESCENT_RATE * Time.deltaTime;
				if (rotationX > 40 & rotationX < 90)
					rotationX = 40f;

				targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY_WHEN_ASCENDING + Vector3.down * MAX_UPWARD_VELOCITY;
                break;

			case BirdState.DescendingAndTurningLeft:
				rotationX = transform.localEulerAngles.x + DESCENT_RATE * Time.deltaTime;
				tiltTowards(-TILT_LIMIT);
				rotationY -= currentTurnSpeed * Time.deltaTime;

				if (rotationX > 40 & rotationX < 90)
					rotationX = 40f;

				targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY_WHEN_ASCENDING + Vector3.down * MAX_UPWARD_VELOCITY;
				break;

			case BirdState.DescendingAndTurningRight:
				rotationX = transform.localEulerAngles.x + DESCENT_RATE * Time.deltaTime;
				tiltTowards(TILT_LIMIT);
				rotationY += currentTurnSpeed * Time.deltaTime;
			
				if (rotationX > 40 & rotationX < 90)
					rotationX = 40f;

				targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY_WHEN_ASCENDING + Vector3.down * MAX_UPWARD_VELOCITY;
				break;
					
			case BirdState.Diving:
                //addMomentum();
                dive();
                tiltTowards(0);

				//rotationY += Input.GetAxisRaw("JoystickAxisX") * Time.deltaTime * TURN_RATE_WHEN_IDLE;

                Vector3 leftright = this.GetComponent<PlayerInput>().GetAxisHorizontal() * this.transform.right * DIVE_STRAFE_RATE;
                Vector3 updown = this.GetComponent<PlayerInput>().GetAxisVertical() * this.transform.up * DIVE_STRAFE_RATE;

			    targetVelocity = leftright + updown + Vector3.down * MAX_DOWNWARD_VELOCITY;
                break;

            case BirdState.DivingAndTurningLeft:
                //addMomentum();
				dive();
				tiltTowards(0);

                targetVelocity = -this.transform.right * 50f + this.transform.forward + Vector3.down * MAX_DOWNWARD_VELOCITY;
                break;

            case BirdState.DivingAndTurningRight:
                //addMomentum();
				dive();
				tiltTowards(0);

				targetVelocity = this.transform.right * 50f + this.transform.forward + Vector3.down * MAX_DOWNWARD_VELOCITY;
				break;
			
			case BirdState.Decelerating:
                tiltTowards(0);
                currentMaxSpeed -= Time.deltaTime * DECELERATION_RATE;
				targetVelocity = Vector3.zero;

                // copy pasta from turning code
                intendedTurnSpeed = this.GetComponent<PlayerInput>().GetAxisHorizontal() * TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed * 30f;
                if (intendedTurnSpeed != 0)
                {
                    currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, intendedTurnSpeed, Time.deltaTime);
                    currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, -TURN_RATE_MAX, TURN_RATE_MAX);
				    rotationY += currentTurnSpeed * Time.deltaTime * TURN_SHARPNESS;
                }
                break;

            case BirdState.TurningLeft:
                if (tilting != -1)
                {
					tiltTowards(-TILT_LIMIT);

                    intendedTurnSpeed = Mathf.Abs(this.GetComponent<PlayerInput>().GetAxisHorizontal()) * TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed * 30f;
					currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, intendedTurnSpeed, Time.deltaTime);
                    currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
					rotationY -= currentTurnSpeed * Time.deltaTime * TURN_SHARPNESS;

                    targetVelocity = this.transform.forward * currentMaxSpeed - this.transform.right * currentMaxSpeed;
                }
                else { targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * LIFT_OFFSET; }
                break;

            case BirdState.TurningRight:
                if (tilting != 1)
                {
					tiltTowards(TILT_LIMIT);

                    intendedTurnSpeed = Mathf.Abs(this.GetComponent<PlayerInput>().GetAxisHorizontal()) * TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed * 30f;
					currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, intendedTurnSpeed, Time.deltaTime);
					currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
					rotationY += currentTurnSpeed * Time.deltaTime * TURN_SHARPNESS;
					
					targetVelocity = this.transform.forward * currentMaxSpeed + this.transform.right * currentMaxSpeed;
				}
				else { targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * LIFT_OFFSET; }
                break;

            case BirdState.Ascending:
                tiltTowards(0);

                rotationX = transform.localEulerAngles.x - DESCENT_RATE * Time.deltaTime;
				clampAngle();

                targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * MAX_UPWARD_VELOCITY;
                break;

            case BirdState.AscendingAndTurningLeft:
                tiltTowards(-TILT_LIMIT);

				rotationX = transform.localEulerAngles.x - DESCENT_RATE * Time.deltaTime;
				intendedTurnSpeed = Mathf.Abs(Input.GetAxisRaw("JoystickAxisX")) * TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed * 30f;
				currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, intendedTurnSpeed, Time.deltaTime);
				currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
				rotationY -= currentTurnSpeed * Time.deltaTime * TURN_SHARPNESS;
				clampAngle();

                targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * MAX_UPWARD_VELOCITY - this.transform.right * currentMaxSpeed;
                break;

            case BirdState.AscendingAndTurningRight:
				tiltTowards(TILT_LIMIT);
				
				rotationX = transform.localEulerAngles.x - DESCENT_RATE * Time.deltaTime;
				intendedTurnSpeed = Mathf.Abs(Input.GetAxisRaw("JoystickAxisX")) * TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed * 30f;
				currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, intendedTurnSpeed, Time.deltaTime);
				currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
				rotationY += currentTurnSpeed * Time.deltaTime * TURN_SHARPNESS;
				clampAngle();
				
				targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * MAX_UPWARD_VELOCITY + this.transform.right * currentMaxSpeed;
			break;
			
			case BirdState.AboutFacing:
                //tiltTowards(0);
                momentum = 0f;
                //currentMaxSpeed = 0f;


				tiltTowards(0);
				currentMaxSpeed -= Time.deltaTime * DECELERATION_RATE;
				//targetVelocity = Vector3.zero;

				//this.GetComponent<Rigidbody>().velocity = currentMaxSpeed;
                targetVelocity = Vector3.zero + Vector3.up * LIFT_OFFSET;
                break;

            case BirdState.FlappingForward:
			    if (!flipped)
			    {
                    // flip once
                    flipped = true;
					rotationY += 180;
				    //this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y + 180f, this.transform.localEulerAngles.z);
			    }

			    //Debug.DrawRay (this.transform.position, hit.normal * 5f, Color.cyan, 2f);
			    currentMaxSpeed = 10f;
			    // instant or velocity over time?
			    targetVelocity = hit.normal * currentMaxSpeed;
			    //this.rigidbody.velocity = hit.normal * currentMaxSpeed;
                break;

            case BirdState.QuickAscending:
            	if (rotationX > 0 & rotationX < 300)
				{
					ease();
				}

				tiltTowards(0);
				this.GetComponent<Rigidbody>().velocity += Vector3.up * 0.2f;
                break;

            case BirdState.RollingLeft:
                tiltTowards(TILT_LIMIT / 2);
                this.GetComponent<Rigidbody>().velocity -= this.transform.right;
                break;

            case BirdState.RollingRight:
                tiltTowards(-TILT_LIMIT / 2);
                this.GetComponent<Rigidbody>().velocity += this.transform.right;
                break;

            case BirdState.Landing:
                ease();
                tiltTowards(0);
                momentum = 0f;
                currentMaxSpeed = 0f;

                this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                targetVelocity = Vector3.zero + Vector3.up * LIFT_OFFSET;

                Vector3 dest;
                dest.x = LandTarget.position.x;
                dest.y = this.transform.position.y;
                dest.z = LandTarget.position.z;

                if (Vector3.Distance(this.transform.position, dest) < 2f)
                {
                    dest.y = LandTarget.position.y;
                }

                // move this body to the center of the landing zone
                this.GetComponent<Rigidbody>().MovePosition(this.GetComponent<Rigidbody>().position + (dest - this.transform.position) * Time.deltaTime * 2f);

				float r = Mathf.Lerp(rotationY, LandTarget.eulerAngles.y, Time.deltaTime * 2f);
                rotationY = r;

                //if (Physics.Raycast(this.transform.position, Vector3.down, out hit, 0.3f))
                if (Vector3.Distance(this.transform.position, LandTarget.position) < 0.5f)
                {
                    //if (hit.collider.tag != "Player")
                        animator.SetBool("b_Grounded", true);
                }

                break;

            case BirdState.Grounded:
                tiltTowards(0);
                momentum = 0f;
                currentMaxSpeed = 0f;

                this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                targetVelocity = Vector3.zero + Vector3.up * LIFT_OFFSET;
                break;

            case BirdState.LiftingOff:
                tiltTowards(0);
                this.GetComponent<Rigidbody>().velocity += Vector3.up * Time.deltaTime;
                targetVelocity = Vector3.zero;
                break;

            case BirdState.DashingForward:
                tiltTowards(0);
                this.GetComponent<Rigidbody>().velocity += this.transform.forward * 0.5f; // instant impulse
                currentMaxSpeed += Time.deltaTime * 10f;
                break;

			//case BirdState.HoveringAndTurningLeft:
				//rotationY -= 50f * Time.deltaTime * TURN_SHARPNESS;								
				//tiltTowards(-TILT_LIMIT * 0.2f);
				//break;
				
			//case BirdState.HoveringAndTurningRight:
				//rotationY += 50f * Time.deltaTime * TURN_SHARPNESS;	
				//tiltTowards(TILT_LIMIT * 0.2f);
				//break;

			case BirdState.Dying:
                tiltTowards(0);
				currentMaxSpeed = 0f;
				this.GetComponent<Rigidbody>().velocity = Vector3.zero;
				targetVelocity = Vector3.zero;

                // turn off all renderers
				foreach (Renderer renderer in this.GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = false;
                }

                // create feather poof
                if (!flipped)
                {
                    flipped = true;
                    GameObject.Instantiate(Resources.Load(Registry.Prefab.FeatherPoof), this.transform.position, Quaternion.identity);

                    if (this.GetComponent<uLinkNetworkView>().isMine)
                    {
                        GameController.SetInputLock(true);
                        StartCoroutine(coRespawn());
                    }
                }
				break;

            case BirdState.Respawning:
                // turn on all renderers
                foreach (Renderer renderer in this.GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = true;
                }

                if (this.GetComponent<uLinkNetworkView>().isMine)
                {
                    this.GetComponent<Rigidbody>().MovePosition(GameController.LastCheckpoint.position);
                    this.GetComponent<Rigidbody>().MoveRotation(GameController.LastCheckpoint.rotation);
                    GameController.SetInputLock(false);
                }
                break;

        }

		transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);

        // update the actual body
        this.GetComponent<Rigidbody>().velocity = Vector3.Lerp(this.GetComponent<Rigidbody>().velocity, targetVelocity, Time.deltaTime);

        // update the animator
        animator.SetFloat("Rotation", this.transform.localEulerAngles.x);
        animator.SetFloat("Momentum", momentum);
        animator.SetFloat("Velocity", this.transform.GetComponent<Rigidbody>().velocity.magnitude);

        if (this.GetComponent<PlayerSync>() != null)
        {
            this.GetComponent<PlayerSync>().SendFloat("Rotation", this.transform.localEulerAngles.x);
            this.GetComponent<PlayerSync>().SendFloat("Momentum", momentum);
            this.GetComponent<PlayerSync>().SendFloat("Velocity", this.transform.GetComponent<Rigidbody>().velocity.magnitude);
        }
    }

    void dive()
    {
        rotationX = transform.localEulerAngles.x + DESCENT_RATE * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, 1f, 85f);
	}

    void ease()
    {
		rotationX = rotationX - EASE_RATE * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, 1f, 85f);
    }

	void clampAngle()
	{
		if (rotationX < 0)
			rotationX += 360f;
		
		if (rotationX > 85 & rotationX < 340)
			rotationX = 340f;
	}
	
	const float TILT_EASE_RATE = 2f;
    void tiltTowards(float f)
    {
        rotationZ = Mathf.Lerp(rotationZ, f, Time.deltaTime * TILT_EASE_RATE);
    }

    const float MOMENTUM_GAIN = 5f;
    void addMomentum()
    {
        momentum += MOMENTUM_GAIN * Time.deltaTime;
    }

    IEnumerator coRespawn()
    {
        yield return new WaitForSeconds(5f);
        animator.SetTrigger("t_Respawn");
        yield return null;
    }

    public BirdState GetState()
    {
        return state;
    }

    public void SetTargetVelocity(Vector3 v)
    {
        targetVelocity = v;
    }

    public void SetCurrentMaxSpeed(float f)
    {
        currentMaxSpeed = f;
    }
}
