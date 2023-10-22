using UnityEditor;
using System.Collections.Generic;

public static class EditorExtensions
{
    public static IEnumerable<SerializedProperty> GetDirectChildren(this SerializedProperty parent, int depth = 1)
    {
        var cpy = parent.Copy();    
        var depthOfParent = cpy.depth;
        var enumerator = cpy.GetEnumerator();

        while (enumerator.MoveNext())
        {
            if (enumerator.Current is not SerializedProperty childProperty) continue;
            if (childProperty.depth > depthOfParent + depth) continue;

            yield return childProperty.Copy();
        }
    }
}