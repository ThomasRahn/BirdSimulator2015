using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour
{
	public enum BirdState
	{
		Floating,
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
		
	}
	
	private Animator animator;
	private Dictionary<int, BirdState> hash = new Dictionary<int, BirdState>();
	public BirdState currentState;
	
	void Awake()
	{
		animator = (Animator)GetComponent(typeof(Animator));
		if (!animator || !animator.runtimeAnimatorController)
		{
			Debug.LogError("fuck you");
		}
	}
	
	void Start()
	{
		foreach (BirdState state in (BirdState[])System.Enum.GetValues(typeof(BirdState)))
		{
			hash.Add(Animator.StringToHash("Base Layer." + state.ToString()), state);
		}
	}
	
	const float MAX_FORWARD_VELOCITY = 30f;
	const float MAX_DOWNWARD_VELOCITY = 30f;
	const float DOWNWARD_ACCELERATION = 2f;
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
	
	// we need this so we don't slowly descend (usually when gliding forward and turning at low speeds)
	// for some dumb reason, the y component of the velocity vector becomes -0.5f, so the player will slowly
	// descend and start hitting stuff. this is really annoying, so we compensate.
	const float LIFT_OFFSET = 0.5f;
	
	public float currentMaxSpeed = 0f;
	public float currentTurnSpeed = TURN_RATE_INITIAL;
	
	float rotation = 0f;
	Vector3 targetVelocity = Vector3.zero;
	Quaternion targetRotation = Quaternion.identity;
	public float rotationY = 0f;
	public float rotationZ = 0f;
	public float momentum = 0f;
	void FixedUpdate()
	{
		rotationY = transform.localEulerAngles.y;
		rotationZ = transform.localEulerAngles.z;
		
		Debug.DrawRay(this.transform.position, this.rigidbody.velocity * 5f, Color.magenta);
		Debug.DrawRay(this.transform.position, this.transform.up * 1f, Color.blue);
		Debug.DrawRay(this.transform.position, this.transform.forward * 1f, Color.green);
		Debug.DrawRay(this.transform.position, Vector3.down * 1f, Color.red);
		
		animator.SetFloat("VerticalAxis", Input.GetAxisRaw("Vertical"));
		animator.SetFloat("HorizontalAxis", Input.GetAxisRaw("Horizontal"));
		//animator.SetFloat("Rotation", this.rigidbody.rotation.x);
		animator.SetFloat("Rotation", this.transform.localEulerAngles.x);
		animator.SetBool("AscendPressed", Input.GetButton("Ascend"));
		animator.SetFloat("Momentum", momentum);
		
		currentState = hash[animator.GetCurrentAnimatorStateInfo(0).nameHash];
		
		currentMaxSpeed -= Time.deltaTime * 0.1f;
		currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, 0, MAX_FORWARD_VELOCITY);
		
		switch (currentState)
		{
		case BirdState.Floating:
			// reset some stuff
			momentum = 0f;
			currentTurnSpeed = TURN_RATE_INITIAL;
			
			// TEMP REPLACE WITH SOMETHING BETTER
			changePlayerRotation(0f);
			
			this.transform.localEulerAngles = new Vector3(rotation, rotationY, 0);
			targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * LIFT_OFFSET;
			break;
			
		case BirdState.Diving:
			addMomentum();
			dive();
			
			targetVelocity = this.transform.forward + Vector3.down * MAX_DOWNWARD_VELOCITY;
			break;
			
		case BirdState.DivingAndTurningLeft:
			addMomentum();
			
			// TEMP REPLACE WITH SOMETHING BETTER
			changePlayerRotation(0f);
			
			rotationY -= currentTurnSpeed * Time.deltaTime;
			currentTurnSpeed += TURN_ACCELERATION_WHILE_DIVING * Time.deltaTime;
			Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
			dive();
			
			targetVelocity = this.transform.forward + Vector3.down * MAX_DOWNWARD_VELOCITY;
			break;
			
		case BirdState.DivingAndTurningRight:
			addMomentum();
			
			// TEMP REPLACE WITH SOMETHING BETTER
			changePlayerRotation(0f);
			
			rotationY += currentTurnSpeed * Time.deltaTime;
			currentTurnSpeed += TURN_ACCELERATION_WHILE_DIVING * Time.deltaTime;
			Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
			dive();
			
			targetVelocity = this.transform.forward + Vector3.down * MAX_DOWNWARD_VELOCITY;
			break;
			
		case BirdState.Easing:
			// TODO diddle with momentum numbers
			currentMaxSpeed += momentum * 2f * Time.deltaTime;
			currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, 0, MAX_FORWARD_VELOCITY);
			ease();
			
			targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY + Vector3.up * MAX_FORWARD_VELOCITY;
			break;
			
		case BirdState.EasingAndTurningLeft:
			rotationY -= currentTurnSpeed * Time.deltaTime;
			currentTurnSpeed += TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed;
			currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
			
			currentMaxSpeed += momentum * Time.deltaTime;
			currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, 0, MAX_FORWARD_VELOCITY);
			ease();
			
			// TEMP REPLACE WITH SOMETHING BETTER
			changePlayerRotation(70f);
			
			targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY + Vector3.up * MAX_FORWARD_VELOCITY - this.transform.right * currentMaxSpeed;
			break;
			
		case BirdState.EasingAndTurningRight:
			rotationY += currentTurnSpeed * Time.deltaTime;
			currentTurnSpeed += TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed;
			currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
			
			currentMaxSpeed += momentum * Time.deltaTime;
			currentMaxSpeed = Mathf.Clamp(currentMaxSpeed, 0, MAX_FORWARD_VELOCITY);
			ease();
			
			// TEMP REPLACE WITH SOMETHING BETTER
			changePlayerRotation(-70f);
			
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
			
		case BirdState.Decelerating:
			// TODO change decel numbers
			currentMaxSpeed -= Time.deltaTime * 20f;
			targetVelocity = this.transform.forward *currentMaxSpeed;
			break;
			
		case BirdState.TurningLeft:
			// OLD too jittery and shit
			//this.transform.Rotate(this.transform.up, -TURN_RATE * Time.deltaTime, Space.Self);
			
			rotationY -= currentTurnSpeed * Time.deltaTime * TURN_SHARPNESS;
			currentTurnSpeed += Mathf.Pow(Mathf.Abs(Input.GetAxisRaw("Horizontal")), 2) * TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed;
			currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
			
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationY, 0);
			
			// TEMP REPLACE WITH SOMETHING BETTER
			changePlayerRotation(70f);
			
			// TODO fuck around with this modifier to make turning feel sharp at high speeds
			targetVelocity = this.transform.forward * currentMaxSpeed - this.transform.right * currentMaxSpeed + Vector3.up * LIFT_OFFSET;
			break;
			
		case BirdState.TurningRight:
			// OLD too jittery and shit
			//this.transform.Rotate(this.transform.up, TURN_RATE * Time.deltaTime, Space.Self);
			
			rotationY += currentTurnSpeed * Time.deltaTime * TURN_SHARPNESS;
			currentTurnSpeed += Mathf.Pow(Mathf.Abs(Input.GetAxisRaw("Horizontal")), 2) * TURN_ACCELERATION * Time.deltaTime * currentMaxSpeed;
			currentTurnSpeed = Mathf.Clamp(currentTurnSpeed, 0, TURN_RATE_MAX);
			
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationY, 0);
			
			// TEMP REPLACE WITH SOMETHING BETTER
			changePlayerRotation(-70f);
			
			// TODO fuck around with this modifier to make turning feel sharp at high speeds
			targetVelocity = this.transform.forward * currentMaxSpeed + this.transform.right * currentMaxSpeed + Vector3.up * LIFT_OFFSET;
			break;
			
		case BirdState.Descending:
			targetVelocity = this.transform.forward * MAX_FORWARD_VELOCITY_WHEN_ASCENDING + Vector3.down * MAX_UPWARD_VELOCITY;
			break;
			
		case BirdState.Ascending:
			// TEMP REPLACE WITH SOMETHING BETTER
			changePlayerRotation(0f);
			
			targetVelocity = this.transform.forward * currentMaxSpeed + Vector3.up * MAX_UPWARD_VELOCITY;
			break;
			
		case BirdState.AscendingAndTurningLeft:
			// TEMP REPLACE WITH SOMETHING BETTER
			changePlayerRotation(70f);
			
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
			// TEMP REPLACE WITH SOMETHING BETTER
			changePlayerRotation(-70f);
			
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
		}
		
		this.rigidbody.velocity = Vector3.Lerp(this.rigidbody.velocity, targetVelocity, Time.deltaTime);
	}
	
	void dive()
	{
		rotation = transform.localEulerAngles.x + DIVE_RATE * Time.deltaTime;	
		rotation = Mathf.Clamp(rotation, 1f, 85f);
		transform.localEulerAngles = new Vector3(rotation, rotationY, 0);
		
	}
	
	void ease()
	{
		rotation = transform.localEulerAngles.x - EASE_RATE * Time.deltaTime;	
		rotation = Mathf.Clamp(rotation, 1f, 85f);
		transform.localEulerAngles = new Vector3(rotation, rotationY, 0);
		
	}
	
	// TEMP REPLACE WITH SOMETHING BETTER
	void changePlayerRotation(float f)
	{
		Vector3 vec = new Vector3(0, 0, f);
		this.transform.GetChild(1).transform.localEulerAngles = vec;
	}
	
	const float MOMENTUM_GAIN = 5f;
	void addMomentum()
	{
		momentum += MOMENTUM_GAIN * Time.deltaTime;
	}
}
