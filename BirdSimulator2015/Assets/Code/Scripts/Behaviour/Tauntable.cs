using UnityEngine;

public abstract class Tauntable : MonoBehaviour{
	
	/// <summary>
	/// Enemy will do something based on the players taunt
	/// </summary>
	public abstract void Taunted (GameObject player);
}