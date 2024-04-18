using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }

    #region Contents
    // private MapManager _map = new MapManager();
    // private ObjectManager _obj = new ObjectManager();
    // private NetworkManager _network = new NetworkManager();

    // public static MapManager Map => Instance._map;
    // public static ObjectManager Object => Instance._obj;
    // public static NetworkManager Network => Instance._network;
    #endregion
    
    
    #region Core
    // private DataManager _data = new DataManager();
    // private InputManager _input = new InputManager();
    private PoolManager _pool = new PoolManager();
    private ResourceManager _resource = new ResourceManager();
    private SceneManagerEx _scene = new SceneManagerEx();
    // private SoundManager _sound = new SoundManager();
    // private UIManager _ui = new UIManager();
    
    // public static DataManager Data => Instance._data;
    // public static InputManager Input => Instance._input;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;
    // public static SoundManager Sound => Instance._sound;
    // public static UIManager UI => Instance._ui;
    #endregion

    void Start()
    {
        Init();
    }
    
    void Update()
    {
        // _input.OnUpdate();
        // _network.Update();
    }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject("@Managers");
                go.AddComponent<Managers>();
            }
            
            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
            
            // s_instance._network.Init();
            // s_instance._data.Init();
            // s_instance._pool.Init();
            // s_instance._sound.Init();
        }
    }

    public static void Clear()
    {
        // Input.Clear();
        // Sound.Clear();
        // Scene.Clear();
        // UI.Clear();
        // Pool.Clear();
    }
}
