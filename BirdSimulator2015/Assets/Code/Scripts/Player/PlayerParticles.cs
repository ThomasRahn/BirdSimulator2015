using UnityEngine;
using System.Collections;

public class PlayerParticles : MonoBehaviour {

	public GameObject worldParticles;
	public float particlesForwardOffset = 30f;
	// Use this for initialization
	void Start () {
		GameObject temp = Instantiate (worldParticles, this.transform.position+this.transform.forward*particlesForwardOffset, Quaternion.identity) as GameObject;
		temp.transform.parent = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
