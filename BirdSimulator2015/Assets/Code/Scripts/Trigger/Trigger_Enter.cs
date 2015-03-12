using UnityEngine;
using System.Collections;

public class Trigger_Enter : MonoBehaviour
{
    public GameObject[] Triggerables;

    void OnTriggerEnter(Collider c)
    {
        if (c.name == "Player(Clone)" || c.name == "PlayerProxy(Clone)")
        {
            foreach (GameObject g in Triggerables)
            {
                if (g != null)
                    g.GetComponent<BaseTriggerable>().Trigger(c, this.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.name == "Player(Clone)" || c.name == "PlayerProxy(Clone)")
        {
            foreach (GameObject g in Triggerables)
            {
                if (g != null)
                    g.GetComponent<BaseTriggerable>().Trigger(c, this.gameObject);
            }
        }
    }
}
