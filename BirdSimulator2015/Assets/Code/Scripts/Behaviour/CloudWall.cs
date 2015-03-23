using UnityEngine;
using System.Collections;

public class CloudWall : MonoBehaviour 
{
	public int level = 1;

	public void PushBack()
	{
		if(level >= 6)
		{
			GameObject.Destroy(this.gameObject);
		}

        StartCoroutine(coPushBack(this.transform.position));
        level++;
	}

    IEnumerator coPushBack(Vector3 orig)
    {
        while (Vector3.Distance(this.transform.position, orig) < 15f)
        {
            this.transform.position += new Vector3(0, 0, -15.0f);
            yield return null;
        }
    }
}
