using UnityEngine;
using System.Collections;

public class LandingZone : MonoBehaviour
{
    public Transform Target;

    void Start()
    {
        Target.GetComponent<Renderer>().enabled = false;
    }
	
	void Update()
    {
	}
}
