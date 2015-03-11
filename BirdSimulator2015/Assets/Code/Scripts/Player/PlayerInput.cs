using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BS2015.Code.Scripts.Player;

public class PlayerInput : MonoBehaviour
{
    protected float JoystickAxisX = 0f;
    protected float JoystickAxisY = 0f;
    protected float JoystickAxis3 = 0f;
    protected float JoystickAxis4 = 0f;
    protected float JoystickAxis5 = 0f;
    protected float JoystickAxis6 = 0f;
    protected float JoystickAxis7 = 0f;
    protected float JoystickAxis8 = 0f;

    protected bool JoystickButton0 = false;
    protected bool JoystickButton1 = false;
    protected bool JoystickButton2 = false;
    protected bool JoystickButton3 = false;
    protected bool JoystickButton4 = false;
    protected bool JoystickButton5 = false;
    protected bool JoystickButton6 = false;
    protected bool JoystickButton7 = false;
    protected bool JoystickButton8 = false;
    protected bool JoystickButton9 = false;
    protected bool JoystickButton10 = false;
    protected bool JoystickButton11 = false;
    protected bool JoystickButton12 = false;

    private Animator animator;
    private float invertY = -1f;

    private const float JOYSTICK_ALT_THUMBSTICK_THRESHOLD = 0.2f;

    void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        JoystickAxisX = Input.GetAxisRaw("JoystickAxisX");
        JoystickAxisY = Input.GetAxisRaw("JoystickAxisY");
        JoystickAxis3 = Input.GetAxisRaw("JoystickAxis3");
        JoystickAxis4 = Input.GetAxisRaw("JoystickAxis4");
        JoystickAxis5 = Input.GetAxisRaw("JoystickAxis5");
        JoystickAxis6 = Input.GetAxisRaw("JoystickAxis6");
        JoystickAxis7 = Input.GetAxisRaw("JoystickAxis7");
        JoystickAxis8 = Input.GetAxisRaw("JoystickAxis8");

        JoystickButton0 = Input.GetButton("JoystickButton0");
        JoystickButton1 = Input.GetButton("JoystickButton1");
        JoystickButton2 = Input.GetButton("JoystickButton2");
        JoystickButton3 = Input.GetButton("JoystickButton3");
        JoystickButton4 = Input.GetButton("JoystickButton4");
        JoystickButton5 = Input.GetButton("JoystickButton5");
        JoystickButton6 = Input.GetButton("JoystickButton6");
		JoystickButton7 = Input.GetKeyDown (KeyCode.T);//Input.GetButtonDown("JoystickButton7");
        JoystickButton8 = Input.GetButton("JoystickButton8");
        JoystickButton9 = Input.GetButton("JoystickButton9");
        JoystickButton10 = Input.GetButton("JoystickButton10");
        JoystickButton11 = Input.GetButton("JoystickButton11");
        JoystickButton12 = Input.GetButton("JoystickButton12");

        animator.ResetTrigger("t_DashForward");
        animator.ResetTrigger("t_Decelerate");
        animator.ResetTrigger("t_QuickAscend");
        animator.ResetTrigger("t_DashRight");
        animator.ResetTrigger("t_DashLeft");
        animator.ResetTrigger("t_DashUp");
        animator.ResetTrigger("t_DashDown");

        if (GameController.Gamepad.GetGamepadType() == GamepadSetup.GamepadType.LOGITECHF310)
        {
            animator.SetFloat("Horizontal", JoystickAxisX);
            animator.SetFloat("Vertical", JoystickAxisY * invertY);

			if(GameController.isWhite)
			{
				animator.SetBool("Taunt", JoystickButton7);
				if(JoystickButton7)
				{
					this.GetComponent<PlayerSkills>().Taunt();
				}

	            if (JoystickButton0)
	            {
	                this.GetComponent<PlayerSync>().SendTrigger("t_DashForward");
	                animator.SetTrigger("t_DashForward");
	            }
			}

            if (JoystickButton1)
            {
                // knees weak moms spaghetti
                if (this.GetComponent<PlayerState>().CanLand)
                {
                    if (this.GetComponent<PlayerState>().GetState() == PlayerState.BirdState.Grounded)
                    {
                        // make some sort of liftoff animation or something
                        animator.SetBool("b_Grounded", false);
                    }
                    else
                    {
                        this.GetComponent<PlayerSync>().SendTrigger("t_Land");

                        animator.SetTrigger("t_Land");
                    }
                }
            }

            // limit this, obviously
            if (JoystickButton2)
            {
                this.GetComponent<PlayerSync>().SendTrigger("t_Decelerate");
                animator.SetTrigger("t_Decelerate");
            }

			if (JoystickButton3)
            {
                this.GetComponent<PlayerSync>().SendTrigger("t_QuickAscend");
                animator.SetTrigger("t_QuickAscend");
            }
			
            if (JoystickAxis3 > JOYSTICK_ALT_THUMBSTICK_THRESHOLD)
                animator.SetTrigger("t_DashRight");
            if (JoystickAxis3 < -JOYSTICK_ALT_THUMBSTICK_THRESHOLD)
                animator.SetTrigger("t_DashLeft");
            if (JoystickAxis4 > JOYSTICK_ALT_THUMBSTICK_THRESHOLD)
                animator.SetTrigger("t_DashUp");
            if (JoystickAxis4 < -JOYSTICK_ALT_THUMBSTICK_THRESHOLD)
                animator.SetTrigger("t_DashDown");
        }
    }
}
