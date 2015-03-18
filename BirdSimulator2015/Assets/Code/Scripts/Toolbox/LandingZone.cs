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
		if (other.tag == "Player")
		{
			GameController.GamepadPopup.SetImage(GamepadSetup.GamepadAction.A);
			GameController.GamepadPopup.FadeIn();

			GameController.Player.GetComponent<PlayerState>().LandPos = Target;
			GameController.Player.GetComponent<PlayerState>().CanLand = true;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			GameController.GamepadPopup.FadeOut();
			GameController.Player.GetComponent<PlayerState>().CanLand = false;
		}
	}
}
