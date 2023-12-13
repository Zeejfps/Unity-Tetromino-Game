using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class DiContainer : ScriptableObject
{
    private readonly Dictionary<Type, IFactory> m_TypeToFactoryTable = new();
    
    private void OnEnable()
    {
        OnInit();
    }

    protected abstract void OnInit();

    public T Get<T>()
    {
        return (T)Get(typeof(T));
    }
    
    protected void RegisterSingleton<TInterface, TConcrete>() where TConcrete : TInterface
    {
        var interfaceType = typeof(TInterface);
        try
        {
            m_TypeToFactoryTable.Add(interfaceType, new SingletonFactory<TConcrete>(this));
        }
        catch (ArgumentException)
        {
            throw new Exception($"Singleton for {interfaceType} type already registered");
        }
    }
    
    private object Get(Type type)
    {
        if (m_TypeToFactoryTable.TryGetValue(type, out var factory))
            return factory.Create();

        return New(type);
    }
    
    private object New(Type type)
    {
        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        var paramValues = new List<object>();
        foreach (var constructor in constructors)
        {
            var isValidConstructor = true;
            var parameters = constructor.GetParameters();
            foreach (var parameter in parameters)
            {
                var paramType = parameter.ParameterType;
                var paramValue = Get(paramType);
                if (paramValue == null)
                {
                    isValidConstructor = false;
                    break;
                }
                
                paramValues.Add(paramValue);
            }
            
            if (isValidConstructor)
                return Activator.CreateInstance(type, paramValues.ToArray());
            
            paramValues.Clear();
        }

        return null;
    }

    public void Inject(MonoBehaviour monoBehaviour)
    {
        var type = monoBehaviour.GetType();
        var properties = type.GetProperties()
            .Where(prop => prop.GetCustomAttribute<InjectedAttribute>() != null);
        foreach (var prop in properties)
        {
            var value = Get(prop.PropertyType);
            prop.SetValue(monoBehaviour, value);
        }
    }
    
    private sealed class SingletonFactory<T> : IFactory
    {
        private readonly DiContainer m_DiContainer;
        private T m_Singleton;

        public SingletonFactory(DiContainer diContainer, T singleton = default)
        {
            m_DiContainer = diContainer;
            m_Singleton = singleton;
        }
        
        public object Create()
        {
            if (m_Singleton == null)
                m_Singleton = (T)m_DiContainer.New(typeof(T));
            return m_Singleton;
        }
    }
}

internal interface IFactory
{
    object Create();
}
