using UnityEngine;
using System.Collections;

public class CloudWall : MonoBehaviour 
{
	public bool move;
	public int level = 1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (move) 
		{
			//if it has been moved twiced, remove it
			if(level >= 3)
			{
				Destroy(this.gameObject);
			}
			level++;
			this.transform.position = this.transform.position + new Vector3(0,0,-20.0f);
			move = false;
		}
	}

	void OnCollisionEnter(Collision col)
	{
		//Kill player?
	}

	public void PushBack()
	{
		move = true;
		Debug.Log (move);
	}

}
