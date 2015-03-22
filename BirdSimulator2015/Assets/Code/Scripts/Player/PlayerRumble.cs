using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class PlayerRumble : MonoBehaviour 
{
	void Start()
    {
	}
	
	void Update()
    {
	}

    public void BumbleRumble(float f, float l, float r)
    {
        StartCoroutine(coBumbleRumble(f, l, r));
    }

    IEnumerator coBumbleRumble(float f, float l, float r)
    {
        float t = f;

        GamePad.SetVibration((PlayerIndex)0, l, r);
        while (t > 0)
        {
            t -= Time.deltaTime;
            yield return null;
        }

        GamePad.SetVibration((PlayerIndex)0, 0, 0);
        yield return null;
    }
}
