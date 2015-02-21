using UnityEngine;

public class NetworkManager : MonoBehaviour
{
	public GameObject Owner;
	public GameObject Proxy;

	Vector3 PLAYER_SPAWN = new Vector3(0f, 1000f, 0f);

	void uLink_OnServerInitialized()
	{
		Debug.Log("OnServerInitialized()");
		uLink.Network.Instantiate(uLink.Network.player, Proxy, Owner, Owner, PLAYER_SPAWN, Quaternion.identity, 0);
		GameController.LoadWorld();
	}

	void uLink_OnConnectedToServer()
	{
		Debug.Log("OnConnectedToServer()");
		uLink.Network.Instantiate(uLink.Network.player, Proxy, Owner, Proxy, PLAYER_SPAWN, Quaternion.identity, 0);
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