using UnityEngine;
using System.Collections;

public class KeepAlive : MonoBehaviour
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
            GameObject.Destroy(this.gameObject);
        }
	}
}
