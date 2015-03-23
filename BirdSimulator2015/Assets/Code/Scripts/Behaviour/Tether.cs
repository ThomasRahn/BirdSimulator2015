using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Tether : MonoBehaviour 
{
	public float tetherDistance;
	public float tetherForce;

	public PlayerState player;
	public GameObject proxy;

	private LineRenderer line;

	private void Awake()
	{
		line = GetComponent<LineRenderer>();
	}

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag(Registry.Tag.Player).GetComponent<PlayerState>();
		proxy = GameObject.FindGameObjectWithTag(Registry.Tag.Proxy);
		FadeIn();
	}
	
	private void Update() 
	{
		if(player.GetState() == PlayerState.BirdState.Dying)
		{
			uLink.NetworkView.Get(this).RPC("FadeOut", uLink.RPCMode.All);
		}

		transform.position = (player.transform.position + proxy.transform.position)/2;
		float distance = Vector3.Distance(player.transform.position, proxy.transform.position);
		if(distance > tetherDistance)
		{
			float scale = (distance - tetherDistance)/10;
			Vector3 towardsPlayer = player.transform.position - proxy.transform.position;

			// Pull the player towards the proxy if they are too far apart
			player.GetComponent<Rigidbody>().AddForce(-towardsPlayer * tetherForce * scale);
		}

		line.SetPosition(0, player.transform.position);
		line.SetPosition(1, proxy.transform.position);
	}

	[RPC]
	public void FadeOut()
	{
		StartCoroutine(LerpColor(Color.white, Color.clear));
		this.enabled = false;
	}

	[RPC]
	public void FadeIn()
	{
		StartCoroutine(LerpColor(Color.clear, Color.white));
		this.enabled = true;
	}

	private IEnumerator LerpColor(Color start, Color end)
	{
		float time = 0f;
		float duration = 1.5f;
		while(!start.Equals(end))
		{
			start = Color.Lerp(start, end, time/duration);
			line.SetColors(start, start);
			time += Time.deltaTime;
			yield return null;
		}
	}
}
