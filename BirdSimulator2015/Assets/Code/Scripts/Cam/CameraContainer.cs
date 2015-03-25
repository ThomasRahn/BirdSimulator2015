﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BirdSimulator2015.Code.Scripts.Cam;

public class CameraContainer : MonoBehaviour 
{
	public enum Type
	{
		RADIAL,
		FREE,
	}

	private TPRadialCamera radial;
	private TPFreeCamera free;

	private bool locked;

	private void Awake() 
	{
		radial = GetComponentInChildren<TPRadialCamera>();
		free = GetComponentInChildren<TPFreeCamera>();
	}
	
	public void Switch(Type type) 
	{
		if(locked)
		{
			return;
		}

		switch(type)
		{
		case Type.RADIAL:
			radial.enabled = true;
			free.enabled = false;
			break;
		case Type.FREE:
			radial.enabled = false;
			free.enabled = true;
			break;
		}
	}

	public void Input(float horizontal, float vertical)
	{
		free.UpdateAngles(horizontal, vertical);
	}

	public void LockCameraSystem(bool locked)
	{
		this.locked = locked;
	}
}
