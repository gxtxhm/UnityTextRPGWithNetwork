using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

/// <summary>
/// 풀에 반환되려면 ReturnToPool 함수를 호출해야 합니다. 
/// 처음에 사용할때 setActive는 false로 설정되어있습니다.
/// </summary>
public class ItemStruct
{
    public bool isActive;
    public GameObject gameObject;

    public ItemStruct(bool isActive, GameObject gameObject)
    {
        this.isActive = isActive;
        this.gameObject = gameObject;
    }

    public void ReturnToPool()
    {
        isActive = false;
        gameObject.transform.SetParent(null);
        gameObject.SetActive(false);
    }
}

public enum PoolingType
{
    JoinRoomBtn,
    InventoryItemBtn
}

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager Instance = new PoolingManager();
    Dictionary<PoolingType, List<ItemStruct>> poolingMap;

    Dictionary<PoolingType, GameObject> prefabsMap;

    const int DefaultPoolSize = 10;

    PoolingManager()
    {
        poolingMap = new Dictionary<PoolingType, List<ItemStruct>>();
        prefabsMap = new Dictionary<PoolingType, GameObject>();
    }
        

    public void AddInMap(PoolingType name,GameObject prefab)
    {
        if(poolingMap.ContainsKey(name))
        {
            Debug.Log($"{name.ToString()}은 poolingMap에 이미 존재합니다.");return;
        }
        poolingMap.Add(name, new List<ItemStruct>(DefaultPoolSize));
        prefabsMap.Add(name, prefab);
        for(int i=0;i<DefaultPoolSize;i++)
        {
            GameObject go = Instantiate(prefab);
            go.SetActive(false);
            poolingMap[name].Add(new ItemStruct(false,go));
        }
    }

    public ItemStruct GetItem(PoolingType name)
    {
        if (poolingMap.ContainsKey(name) == false)
        {
            Debug.LogError($"{name}은 poolingMap에 없습니다.");return null;
        }
        
        foreach(ItemStruct item in poolingMap[name])
        {
            if (item.isActive == false)
            {
                item.isActive = true;
                return item;
            }
        }

        // 만약 전부 사용중이라면
        for(int i=0;i<DefaultPoolSize;i++)
            poolingMap[name].Add(new ItemStruct(false, Instantiate(prefabsMap[name])));

        poolingMap[name][^DefaultPoolSize].isActive = true;
        return poolingMap[name][^DefaultPoolSize];
    }

    public void InActiveAllItemByType(PoolingType type)
    {
        poolingMap[type].ForEach(item=>item.ReturnToPool());
    }
}
