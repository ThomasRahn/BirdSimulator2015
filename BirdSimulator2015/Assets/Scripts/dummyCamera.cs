using UnityEngine;
using System.Collections;

public class dummyCamera : MonoBehaviour {

	public GameObject player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//always stare at the bird
		this.transform.LookAt (player.transform.position);
	}
}
