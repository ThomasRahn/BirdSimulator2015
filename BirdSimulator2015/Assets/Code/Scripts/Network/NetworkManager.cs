using UnityEngine;

public class NetworkManager : MonoBehaviour
{
	public GameObject Owner;
	public GameObject Proxy;
	
	void uLink_OnServerInitialized()
	{
		Debug.Log("OnServerInitialized()");
		uLink.Network.Instantiate(uLink.Network.player, Proxy, Owner, Owner, Vector3.zero, Quaternion.identity, 0);
	}

	void uLink_OnConnectedToServer()
	{
		Debug.Log("OnConnectedToServer()");
	}

	void uLink_OnPlayerConnected(uLink.NetworkPlayer player)
	{
		Debug.Log("OnPlayerConnected() " + player.ipAddress + ":" + player.port);
		uLink.Network.Instantiate(player, Proxy, Owner, Proxy, Vector3.zero, Quaternion.identity, 0);
	}

	void uLink_OnPlayerDisconnected(uLink.NetworkPlayer player)
	{
		uLink.Network.DestroyPlayerObjects(player);
		uLink.Network.RemoveRPCs(player);
	}

	void uLink_OnFailedToConnect(uLink.NetworkConnectionError error)
	{
		Debug.LogError("OnFailedToConnect() " + error);
	}
}