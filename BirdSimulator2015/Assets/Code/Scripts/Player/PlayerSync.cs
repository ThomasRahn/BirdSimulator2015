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

    [RPC]
    public void SendTrigger(string s)
    {
        //Debug.Log("Send " + s);
        networkView.RPC("SendTrigger_Proxy", uLink.RPCMode.Others, s);
    }

    [RPC]
    public void SendTrigger_Proxy(string s)
    {
        animator.ResetTrigger("t_DashForward");
        animator.ResetTrigger("t_Decelerate");
        animator.ResetTrigger("t_QuickAscend");
        animator.ResetTrigger("t_DashRight");
        animator.ResetTrigger("t_DashLeft");
        animator.ResetTrigger("t_DashUp");
        animator.ResetTrigger("t_DashDown");

        //Debug.Log("Receive " + s);
        animator.SetTrigger(s);
    }

    [RPC]
    public void SendFloat(string s, float f)
    {
        //Debug.Log("Send " + s);
        networkView.RPC("SendFloat_Proxy", uLink.RPCMode.Others, s, f);
    }

    [RPC]
    public void SendFloat_Proxy(string s, float f)
    {
        //Debug.Log("Receive " + s);
        animator.SetFloat(s, f);
    }
}
