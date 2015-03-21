using UnityEngine;
using System.Collections.Generic;

public class ResourceFactory
{
	private static ResourceFactory instance;

	private Dictionary<string, GameObject> resources;

	private ResourceFactory()
	{
		resources = new Dictionary<string, GameObject>();
	}

	public static ResourceFactory GetInstance()
	{
		if(instance == null)
		{
			instance = new ResourceFactory();
		}
		return instance;
	}

	public GameObject GetPrefab(string prefab)
	{
		if(!resources.ContainsKey(prefab))
		{
			resources.Add(prefab, Resources.Load(prefab) as GameObject);
		}
		return resources[prefab];
	}
}
