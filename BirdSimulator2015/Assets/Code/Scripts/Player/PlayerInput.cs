﻿using UnityEngine;
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
        animator.ResetTrigger("t_Tornado");
        animator.ResetTrigger("t_Flash");

        if (Locked)
            return;

        if (GameController.Gamepad.GetGamepadType() == GamepadSetup.GamepadType.XBOX360)
        {
            cameraMultiplier = 7f;

            JoystickAxisX = Input.GetAxis("JoystickAxisX"); // left thumbstick horizontal
            JoystickAxisY = Input.GetAxis("JoystickAxisY"); // left thumbstick vertical

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
            JoystickAxis4 = Input.GetAxis("Mouse X");
            JoystickAxis5 = Input.GetAxis("Mouse Y");

            JoystickButton0 = Input.GetButton("KeyboardX");
            JoystickButton1 = Input.GetButton("KeyboardF");
            JoystickButton2 = Input.GetButton("KeyboardE");
            JoystickButton3 = Input.GetButton("KeyboardQ");
            JoystickButton5 = Input.GetButton("KeyboardShift");
        }

        animator.SetFloat("Horizontal", JoystickAxisX);
        animator.SetFloat("Vertical", JoystickAxisY * GameController.Gamepad.Inverted);

        if (JoystickAxisX != 0 || JoystickAxisY != 0 || JoystickButton5)
		{
			Cameras.Radial(true);
		}
        else if (JoystickAxis4 != 0 || JoystickAxis5 != 0)
		{
			Cameras.Radial(false);
            Cameras.Input(JoystickAxis4 * cameraMultiplier, JoystickAxis5 * cameraMultiplier);
		}

        if (JoystickButton2)
        {
            this.GetComponent<PlayerSync>().SendTrigger("t_DashForward");
            animator.SetTrigger("t_DashForward");
        }

        if (JoystickButton0)
        {
            if (this.GetComponent<PlayerState>().LandTarget != null)
            {
                if (this.GetComponent<PlayerState>().GetState() == PlayerState.BirdState.Grounded)
                {
                    animator.SetBool("b_Grounded", false);
                }
                else
                {
                    this.GetComponent<PlayerSync>().SendTrigger("t_Land");
                    animator.SetTrigger("t_Land");
                }
            }
            else
            {
                if (GameController.IsWhite)
                {
                    animator.SetTrigger("t_Flash");
                }
                else
                {
                    animator.SetTrigger("t_Tornado");
                }
            }
        }

        if (JoystickButton1)
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

        if (JoystickButton4)
        {
            GameController.LocatorPopup.ResetFade();
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
        this.GetComponent<PlayerSync>().SendTrigger(s);
		animator.SetTrigger(s);
	}

    public void SetBool(string s, bool b)
    {
        this.GetComponent<PlayerSync>().SendBool(s, b);
        animator.SetBool(s, b);
    }

    public float GetAxisHorizontal()
    {
        return JoystickAxisX;
    }

    public float GetAxisVertical()
    {
        return JoystickAxisY;
    }

    private void DoXBOXWin()
    {
        JoystickAxis3 = Input.GetAxis("JoystickAxis3"); // right and left trigger
        JoystickAxis4 = Input.GetAxis("JoystickAxis4"); // right thumbstick horizontal
        JoystickAxis5 = Input.GetAxis("JoystickAxis5"); // right thumbstick vertical

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
