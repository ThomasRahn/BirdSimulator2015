using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour
{

	void Start()
    {
	}

	void Update()
    {
        this.transform.Rotate(this.transform.up, 50f * Time.deltaTime);
	}
}
