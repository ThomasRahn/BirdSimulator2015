using UnityEngine;
using System.Collections;

namespace BirdSimulator2015.Code.Scripts.Cam
{
	/// <summary>
	/// Camera target that follows the bird position but rotates more slowly.
	/// </summary>
	public class CameraTarget : MonoBehaviour 
	{
		public GameObject Bird;

		private void Update() 
		{
			transform.position = Bird.transform.position;

			Vector3 targetRotation = Bird.transform.rotation.eulerAngles;
			if(targetRotation.z > 180)
			{
				targetRotation.z = Mathf.Clamp(targetRotation.z, 330, 360);
			}
			else
			{
				targetRotation.z = Mathf.Clamp(targetRotation.z, 0, 30);
			}

			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), 2 * Time.deltaTime);
		}
	}
}
