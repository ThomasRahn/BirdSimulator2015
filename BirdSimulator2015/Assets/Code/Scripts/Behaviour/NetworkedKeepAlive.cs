using UnityEngine;
using System.Collections;

public class NetworkedKeepAlive : MonoBehaviour 
{
	public float Lifetime = 0;
	private float lifetime;
	
	void Start()
	{
		lifetime = Lifetime;
	}
	
	void Update()
	{
		lifetime -= Time.deltaTime;
		
		if (lifetime < 0)
		{
			uLink.Network.Destroy(this.gameObject);
		}
	}
}
