﻿using UnityEngine;
using System.Collections;

public class Triggerable_TorchLit : BaseTriggerable<BaseTriggerable> {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void Trigger(Collider c, GameObject g)
	{
		Flame flame_component = this.GetComponentInChildren<Flame> ();
		if (!flame_component.IsLit) {
			flame_component.ToggleLit();
		}
		base.Trigger(c, g);
	}
}