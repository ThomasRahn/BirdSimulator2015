using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

[ExecuteInEditMode]
public class NestedPrefab : MonoBehaviour
{
	public GameObject Prefab;
	
	void Start()
	{
        #if UNITY_EDITOR
		if (EditorApplication.isPlaying)
		{
			foreach (NestedPrefab np in UnityEngine.Object.FindObjectsOfType(typeof(NestedPrefab)))
			{
				Bake(np);
			}
		}
        #endif
	}
	
#if UNITY_EDITOR
	[System.NonSerializedAttribute]
	public List<Components> components = new List<Components>();
			
	public struct Components
	{
		public Mesh mesh;
		public Matrix4x4 matrix;
		public List<Material> materials;
	}

	void OnValidate()
	{
		components.Clear();
		if (enabled)
		{
			Rebuild(Prefab, Matrix4x4.identity);
		}
	}
	
	void OnEnable()
	{
		components.Clear();
		if (enabled)
		{
			Rebuild(Prefab, Matrix4x4.identity);
		}
	}
	
	void Rebuild(GameObject go, Matrix4x4 m)
	{
		if (!go)
		{
			return;
		}
		
		Matrix4x4 matrix = m * Matrix4x4.TRS(-go.transform.position, Quaternion.identity, Vector3.one);
		
		foreach (Renderer renderer in go.GetComponentsInChildren(typeof(Renderer), true))
		{
			components.Add(
				new Components()
				{
					//mesh = renderer.GetComponent<MeshFilter>().sharedMesh,
					mesh = (renderer.GetComponent<MeshFilter>() ? renderer.GetComponent<MeshFilter>().sharedMesh : null),
					matrix = matrix * renderer.transform.localToWorldMatrix,
					materials = new List<Material>(renderer.sharedMaterials)
				}
			);
		}
		
		foreach (NestedPrefab np in go.GetComponentsInChildren(typeof(NestedPrefab), true))
		{
			if (np.enabled && np.gameObject.activeSelf)
			{
				Rebuild(np.Prefab, matrix * np.transform.localToWorldMatrix);
			}
		}		
	}
	
	void Update()
	{
		if (EditorApplication.isPlaying)
		{
			return;
		}
		
		Matrix4x4 matrix = transform.localToWorldMatrix;
		foreach (Components c in components)
		{
			for (int i = 0; i < c.materials.Count; i++)
			{
				Graphics.DrawMesh (c.mesh, matrix * c.matrix, c.materials[i], gameObject.layer, null, i);
			}
		}
	}
	
	void OnDrawGizmos()
	{
		DrawGizmos(new Color(0, 0, 0, 0));
	}
	
	void OnDrawGizmosSelected()
	{
		DrawGizmos(new Color(0, 0, 1f, 0.2f));
	}
	
	void DrawGizmos(Color l)
	{
		if (EditorApplication.isPlaying)
		{
			return;
		}
		
		Gizmos.color = l;
		Matrix4x4 matrix = transform.localToWorldMatrix;
		foreach (Components c in components)
		{
			Gizmos.matrix = matrix * c.matrix;
			if (c.mesh)
				Gizmos.DrawCube(c.mesh.bounds.center, c.mesh.bounds.size);
		}		
	}
	
	[PostProcessScene(-2)]
	public static void OnPostprocessScene()
	{ 
		foreach (NestedPrefab np in UnityEngine.Object.FindObjectsOfType(typeof(NestedPrefab)))
			Bake(np);
	}

#endif

    public static void Bake(NestedPrefab np)
	{
		if (!np.Prefab || !np.enabled)
		{
			return;
		}
		
		np.enabled = false;
		//GameObject go = PrefabUtility.InstantiatePrefab(np.Prefab) as GameObject;
        GameObject go = GameObject.Instantiate(np.Prefab) as GameObject;
		Quaternion rot = go.transform.localRotation;
		Vector3 scale = go.transform.localScale;
		go.transform.parent = np.transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = scale;
		go.transform.localRotation = rot;
		np.Prefab = null;
		foreach (NestedPrefab child in go.GetComponentsInChildren<NestedPrefab>())
		{
			Bake(child);
		}
	}
}
