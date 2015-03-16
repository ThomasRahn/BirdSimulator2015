using UnityEngine;
using System.Collections;

public class Contrail : MonoBehaviour
{
    public GameObject root;
    private TrailRenderer tr;
    private Rigidbody rb;
    private PlayerState ps;
	private float SPEED_THRESHOLD = 15f;

    void Awake()
    {
        tr = this.GetComponent<TrailRenderer>();
        rb = root.GetComponent<Rigidbody>();
        ps = root.GetComponent<PlayerState>();
    }

	void Start()
    {
	}
	
	void Update()
    {
		if (rb.velocity.magnitude > SPEED_THRESHOLD
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
