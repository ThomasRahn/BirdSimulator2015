using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerState : MonoBehaviour
{
    public enum BirdState
    {
        Hovering,

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

        Ascending,
        AscendingAndTurningLeft,
        AscendingAndTurningRight,

        AboutFacing,
        FlappingForward,
        Landing,
        Grounded,
		LiftingOff,

		QuickAscending,
		DashingForward,

		Dying,
        Respawning,

        SpeedyMode,

        Tornadoing,
        Flashing,
	}
	
	private Animator animator;
	private PlayerInput input;
    private Dictionary<int, BirdState> hash = new Dictionary<int, BirdState>();
    private BirdState state;

	const float MIN_FORWARD_VELOCITY = 15f;
    const float MAX_FORWARD_VELOCITY = 35f;
    const float MAX_DOWNWARD_VELOCITY = 60f;
    const float DOWNWARD_ACCELERATION = 5f;
    const float MAX_UPWARD_VELOCITY = 5f;
    const float MAX_FORWARD_VELOCITY_WHEN_ASCENDING = 25f;
    const float DESCENT_RATE = 50f;
    const float DIVE_RATE = 200f;
    const float TURN_RATE_INITIAL = 20f;
    const float TURN_RATE_MAX = 150f;
    const float TURN_ACCELERATION = 10f;
    const float TURN_ACCELERATION_WHILE_DIVING = 5f;
    const float TURN_RATE_WHEN_DIVING = 200f;
	const float TURN_RATE_WHEN_IDLE = 80f;
	const float EASE_RATE = 100f;
    const float MOMENTUM_LOSS_RATE = 50f;
    const float MOMENTUM_GAIN_MULTI = 2f;
    const float TURN_SHARPNESS = 1.4f;
    const float DECELERATION_RATE = 500f;
    const float DIVE_STRAFE_RATE = 20f;
	const float TAUNT_DISTANCE = 20.0f;
	const float WHIRLWIND_DISTANCE = 20.0f;
    const float TILT_LIMIT = 70f;
    const float ABOUT_FACE_ANGLE = 35f;

    const float ROTATION_X_LIMIT = 40f;


    private float currentMaxSpeed = 0f;
    private float currentTurnSpeed = TURN_RATE_INITIAL;
    private Vector3 targetVelocity = Vector3.zero;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private float rotationZ = 0f; // of child transform
    private float momentum = 0f;

    // collision
    private RaycastHit hit;
    private int tilting = 0;
    
    // do once triggers
    private bool flipOnce = false;
    private bool respawnOnce = false;
    private bool skillOnce = false;
    private bool yodoYouOnlyDieOnce = false;
    private bool audioOnce = false;

    public Transform LandTarget; // collision trigger (landing)
	public LayerMask layerMask;
	public Egg HeldEgg {get; set;}

    void Awake()
    {
        animator = this.GetComponent<Animator>();
		input = this.GetComponent<PlayerInput>();
    }

    void Start()
    {
        foreach (BirdState state in (BirdState[])System.Enum.GetValues(typeof(BirdState)))
        {
            hash.Add(Animator.StringToHash(state.ToString()), state);
        }
    }
	
    void FixedUpdate()
    {
        state = hash[animator.GetCurrentAnimatorStateInfo(0).shortNameHash];

        //Debug.DrawRay(this.transform.position, this.GetComponent<Rigidbody>().velocity * 5f, Color.magenta);
        //Debug.DrawRay(this.transform.position, this.transform.up * 1f, Color.blue);
        //Debug.DrawRay(this.transform.position, this.transform.forward * 1f, Color.green);
        //Debug.DrawRay(this.transform.position, Vector3.down * 1f, Color.red);

        Vector3 from = this.transform.position;
        Vector3 direction = this.transform.forward;

        //Debug.Log(this.GetComponent<Rigidbody>().velocity.magnitude);
        if (checkDiveCollision())
        {
            this.GetComponent<PlayerSync>().SendBool(Registry.Animator.CanDive, false);
            this.GetComponent<PlayerSync>().SendBool(Registry.Animator.Diving, false);
            animator.SetBool(Registry.Animator.CanDive, false);
            animator.SetBool(Registry.Animator.Diving, false);
        }
        else
        {
            this.GetComponent<PlayerSync>().SendBool(Registry.Animator.CanDive, true);
            animator.SetBool(Registry.Animator.CanDive, true);

            float check = DIVE_SWOOP_DISTANCE;
            if (this.GetComponent<Rigidbody>().velocity.magnitude < check)
                check = this.GetComponent<Rigidbody>().velocity.magnitude;

            if (Physics.Raycast(from, this.transform.forward, out hit, check, layerMask))
            {
                if (state != BirdState.AboutFacing && state != BirdState.FlappingForward)
                {
                    if (Vector3.Angle(hit.normal, -direction) < ABOUT_FACE_ANGLE)
                    {
                        this.GetComponent<PlayerSync>().SendTrigger(Registry.Animator.AboutFace);
                        animator.SetTrigger(Registry.Animator.AboutFace);
                    }
                }
            }
        }

        //Debug.DrawRay(from, (this.transform.forward + this.transform.right) * 0.8f, Color.black);
        //Debug.DrawRay(from, (this.transform.forward - this.transform.right) * 0.8f, Color.black);
        if (Physics.Raycast(from, this.transform.forward + this.transform.right, out hit, 1f, layerMask))
        {
            tilting = 1; // right
            tiltTowards(-TILT_LIMIT); // tilt away (opposite)

            if (Physics.Raycast(from, this.transform.forward + this.transform.right, out hit, 0.7f, layerMask))
            {
                rotationY = Mathf.Lerp(rotationY, rotationY - 20f, Time.deltaTime);
                this.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationY, 0);
            }

        }
        else if (Physics.Raycast(from, this.transform.forward - this.transform.right, out hit, 1f, layerMask))
        {
            tilting = -1; // left
            tiltTowards(TILT_LIMIT);

            if (Physics.Raycast(from, this.transform.forward - this.transform.right, out hit, 0.7f, layerMask))
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
    float speedChange;
    void Update()
    {
        speedChange = 1.0f;

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

        // reset blur amount if not diving
        if (state != BirdState.Diving & state != BirdState.SpeedyMode & state != BirdState.DashingForward)
        {
            float b = Camera.main.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount;
            if (b > 0)
            {
                b -= Time.deltaTime;
                Camera.main.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount = b;
            }
        }

        switch (state)
        {
            case BirdState.Hovering:
				resetBools();

                ease();
                tiltTowards(0);
				rotationY += input.GetAxisHorizontal() * Time.deltaTime * TURN_RATE_WHEN_IDLE;
                targetVelocity = Vector3.zero;
				break;

            case BirdState.Gliding:
                // reset some stuff
                flipOnce = false;
                momentum = 0f;
                currentTurnSpeed = TURN_RATE_INITIAL;
                this.GetComponent<PlayerSync>().SendBool(Registry.Animator.Grounded, false);
                animator.SetBool(Registry.Animator.Grounded, false); // just in case

				resetBools();

				// make sure we're not slamming against a wall, since the gliding state applies there also
                if (tilting == 0)
                    tiltTowards(0);

                targetVelocity = this.transform.forward * currentMaxSpeed;
                break;

            case BirdState.Descending:
				tiltTowards(0);
                currentTurnSpeed = TURN_RATE_INITIAL;

				rotationX = transform.localEulerAngles.x + DESCENT_RATE * Time.deltaTime;
				if (rotationX > ROTATION_X_LIMIT & rotationX < 90)
                    rotationX = ROTATION_X_LIMIT;

				targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY_WHEN_ASCENDING + Vector3.down * MAX_UPWARD_VELOCITY;
                break;

			case BirdState.DescendingAndTurningLeft:
                tiltTowards(-TILT_LIMIT);

				rotationX = transform.localEulerAngles.x + DESCENT_RATE * Time.deltaTime;
                if (rotationX > ROTATION_X_LIMIT & rotationX < 90)
                    rotationX = ROTATION_X_LIMIT;

                turnLeft();
                rotationY -= currentTurnSpeed * Time.deltaTime;

				targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY_WHEN_ASCENDING + Vector3.down * MAX_UPWARD_VELOCITY;
				break;

			case BirdState.DescendingAndTurningRight:
                tiltTowards(TILT_LIMIT);

				rotationX = transform.localEulerAngles.x + DESCENT_RATE * Time.deltaTime;
                if (rotationX > ROTATION_X_LIMIT & rotationX < 90)
                    rotationX = ROTATION_X_LIMIT;

                turnRight();
                //rotationY += currentTurnSpeed * Time.deltaTime;

				targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY_WHEN_ASCENDING + Vector3.down * MAX_UPWARD_VELOCITY;
				break;
					
			case BirdState.Diving:
                float b = Camera.main.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount;
                if (b < 0.5f)
                {
                    b += Time.deltaTime;
                    Camera.main.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount = b;
                }

                tiltTowards(input.GetAxisHorizontal() * TILT_LIMIT);
                addMomentum();
                dive();

				Vector3 leftright = input.GetAxisHorizontal() * this.transform.right * DIVE_STRAFE_RATE;
				Vector3 updown = input.GetAxisVertical() * this.transform.up * DIVE_STRAFE_RATE;
			    targetVelocity = leftright + updown + Vector3.down * MAX_DOWNWARD_VELOCITY;

                //if (!audioOnce)
                //{
                //    audioOnce = true;
                //    this.GetComponent<PlayerAudio>().PlayDive();
                //}

                break;

            case BirdState.Easing:
                tiltTowards(0);
                ease();

                currentMaxSpeed += momentum * MOMENTUM_GAIN_MULTI * Time.deltaTime;
                targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * Mathf.Abs(this.GetComponent<Rigidbody>().velocity.y);

                //this.GetComponent<AudioSource>().Stop();
                break;

            case BirdState.EasingAndTurningLeft:
                tiltTowards(-TILT_LIMIT);
                ease();

                turnLeft();

                //currentMaxSpeed += momentum * MOMENTUM_GAIN_MULTI * Time.deltaTime;
                //currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, 0, MAX_FORWARD_VELOCITY);
                targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * Mathf.Abs(this.GetComponent<Rigidbody>().velocity.y) - this.transform.right * currentMaxSpeed;
                break;

            case BirdState.EasingAndTurningRight:
                tiltTowards(TILT_LIMIT);
                ease();

                turnRight();

                //currentMaxSpeed += momentum * MOMENTUM_GAIN_MULTI * Time.deltaTime;
                //currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, 0, MAX_FORWARD_VELOCITY);
                targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * Mathf.Abs(this.GetComponent<Rigidbody>().velocity.y) + this.transform.right * currentMaxSpeed;
                break;
			
			case BirdState.Decelerating:
                tiltTowards(0);
                currentMaxSpeed -= Time.deltaTime * DECELERATION_RATE;
				targetVelocity = Vector3.zero;
				speedChange = 3.0f;

                float f = this.GetComponent<PlayerInput>().GetAxisHorizontal();
                if (f < 0)
                {
                    turnLeft();
                }
                else if (f > 0)
                {
                    turnRight();
                }
                break;

            case BirdState.TurningLeft:
                if (tilting != -1)
                {
					tiltTowards(-TILT_LIMIT);
                    turnLeft();
                    targetVelocity = this.transform.forward * currentMaxSpeed - this.transform.right * currentMaxSpeed;
                }
                else { targetVelocity = this.transform.forward * currentMaxSpeed; }
                break;

            case BirdState.TurningRight:
                if (tilting != 1)
                {
                    tiltTowards(TILT_LIMIT);
                    turnRight();
                    targetVelocity = this.transform.forward * currentMaxSpeed + this.transform.right * currentMaxSpeed;
                }
                else { targetVelocity = this.transform.forward * currentMaxSpeed; }
                break;

            case BirdState.Ascending:
                tiltTowards(0);
                currentTurnSpeed = TURN_RATE_INITIAL;

                rotationX = transform.localEulerAngles.x - DESCENT_RATE * Time.deltaTime;
                rotationX = clampAngle(rotationX);

                targetVelocity = this.transform.forward * currentMaxSpeed;
                break;

            case BirdState.AscendingAndTurningLeft:
                tiltTowards(-TILT_LIMIT);
                turnLeft();

				rotationX = transform.localEulerAngles.x - DESCENT_RATE * Time.deltaTime;
                rotationX = clampAngle(rotationX);

                targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * MAX_UPWARD_VELOCITY - this.transform.right * currentMaxSpeed;
                break;

            case BirdState.AscendingAndTurningRight:
				tiltTowards(TILT_LIMIT);
                turnRight();

                rotationX = transform.localEulerAngles.x - DESCENT_RATE * Time.deltaTime;
                rotationX = clampAngle(rotationX);
				
				targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * MAX_UPWARD_VELOCITY + this.transform.right * currentMaxSpeed;
			break;
			
			case BirdState.AboutFacing:
                animator.ResetTrigger(Registry.Animator.AboutFace);

                tiltTowards(0);

                momentum = 0f;
				currentMaxSpeed -= Time.deltaTime * DECELERATION_RATE;
                targetVelocity = Vector3.zero;
                break;

            case BirdState.FlappingForward:
			    if (!flipOnce)
			    {
                    flipOnce = true;
					rotationY += 180;
                    this.GetComponent<PlayerAudio>().PlaySwoop();
			    }

			    currentMaxSpeed = MIN_FORWARD_VELOCITY;

			    // instant or velocity over time?
			    targetVelocity = this.transform.forward * currentMaxSpeed;
                break;

            case BirdState.QuickAscending:
                tiltTowards(0);
                if (rotationX < 300)
				    ease();

				this.GetComponent<Rigidbody>().velocity += Vector3.up * 1f;

                if (!audioOnce)
                {
                    audioOnce = true;
                    this.GetComponent<PlayerAudio>().PlaySwoop();
                }

                break;

            case BirdState.Landing:
                tiltTowards(0);
                ease();

                zeroAllForces();

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

				float r = Mathf.Lerp(rotationY, LandTarget.eulerAngles.y, Time.deltaTime * 5f);
                rotationY = r;

                //if (Physics.Raycast(this.transform.position, Vector3.down, out hit, 0.3f))
                //Debug.Log(Vector3.Distance(this.transform.position, LandTarget.position));
                if (Vector3.Distance(this.transform.position, LandTarget.position) < 1.5f)
                {
                    //if (hit.collider.tag != "Player")
                    this.GetComponent<PlayerSync>().SendBool(Registry.Animator.Grounded, true);
                    animator.SetBool(Registry.Animator.Grounded, true);
                }
                break;

            case BirdState.Grounded:
                tiltTowards(0);
                ease();

                zeroAllForces();

                break;

            case BirdState.LiftingOff:
                tiltTowards(0);

                this.GetComponent<Rigidbody>().velocity += Vector3.up * Time.deltaTime;
                targetVelocity = Vector3.zero;
				LandTarget = null;
                break;

            case BirdState.DashingForward:
                b = Camera.main.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount;
                if (b < 0.5f)
                {
                    b += Time.deltaTime * 5f;
                    Camera.main.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount = b;
                }

                tiltTowards(0);

                if (!audioOnce)
                {
                    audioOnce = true;
                    this.GetComponent<PlayerAudio>().PlaySwoop();
                }

                this.GetComponent<Rigidbody>().velocity += this.transform.forward * 0.5f; // instant impulse
                currentMaxSpeed += Time.deltaTime * MIN_FORWARD_VELOCITY;
                break;

			case BirdState.Dying:
                tiltTowards(0);

                zeroAllForces();

                if (!yodoYouOnlyDieOnce)
                {
                    yodoYouOnlyDieOnce = true;

                    // turn off all renderers
                    foreach (Renderer renderer in this.GetComponentsInChildren<Renderer>())
                    {
                        this.GetComponent<PlayerSync>().ToggleRenderer(false);
                        renderer.enabled = false;
                    }
                    // let go of held items
                    if (HeldEgg != null)
                    {
                        HeldEgg.Reset();
                    }

                    if (GameController.IsWhite)
                    {
                        this.GetComponent<PlayerSync>().SpawnPrefab(Registry.Prefab.FeatherPoof_White, this.transform.position, Quaternion.identity);
                        GameObject.Instantiate(Resources.Load(Registry.Prefab.FeatherPoof_White), this.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        this.GetComponent<PlayerSync>().SpawnPrefab(Registry.Prefab.FeatherPoof_Black, this.transform.position, Quaternion.identity);
                        GameObject.Instantiate(Resources.Load(Registry.Prefab.FeatherPoof_Black), this.transform.position, Quaternion.identity);
                    }

                    // sound
                    this.GetComponent<PlayerAudio>().PlayDeath();
                    // rumble
                    this.GetComponent<PlayerRumble>().BumbleRumble(0.3f, 0.7f, 0.7f);

                    if (this.GetComponent<uLinkNetworkView>().isMine)
                    {
                        GameController.SetInputLock(true);
                        StartCoroutine(coRespawn());
                    }

                    GameController.DeathPopup.Activate();
                }

				break;

            case BirdState.Respawning:
				if (!respawnOnce)
				{
					// turn on all renderers
	                foreach (Renderer renderer in this.GetComponentsInChildren<Renderer>())
	                {
                        this.GetComponent<PlayerSync>().ToggleRenderer(true);
                        renderer.enabled = true;
	                }
	
	                if (this.GetComponent<uLinkNetworkView>().isMine)
	                {
						this.transform.position = GameController.GetSpawnLocation();
	                    this.transform.rotation = GameController.LastCheckpoint.rotation;
	                    GameController.SetInputLock(false);
	                }
				}
                break;

            case BirdState.SpeedyMode:
				resetBools();
                b = Camera.main.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount;
                if (b < 0.6f)
                {
                    b += Time.deltaTime;
                    Camera.main.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount = b;
                }

                rotationX = 0f;
                rotationY = 90f;
                //rotationZ = 0f;
                tiltTowards(input.GetAxisHorizontal() * TILT_LIMIT);

                leftright = input.GetAxisHorizontal() * this.transform.right * 100f;
				updown = input.GetAxisVertical() * this.transform.up * 100f;

                if (Physics.Raycast(this.transform.position, leftright, out hit, 10f, layerMask))
                {
                    leftright = -leftright;
                }
                if (Physics.Raycast(this.transform.position, updown, out hit, 10f, layerMask))
                {
                    updown = -updown;
                }

                speedChange = 3.0f;
			    targetVelocity = leftright + updown + SpeedyModeForward * 70f;
                break;

            case BirdState.Tornadoing:
                // insert tim's sweet particles here
                if (!skillOnce)
                {
                    skillOnce = true;
                    this.GetComponent<PlayerSync>().SpawnPrefab(Registry.Prefab.WhirlyWind, this.transform.position, Quaternion.identity);
                    GameObject.Instantiate(Resources.Load(Registry.Prefab.WhirlyWind), this.transform.position, Quaternion.identity);

                    this.GetComponent<PlayerAudio>().PlayTornado();
					
					Collider[] colliders = Physics.OverlapSphere (this.transform.position, WHIRLWIND_DISTANCE);
					for(int i = 0; i < colliders.Length; i++)
					{
						FireballZone[] fbZone = colliders[i].gameObject.GetComponentsInChildren<FireballZone>();
						if(fbZone.Length > 0)
						{
							Vector3 force = colliders[i].gameObject.transform.position - this.transform.position;
							fbZone[0].PushBack(force * 10.0f);	
						}
					}
                }

                targetVelocity = this.transform.forward * currentMaxSpeed;
                break;

            case BirdState.Flashing:
                // insert tim's sweet particles here
                if (!skillOnce)
                {
                    skillOnce = true;
                    this.GetComponent<PlayerSync>().SpawnPrefab(Registry.Prefab.FlashyFlash, this.transform.position, Quaternion.identity);
                    GameObject.Instantiate(Resources.Load(Registry.Prefab.FlashyFlash), this.transform.position, Quaternion.identity);

                    this.GetComponent<PlayerAudio>().PlayFlash();

                    // light torches
					Collider[] colliders = Physics.OverlapSphere (this.transform.position, TAUNT_DISTANCE);
					if (colliders.Length > 0) 
					{
						for(int i = 0; i < colliders.Length; i++)
						{
							Triggerable_TorchLit[] lightable_torches = colliders[i].gameObject.GetComponentsInChildren<Triggerable_TorchLit>();
							
							foreach(Triggerable_TorchLit torch in lightable_torches)
							{
								torch.Trigger(colliders[i],this.gameObject);
							}
						}
					}
                }

                targetVelocity = this.transform.forward * currentMaxSpeed;
                break;
        }

		transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);

        // update the actual body
        this.GetComponent<Rigidbody>().velocity = Vector3.Lerp(this.GetComponent<Rigidbody>().velocity, targetVelocity, speedChange * Time.deltaTime);

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
        //rotationX = transform.localEulerAngles.x + DIVE_RATE * Time.deltaTime;
        rotationX = rotationX + DIVE_RATE * Time.deltaTime;

        if (rotationX > 300)
        {

        }
        else
        {
            rotationX = Mathf.Clamp(rotationX, 0f, 85f);
        }
	}

    void ease()
    {
        
        if (rotationX > 300)
        {
            rotationX = rotationX + EASE_RATE * Time.deltaTime;
        }
        else
        {
            rotationX = rotationX - EASE_RATE * Time.deltaTime;
            rotationX = Mathf.Clamp(rotationX, 0f, 85f);
        }
    }

	float clampAngle(float f)
	{
		if (f < 0)
			f += 360f;
		
		if (f > 85 & f < 340)
			f = 340f;

        return f;
	}
	
	const float TILT_EASE_RATE = 2f;
    void tiltTowards(float f)
    {
        rotationZ = Mathf.Lerp(rotationZ, f, Time.deltaTime * TILT_EASE_RATE);
    }

    void turnLeft()
    {
        intendedTurnSpeed = Mathf.Abs(input.GetAxisHorizontal()) * TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed * 30f;
        currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, intendedTurnSpeed, Time.deltaTime);
        currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
        //rotationY -= currentTurnSpeed * Time.deltaTime * TURN_SHARPNESS;
        rotationY = Mathf.Lerp(rotationY, rotationY - currentTurnSpeed * TURN_SHARPNESS, Time.deltaTime);
    }

    void turnRight()
    {
        intendedTurnSpeed = Mathf.Abs(input.GetAxisHorizontal()) * TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed * 30f;
        currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, intendedTurnSpeed, Time.deltaTime);
        currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
        //rotationY += currentTurnSpeed * Time.deltaTime * TURN_SHARPNESS;
        rotationY = Mathf.Lerp(rotationY, rotationY + currentTurnSpeed * TURN_SHARPNESS, Time.deltaTime);
    }

    const float MOMENTUM_GAIN = 5f;
    void addMomentum()
    {
        momentum += MOMENTUM_GAIN * Time.deltaTime;
    }

    void zeroAllForces()
    {
        momentum = 0f;
        currentMaxSpeed = 0f;
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        targetVelocity = Vector3.zero;
    }

    const float DIVE_SWOOP_DISTANCE = 7f;
    bool checkDiveCollision()
    {
        float f = Mathf.Abs(this.GetComponent<Rigidbody>().velocity.y);
        if (f < DIVE_SWOOP_DISTANCE)
            f = DIVE_SWOOP_DISTANCE;

        //Debug.DrawRay(this.transform.position, Vector3.down * f, Color.black);
        if (Physics.Raycast(this.transform.position, Vector3.down, out hit, f, layerMask))
        {
            return true;
        }
        return false;
    }

    IEnumerator coRespawn()
    {
        yield return new WaitForSeconds(3.5f);
        animator.SetTrigger(Registry.Animator.Respawn);
        yield return null;
    }

	private void resetBools()
	{
		yodoYouOnlyDieOnce = false;
		respawnOnce = false;
		skillOnce = false;
		audioOnce = false;
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

    public Vector3 SpeedyModeForward;
    public void SetSpeedyMode(bool b, Vector3 v)
    {
        this.GetComponent<PlayerSync>().SendBool(Registry.Animator.Speedy, b);
        animator.SetBool(Registry.Animator.Speedy, b);
        SpeedyModeForward = v;
    }
}
