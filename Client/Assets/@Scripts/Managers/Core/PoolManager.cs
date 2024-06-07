using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

using UnityEngine;

/// <summary>
/// 게임 오브젝트 풀을 나타내는 클래스입니다.
/// </summary>
public class Pool
{
    private GameObject _prefab; // 풀에서 인스턴스화할 프리팹
    private IObjectPool<GameObject> _pool; // 실제 객체 풀

    private Transform _root; // 풀의 루트 부모 오브젝트
    Transform Root
    {
        get
        {
            // 루트가 없으면 새로 생성합니다.
            if (_root == null)
            {
                GameObject go = new GameObject() { name = $"{_prefab.name}Root" };
                _root = go.transform;
            }

            return _root;
        }
    }
    
    public Pool(GameObject prefab)
    {
        _prefab = prefab;
        _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
    }

    /// <summary>
    /// 풀에 게임 오브젝트를 반환합니다.
    /// </summary>
    /// <param name="go">풀로 반환할 게임 오브젝트입니다.</param>
    public void Push(GameObject go)
    {
        _pool.Release(go);
    }
    
    /// <summary>
    /// 풀에서 게임 오브젝트를 가져옵니다.
    /// </summary>
    /// <returns>풀에서 가져온 게임 오브젝트입니다.</returns>
    public GameObject Pop()
    {
        return _pool.Get();
    }
    
    #region Callbacks
    /// <summary>
    /// 새로운 객체를 생성하는 콜백 메서드입니다.
    /// </summary>
    /// <returns>생성된 게임 오브젝트입니다.</returns>
    GameObject OnCreate()
    {
        GameObject go = GameObject.Instantiate(_prefab);
        go.transform.parent = Root;
        go.name = _prefab.name;
        return go;
    }

    /// <summary>
    /// 객체를 가져올 때 호출되는 콜백 메서드입니다.
    /// </summary>
    /// <param name="go">가져온 게임 오브젝트입니다.</param>
    void OnGet(GameObject go)
    {
        go.SetActive(true);
    }
    
    /// <summary>
    /// 객체를 반환할 때 호출되는 콜백 메서드입니다.
    /// </summary>
    /// <param name="go">반환할 게임 오브젝트입니다.</param>
    void OnRelease(GameObject go)
    {
        go.SetActive(false);
    }
    
    /// <summary>
    /// 객체가 destroy될 때 호출되는 콜백 메서드입니다.
    /// </summary>
    /// <param name="go">destroy될 게임 오브젝트입니다.</param>
    void OnDestroy(GameObject go)
    {
        GameObject.Destroy(go);
    }
    #endregion
}


public class PoolManager
{
    private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

    /// <summary>
    /// 지정된 프리팹과 연관된 풀에서 게임 오브젝트를 가져옵니다.
    /// 만약 해당 프리팹의 풀이 존재하지 않는다면 새로운 풀을 생성합니다.
    /// </summary>
    /// <param name="prefab">풀에서 인스턴스화할 프리팹입니다.</param>
    /// <returns>인스턴스화된 게임 오브젝트입니다.</returns>
    public GameObject Pop(GameObject prefab)
    {
        // 해당 프리팹의 풀이 존재하는지 확인하고, 없으면 새로운 풀을 생성합니다.
        if (_pools.ContainsKey(prefab.name) == false)
            CreatePool(prefab);

        // 풀에서 오브젝트를 가져와 반환합니다.
        return _pools[prefab.name].Pop();
    }

    /// <summary>
    /// 게임 오브젝트를 해당하는 풀로 반환합니다.
    /// </summary>
    /// <param name="go">풀로 반환할 게임 오브젝트입니다.</param>
    /// <returns>게임 오브젝트가 성공적으로 풀로 반환되었는지 여부입니다.</returns>
    public bool Push(GameObject go)
    {
        // 해당하는 이름의 풀이 존재하는지 확인합니다.
        if (_pools.ContainsKey(go.name) == false)
            return false;
        
        // 게임 오브젝트를 해당하는 풀로 반환합니다.
        _pools[go.name].Push(go);
        return true;
    }

    /// <summary>
    /// 지정된 프리팹에 대한 새로운 풀을 생성합니다.
    /// </summary>
    /// <param name="prefab">새로운 풀을 생성할 프리팹입니다.</param>
    private void CreatePool(GameObject prefab)
    {
        // 지정된 프리팹에 대한 새로운 풀을 생성합니다.
        Pool pool = new Pool(prefab);
        _pools.Add(prefab.name, pool);
    }
}