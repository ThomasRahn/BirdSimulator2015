using UnityEngine;
using System.Collections;

public class AiInput : MonoBehaviour
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
        //animator.SetFloat("Horizontal", -1);
	}

    public void DoAction(string s)
    {
        if (s == "Dive")
        {
            StartCoroutine(coDive());
        }
        else if (s == "Tutorial_Land_L")
        {
            StartCoroutine(coLand(-1));
        }
        else if (s == "Tutorial_Land_R")
        {
            StartCoroutine(coLand(1));
        }
        else if (s == "RoundRoom_Dive")
        {
            StartCoroutine(coRoundRoomDive());
        }
    }


    IEnumerator coDive()
    {
        this.GetComponent<PlayerState>().SetTargetVelocity(this.transform.forward * 15f);

        yield return new WaitForSeconds(3f);
        animator.SetFloat("Vertical", 1);
        yield return new WaitForSeconds(3f);
        animator.SetFloat("Vertical", 0);
    }

    IEnumerator coLand(int i)
    {
        this.GetComponent<PlayerState>().SetTargetVelocity(this.transform.forward * 10f);
        this.GetComponent<PlayerState>().SetCurrentMaxSpeed(10f);

        animator.SetFloat("Vertical", 0.2f);
        yield return new WaitForSeconds(0.5f);
        animator.SetFloat("Vertical", 0f);
        yield return new WaitForSeconds(3f);

        // turn left or right towards landing zone
        animator.SetFloat("Horizontal", i);
        yield return new WaitForSeconds(1.1f);
        // stop turning
        animator.SetFloat("Horizontal", 0f);

        // go forward a bit
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("t_Land");
        yield return new WaitForSeconds(5f);
    }

    IEnumerator coRoundRoomDive()
    {
        this.GetComponent<PlayerState>().SetTargetVelocity(this.transform.forward * 20f);
        this.GetComponent<PlayerState>().SetCurrentMaxSpeed(20f);
        animator.SetFloat("Vertical", 0.2f);
        yield return new WaitForSeconds(0.1f);
        animator.SetFloat("Vertical", 0f);

        yield return new WaitForSeconds(1f);
        animator.SetFloat("Vertical", 1);
        yield return new WaitForSeconds(3f);

        GameObject.Destroy(this.gameObject, 3f);
    }
}
