using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Contrail : MonoBehaviour
{
    public GameObject root;
    private TrailRenderer tr;
    private Rigidbody rb;
    //private PlayerState ps;
    private Animator animator;
    private Dictionary<int, PlayerState.BirdState> hash = new Dictionary<int, PlayerState.BirdState>();
    private PlayerState.BirdState state;
	private float SPEED_THRESHOLD = 15f;

    void Awake()
    {
        tr = this.GetComponent<TrailRenderer>();
        rb = root.GetComponent<Rigidbody>();
        animator = root.GetComponent<Animator>();
        //ps = root.GetComponent<PlayerState>();
    }

	void Start()
    {
        foreach (PlayerState.BirdState state in (PlayerState.BirdState[])System.Enum.GetValues(typeof(PlayerState.BirdState)))
        {
            hash.Add(Animator.StringToHash(state.ToString()), state);
        }
	}
	
	void Update()
    {
        state = hash[animator.GetCurrentAnimatorStateInfo(0).shortNameHash];

		if (rb.velocity.magnitude > SPEED_THRESHOLD
            && (
                state == PlayerState.BirdState.TurningLeft
                || state == PlayerState.BirdState.TurningRight
            ))
        {
            tr.enabled = true;
        }
        else
        {
            tr.enabled = false;
        }
	}
}
