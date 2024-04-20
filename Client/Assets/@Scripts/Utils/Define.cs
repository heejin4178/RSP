
public class Define
{
    // public enum CreatureState
    // {
    //     Idle,
    //     Moving,
    //     Skill,
    //     Dead,
    // }
    public enum WorldObject
    {
        Unknown,
        Player,
        Monster,
    }
    public enum Layer
    {
        Moster = 8,
        Ground = 9,
        Block = 10,
    }
    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
    }
    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount
    }
    
    public enum UIEvent
    {
        Click,
        Drag,
    }
    public enum MouseEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click,
    }
    public enum CameraMode
    {
        QuarterView,
    }
}
