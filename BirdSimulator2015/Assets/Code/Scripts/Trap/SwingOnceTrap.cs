using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SwingOnceTrap : MonoBehaviour 
{
    Vector3 OFFSET = new Vector3(-70f, -20f, 0f);

	protected void PlaceTrigger(Vector3 position)
	{
		GameObject trigger = ResourceFactory.GetInstance().GetPrefab(Registry.Prefab.TrapTrigger);

        trigger = GameObject.Instantiate(trigger, position + OFFSET, Quaternion.identity) as GameObject;
		trigger.transform.parent = this.transform.parent;
	}

	public abstract void Swing();
}
