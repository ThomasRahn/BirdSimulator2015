using UnityEngine;
using System.Collections;

namespace BirdSimulator2015.Code.Scripts.Cam
{
	public class TPFreeCamera : TPCamera 
	{
		public float angularVelocity = 20f;


		protected override void Awake() 
		{
			base.Awake ();
			positionBehind();
		}
		
		protected override void UpdatePosition()
		{
			updateAngles();
			positionBehind();
		}

		private void updateAngles()
		{
			float horizontal = Input.GetAxis("Mouse X");
			float vertical = -Input.GetAxis("Mouse Y");
			
			Vector3 angles = parent.rotation.eulerAngles;
			
			float deltaX = vertical * angularVelocity * Time.deltaTime;
			if(angles.x > 180)
			{
				angles.x = Mathf.Clamp(angles.x + deltaX, 290, 360);
			}
			else
			{
				angles.x = Mathf.Clamp(angles.x + deltaX, -10, 70);
			}
			angles.y += horizontal * angularVelocity * Time.deltaTime;
			
			parent.rotation = Quaternion.Euler(angles);
		}

		private void OnEnable()
		{
			SendMessageUpwards("ToggleRotation", false);
		}
	}
}
