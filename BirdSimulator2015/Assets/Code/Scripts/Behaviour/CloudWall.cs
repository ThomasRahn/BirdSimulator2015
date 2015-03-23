using UnityEngine;
using System.Collections;

public class CloudWall : MonoBehaviour 
{
	public int level = 0;

	public void PushBack()
	{
		if(level >= 5)
		{
			GameObject.Destroy(this.gameObject);
		}

        if (level % 2 == 0)
            StartCoroutine(coPushBack(this.transform.localPosition));

        level++;
	}

    IEnumerator coPushBack(Vector3 orig)
    {
        float z = orig.z;
        Debug.Log(Mathf.Abs(z - this.transform.localPosition.z));
        while (Mathf.Abs(z - this.transform.localPosition.z) < 15f)
        {
            Debug.Log(Mathf.Abs(z - this.transform.localPosition.z));
            this.transform.Translate(-this.transform.forward * Time.deltaTime * 2f);
            yield return null;
        }
    }
}
