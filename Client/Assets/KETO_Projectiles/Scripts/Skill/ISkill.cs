using UnityEngine;
public enum SPAWN_TYPE
{
    CHILD,
    NONE
}

public interface ISkill
{
    void Initialize();
    void StartSkill(Vector3 _position);
    void Cancel();
    string GetName();
}
