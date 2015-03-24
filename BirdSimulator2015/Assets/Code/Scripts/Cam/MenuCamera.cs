using UnityEngine;
using System.Collections;

public class MenuCamera : MonoBehaviour
{
    private float rate = 0.2f;
    private float origin;

	void Start()
    {
        origin = this.transform.localRotation.eulerAngles.y;
	}
	
	void Update()
    {
        this.transform.Rotate(Vector3.up, rate * Time.deltaTime);

        if (this.transform.localRotation.eulerAngles.y < origin - 10)
        {
            rate *= -1;
        }
        else if (this.transform.localRotation.eulerAngles.y > origin + 10)
        {
            rate *= -1;
        }
	}
}
