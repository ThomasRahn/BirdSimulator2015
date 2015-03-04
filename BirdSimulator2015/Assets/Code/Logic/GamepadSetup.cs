using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamepadSetup
{
    public enum Type
    {
        KEYBOARD,
        XBOX360,
        PLAYSTATION3,
        LOGITECHF310,

    }

    public enum ImageAction
    {
        A,
        B,
        X,
        Y,

    }

    Type type;
    private Dictionary<ImageAction, Sprite> lookup = new Dictionary<ImageAction, Sprite>(); 

    public GamepadSetup(Type type)
    {
        this.type = type;

        Sprite s;
        if (type == Type.LOGITECHF310)
        {
            s = Resources.Load<Sprite>("UI/Gamepad/Gamepad_F310_A");
            lookup.Add(ImageAction.A, s);

            // TODO add some more contexts here
        }
    }

    public Sprite GetSprite(ImageAction ia)
    {
        Sprite s;
        lookup.TryGetValue(ia, out s);
        return s;
    }

    public Type GetType()
    {
        return type;
    }
}
