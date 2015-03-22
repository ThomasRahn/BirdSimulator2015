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
        timer -= Time.deltaTime;

        if (timer < 0)
        {
			timer = Random.Range(TimerMin, TimerMax);
			int r = Random.Range(0, Rocks.GetLength(0));
			GameObject g = GameObject.Instantiate(Rocks[r]) as GameObject;
            g.transform.rotation = Random.rotation;
            Vector3 v = this.transform.position;
            v.x += Random.Range(-Width, Width);
            v.z += Random.Range(-Width, Width);
            g.transform.position = v;
        }
	}
}
