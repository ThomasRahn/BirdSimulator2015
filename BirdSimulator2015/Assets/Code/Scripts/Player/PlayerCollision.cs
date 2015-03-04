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

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "LandingZone")
        {
            this.GetComponent<PlayerState>().LandPos = other.GetComponent<LandingZone>().Target.position;
            animator.SetTrigger("t_Land");
        }
    }
}
