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

    }

    GamepadType type;
    private Dictionary<GamepadAction, Sprite> lookup = new Dictionary<GamepadAction, Sprite>(); 

    public GamepadSetup(GamepadType type)
    {
        this.type = type;

        Sprite s;
        if (type == GamepadType.LOGITECHF310)
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
