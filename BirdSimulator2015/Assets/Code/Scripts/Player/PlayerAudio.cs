using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour
{
    public AudioClip Death;
    public AudioClip Swoop;
    public AudioClip Tornado;
    public AudioClip Flash;

	void Start()
    {
	}
	
	void Update()
    {
	}

    public void PlayDeath()
    {
        this.GetComponent<AudioSource>().PlayOneShot(Death);
    }

    public void PlaySwoop()
    {
        this.GetComponent<AudioSource>().PlayOneShot(Swoop);
    }

    public void PlayTornado()
    {
        this.GetComponent<AudioSource>().PlayOneShot(Tornado);
    }

    public void PlayFlash()
    {
        this.GetComponent<AudioSource>().PlayOneShot(Flash);
    }

}
