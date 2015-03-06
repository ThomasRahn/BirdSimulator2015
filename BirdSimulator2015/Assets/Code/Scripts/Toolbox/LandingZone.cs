using UnityEngine;
using System.Collections;

public class LandingZone : MonoBehaviour
{
    public Transform Target;

    void Start()
    {
        Target.renderer.enabled = false;
    }
	
	void Update()
    {
	}
}
