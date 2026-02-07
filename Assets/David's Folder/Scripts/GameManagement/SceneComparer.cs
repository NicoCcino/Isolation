using System.Collections.Generic;

public class SceneComparer : IEqualityComparer<SceneReference>
{
    public bool Equals(SceneReference x, SceneReference y)
    {
        return x.ScenePath == y.ScenePath;
    }

    public int GetHashCode(SceneReference obj)
    {
        return obj.ScenePath.GetHashCode();
    }
}
