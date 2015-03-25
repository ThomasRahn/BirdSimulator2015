using UnityEngine;
using System.Collections;

public class CanyonBuilder : MonoBehaviour {

    public GameObject[] rocks;
    public int Wide = 0;
    public int Tall = 0;

    public float WideStep = 0f;
    public float TallStep = 0f;
    public float DepthStep = 0f;

    public float Indentation = 0f;

	void Start()
    {
	    for (int i = 0; i < Wide; i++)
        {
            for (int j = 0; j < Tall; j++)
            {
                int r = Random.Range(0, rocks.Length);
                GameObject g = GameObject.Instantiate(rocks[r]) as GameObject;
                Vector3 v = Vector3.zero;
                v.x = -(WideStep * Wide / 2) + (i * WideStep);
                if (j % 2 == 0)
                    v.x += Indentation;
                v.y = -(TallStep * Tall / 2) + (j * TallStep);
                v.z = j * DepthStep;
                g.transform.SetParent(this.transform);
                g.transform.rotation = this.transform.rotation;
                g.transform.localScale = new Vector3(2f, 2f, 2f);
                g.transform.localPosition = v;
            }
        }
	}
	
	void Update()
    {
	}

    void OnDrawGizmos()
    {
        DrawGizmos(new Color(0, 0, 0, 0.5f));
    }

    void OnDrawGizmosSelected()
    {
        DrawGizmos(new Color(0, 0, 1f));
    }

    void DrawGizmos(Color l)
    {
        Gizmos.color = l;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, new Vector3(Wide * WideStep, Tall * TallStep, 50f));
    }
}
