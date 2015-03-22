using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour
{
    public enum BGMTrack
    {
        NONE,
        Overworld,
        Underworld,
        Chase,

    }

    public AudioSource BGM;
    public AudioClip Overworld;
    public AudioClip Underworld;
    public AudioClip Chase;

    public void PlayTrack(BGMTrack track)
    {
        if (track == BGMTrack.Overworld)
            StartCoroutine(coFadeOut(Overworld));

        if (track == BGMTrack.Underworld)
            StartCoroutine(coFadeOut(Underworld));

        if (track == BGMTrack.Chase)
            StartCoroutine(coFadeOut(Chase));
    }

    public void FadeOut()
    {
        StartCoroutine(coFadeOut(null));
    }

    public void FadeIn(BGMTrack track)
    {
        if (track == BGMTrack.Overworld)
        {
            BGM.clip = Overworld;
            BGM.Play();
            StartCoroutine(coFadeIn());
        }

        if (track == BGMTrack.Underworld)
        {
            BGM.clip = Underworld;
            BGM.Play();
            StartCoroutine(coFadeIn());
        }

        if (track == BGMTrack.Chase)
        {
            BGM.clip = Chase;
            BGM.Play();
            StartCoroutine(coFadeIn());
        }
    }

    IEnumerator coFadeOut(AudioClip clip)
    {
        while (BGM.volume > 0)
        {
            BGM.volume -= Time.deltaTime;
            yield return null;
        }

        if (clip != null)
        {
            BGM.clip = clip;
            BGM.Play();
            StartCoroutine(coFadeIn());
        }

        yield return null;
    }

    IEnumerator coFadeIn()
    {
        while (BGM.volume < 1)
        {
            BGM.volume += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }
}
