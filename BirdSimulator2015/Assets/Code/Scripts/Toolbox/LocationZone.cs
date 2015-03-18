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
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == Registry.Tag.Player)
		{
			GameController.LocationPopup.SetText(Text);
			GameController.LocationPopup.Popup();
		}
	}
}
