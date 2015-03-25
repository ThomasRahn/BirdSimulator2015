using UnityEngine;
using System.Collections;

public abstract class BaseTriggerable : uLink.MonoBehaviour
{
    public virtual void Trigger(Collider c, GameObject g)
    {
    }
}

public class BaseTriggerable<T> : BaseTriggerable
{
}
