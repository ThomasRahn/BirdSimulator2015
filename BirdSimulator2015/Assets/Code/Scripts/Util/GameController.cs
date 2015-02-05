using UnityEngine;
using System.Collections;
using BirdSimulator2015.Code.Scripts.Cam;

public class GameController : ScriptableObject
{
	[HideInInspector] public static GameObject CameraTarget;
	[HideInInspector] public static GameObject Player;
	public GameObject Canvas;
	static Transform World;
	
	void Start()
	{
		// hide the canvas until runtime because it's really annoying in the scene view
		Canvas.SetActive(true);

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
		CameraTarget.GetComponent<CameraTarget>().Bird = Player;

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
