using UnityEngine;
using System.Collections;

public class Flight : MonoBehaviour {
	public float speed;
	public float verticalSpeed;
	public float horizontalSpeed;
	public GameObject target;
	public bool invertY;
	float turnBonus;
	int invert;
	// Use this for initialization
	void Start () {
		if (invertY)
						invert = 1;
				else
						invert = -1;
	}
	
	// Update is called once per frame
	void Update () {
		//simple flight
		//Move forwards, turn, and able to ascend and descend

		//shoulder buttons make sharper turns
		if (Input.GetKey ((KeyCode.JoystickButton4)) || Input.GetKey (KeyCode.JoystickButton5))
						turnBonus = 2;
				else
						turnBonus = 1;


		//move forward
		this.transform.position = Vector3.MoveTowards (this.transform.position,
		                                               this.transform.position + this.transform.forward,
		                                               speed);
		//turning
		this.transform.RotateAround (this.transform.position,
		                            Vector3.up,
		                             Input.GetAxis ("Horizontal")*horizontalSpeed*turnBonus);
		//altitude
		this.transform.position += new Vector3(0,Input.GetAxis ("Vertical")*verticalSpeed*invert,0);

		//dive bomb
		if (Input.GetKey ((KeyCode.JoystickButton4)) && Input.GetKey (KeyCode.JoystickButton5) &&
						Input.GetAxis ("Vertical") > 0)
						this.transform.position -= new Vector3 (0, speed, 0);
	}
}



/*	
		reference controls
		if(Input.GetKey(KeyCode.JoystickButton0))
		   Debug.Log("joystic 0"); //square
		if(Input.GetKey(KeyCode.JoystickButton1))
			Debug.Log("joystic 1"); //X
		if(Input.GetKey(KeyCode.JoystickButton2))
			Debug.Log("joystic 2"); //Circle
		if(Input.GetKey(KeyCode.JoystickButton3))
			Debug.Log("joystic 3"); //Triangle
		if(Input.GetKey(KeyCode.JoystickButton4))
			Debug.Log("joystic 4"); //L1
		if(Input.GetKey(KeyCode.JoystickButton5))
			Debug.Log("joystic 5"); //R1
		if(Input.GetKey(KeyCode.JoystickButton6))
			Debug.Log("joystic 6"); //L2
		if(Input.GetKey(KeyCode.JoystickButton7))
			Debug.Log("joystic 7"); //R2
		if(Input.GetKey(KeyCode.JoystickButton8))
			Debug.Log("joystic 8"); //select


		//if(Input.GetAxis()
		//   Debug.Log(Input.GetAxis("Horizontal")); // -left, +right
		Debug.Log(Input.GetAxis("Vertical")); // + up -down
		if(Input.GetKey(KeyCode.JoystickButton10))
			Debug.Log("joystic 10"); 

		#endregion
*/