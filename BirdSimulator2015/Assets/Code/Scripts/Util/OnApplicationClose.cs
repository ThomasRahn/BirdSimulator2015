using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class OnApplicationClose : MonoBehaviour
{
    void OnApplicationQuit()
    {
        GamePad.SetVibration((PlayerIndex)0, 0f, 0f);
        //PlayerPrefs.Save();
    }
}
