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

        if (this.transform.localRotation.eulerAngles.y < 120)
        {
            rate *= -1;
        }
        else if (this.transform.localRotation.eulerAngles.y > 140)
        {
            rate *= -1;
        }
	}
}
