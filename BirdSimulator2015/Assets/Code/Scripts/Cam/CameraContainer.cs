using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BirdSimulator2015.Code.Scripts.Cam;

public class CameraContainer : MonoBehaviour 
{
	private TPRadialCamera radial;
	private TPFreeCamera free;

	private bool locked;

	private void Awake() 
	{
		radial = GetComponentInChildren<TPRadialCamera>();
		free = GetComponentInChildren<TPFreeCamera>();
	}
	
	public void Radial(bool isRadial) 
	{
		if(locked)
		{
			return;
		}

		if(isRadial)
		{
			radial.enabled = true;
			free.enabled = false;
		}
		else
		{
			radial.enabled = false;
			free.enabled = true;
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
