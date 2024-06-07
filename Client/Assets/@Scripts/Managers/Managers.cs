using System;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }

    #region Contents
    private GameManager _game = new GameManager();
    private ObjectManager _obj = new ObjectManager();
    private NetworkManager _network = new NetworkManager();
    
    public static GameManager Game => Instance._game;
    public static ObjectManager Object => Instance._obj;
    public static NetworkManager Network => Instance._network;
    #endregion
    
    
    #region Core
    private PoolManager _pool = new PoolManager();
    private ResourceManager _resource = new ResourceManager();
    private SceneManagerEx _scene = new SceneManagerEx();
    private UIManager _ui = new UIManager();
    
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;
    public static UIManager UI => Instance._ui;
    #endregion

    void Start()
    {
        Init();
    }
    
    void Update()
    {
        _network.Update();
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
        }
    }

    public static void Clear()
    {
        
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Game Quit!!");
        Network.Disconnect();
    }
}
