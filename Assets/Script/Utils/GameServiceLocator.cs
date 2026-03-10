using System.Collections.Generic;
using UnityEngine;
using System;

public static class GameServices
{
    private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static void Register<TInterface>(TInterface serviceInstance)
    {
        Type interfaceType = typeof(TInterface);
        services[interfaceType] = serviceInstance;
        Debug.Log($"Service {interfaceType.Name} enregistré");
    }

    public static TInterface Get<TInterface>()
    {
        Type interfaceType = typeof(TInterface);

        if (services.ContainsKey(interfaceType))
        {
            return (TInterface)services[interfaceType];
        }

        Debug.LogError($"Service {interfaceType.Name} non trouvé !");
        return default(TInterface);
    }

    public static void Unregister<TInterface>(TInterface serviceIntance)
    {
        Type interfaceType = typeof(TInterface);

        if (services.ContainsKey(interfaceType))
        {
            services.Remove(interfaceType);
        }
        else
        {
            Debug.Log("Le service n'existe pas");
        }
    }
}
