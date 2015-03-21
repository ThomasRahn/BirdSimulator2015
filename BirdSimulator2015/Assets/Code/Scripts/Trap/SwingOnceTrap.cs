using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SwingOnceTrap : MonoBehaviour 
{
	protected void PlaceTrigger(Vector3 position)
	{
		GameObject trigger = ResourceFactory.GetInstance().GetPrefab(Registry.Prefab.TrapTrigger);

		trigger = GameObject.Instantiate(trigger, position, Quaternion.identity) as GameObject;
		trigger.transform.parent = this.transform.parent;
	}

	public abstract void Swing();
}
