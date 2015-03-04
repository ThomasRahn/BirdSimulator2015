using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
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

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "LandingZone")
        {
            GameController.GamepadPopup.SetImage(GamepadSetup.ImageAction.A);
            GameController.GamepadPopup.FadeIn();
            // TODO refactor into base later (important!!!!!!!!!!!!!!!!!!!!)
            this.GetComponent<PlayerState>().LandPos = other.GetComponent<LandingZone>().Target.position;
            this.GetComponent<PlayerState>().CanLand = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "LandingZone")
        {
            GameController.GamepadPopup.FadeOut();
            this.GetComponent<PlayerState>().CanLand = false;
        }
    }
}
