using UnityEngine;

public class Spawn : MonoBehaviour
{
	public GameObject Owner;
	public GameObject Proxy;

	void uLink_OnPlayerConnected(uLink.NetworkPlayer player)
	{
		uLink.Network.Instantiate(player, Proxy, Owner, Proxy, Vector3.zero, Quaternion.identity, 0);
	}

	void uLink_OnConnectedToServer() 
	{
	} 

	void uLink_OnServerInitialized()
	{
		uLink.Network.Instantiate(uLink.Network.player, Proxy, Owner, Owner, Vector3.zero, Quaternion.identity, 0);
	}
}