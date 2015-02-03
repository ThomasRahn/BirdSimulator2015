using UnityEngine;

public class Connect : MonoBehaviour
{
	public string ip = "127.0.0.1";
	public int port = 7100;

	void OnGUI()
	{
		// are we connected to the server?
		if (uLink.Network.peerType == uLink.NetworkPeerType.Disconnected)
		{
			ip = GUI.TextField(new Rect(120,10,100,20), ip);
			port = int.Parse(GUI.TextField(new Rect(230,10,40,20), port.ToString()));

			if (GUI.Button(new Rect(10,10,100,30),"Connect"))
			{
				uLink.Network.Connect(ip, port);
			}
			if (GUI.Button (new Rect(10,50,100,30),"Start Server"))
			{
				uLink.Network.isAuthoritativeServer = false;
				uLink.Network.InitializeServer(32, port);
			}
		}
		else
		{
			// connection to server established
			GUI.Label(new Rect(140,20,250,40), "IP: " + uLink.Network.player.ipAddress + ":" + uLink.Network.player.port.ToString());

			if (uLink.Network.isServer)
			{
				GUI.Label(new Rect(140,60,350,40), "Running as server");
			}
			else if (uLink.Network.isClient)
			{
				GUI.Label(new Rect(140,60,350,40), "Running as client");
			}
		}
	}

	void uLink_OnServerInitialized()
	{
		Debug.Log("OnServerInitialized()");
	}

	void uLink_OnConnectedToServer()
	{
		Debug.Log("OnConnectedToServer()");
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

	void uLink_OnPlayerConnected(uLink.NetworkPlayer player)
	{
		Debug.Log("OnPlayerConnected() " + player.ipAddress + ":" + player.port);
	}
}