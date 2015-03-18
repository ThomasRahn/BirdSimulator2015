using UnityEngine;
using System.Collections;

public class DisableRenderer : MonoBehaviour
{
	void Start()
    {
        this.GetComponent<Renderer>().enabled = false;
	}
}
