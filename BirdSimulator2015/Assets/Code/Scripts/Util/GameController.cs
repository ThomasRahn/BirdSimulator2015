using UnityEngine;
using System.Collections;
using BirdSimulator2015.Code.Scripts.Cam;

public class GameController : ScriptableObject
{
	[HideInInspector] public static GameObject CameraTarget;
	[HideInInspector] public static GameObject Player;
    [HideInInspector] public static GamepadPopup GamepadPopup;
	[HideInInspector] public static LocationPopup LocationPopup;
	[HideInInspector] public static GamepadSetup Gamepad;
	public GameObject Canvas;
    private static GameObject canvas;
	private static Transform world;

    void Awake()
    {
        Gamepad = new GamepadSetup(GamepadSetup.GamepadType.LOGITECHF310);
    }

	void Start()
	{
		// hide the canvas until runtime because it's really annoying in the scene view
		Canvas.SetActive(true);
        canvas = Canvas;

        // setup everything related to the ui
        GamepadPopup = GameObject.Find("GamepadPopup").GetComponent<GamepadPopup>();
		LocationPopup = GameObject.Find("LocationPopup").GetComponent<LocationPopup>();

		// just for organizing the world
		world = GameObject.Find("World").transform;
	}

	void Update()
	{
	}

	public static void LoadWorld()
	{
        GameObject.Destroy(GameObject.Find("MenuCamera"));

		//loadChunks();
		
		// player has been instantiated already, so we can get this reference
		Player = GameObject.FindWithTag("Player");
		//Debug.Log(Player);

		// camera setup (make sure this comes after the player is loaded)
		CameraTarget = GameObject.Instantiate(Resources.Load("Player/CameraTarget")) as GameObject;
		CameraTarget.BroadcastMessage("SetTarget", Player);

		// land on branch
		Player.GetComponent<PlayerInput>().SetTrigger("t_Land");

        // if server, load in all objects that must be networked
        if (uLink.Network.isServer)
        {
			Debug.Log("Server load objects");
			uLink.Network.Instantiate(uLink.Network.player, "Egg", "Egg", "Egg", new Vector3(-2400f, -832f, 226.9f), Quaternion.identity, 0);
			uLink.Network.Instantiate(uLink.Network.player, "Egg", "Egg", "Egg", new Vector3(976.3f, 1310f, -832.4f), Quaternion.identity, 0);

			GameObject.Instantiate(Resources.Load(Registry.Prefab.FireballZone), new Vector3(-1314f, -10f, -637f), Quaternion.identity);
        }
	}

	static void loadChunks()
	{
		foreach (GameObject g in Resources.LoadAll("World/Chunk", typeof(GameObject)))
		{
			//Debug.Log("Loading chunk: " + g.name);
			GameObject h = GameObject.Instantiate(g) as GameObject;
			h.transform.parent = world;
		}
		
		foreach (GameObject g in Resources.LoadAll("World/Dungeon", typeof(GameObject)))
		{
			GameObject h = GameObject.Instantiate(g) as GameObject;
			h.transform.parent = world;
		}
		
		foreach (GameObject g in Resources.LoadAll("World/Environment", typeof(GameObject)))
		{
			GameObject h = GameObject.Instantiate(g) as GameObject;
			h.transform.parent = world;
		}
	}

    public static GameObject GetCanvas()
    {
        return canvas;
    }
}
