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

        tr.enabled = false;

        if (state == PlayerState.BirdState.Tornadoing)
        {
            tr.startWidth = 0.4f;
            tr.endWidth = 0.4f;
            tr.enabled = true;
        }
        else
        {
            tr.startWidth = 0.01f;
            tr.endWidth = 0.01f;
        }

        if (rb.velocity.magnitude > SPEED_THRESHOLD
            && (
                state == PlayerState.BirdState.TurningLeft
                || state == PlayerState.BirdState.TurningRight
                || state == PlayerState.BirdState.SpeedyMode
            ))
        {
            if (state == PlayerState.BirdState.SpeedyMode && this.name == "ContrailRight")
            {
                Vector3 pos;
                pos.x = -0.002f;
                pos.y = 0.108f;
                pos.z = 0.202f;
                this.transform.localPosition = pos;
            }
            else
            {
                Vector3 pos = this.transform.position;
                pos.x = 0.494f;
                pos.y = -0.077f;
                pos.z = 0.002f;
            }

            tr.enabled = true;
        }
	}
}
