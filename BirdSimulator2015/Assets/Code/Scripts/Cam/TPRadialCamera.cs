using UnityEngine;
using System.Collections;

namespace BirdSimulator2015.Code.Scripts.Cam
{
    public class TPRadialCamera : TPCamera 
    {	
		protected override void Awake()
		{
			base.Awake();
			positionBehind();
		}

		protected override Vector3 UpdatePosition() 
	    {
			return positionBehind();
        }

		private void OnEnable()
		{
			SendMessageUpwards("ToggleRotation", true);
		}
    }
}
