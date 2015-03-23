using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class MainMenuBehaviour : MonoBehaviour
{
	string ip = "127.0.0.1";
	int port = 1337;
	
	void Start()
	{
	}

	void Update()
	{
	}

	public void Server()
	{
		toggleMenu("MainMenu", false);
		uLink.Network.isAuthoritativeServer = false;
		uLink.Network.InitializeServer(32, port);
	}

	public void Client()
	{
        toggleMenu("MainMenu", false);
		uLink.Network.Connect(ip, port);
	}

    public void Options()
    {
        toggleMenu("MainMenu", false);
        toggleMenu("OptionsMenu", true);
        GameObject.Find("IsInverted").GetComponent<Text>().text = (GameController.Gamepad.Inverted == 1 ? "Yes" : "No");
        GameObject.Find("Gamepad").GetComponent<Text>().text = GameController.Gamepad.GetGamepadType() + "";
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Back"));
    }

    public void Options_Controller()
    {
        int i = Enum.GetNames(typeof(GamepadSetup.GamepadType)).Length;
        //Debug.Log("GamepadType count: " + i);

        int c = (int)GameController.Gamepad.GetGamepadType();
        //Debug.Log("Current GamepadType: " + c);

        if ((c + 1) >= i)
        {
            c = 0;
        }
        else
        {
            c++;
        }

        GameController.Gamepad = new GamepadSetup((GamepadSetup.GamepadType)c);
        GameObject.Find("Gamepad").GetComponent<Text>().text = GameController.Gamepad.GetGamepadType() + "";
    }

    public void Options_Port()
    {
        EventSystem.current.SetSelectedGameObject(GameObject.Find("PortField"));
    }

    public void Options_Port_EndEdit(string s)
    {
        int p;
        bool b = Int32.TryParse(this.GetComponent<InputField>().text, out p);
        if (b) port = p;
        Debug.Log("Port changed to: " + port);

        EventSystem.current.SetSelectedGameObject(GameObject.Find("Port"));
    }

    public void Options_IP()
    {
        EventSystem.current.SetSelectedGameObject(GameObject.Find("IPField"));
    }

    public void Options_IP_EndEdit(string s)
    {
        ip = this.GetComponent<InputField>().text;
        Debug.Log("IP changed to: " + ip);

        EventSystem.current.SetSelectedGameObject(GameObject.Find("IP"));
    }

    public void Options_InvertY()
    {
        GameController.Gamepad.Inverted = -GameController.Gamepad.Inverted;
        GameObject.Find("IsInverted").GetComponent<Text>().text = (GameController.Gamepad.Inverted == 1 ? "Yes" : "No");
    }

    public void Options_Back()
    {
        toggleMenu("OptionsMenu", false);
        toggleMenu("MainMenu", true);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Play"));
    }

    private void toggleMenu(string s, bool b)
    {
        GameObject menu = GameObject.Find(s);
        int children = menu.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            menu.transform.GetChild(i).gameObject.SetActive(b);
        }
    }
}
