using UnityEngine;
using System.Collections;

namespace Code.Scripts.Trap
{
	[RequireComponent(typeof(HingeJoint))]
	public class SwingBlade : MonoBehaviour 
	{
		public float rotationAngle;

		private HingeJoint joint;

		private void Start() 
		{
			joint = gameObject.GetComponent<HingeJoint>();
			joint.connectedAnchor = transform.position;
			//resetTrap();
		}

		private void resetTrap()
		{
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			transform.rotation = Quaternion.Euler(Vector3.forward * rotationAngle);
		}
	}
}
