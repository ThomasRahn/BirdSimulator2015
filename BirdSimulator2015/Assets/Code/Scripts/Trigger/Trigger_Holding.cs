using UnityEngine;
using System.Collections;

public class Trigger_Holding : MonoBehaviour
{
    public GameObject[] Items; // items that must be present for true
    public GameObject[] Triggerables;

    //private bool alreadyTriggered = false;

	void Start()
    {
	}
	
	void Update()
    {
	}

    void OnTriggerEnter(Collider c)
    {
        //if (!alreadyTriggered)
        //{
            foreach (GameObject i in Items)
            {
                if (i.name == c.name || i.name + "(Clone)" == c.name)
                {
                    //alreadyTriggered = true;

                    foreach (GameObject g in Triggerables)
                    {
                        if (g != null)
                            g.GetComponentInChildren<BaseTriggerable>().Trigger(c, this.gameObject);
                    }
                }
            }
        //}
    }
}
