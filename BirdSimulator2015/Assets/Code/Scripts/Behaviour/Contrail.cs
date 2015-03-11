using UnityEngine;
using System.Collections;

public class Contrail : MonoBehaviour
{
    public GameObject root;
    private TrailRenderer tr;
    private Rigidbody rb;
    private PlayerState ps;

    void Awake()
    {
        tr = this.GetComponent<TrailRenderer>();
        rb = root.rigidbody;
        ps = root.GetComponent<PlayerState>();
    }

	void Start()
    {
	}
	
	void Update()
    {
        if (rb.velocity.magnitude > 20f
            && (
                ps.GetState() == PlayerState.BirdState.Gliding
                || ps.GetState() == PlayerState.BirdState.TurningLeft
                || ps.GetState() == PlayerState.BirdState.TurningRight
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
