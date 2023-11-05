using System.Collections.Generic;
using UnityEngine;

public interface IUpdatableInput
{
    void Update();
}

sealed class GameInputUpdater : MonoBehaviour
{
    private readonly HashSet<IUpdatableInput> m_Inputs = new();
    
    public void Add(IUpdatableInput input)
    {
        m_Inputs.Add(input);
    }
    
    private void Update()
    {
        foreach (var input in m_Inputs)
            input.Update();
    }
}