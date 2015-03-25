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
	private PlayerState state;
    private float cameraMultiplier = 1f;

    private float _boostTimer = 1.5f;
    private float boostTimer;
    private float _ascendTimer = 1.2f;
    private float ascendTimer;
    private float _skillTimer = 2f;
    private float skillTimer;

    void Awake()
    {
		state = this.GetComponent<PlayerState>();
        animator = this.GetComponent<Animator>();
    }

	void Start()
	{
        boostTimer = _boostTimer;
        ascendTimer = _ascendTimer;
        skillTimer = _skillTimer;
	}

    void Update()
    {
        animator.ResetTrigger(Registry.Animator.DashForward);
        animator.ResetTrigger(Registry.Animator.Decelerate);
        animator.ResetTrigger(Registry.Animator.QuickAscend);
        animator.ResetTrigger(Registry.Animator.DashRight);
        animator.ResetTrigger(Registry.Animator.DashLeft);
        animator.ResetTrigger(Registry.Animator.DashUp);
        animator.ResetTrigger(Registry.Animator.DashDown);
        animator.ResetTrigger(Registry.Animator.Tornado);
        animator.ResetTrigger(Registry.Animator.Flash);

        JoystickAxisX = 0f;
        JoystickAxisY = 0f;

        if (Locked)
            return;

        if (GameController.Gamepad.GetGamepadType() == GamepadSetup.GamepadType.XBOX360)
        {
            cameraMultiplier = 7f;

            JoystickAxisX = Input.GetAxis("JoystickAxisX"); // left thumbstick horizontal
            JoystickAxisY = Input.GetAxis("JoystickAxisY"); // left thumbstick vertical

            if (state.GetState() == PlayerState.BirdState.SpeedyMode)
                return;

#if UNITY_EDITOR_WIN
            DoXBOXWin();
#endif
#if UNITY_STANDALONE_WIN
            DoXBOXWin();
#endif
#if UNITY_EDITOR_OSX
            DoXBOXMac();
#endif
#if UNITY_STANDALONE_OSX
            DoXBOXMac();
#endif
        }
        else if (GameController.Gamepad.GetGamepadType() == GamepadSetup.GamepadType.KEYBOARD)
        {
            JoystickAxisX = Input.GetAxis("KeyboardAxisX");
            JoystickAxisY = Input.GetAxis("KeyboardAxisY");

            if (state.GetState() == PlayerState.BirdState.SpeedyMode)
                return;

            JoystickAxis4 = Input.GetAxis("Mouse X");
            JoystickAxis5 = Input.GetAxis("Mouse Y");
			JoystickAxis6 = Input.GetAxis("KeyboardAxisJL"); // d-pad horizontal
			JoystickAxis7 = Input.GetAxis("KeyboardAxisIK"); // d-pad vertical

            JoystickButton0 = Input.GetButton("KeyboardX");
            JoystickButton1 = Input.GetButton("KeyboardF");
            JoystickButton2 = Input.GetButton("KeyboardE");
            JoystickButton3 = Input.GetButton("KeyboardQ");
            JoystickButton5 = Input.GetButton("KeyboardShift");
        }

        animator.SetFloat("Horizontal", JoystickAxisX);
        animator.SetFloat("Vertical", JoystickAxisY * GameController.Gamepad.Inverted);
		animator.SetFloat(Registry.Animator.Horizontal2, JoystickAxis6);
		animator.SetFloat(Registry.Animator.Vertical2, JoystickAxis7);

        // cooldowns
        if (boostTimer < _boostTimer)
        {
            boostTimer -= Time.deltaTime;

            if (boostTimer < 0)
                boostTimer = _boostTimer;
        }
        if (ascendTimer < _ascendTimer)
        {
            ascendTimer -= Time.deltaTime;

            if (ascendTimer < 0)
                ascendTimer = _ascendTimer;
        }
        if (skillTimer < _skillTimer)
        {
            skillTimer -= Time.deltaTime;

            if (skillTimer < 0)
                skillTimer = _skillTimer;
        }

		if(state.GetState() == PlayerState.BirdState.Hovering
		   || state.GetState() == PlayerState.BirdState.HoveringAscend
		   || state.GetState() == PlayerState.BirdState.HoveringDescend
		   || state.GetState() == PlayerState.BirdState.HoveringStrafe)
		{
			Cameras.Switch(CameraContainer.Type.RADIAL); // Always use radial in hovering state
		}
		else
		{
			if (JoystickAxisX != 0 || JoystickAxisY != 0 || JoystickButton5)
			{
				Cameras.Switch(CameraContainer.Type.RADIAL);
			}
			else if (JoystickAxis4 != 0 || JoystickAxis5 != 0)
			{
				Cameras.Switch(CameraContainer.Type.FREE);
				Cameras.Input(JoystickAxis4 * cameraMultiplier, JoystickAxis5 * cameraMultiplier);
			}
		}

        if (JoystickButton2
            & state.GetState() != PlayerState.BirdState.Diving
		    & state.GetState() != PlayerState.BirdState.Grounded
		    & state.GetState() != PlayerState.BirdState.LiftingOff
            & boostTimer == _boostTimer)
        {
            boostTimer -= Time.deltaTime;
            this.GetComponent<PlayerSync>().SendTrigger(Registry.Animator.DashForward);
            animator.SetTrigger(Registry.Animator.DashForward);
        }

        if (JoystickButton0)
        {
			if (state.LandTarget != null)
            {
				if (state.GetState() == PlayerState.BirdState.Grounded)
                {
                    this.GetComponent<PlayerSync>().SendBool(Registry.Animator.Grounded, false);
                    animator.SetBool(Registry.Animator.Grounded, false);
                }
                else
                {
                    this.GetComponent<PlayerSync>().SendTrigger(Registry.Animator.Land);
                    animator.SetTrigger(Registry.Animator.Land);
                }
            }
            else
            {
				if (state.GetState() == PlayerState.BirdState.Hovering
				    || state.GetState() == PlayerState.BirdState.Gliding)
                {
                    if (skillTimer == _skillTimer)
                    {
                        skillTimer -= Time.deltaTime;
                        if (GameController.IsWhite)
                        {
                            this.GetComponent<PlayerSync>().SendTrigger(Registry.Animator.Flash);
                            animator.SetTrigger(Registry.Animator.Flash);
                        }
                        else
                        {
                            this.GetComponent<PlayerSync>().SendTrigger(Registry.Animator.Tornado);
                            animator.SetTrigger(Registry.Animator.Tornado);
                        }
                    }
                }
            }
        }

        if (JoystickButton1)
        {
            boostTimer = _boostTimer;
            this.GetComponent<PlayerSync>().SendBool(Registry.Animator.Decelerating, true);
			animator.SetBool(Registry.Animator.Decelerating, true);
        }
		else
		{
			this.GetComponent<PlayerSync>().SendBool(Registry.Animator.Decelerating, false);
			animator.SetBool(Registry.Animator.Decelerating, false);
		}

        if (JoystickButton3
		    & state.GetState() != PlayerState.BirdState.QuickAscending
            & ascendTimer == _ascendTimer)
		{
            ascendTimer -= Time.deltaTime;
			this.GetComponent<PlayerSync>().SendTrigger(Registry.Animator.QuickAscend);
            animator.SetTrigger(Registry.Animator.QuickAscend);
        }

        if (JoystickButton4)
        {
            GameController.LocatorPopup.ResetFade();
        }

		if (JoystickButton5)
		{
            this.GetComponent<PlayerSync>().SendBool(Registry.Animator.Diving, true);
            animator.SetBool(Registry.Animator.Diving, true);
		}
		else
		{
            this.GetComponent<PlayerSync>().SendBool(Registry.Animator.Diving, false);
			animator.SetBool(Registry.Animator.Diving, false);
		}

        if (JoystickButton8)
        {
			if (state.GetState() == PlayerState.BirdState.Hovering
			    || state.GetState() == PlayerState.BirdState.Gliding)
            {
                animator.SetTrigger(Registry.Animator.AboutFace);
            }
        }
	}

	public void SetTrigger(string s)
	{
        this.GetComponent<PlayerSync>().SendTrigger(s);
		animator.SetTrigger(s);
	}

    public void SetBool(string s, bool b)
    {
        this.GetComponent<PlayerSync>().SendBool(s, b);
        animator.SetBool(s, b);
    }

    public float GetLeftStickHorizontal() { return JoystickAxisX; }

    public float GetLeftStickVertical() { return JoystickAxisY; }

	public float GetDPadHorizontal() { return JoystickAxis6; }

	public float GetDPadVertical() { return JoystickAxis7; }

	public float GetRightStickHorizontal() { return JoystickAxis4; }

	public float GetRightStickVertical() { return JoystickAxis5; }

    private void DoXBOXWin()
    {
        JoystickAxis3 = Input.GetAxis("JoystickAxis3"); // right and left trigger
        JoystickAxis4 = Input.GetAxis("JoystickAxis4"); // right thumbstick horizontal
        JoystickAxis5 = Input.GetAxis("JoystickAxis5"); // right thumbstick vertical
		JoystickAxis6 = Input.GetAxis("JoystickAxis6"); // d-pad horizontal
		JoystickAxis7 = Input.GetAxis("JoystickAxis7"); // d-pad vertical

        JoystickButton0 = Input.GetButton("JoystickButton0"); // bottom button
        JoystickButton1 = Input.GetButton("JoystickButton1"); // right button
        JoystickButton2 = Input.GetButton("JoystickButton2"); // left button
        JoystickButton3 = Input.GetButton("JoystickButton3"); // top button

        JoystickButton4 = Input.GetButton("JoystickButton4"); // left bumper
        JoystickButton5 = Input.GetButton("JoystickButton5"); // right bumper
        JoystickButton6 = Input.GetButton("JoystickButton6"); // back
        JoystickButton7 = Input.GetButton("JoystickButton7"); // start

        JoystickButton8 = Input.GetButton("JoystickButton8"); // left thumbstick in
        JoystickButton9 = Input.GetButton("JoystickButton9"); // right thumbstick in
    }

    private void DoXBOXMac()
    {
        JoystickAxis4 = Input.GetAxis("JoystickAxis3"); // right thumbstick horizontal
        JoystickAxis5 = Input.GetAxis("JoystickAxis4"); // right thumbstick vertical
		JoystickAxis6 = (Input.GetButton("JoystickButton8") ? 1 : 0) + (Input.GetButton("JoystickButton7") ? -1 : 0); // d-pad horizontal
		JoystickAxis7 = (Input.GetButton("JoystickButton5") ? 1 : 0) + (Input.GetButton("JoystickButton6") ? -1 : 0); // d-pad vertical

        JoystickButton0 = Input.GetButton("JoystickButton16"); // bottom button
        JoystickButton1 = Input.GetButton("JoystickButton17"); // right button
        JoystickButton2 = Input.GetButton("JoystickButton18"); // left button
        JoystickButton3 = Input.GetButton("JoystickButton19"); // top button

        JoystickButton4 = Input.GetButton("JoystickButton13"); // left bumper
        JoystickButton5 = Input.GetButton("JoystickButton14"); // right bumper
        JoystickButton6 = Input.GetButton("JoystickButton10"); // back
        JoystickButton7 = Input.GetButton("JoystickButton9"); // start

        JoystickButton8 = Input.GetButton("JoystickButton11"); // left thumbstick in
        JoystickButton9 = Input.GetButton("JoystickButton12"); // right thumbstick in
    }
}
