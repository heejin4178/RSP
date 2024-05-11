using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    private Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
    
    public static GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    public void Add(ObjectInfo info, bool myPlayer = false)
    {
        GameObjectType objectType = GetObjectTypeById(info.ObjectId);

        if (objectType == GameObjectType.Player)
        {
            GameObject go = CreatureInstantiate(info);
            
            go.name = info.Name;
            _objects.Add(info.ObjectId, go);
            
            if (myPlayer)
            {
                MyPlayer = go.GetOrAddComponent<MyPlayerController>();
                MyPlayer.Speed = 10.0f;
                MyPlayer.Id = info.ObjectId;
                MyPlayer.PosInfo = info.PosInfo;
                MyPlayer.SyncPos();
                
                Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(go);
            }
            else
            {
                PlayerController pc = go.GetOrAddComponent<PlayerController>();
                pc.Id = info.ObjectId;
                pc.PosInfo = info.PosInfo;
                pc.Stat = info.StatInfo;
                pc.Speed = 10.0f;
                pc.SyncPos();
            }
        }
        else if (objectType == GameObjectType.Aiplayer)
        {
            GameObject go = CreatureInstantiate(info);
            
            go.name = info.Name;
            _objects.Add(info.ObjectId, go);
            
            PlayerController pc = go.GetOrAddComponent<PlayerController>();
            pc.Id = info.ObjectId;
            pc.PosInfo = info.PosInfo;
            pc.Stat = info.StatInfo;
            pc.Speed = 10.0f;
            pc.SyncPos();
        }
        
        else if (objectType == GameObjectType.Projectile)
        {
            GameObject go = ProjectileInstantiate(info);
            go.name = $"{info.PlayerType}_Arrow";
            _objects.Add(info.ObjectId, go);
        
            HandController hc = go.GetOrAddComponent<HandController>();
            hc.Id = info.ObjectId;
            hc.PosInfo = info.PosInfo;
            hc.Stat = info.StatInfo;
            hc.SyncPos();
        }
    }

    private GameObject CreatureInstantiate(ObjectInfo info)
    {
        GameObject go = null;

        switch (info.PlayerType)
        {
            case PlayerType.Rock:
                go = Managers.Resource.Instantiate("Rock Knight.prefab", pooling: true);
                break;
            case PlayerType.Scissors:
                go = Managers.Resource.Instantiate("Scissors Knight.prefab", pooling: true);
                break;
            case PlayerType.Paper:
                go = Managers.Resource.Instantiate("Paper Knight.prefab", pooling: true);
                break;
        }

        return go;
    }
    
    private GameObject ProjectileInstantiate(ObjectInfo info)
    {
        GameObject go = null;

        switch (info.PlayerType)
        {
            case PlayerType.Rock:
                go = Managers.Resource.Instantiate("Rock Hand.prefab", pooling: true);
                break;
            case PlayerType.Scissors:
                go = Managers.Resource.Instantiate("Scissors Hand.prefab", pooling: true);
                break;
            case PlayerType.Paper:
                go = Managers.Resource.Instantiate("Paper Hand.prefab", pooling: true);
                break;
        }

        return go;
    }
    
    public void Remove(int id)
    {
        GameObject go = FindById(id);
        if (go == null)
            return;
        
        _objects.Remove(id);
        Managers.Resource.Destroy(go);
    }
    
    public void RemoveAll()
    {
        Clear();
        MyPlayer = null;
    }

    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }

    // public GameObject FindCreature(Vector3Int cellPos)
    // {
    //     foreach (GameObject obj in _objects.Values)
    //     {
    //         CreatureController cc = obj.GetComponent<CreatureController>();
    //         if (cc == null)
    //             continue;
    //
    //         if (cc.CellPos == cellPos)
    //             return obj;
    //     }
    //
    //     return null;
    // }
    
    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach (GameObject obj in _objects.Values)
        {
            if (condition.Invoke(obj))
                return obj;
        }

        return null;
    }
    
    public void Clear()
    {
        foreach (GameObject obj in _objects.Values)
            Managers.Resource.Destroy(obj);
        _objects.Clear();
        MyPlayer = null;
    }
}
