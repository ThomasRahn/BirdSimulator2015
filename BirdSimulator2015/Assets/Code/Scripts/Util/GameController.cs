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
	[HideInInspector] public static GamepadSetup Gamepad;
    [HideInInspector] public static Transform LastCheckpoint;

	public GameObject Canvas;
    private static GameObject canvas;
	private static Transform world;

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

		// just for organizing the world
		world = GameObject.Find("World").transform;
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
		//loadChunks();
		
        // disable the wrong spawn
        if (IsWhite)
            GameObject.FindWithTag(Registry.Tag.SpawnBlack).SetActive(false);
        else
            GameObject.FindWithTag(Registry.Tag.SpawnWhite).SetActive(false);

		// player has been instantiated already, so we can get this reference
		Player = GameObject.FindWithTag(Registry.Tag.Player);
		//Debug.Log(Player);

        // set up player colours

            foreach (Renderer renderer in Player.GetComponentsInChildren<Renderer>())
            {
                foreach (Material material in renderer.materials)
                {
                    if (IsWhite)
                    {
                        material.color = Color.white;
                    }
                    else
                    {
                        material.color = Color.black;
                    }
                }
            }


		// camera setup (make sure this comes after the player is loaded)
		CameraTarget = GameObject.Instantiate(Resources.Load("Player/CameraTarget")) as GameObject;
		CameraTarget.BroadcastMessage("SetTarget", Player);

		// land on branch
		if(IsWhite)
		{
			Player.GetComponent<PlayerState>().LandTarget = GameObject.FindGameObjectWithTag(Registry.Tag.SpawnWhite).transform;
		}
		else
		{
			Player.GetComponent<PlayerState>().LandTarget = GameObject.FindGameObjectWithTag(Registry.Tag.SpawnBlack).transform;
		}

        // if server, load in all objects that must be networked
        if (uLink.Network.isServer)
        {
			Debug.Log("Server load objects");
			uLink.Network.Instantiate(uLink.Network.player, Registry.Prefab.EggProxy, Registry.Prefab.Egg, Registry.Prefab.Egg, new Vector3(-2400f, -872f, 227f), Quaternion.identity, 0);
            uLink.Network.Instantiate(uLink.Network.player, Registry.Prefab.EggProxy, Registry.Prefab.Egg, Registry.Prefab.Egg, new Vector3(-2400f, -872f, -233f), Quaternion.identity, 0);

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

    public static void SetInputLock(bool b)
    {
        Player.GetComponent<PlayerInput>().Locked = b;
    }

	public static void SetLastCheckpoint(Transform t)
	{
        LastCheckpoint = t;
	}
}
