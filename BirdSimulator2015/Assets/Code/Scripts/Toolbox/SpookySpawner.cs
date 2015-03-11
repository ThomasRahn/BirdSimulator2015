using UnityEngine;
using System.Collections;

public class SpookySpawner : MonoBehaviour
{
    public string Action;
    private bool alreadyTriggered = false;

    void Awake()
    {
        this.transform.GetChild(0).renderer.enabled = false;
    }

	void Start()
    {
	}
	
	void Update()
    {
	}

    void OnTriggerEnter(Collider c)
    {
        if (!alreadyTriggered)
        {
            alreadyTriggered = true;

            // TODO rename proper
            GameObject g = GameObject.Instantiate(Resources.Load("Prefabs/Toolbox/Res/SpookyRaven")) as GameObject;

            // set position
            g.transform.position = this.transform.GetChild(0).position;

            // set rotation
            // add 180 because raven model is flipped
            Vector3 rot = new Vector3(0f, this.transform.GetChild(0).localEulerAngles.y + 180f, 0f);
            g.transform.localEulerAngles = rot;

            // run the ai
            g.GetComponent<AiInput>().DoAction(Action);
        }
    }
}
