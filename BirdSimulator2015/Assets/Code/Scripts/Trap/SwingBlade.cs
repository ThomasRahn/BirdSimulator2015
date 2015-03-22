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

		private AudioSource audioSource;
		private bool playedSound;
		private const int SOUND_LEEWAY = 3;

		private void Start() 
		{
            phaseShift = this.transform.parent.localEulerAngles.z;
			this.GetComponent<HingeJoint>().connectedAnchor = transform.position;

			this.audioSource = this.GetComponentInChildren<AudioSource>();
			this.playedSound = false;
		}

		private void FixedUpdate()
		{
			float rotation = maxAngle * Mathf.Cos((2 * Mathf.PI/period) * Time.time + phaseShift);
            transform.rotation = Quaternion.AngleAxis(rotation, transform.forward);

			float rotationMagnitude = Mathf.Abs(rotation);
			if(rotationMagnitude < SOUND_LEEWAY && !playedSound)
			{
				playedSound = true;
				audioSource.Stop();
				audioSource.Play();
			}
			else if(rotationMagnitude > maxAngle - SOUND_LEEWAY)
			{
				playedSound = false;
			}
		}
	}
}
