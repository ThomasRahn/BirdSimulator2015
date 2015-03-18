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
        //Debug.DrawRay(this.transform.position, this.rigidbody.velocity * 5f, Color.magenta);
        //Debug.DrawRay(this.transform.position, this.transform.up * 1f, Color.blue);
        //Debug.DrawRay(this.transform.position, this.transform.forward * 1f, Color.green);
        //Debug.DrawRay(this.transform.position, Vector3.down * 1f, Color.red);

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

		if (GameObject.FindWithTag(Registry.Tag.Player))
		{
			string t = "";
			//t += "rotationZ= " + GameObject.FindWithTag("Player").transform.GetChild(0).transform.localEulerAngles.z + "\n";
			//t += GameObject.FindWithTag("Player").GetComponent<PlayerState>().currentState + "\n";
            //t += "rotationY= " + GameObject.FindWithTag("Player").GetComponent<PlayerState>().rotationY + "\n";
            //t += "momentum= " + GameObject.FindWithTag("Player").GetComponent<PlayerState>().momentum + "\n";
			t += "velocity= " + GameObject.FindWithTag(Registry.Tag.Player).GetComponent<Rigidbody>().velocity + "\n";
           // t += "currentMaxSpeed= " + GameObject.FindWithTag("Player").GetComponent<PlayerState>().currentMaxSpeed + "\n";
           // t += "currentTurnSpeed= " + GameObject.FindWithTag("Player").GetComponent<PlayerState>().currentTurnSpeed + "\n";
			Left.text = t;
		}

	}
}
