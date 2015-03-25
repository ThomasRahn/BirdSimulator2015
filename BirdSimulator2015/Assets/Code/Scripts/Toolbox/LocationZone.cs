using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocationZone : MonoBehaviour
{
    public LocationPopup.Location Location;

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
            GameController.LocationPopup.SetText(Location);
			GameController.LocationPopup.Popup();
		}
	}
}
