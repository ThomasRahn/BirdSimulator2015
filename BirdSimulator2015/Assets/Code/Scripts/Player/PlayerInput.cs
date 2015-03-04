using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour
{
    protected enum Controller
    {
        KEYBOARD,
        XBOX360,
        PLAYSTATION3,
        LOGITECHF310,

    }

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
    private Controller controller;
    private float invertY = -1f;

    private const float JOYSTICK_ALT_THUMBSTICK_THRESHOLD = 0.2f;

    void Awake()
    {
        animator = this.GetComponent<Animator>();
        controller = Controller.LOGITECHF310;
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
        JoystickButton7 = Input.GetButton("JoystickButton7");
        JoystickButton8 = Input.GetButton("JoystickButton8");
        JoystickButton9 = Input.GetButton("JoystickButton9");
        JoystickButton10 = Input.GetButton("JoystickButton10");
        JoystickButton11 = Input.GetButton("JoystickButton11");
        JoystickButton12 = Input.GetButton("JoystickButton12");

        animator.ResetTrigger("DashForward");
        animator.ResetTrigger("Decelerate");
        animator.ResetTrigger("QuickAscend");
        animator.ResetTrigger("DashRight");
        animator.ResetTrigger("DashLeft");
        animator.ResetTrigger("DashUp");
        animator.ResetTrigger("DashDown");

        if (controller == Controller.LOGITECHF310)
        {
            animator.SetFloat("Horizontal", JoystickAxisX);
            animator.SetFloat("Vertical", JoystickAxisY * invertY);

            if (JoystickButton0)
                animator.SetTrigger("DashForward");

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
                        animator.SetTrigger("t_Land");
                    }
                }
                else
                {
                    animator.SetTrigger("QuickAscend");
                }
            }

            if (JoystickButton2)
                animator.SetTrigger("Decelerate");

            if (JoystickAxis3 > JOYSTICK_ALT_THUMBSTICK_THRESHOLD)
                animator.SetTrigger("DashRight");
            if (JoystickAxis3 < -JOYSTICK_ALT_THUMBSTICK_THRESHOLD)
                animator.SetTrigger("DashLeft");
            if (JoystickAxis4 > JOYSTICK_ALT_THUMBSTICK_THRESHOLD)
                animator.SetTrigger("DashUp");
            if (JoystickAxis4 < -JOYSTICK_ALT_THUMBSTICK_THRESHOLD)
                animator.SetTrigger("DashDown");
        }
    }
}
