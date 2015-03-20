using UnityEngine;
using System.Collections;

public class Triggerable_EggNest : BaseTriggerable<BaseTriggerable>
{
    public GameObject[] Torches;
    public GameObject[] Pillars;
    public GameObject Light;

    private int eggz = 0;
	private float VERTICAL_OFFSET = 5f;

    void Start()
    {
        Pillars[0].GetComponentInChildren<LandingZone>().enabled = false;
        Pillars[1].GetComponentInChildren<LandingZone>().enabled = false;

        Pillars[0].GetComponentInChildren<SphereCollider>().enabled = false;
        Pillars[1].GetComponentInChildren<SphereCollider>().enabled = false;
    }

    void Update()
    {
    }

    public override void Trigger(Collider c, GameObject g)
    {
        eggz++;

		c.transform.position = this.transform.position - Vector3.up * VERTICAL_OFFSET;
		c.GetComponentInChildren<Egg>().Detach();

        if (eggz == 1)
        {
            Torches[0].GetComponentInChildren<BaseTriggerable>().Trigger(c, this.gameObject);
        }
        else if (eggz == 2)
        {
            Torches[1].GetComponentInChildren<BaseTriggerable>().Trigger(c, this.gameObject);

            Pillars[0].GetComponentInChildren<LandingZone>().enabled = true;
            Pillars[1].GetComponentInChildren<LandingZone>().enabled = true;

            Pillars[0].GetComponentInChildren<SphereCollider>().enabled = true;
            Pillars[1].GetComponentInChildren<SphereCollider>().enabled = true;

            Light.GetComponent<Light>().enabled = true;
            Light.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;

            StartCoroutine(coFadeInLight());
        }

        base.Trigger(c, g);
    }

    IEnumerator coFadeInLight()
    {
        while (true)
        {
            if (Light.GetComponent<Light>().intensity < 8)
            {
                Light.GetComponent<Light>().intensity += Time.deltaTime;
            }
            if (Light.transform.GetChild(0).GetComponent<MeshRenderer>().material.color.a < 0.15f)
            {
                Color c = Light.transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
                c.a += Time.deltaTime * 0.1f;
                Light.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = c;
            }

            yield return null;
        }
    }
}
