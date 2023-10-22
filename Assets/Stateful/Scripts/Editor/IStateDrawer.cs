using System;
using UnityEditor;

public interface IStateDrawer
{
    event Action StateAnimation;
    public SerializedProperty StateProp { get; }
    void DrawState();
    void UpdateStateProp(int index, SerializedProperty stateProp);
    IState GetState();
    string GetStateName();
}