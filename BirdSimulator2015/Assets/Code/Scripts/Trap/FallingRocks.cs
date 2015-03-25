using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallingRocks : MonoBehaviour
{
	public float TimerMin = 1f;
	public float TimerMax = 2f;
    public float Width = 0f;
    public GameObject[] Rocks;

	private float timer;

	void Start()
    {
		timer = TimerMin;
	}
	
	void Update()
    {
		if(!uLink.Network.isServer) // Only server spawns rocks
		{
			return;
		}

        timer -= Time.deltaTime;

        if (timer < 0)
        {
			Vector3 randomPosition = this.transform.position;
			randomPosition.x += Random.Range(-Width, Width);
			randomPosition.z += Random.Range(-Width, Width);

			timer = Random.Range(TimerMin, TimerMax);
			int r = Random.Range(0, Rocks.GetLength(0));

			uLink.Network.Instantiate(Rocks[r], randomPosition, Random.rotation, 0);
        }
	}
}
