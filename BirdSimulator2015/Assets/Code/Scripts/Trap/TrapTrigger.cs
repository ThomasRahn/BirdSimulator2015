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
		if(c.tag == Registry.Tag.Player || c.tag == Registry.Tag.Proxy)
		{
			trap.Swing();
            trap.Invoke("ResetTrap", 7f);
		}
	}
}
