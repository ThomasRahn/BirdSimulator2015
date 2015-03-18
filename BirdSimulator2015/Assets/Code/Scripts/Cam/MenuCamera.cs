using UnityEngine;
using System.Collections;

public class MenuCamera : MonoBehaviour
{
    private float rate = 0.2f;

	void Start()
    {
	}
	
	void Update()
    {
        this.transform.Rotate(Vector3.up, rate * Time.deltaTime);

        if (this.transform.localRotation.eulerAngles.y < 240)
        {
            rate *= -1;
        }
        else if (this.transform.localRotation.eulerAngles.y > 260)
        {
            rate *= -1;
        }
	}
}
