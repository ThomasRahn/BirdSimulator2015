using UnityEngine;
using System.Collections;

public class PlayerParticles : MonoBehaviour {

	public GameObject worldParticles;

	void Start () {
		GameObject temp = Instantiate (worldParticles, this.transform.position, Quaternion.identity) as GameObject;
		temp.transform.parent = this.transform;
		temp.transform.LookAt (this.transform.forward);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
