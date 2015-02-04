using UnityEngine;
using System.Collections;

public class MainMenuBehaviour : MonoBehaviour
{
	string ip = "127.0.0.1";
	int port = 7100;
	
	void Start()
	{
	}

	void Update()
	{
	}

	public void Server()
	{
		hideMenu();
		uLink.Network.isAuthoritativeServer = false;
		uLink.Network.InitializeServer(32, port);
	}

	public void Client()
	{
		hideMenu();
		uLink.Network.Connect(ip, port);
	}

	void hideMenu()
	{
		GameObject.Find("MainMenu").SetActive(false);
	}
}
