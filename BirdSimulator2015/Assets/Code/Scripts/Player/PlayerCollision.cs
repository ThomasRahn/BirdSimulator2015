using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
	private Animator animator;

	void Awake()
	{
		animator = this.GetComponent<Animator>();
	}

	void Start()
    {
	}
	
	void Update()
    {
	}

	void OnTriggerEnter(Collider c)
	{
		if (c.name == Registry.Prefab.SwingBlade)
		{
			animator.SetTrigger("t_Die");
		}
	}
}
