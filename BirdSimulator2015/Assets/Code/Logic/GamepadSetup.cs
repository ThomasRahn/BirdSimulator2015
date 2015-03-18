using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamepadSetup
{
    public enum GamepadType
    {
        KEYBOARD,
        XBOX360,
        PLAYSTATION3,
        LOGITECHF310,

    }

    public enum GamepadAction
    {
        A,
        B,
        X,
        Y,
		BUMPER_L,
		BUMPER_R,

    }

    GamepadType type;
    private Dictionary<GamepadAction, Sprite> lookup = new Dictionary<GamepadAction, Sprite>(); 

    public GamepadSetup(GamepadType type)
    {
        this.type = type;

        Sprite s;
        if (type == GamepadType.LOGITECHF310 || type == GamepadType.XBOX360)
        {
            s = Resources.Load<Sprite>("UI/Gamepad/Gamepad_XBOX360_A");
            lookup.Add(GamepadAction.A, s);
            s = Resources.Load<Sprite>("UI/Gamepad/Gamepad_XBOX360_B");
            lookup.Add(GamepadAction.B, s);
            s = Resources.Load<Sprite>("UI/Gamepad/Gamepad_XBOX360_X");
            lookup.Add(GamepadAction.X, s);
            s = Resources.Load<Sprite>("UI/Gamepad/Gamepad_XBOX360_Y");
            lookup.Add(GamepadAction.Y, s);
            s = Resources.Load<Sprite>("UI/Gamepad/Gamepad_XBOX360_LB");
			lookup.Add(GamepadAction.BUMPER_L, s);
            s = Resources.Load<Sprite>("UI/Gamepad/Gamepad_XBOX360_RB");
			lookup.Add(GamepadAction.BUMPER_R, s);
        }
        else if (type == GamepadType.KEYBOARD)
        {
            s = Resources.Load<Sprite>("UI/Gamepad/Gamepad_F310_A");
            lookup.Add(GamepadAction.A, s);
            s = Resources.Load<Sprite>("UI/Gamepad/Gamepad_F310_B");
            lookup.Add(GamepadAction.B, s);
            s = Resources.Load<Sprite>("UI/Gamepad/Gamepad_F310_X");
            lookup.Add(GamepadAction.X, s);
            s = Resources.Load<Sprite>("UI/Gamepad/Gamepad_F310_Y");
            lookup.Add(GamepadAction.Y, s);
        }
    }

    public Sprite GetSprite(GamepadAction ia)
    {
        Sprite s;
        lookup.TryGetValue(ia, out s);
        return s;
    }

    public GamepadType GetGamepadType()
    {
        return type;
    }
}
