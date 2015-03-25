using UnityEngine;
using System.Collections;

public class Triggerable_LightMany : BaseTriggerable<BaseTriggerable>
{
    public int HowMany = 0; // how many triggers needed for true
    public float DelayBefore = 0; // delay before starting to light first torch
    public float DelayDuring = 0; // time between lights
    public GameObject[] Torches;

    private bool alreadyTriggered = false;
    private int current = 0;

    void Start()
    {
    }

    void Update()
    {
        if (!alreadyTriggered && current >= HowMany)
        {
            alreadyTriggered = true;
            StartCoroutine(coLightEmUp());
        }
    }

    public override void Trigger(Collider c, GameObject g)
    {
        current++;
        base.Trigger(c, g);
    }

    IEnumerator coLightEmUp()
    {
        yield return new WaitForSeconds(DelayBefore);

        for (int i = 0; i < Torches.GetLength(0); i++)
        {
            Torches[i].GetComponentInChildren<Flame>().ToggleLit();
            Torches[i + 1].GetComponentInChildren<Flame>().ToggleLit();
            i++;
            yield return new WaitForSeconds(DelayDuring);
        }
    }
}
