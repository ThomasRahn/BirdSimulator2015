using UnityEngine;
using System.Collections;

public class LandingZone : MonoBehaviour
{
    public Transform Target;

    void Start()
    {
    }
	
	void Update()
    {
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == Registry.Tag.Player)
		{
			//GameController.GamepadPopup.SetImage(GamepadSetup.GamepadAction.A);
			//GameController.GamepadPopup.FadeIn();
			GameController.Player.GetComponent<PlayerState>().LandTarget = Target;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.tag == Registry.Tag.Player)
		{
			//GameController.GamepadPopup.FadeOut();
            GameController.Player.GetComponent<PlayerState>().LandTarget = null;
		}
	}
}
