using UnityEngine;
using System.Collections;
using BirdSimulator2015.Code.Scripts.Cam;

public class GameController : ScriptableObject
{
	[HideInInspector] public static GameObject CameraTarget;
	[HideInInspector] public static GameObject Player;
    [HideInInspector] public static GamepadPopup GamepadPopup;
    [HideInInspector] public static GamepadSetup Gamepad;
	public GameObject Canvas;
	static Transform World;

    void Awake()
    {
        // TODO this will probably be delegated to an options menu
        Gamepad = new GamepadSetup(GamepadSetup.Type.LOGITECHF310);
    }

	void Start()
	{
		// hide the canvas until runtime because it's really annoying in the scene view
		Canvas.SetActive(true);

        // setup everything related to the ui
        GamepadPopup = GameObject.Find("GamepadPopup").GetComponent<GamepadPopup>();

		// just for organizing the world
		World = GameObject.Find("World").transform;
	}

	void Update()
	{
	}

	public static void LoadWorld()
	{
		loadChunks();
		
		// player has been instantiated already, so we can get this reference
		Player = GameObject.FindWithTag("Player");
		//Debug.Log(Player);

		// camera setup (make sure this comes after the player is loaded)
		CameraTarget = GameObject.Instantiate(Resources.Load("Player/CameraTarget")) as GameObject;
		CameraTarget.BroadcastMessage("SetTarget", Player);

	}

	static void loadChunks()
	{
		foreach (GameObject g in Resources.LoadAll("World/Chunk", typeof(GameObject)))
		{
			//Debug.Log("Loading chunk: " + g.name);
			GameObject h = GameObject.Instantiate(g) as GameObject;
			h.transform.parent = World;
		}
		
		foreach (GameObject g in Resources.LoadAll("World/Dungeon", typeof(GameObject)))
		{
			GameObject h = GameObject.Instantiate(g) as GameObject;
			h.transform.parent = World;
		}
		
		foreach (GameObject g in Resources.LoadAll("World/Environment", typeof(GameObject)))
		{
			GameObject h = GameObject.Instantiate(g) as GameObject;
			h.transform.parent = World;
		}
	}
}
