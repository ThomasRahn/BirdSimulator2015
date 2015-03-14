using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallingRocks : MonoBehaviour
{
    public float Width = 0f;

    private List<Object> rocks = new List<Object>();
    private float timer = 1f;

	void Start()
    {
        Object r1 = Resources.Load("Prefabs/Crypt/Environment/pf_Env_FallingRock1");
        Object r2 = Resources.Load("Prefabs/Crypt/Environment/pf_Env_FallingRock2");
        Object r3 = Resources.Load("Prefabs/Crypt/Environment/pf_Env_FallingRock3");
        rocks.Add(r1);
        rocks.Add(r2);
        rocks.Add(r3);
	}
	
	void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            timer = Random.Range(1f, 5f);
            int r = Random.Range(0, rocks.Count);
            GameObject g = GameObject.Instantiate(rocks[0]) as GameObject;
            g.transform.rotation = Random.rotation;
            Vector3 v = this.transform.position;
            v.x += Random.Range(-Width, Width);
            g.transform.position = v;
        }
	}
}
