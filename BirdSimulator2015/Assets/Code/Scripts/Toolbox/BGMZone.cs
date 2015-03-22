using UnityEngine;
using System.Collections;

public class BGMZone : MonoBehaviour
{
    public AudioController.BGMTrack Track;

	void Start()
    {
	}
	
	void Update()
    {
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == Registry.Tag.Player)
        {
            GameObject.FindWithTag("AudioController").GetComponent<AudioController>().PlayTrack(Track);
        }
    }
}
