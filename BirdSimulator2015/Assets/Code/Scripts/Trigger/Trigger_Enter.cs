using UnityEngine;
using System.Collections;

public class Trigger_Enter : MonoBehaviour
{
    public GameObject[] Triggerables;

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == Registry.Tag.Player || c.tag == Registry.Tag.Proxy)
        {
            foreach (GameObject g in Triggerables)
            {
                if (g != null)
                    g.GetComponentInChildren<BaseTriggerable>().Trigger(c, this.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.tag == Registry.Tag.Player || c.tag == Registry.Tag.Proxy)
        {
            foreach (GameObject g in Triggerables)
            {
                if (g != null)
                    g.GetComponentInChildren<BaseTriggerable>().Trigger(c, this.gameObject);
            }
        }
    }
}
