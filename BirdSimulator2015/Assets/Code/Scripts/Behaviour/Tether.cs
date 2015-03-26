using UnityEngine;
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

        line.material.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
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
			uLink.NetworkView.Get(proxySync).RPC("Die", uLink.RPCMode.Others);
			uLink.NetworkView.Get(this).RPC("SetPartnerStatus", uLink.RPCMode.All, true);
			uLink.NetworkView.Get(this).RPC("WaitRespawns", uLink.RPCMode.Others, false);
			WaitRespawns(true);
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
		//StartCoroutine(LerpColor(Color.white, Color.clear));
        StartCoroutine(coFadeOut());
		this.enabled = false;
	}

	public void FadeIn(float rate)
	{
		//StartCoroutine(LerpColor(Color.clear, Color.white));
        StartCoroutine(coFadeIn(rate));
		this.enabled = true;
	}

    IEnumerator coFadeIn(float rate)
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * rate;
            line.material.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
            yield return null;
        }

        yield return null;
    }

    IEnumerator coFadeOut()
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * 2f;
            line.material.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
            yield return null;
        }

        yield return null;
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
	public void WaitRespawns(bool iDied)
	{
		GameController.SetInputLock(true);
		player.SetTargetVelocity(Vector3.zero);
		FadeOut();
		StartCoroutine(coWaitRespawns(iDied));
	}

	[RPC]
	public void SetPartnerStatus(bool dead)
	{
		partnerDead = dead;
	}

	private IEnumerator coWaitRespawns(bool iDied)
	{
		// Wait for myself to die because of other bird
		if(!iDied && player.GetState() != PlayerState.BirdState.Dying)
		{
			while(player.GetState() != PlayerState.BirdState.Dying)
			{
				yield return null;
			}
		}

		// Wait for bird to be finish respawning
		while(player.GetState() == PlayerState.BirdState.Dying)
		{
			yield return null;
		}
		while(player.GetState() == PlayerState.BirdState.Respawning)
		{
			keepStill();
			yield return null;
		}
		// Tell partner that I have respawned
		uLink.NetworkView.Get(this).RPC("SetPartnerStatus", uLink.RPCMode.Others, false);

		// Wait for other bird to show up
		while(partnerDead)
		{
			keepStill();
			yield return null;
		}

		FadeIn(0.3f); // Fade the tether back in while keeping bird positions
		float timeLeft = FADE_TIMER/3;
		while(timeLeft > 0)
		{
			keepStill();
			timeLeft -= Time.deltaTime;
			yield return null;
		}

		GameController.SetInputLock(false);
		this.enabled = true;
	}

	private void keepStill()
	{
		GameController.SetInputLock(true); // need to keep locking after respawned
		player.transform.position = GameController.GetSpawnLocation();
		player.SetTargetVelocity(Vector3.zero);
    }
}
