using UnityEngine;
using System.Collections;

public class PlayerSync : uLink.MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

	void Start()
    {
	}
	
	void Update()
    {
	}

    public void SendBool(string s, bool b)
    {
        networkView.RPC("SendBool_Proxy", uLink.RPCMode.Others, s, b);
    }

    [RPC]
    public void SendBool_Proxy(string s, bool b)
    {
        animator.SetBool(s, b);
    }

    public void SendTrigger(string s)
    {
        networkView.RPC("SendTrigger_Proxy", uLink.RPCMode.Others, s);
    }

    [RPC]
    public void SendTrigger_Proxy(string s)
    {
        animator.ResetTrigger(Registry.Animator.DashForward);
        animator.ResetTrigger(Registry.Animator.Decelerate);
        animator.ResetTrigger(Registry.Animator.QuickAscend);
        animator.ResetTrigger(Registry.Animator.DashRight);
        animator.ResetTrigger(Registry.Animator.DashLeft);
        animator.ResetTrigger(Registry.Animator.DashUp);
        animator.ResetTrigger(Registry.Animator.DashDown);

        animator.SetTrigger(s);
    }

    public void SendFloat(string s, float f)
    {
        networkView.RPC("SendFloat_Proxy", uLink.RPCMode.Others, s, f);
    }

    [RPC]
    public void SendFloat_Proxy(string s, float f)
    {
        animator.SetFloat(s, f);
    }

    public void ToggleRenderer(bool b)
    {
        networkView.RPC("ToggleRenderer_Proxy", uLink.RPCMode.Others, b);
    }

    [RPC]
    public void ToggleRenderer_Proxy(bool b)
    {
        foreach (Renderer renderer in this.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = b;
        }
    }

    public void SpawnPrefab(string prefab, Vector3 position, Quaternion rotation)
    {
        networkView.RPC("SpawnPrefab_Proxy", uLink.RPCMode.Others, prefab, position, rotation);
    }

    [RPC]
    public void SpawnPrefab_Proxy(string prefab, Vector3 position, Quaternion rotation)
    {
        GameObject.Instantiate(Resources.Load(prefab), position, rotation);
    }

	[RPC]
	public void Die()
	{
		animator.SetTrigger(Registry.Animator.Die);
	}

    public void PushBack(Vector3 v)
    {
        networkView.RPC("PushBack_Proxy", uLink.RPCMode.Others, v);
    }

    [RPC]
    public void PushBack_Proxy(Vector3 v)
    {
        Debug.Log(v);
        Collider[] colliders = Physics.OverlapSphere(v, 15f);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].name == Registry.Prefab.FireballMini + "(Clone)"
                || colliders[i].name == Registry.Prefab.Fireball + "(Clone)")
            {
                //Debug.Log("Pushing back");
                colliders[i].transform.position = new Vector3(-2392f, -874f, -232f);
            }
        }
    }
}
