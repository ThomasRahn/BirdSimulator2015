using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocationZone : MonoBehaviour
{
    public string Text;

    void Start()
    {
    }

    void Update()
    {
    }

    void OnTriggerEnter(Collider c)
    {
        GameObject g = GameObject.Instantiate(Resources.Load("Prefabs/Toolbox/Res/LocationPopup")) as GameObject;
        g.transform.SetParent(GameController.GetCanvas().transform);
        g.transform.localScale = new Vector3(1f, 1f, 1f);
        g.transform.position = new Vector3(0f, 0f, 0f);
        g.transform.localPosition = new Vector3(0f, 0f, 0f); 
        g.GetComponent<Text>().text = Text;
    }
}
