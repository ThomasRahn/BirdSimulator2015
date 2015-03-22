using UnityEngine;

public class NetworkManager : MonoBehaviour
{
	public GameObject Owner;
	public GameObject Proxy;

    // ravenhome
    //Vector3 WHITE_SPAWN = new Vector3(38.8f, 118f, -3.8f);
    //Vector3 BLACK_SPAWN = new Vector3(38.8f, 118f, 1.8f);

    // TESTING BIGROOM
    Vector3 WHITE_SPAWN = new Vector3(-2306f, -878.968f, 10f);
    Vector3 BLACK_SPAWN = new Vector3(-2306f, -878.968f, -10f);

    Quaternion SPAWN_ROTATION = Quaternion.Euler(new Vector3(0f, 270f, 0f));

	void uLink_OnServerInitialized()
	{
		Debug.Log("OnServerInitialized()");
        uLink.Network.Instantiate(uLink.Network.player, Proxy, Owner, Owner, WHITE_SPAWN, SPAWN_ROTATION, 0);
		GameController.LoadWorld();
	}

	void uLink_OnConnectedToServer()
	{
		Debug.Log("OnConnectedToServer()");
        uLink.Network.Instantiate(uLink.Network.player, Proxy, Owner, Proxy, BLACK_SPAWN, SPAWN_ROTATION, 0);
		GameController.LoadWorld();
	}

	void uLink_OnPlayerConnected(uLink.NetworkPlayer player)
	{
		Debug.Log("OnPlayerConnected() " + player.ipAddress + ":" + player.port);
	}

	void uLink_OnPlayerDisconnected(uLink.NetworkPlayer player)
	{
		Debug.Log("uLink_OnPlayerDisconnected() " + player.ipAddress);
		uLink.Network.DestroyPlayerObjects(player);
		uLink.Network.RemoveRPCs(player);
	}

	void uLink_OnFailedToConnect(uLink.NetworkConnectionError error)
	{
		Debug.LogError("OnFailedToConnect() " + error);
	}
}