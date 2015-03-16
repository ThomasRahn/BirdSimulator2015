using UnityEngine;

public class NetworkManager : MonoBehaviour
{
	public GameObject Owner;
	public GameObject Proxy;
	public GameObject Tether;

    Vector3 PLAYER_ROTATION = new Vector3(0f, 260f, 0f); 

    // ravenhome
	Vector3 PLAYER_SPAWN = new Vector3(-7.84f, 70f, 0);

	void uLink_OnServerInitialized()
	{
		Debug.Log("OnServerInitialized()");
        uLink.Network.Instantiate(uLink.Network.player, Proxy, Owner, Owner, PLAYER_SPAWN, Quaternion.Euler(PLAYER_ROTATION), 0);
		GameController.LoadWorld();
	}

	void uLink_OnConnectedToServer()
	{
		Debug.Log("OnConnectedToServer()");
		uLink.Network.Instantiate(uLink.Network.player, Proxy, Owner, Proxy, PLAYER_SPAWN, Quaternion.identity, 0);
		uLink.Network.Instantiate(uLink.Network.player, Tether, Tether, Tether, PLAYER_SPAWN, Quaternion.identity, 0);
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