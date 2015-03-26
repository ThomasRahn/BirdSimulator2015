using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class MainMenuBehaviour : MonoBehaviour
{
	string ip;
	int port;
	
	void Start()
	{
        ip = PlayerPrefs.GetString("ip");
        port = PlayerPrefs.GetInt("port");
	}

	void Update()
	{
	}

	public void Server()
    {
        GameObject.Find("EventSystem").GetComponent<EventSystem>().sendNavigationEvents = false;
        GameController.DeathPopup.FadeIn();
        StartCoroutine(coServer());
	}

    IEnumerator coServer()
    {
        yield return new WaitForSeconds(3.1f);
        toggleMenu("MainMenu", false);
        uLink.Network.isAuthoritativeServer = false;
        uLink.Network.InitializeServer(32, port);
        GameController.DeathPopup.FadeOut();
    }

	public void Client()
	{
        GameObject.Find("EventSystem").GetComponent<EventSystem>().sendNavigationEvents = false;
        GameController.DeathPopup.FadeIn();
        StartCoroutine(coClient());
	}

    IEnumerator coClient()
    {
        yield return new WaitForSeconds(3.1f);
        toggleMenu("MainMenu", false);
        uLink.Network.Connect(ip, port);
        GameController.DeathPopup.FadeOut();
    }

    public void Options()
    {
        toggleMenu("MainMenu", false);
        toggleMenu("OptionsMenu", true);
        GameObject.Find("IsInverted").GetComponent<Text>().text = (GameController.Gamepad.Inverted == 1 ? "Yes" : "No");
        GameObject.Find("Gamepad").GetComponent<Text>().text = GameController.Gamepad.GetGamepadType() + "";

        Debug.Log("Setting port as : " + PlayerPrefs.GetInt("port"));
        Debug.Log("Setting IP as : " + PlayerPrefs.GetString("ip"));

        GameObject.Find("PortField").GetComponent<InputField>().textComponent.text = PlayerPrefs.GetInt("port") + "";
        GameObject.Find("PortField").GetComponent<InputField>().placeholder.GetComponent<Text>().text = PlayerPrefs.GetInt("port") + "";
        GameObject.Find("IPField").GetComponent<InputField>().textComponent.text = PlayerPrefs.GetString("ip");
        GameObject.Find("IPField").GetComponent<InputField>().placeholder.GetComponent<Text>().text = PlayerPrefs.GetString("ip");
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
        PlayerPrefs.SetInt("port", port);
        PlayerPrefs.Save();

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
        PlayerPrefs.SetString("ip", ip);
        PlayerPrefs.Save();

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
