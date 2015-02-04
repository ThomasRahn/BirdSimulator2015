using UnityEngine;
using System.Collections;

namespace BirdSimulator2015.Code.Scripts.Cam
{
    public class TPRadialCamera : TPCamera 
    {	
		public float radius = 1;

		protected override void Awake()
		{
			base.Awake();
			Vector3 localPosition = transform.localPosition;
			localPosition = -Vector3.forward * radius + Vector3.up * UpOffset;
			transform.localPosition = localPosition;
		}

		protected override void UpdatePosition() 
	    {
        }
    }
}
