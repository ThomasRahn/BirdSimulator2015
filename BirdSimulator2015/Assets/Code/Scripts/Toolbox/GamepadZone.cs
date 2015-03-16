using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GamepadZone : MonoBehaviour
{
	public GamepadSetup.GamepadAction Action;
	
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
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			GameController.GamepadPopup.FadeOut();
		}
	}
}
