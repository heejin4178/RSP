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
            // TODO : Data
            GameObject go = Managers.Resource.Instantiate("Scissors Knight.prefab");
            // go.name = info.Name;
            // _objects.Add(info.ObjectId, go);
            //
            // MyPlayer = go.GetComponent<MyPlayerController>();
            // MyPlayer.Id = info.ObjectId;
            // MyPlayer.PosInfo = info.PosInfo;
            // MyPlayer.Stat = info.StatInfo;
            // MyPlayer.SyncPos();
        }
        // else if (objectType == GameObjectType.Projectile)
        // {
        //     GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        //     go.name = "Arrow";
        //     _objects.Add(info.ObjectId, go);
        //
        //     ArrowController ac = go.GetComponent<ArrowController>();
        //     ac.PosInfo = info.PosInfo;
        //     ac.Stat = info.StatInfo;
        //     ac.SyncPos();
        // }
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
