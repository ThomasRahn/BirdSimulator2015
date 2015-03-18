using UnityEngine;
using System.Collections;

public class TrapTrigger : MonoBehaviour 
{
	private SwingOnceTrap trap;

	private void Start()
	{
		trap = transform.parent.GetComponentInChildren<SwingOnceTrap>();
	}

	private void OnTriggerEnter(Collider c)
	{
		if(c.tag == Registry.Tag.Player)
		{
			trap.Swing();
		}
	}
}
