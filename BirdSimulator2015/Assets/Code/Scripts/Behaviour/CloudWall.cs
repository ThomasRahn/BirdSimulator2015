using UnityEngine;
using System.Collections;

public class CloudWall : MonoBehaviour 
{
	public int level = 1;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter(Collision col)
	{
		//Kill player?
	}

	public void PushBack()
	{
		//if all 6 torches have been lit
		if(level >= 6)
		{
			Destroy(this.gameObject);
		}
		level++;
		this.transform.position = this.transform.position + new Vector3(0,0,-10.0f);
	}

}
