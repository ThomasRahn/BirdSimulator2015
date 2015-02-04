using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugOverlay : MonoBehaviour
{
	public Text Left;
	public Text Right;

	private float interval = 0.5F;
	private float accum = 0; // fps accumulated over the interval
	private int frames = 0; // frames drawn over the interval
	private float left; // time left for current interval

	void Start()
	{
	}

	void Update()
	{
		left -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		++frames;

		if (left <= 0)
		{
			float fps = accum / frames;

			string s = "";
			s += System.String.Format("<size=24>{0:F0}</size> <size=12>FPS</size> \n", fps);
			s += "<size=24>" + uLink.NetworkPlayer.server.averagePing + "</size> <size=12>MS</size>";
			Right.text = s;

			left = interval;
			accum = 0.0F;
			frames = 0;
		}

		if (GameObject.FindWithTag("Player"))
		{
			string t = "";
			t += "rotationZ= " + GameObject.FindWithTag("Player").transform.GetChild(0).transform.localEulerAngles.z + "\n";
			t += GameObject.FindWithTag("Player").GetComponent<PlayerInput>().currentState + "\n";
			t += "rotationY= " + GameObject.FindWithTag("Player").GetComponent<PlayerInput>().rotationY + "\n";
			t += "momentum= " + GameObject.FindWithTag("Player").GetComponent<PlayerInput>().momentum + "\n";
			t += "velocity= " + GameObject.FindWithTag("Player").rigidbody.velocity + "\n";
			t += "currentMaxSpeed= " + GameObject.FindWithTag("Player").GetComponent<PlayerInput>().currentMaxSpeed + "\n";
			t += "currentTurnSpeed= " + GameObject.FindWithTag("Player").GetComponent<PlayerInput>().currentTurnSpeed + "\n";
			Left.text = t;
		}

	}
}
