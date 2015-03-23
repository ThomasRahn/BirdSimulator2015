﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Tether : MonoBehaviour 
{
	public float tetherDistance;
	public float tetherForce;

	public PlayerState player;
	public GameObject proxySync;
	public GameObject proxyModel;

	private LineRenderer line;
	private bool partnerDead;
	private const int FADE_TIMER = 3;

	private void Awake()
	{
		line = GetComponent<LineRenderer>();
		partnerDead = false;
	}

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag(Registry.Tag.Player).GetComponent<PlayerState>();
		proxySync = GameObject.FindGameObjectWithTag(Registry.Tag.Proxy);
		proxyModel = proxySync.transform.Find("Raven/Raven").gameObject;
	}
	
	private void Update() 
	{
		if(player.GetState() == PlayerState.BirdState.Dying)
		{
			uLink.NetworkView.Get(this).RPC("SetPartnerStatus", uLink.RPCMode.All, true);
			uLink.NetworkView.Get(proxySync).RPC("Die", uLink.RPCMode.Others);
			uLink.NetworkView.Get(this).RPC("WaitRespawns", uLink.RPCMode.All);
			return;
		}

		transform.position = (player.transform.position + proxyModel.transform.position)/2;
		float distance = Vector3.Distance(player.transform.position, proxyModel.transform.position);
		if(distance > tetherDistance)
		{
			float scale = (distance - tetherDistance)/10;
			Vector3 towardsPlayer = player.transform.position - proxyModel.transform.position;

			// Pull the player towards the proxy if they are too far apart
			player.GetComponent<Rigidbody>().AddForce(-towardsPlayer * tetherForce * scale);
		}

		line.SetPosition(0, player.transform.position);
		line.SetPosition(1, proxyModel.transform.position);
	}

	public void FadeOut()
	{
		StartCoroutine(LerpColor(Color.white, Color.clear));
		this.enabled = false;
	}

	public void FadeIn()
	{
		StartCoroutine(LerpColor(Color.clear, Color.white));
		this.enabled = true;
	}

	private IEnumerator LerpColor(Color start, Color end)
	{
		float time = 0f;
		float duration = FADE_TIMER;

		Color current = start;
		while(!current.Equals(end))
		{
			current = Color.Lerp(start, end, time/duration);
			line.SetColors(current, current);
			time += Time.deltaTime;
			yield return null;
		}
	}

	[RPC]
	public void WaitRespawns()
	{
		GameController.SetInputLock(true);
		player.SetTargetVelocity(Vector3.zero);
		FadeOut();
		StartCoroutine(coWaitRespawns());
	}

	[RPC]
	public void SetPartnerStatus(bool dead)
	{
		partnerDead = dead;
		if(dead)
		{
			player.SetSpeedyMode(false, Vector3.right);
		}
	}

	private IEnumerator coWaitRespawns()
	{
		// Wait for bird to be in finish respawning
		while(player.GetState() == PlayerState.BirdState.Dying || player.GetState() == PlayerState.BirdState.Respawning)
		{
			keepStill();
			yield return null;
		}
		uLink.NetworkView.Get(this).RPC("SetPartnerStatus", uLink.RPCMode.Others, false);

		// Wait for other bird to show up
		while(partnerDead)
		{
			keepStill();
			yield return null;
		}

		FadeIn(); 
		// Fade the tether back in while keeping bird positions
		float timeLeft = FADE_TIMER;
		while(timeLeft > 0)
		{
			keepStill();
			timeLeft -= Time.deltaTime;
			yield return null;
		}

		GameController.SetInputLock(false);
		player.SetSpeedyMode(true, Vector3.right);
		this.enabled = true;
	}

	private void keepStill()
	{
		GameController.SetInputLock(true); // need to keep locking after respawned
		player.transform.position = GameController.LastCheckpoint.position;
		player.SetTargetVelocity(Vector3.zero);
    }
}
