using UnityEngine;
using System.Collections;

namespace Code.Scripts.Trap
{
	[RequireComponent(typeof(HingeJoint))]
	public class SwingBlade : MonoBehaviour 
	{
		public float maxAngle;
		public float period;
		public float phaseShift;

		private HingeJoint joint;

		private void Start() 
		{
			joint = gameObject.GetComponent<HingeJoint>();
			joint.connectedAnchor = transform.position;
		}

		private void FixedUpdate()
		{
			float rotation = maxAngle * Mathf.Cos((2*Mathf.PI/period) * Time.time + phaseShift);
			transform.rotation = Quaternion.AngleAxis(rotation, transform.forward);
		}
	}
}
