using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour
{
    [HideInInspector] public bool Locked = false;
	public CameraContainer Cameras { get; set; }

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
    private float cameraMultiplier = 1f;

    void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

	void Start()
	{
	}

    void Update()
    {
        animator.ResetTrigger("t_DashForward");
        animator.ResetTrigger("t_Decelerate");
        animator.ResetTrigger("t_QuickAscend");
        animator.ResetTrigger("t_DashRight");
        animator.ResetTrigger("t_DashLeft");
        animator.ResetTrigger("t_DashUp");
        animator.ResetTrigger("t_DashDown");

        if (GameController.Gamepad.GetGamepadType() == GamepadSetup.GamepadType.LOGITECHF310)
        {
            cameraMultiplier = 5f;

            JoystickAxisX = Input.GetAxisRaw("JoystickAxisX");
            JoystickAxisY = Input.GetAxisRaw("JoystickAxisY");
            JoystickAxis4 = Input.GetAxisRaw("JoystickAxis3");
            JoystickAxis5 = Input.GetAxisRaw("JoystickAxis4");

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

        }
        else if (GameController.Gamepad.GetGamepadType() == GamepadSetup.GamepadType.KEYBOARD)
        {
            JoystickAxisX = Input.GetAxisRaw("KeyboardAxisX");
            JoystickAxisY = Input.GetAxisRaw("KeyboardAxisY");
			JoystickAxis4 = Input.GetAxis("Mouse X");
			JoystickAxis5 = Input.GetAxis("Mouse Y");

            JoystickButton0 = Input.GetButton("KeyboardE");
            JoystickButton1 = Input.GetButton("KeyboardX");
            JoystickButton2 = Input.GetButton("KeyboardF");
            JoystickButton3 = Input.GetButton("KeyboardQ");
        }
        else if (GameController.Gamepad.GetGamepadType() == GamepadSetup.GamepadType.XBOX360)
        {
            cameraMultiplier = 5f;

            JoystickAxisX = Input.GetAxisRaw("JoystickAxisX");
            JoystickAxisY = Input.GetAxisRaw("JoystickAxisY");

#if UNITY_STANDALONE_WIN
            JoystickAxis4 = Input.GetAxisRaw("JoystickAxis4");
            JoystickAxis5 = Input.GetAxisRaw("JoystickAxis5");
            JoystickButton0 = Input.GetButton("JoystickButton0");
            JoystickButton1 = Input.GetButton("JoystickButton1");
            JoystickButton2 = Input.GetButton("JoystickButton2");
            JoystickButton3 = Input.GetButton("JoystickButton3");
#endif
#if UNITY_STANDALONE_OSX
            JoystickAxis4 = Input.GetAxisRaw("JoystickAxis3");
            JoystickAxis5 = Input.GetAxisRaw("JoystickAxis4");
            JoystickButton0 = Input.GetButton("JoystickButton18");
            JoystickButton1 = Input.GetButton("JoystickButton16");
            JoystickButton2 = Input.GetButton("JoystickButton17");
            JoystickButton3 = Input.GetButton("JoystickButton19");
#endif
        }

        if (Locked)
            return;

        animator.SetFloat("Horizontal", JoystickAxisX);
        animator.SetFloat("Vertical", JoystickAxisY * invertY);

        if (JoystickAxisX != 0 || JoystickAxisY != 0 || JoystickButton5)
		{
			Cameras.Radial(true);
		}
		else if(JoystickAxis4 != 0 || JoystickAxis5 != 0)
		{
			Cameras.Radial(false);
            Cameras.Input(JoystickAxis4 * cameraMultiplier, JoystickAxis5 * cameraMultiplier);
		}

        if (JoystickButton0)
        {
            this.GetComponent<PlayerSync>().SendTrigger("t_DashForward");
            animator.SetTrigger("t_DashForward");
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

        if (JoystickButton2)
        {
            this.GetComponent<PlayerSync>().SendBool("b_Decelerating", true);
			animator.SetBool("b_Decelerating", true);
        }
		else
		{
			this.GetComponent<PlayerSync>().SendBool("b_Decelerating", false);
			animator.SetBool("b_Decelerating", false);
		}
		
		if (JoystickButton3)
		{
			this.GetComponent<PlayerSync>().SendTrigger("t_QuickAscend");
            animator.SetTrigger("t_QuickAscend");
        }

		if (JoystickButton5)
		{
            this.GetComponent<PlayerSync>().SendBool("b_Diving", true);
            animator.SetBool("b_Diving", true);
		}
		else
		{
            this.GetComponent<PlayerSync>().SendBool("b_Diving", false);
			animator.SetBool("b_Diving", false);
		}
	}

	public void SetTrigger(string s)
	{
		animator.SetTrigger(s);
	}

    public float GetAxisHorizontal()
    {
        return JoystickAxisX;
    }
}
