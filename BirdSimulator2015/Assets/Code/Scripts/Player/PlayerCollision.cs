﻿using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
	private Animator animator;

    private float fireballTimer;
    private const float _fireballTimer = 5f;

	void Awake()
	{
		animator = this.GetComponent<Animator>();
	}

	void Start()
    {
        fireballTimer = _fireballTimer;
	}
	
	void Update()
    {
	}

    void OnTriggerStay(Collider c)
    {
        if (c.name == Registry.Prefab.Fireball + "(Clone)"
            || c.name == Registry.Prefab.FireballMini + "(Clone)")
        {
            fireballTimer -= Time.deltaTime;

            if (fireballTimer < 0)
            {
                fireballTimer = _fireballTimer;
                animator.SetTrigger(Registry.Animator.Die);
            }
        }
    }

	void OnTriggerEnter(Collider c)
	{
        if (c.name == Registry.Prefab.SwingBlade
            || c.name == Registry.Prefab.SpikeLog
            || c.name == Registry.Prefab.SpikeRock
            || c.name == Registry.Prefab.WallGrate
            || c.name == Registry.Prefab.FallingRock1
            || c.name == Registry.Prefab.FallingRock2
            || c.name == Registry.Prefab.FallingRock3)
        {
            animator.SetTrigger(Registry.Animator.Die);
        }
	}

    void OnTriggerExit(Collider c)
    {
        if (c.name == Registry.Prefab.Fireball)
        {
            fireballTimer = _fireballTimer;
        }
    }
}
