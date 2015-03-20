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

	public GameObject GetChainLink()
	{
		if(!resources.ContainsKey(Registry.Prefab.ChainLink))
		{
			resources.Add(Registry.Prefab.ChainLink, Resources.Load(Registry.Prefab.ChainLink) as GameObject);
		}
		return resources[Registry.Prefab.ChainLink];
	}

	public GameObject GetTrapTrigger()
	{
		if(!resources.ContainsKey(Registry.Prefab.TrapTrigger))
		{
			resources.Add(Registry.Prefab.TrapTrigger, Resources.Load(Registry.Prefab.TrapTrigger) as GameObject);
		}
		return resources[Registry.Prefab.TrapTrigger];
	}

    public GameObject GetTether()
    {
        if (!resources.ContainsKey(Registry.Prefab.Tether))
        {
            resources.Add(Registry.Prefab.Tether, Resources.Load(Registry.Prefab.Tether) as GameObject);
        }
        return resources[Registry.Prefab.Tether];
    }
}
