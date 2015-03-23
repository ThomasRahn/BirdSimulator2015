using UnityEngine;
using System.Collections;
using BirdSimulator2015.Code.Scripts.Cam;

public class GameController : ScriptableObject
{
	[HideInInspector] public static GameObject CameraTarget;
	[HideInInspector] public static GameObject Player;
    [HideInInspector] public static bool IsWhite = false;
    [HideInInspector] public static GamepadPopup GamepadPopup;
	[HideInInspector] public static LocationPopup LocationPopup;
    [HideInInspector] public static CinematicPopup CinematicPopup;
    [HideInInspector] public static LocatorPopup LocatorPopup;
    [HideInInspector] public static DeathPopup DeathPopup;
	[HideInInspector] public static GamepadSetup Gamepad;
    [HideInInspector] public static Transform LastCheckpoint;

	public GameObject Canvas;
    private static GameObject canvas;

    void Awake()
    {
        Gamepad = new GamepadSetup(GamepadSetup.GamepadType.XBOX360);
    }

	void Start()
	{
		// hide the canvas until runtime because it's really annoying in the scene view
		Canvas.SetActive(true);
        canvas = Canvas;

        // setup everything related to the ui
        GamepadPopup = GameObject.Find("GamepadPopup").GetComponent<GamepadPopup>();
		LocationPopup = GameObject.Find("LocationPopup").GetComponent<LocationPopup>();
        CinematicPopup = GameObject.Find("CinematicPopup").GetComponent<CinematicPopup>();
        LocatorPopup = GameObject.Find("LocatorPopup").GetComponent<LocatorPopup>();
        DeathPopup = GameObject.Find("DeathPopup").GetComponent<DeathPopup>();
	}

	void Update()
	{
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}

	public static void LoadWorld()
	{
        GameObject.Destroy(GameObject.Find("MenuCamera"));
		if (uLink.Network.isServer) 
		{
			IsWhite = true;
		}
		else 
		{
			IsWhite = false;
		}

        // disable the wrong spawn
        if (IsWhite)
            GameObject.FindWithTag(Registry.Tag.SpawnBlack).SetActive(false);
        else
            GameObject.FindWithTag(Registry.Tag.SpawnWhite).SetActive(false);

		// player has been instantiated already, so we can get this reference
		Player = GameObject.FindWithTag(Registry.Tag.Player);
		//Debug.Log(Player);

		// camera setup (make sure this comes after the player is loaded)
		CameraTarget = GameObject.Instantiate(Resources.Load(Registry.Prefab.CameraTarget)) as GameObject;
		CameraTarget.BroadcastMessage("SetTarget", Player);

		// land on branch
		if(IsWhite)
		{
			//Player.GetComponent<PlayerState>().LandTarget = GameObject.FindGameObjectWithTag(Registry.Tag.SpawnWhite).transform;
		}
		else
		{
			//Player.GetComponent<PlayerState>().LandTarget = GameObject.FindGameObjectWithTag(Registry.Tag.SpawnBlack).transform;
		}
        //Player.GetComponent<PlayerInput>().SetTrigger("t_Land");

        // if server, load in all objects that must be networked
        if (uLink.Network.isServer)
        {
			Debug.Log("Server load objects");
			//uLink.Network.Instantiate(uLink.Network.player, Registry.Prefab.EggProxy, Registry.Prefab.Egg, Registry.Prefab.Egg, new Vector3(-2400f, -872f, 227f), Quaternion.identity, 0);
            //uLink.Network.Instantiate(uLink.Network.player, Registry.Prefab.EggProxy, Registry.Prefab.Egg, Registry.Prefab.Egg, new Vector3(-2400f, -872f, -233f), Quaternion.identity, 0);

            // CHASE TEST
            uLink.Network.Instantiate(uLink.Network.player, Registry.Prefab.EggProxy, Registry.Prefab.Egg, Registry.Prefab.Egg, new Vector3(-2400f, -882f, 0f), Quaternion.identity, 0);
            uLink.Network.Instantiate(uLink.Network.player, Registry.Prefab.EggProxy, Registry.Prefab.Egg, Registry.Prefab.Egg, new Vector3(-2400f, -882f, 0f), Quaternion.identity, 0);

			GameObject.Instantiate(Resources.Load(Registry.Prefab.FireballZone), new Vector3(-1314f, -10f, -637f), Quaternion.identity);
        }
	}

    public static GameObject GetCanvas()
    {
        return canvas;
    }

    public static void SetInputLock(bool b)
    {
        Player.GetComponent<PlayerInput>().Locked = b;
    }

	public static void SetLastCheckpoint(Transform t)
	{
        LastCheckpoint = t;
	}
}
