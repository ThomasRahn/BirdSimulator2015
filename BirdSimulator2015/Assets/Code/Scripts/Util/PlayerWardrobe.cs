using UnityEngine;
using System.Collections;

public class PlayerWardrobe : MonoBehaviour
{
    public Material MaterialFeathersWhite;
    public Material MaterialBodyWhite;
    public Material MaterialFeathersBlack;
    public Material MaterialBodyBlack;

    public Renderer LeftWing;
    public Renderer RightWing;
    public Renderer Body;
    public Renderer Tail;

    public void Start()
    {
        if (uLink.Network.isServer)
        {
            if (this.tag == "Player")
                MisterDressup(true);
            else
                MisterDressup(false);
        }
        else
        {
            if (this.tag == "Player")
                MisterDressup(false);
            else
                MisterDressup(true);
        }
    }

    public void MisterDressup(bool b)
    {
        if (b)
        {
            LeftWing.material = MaterialFeathersWhite;
            RightWing.material = MaterialFeathersWhite;
            Body.material = MaterialBodyWhite;
            Tail.material = MaterialFeathersWhite;
        }
        else
        {
            LeftWing.material = MaterialFeathersBlack;
            RightWing.material = MaterialFeathersBlack;
            Body.material = MaterialBodyBlack;
            Tail.material = MaterialFeathersBlack;
        }
    }
}
