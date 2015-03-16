using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour {

	public GameObject player;

	public float MaxAcceleration = 10.0f;
	public Vector3 Velocity = Vector3.zero;
	public float MaxVelocity = 5.0f;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (player != null) {
			Vector3 accel = (MaxAcceleration) * (player.transform.position - this.transform.position).normalized;

			Velocity = Velocity + accel * Time.deltaTime;
			
			Velocity = Vector3.ClampMagnitude (Velocity, MaxVelocity);
			
			this.transform.position = this.transform.position + Velocity * Time.deltaTime;

			if (Velocity.sqrMagnitude > 0f)
				transform.rotation = Quaternion.LookRotation (Velocity.normalized, Vector3.up);
		
		}
		 else {
			player = GameObject.FindGameObjectWithTag ("Player");
		}
	}
}
