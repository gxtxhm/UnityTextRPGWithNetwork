using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

/// <summary>
/// Ǯ�� ��ȯ�Ƿ��� ReturnToPool �Լ��� ȣ���ؾ� �մϴ�. 
/// ó���� ����Ҷ� setActive�� false�� �����Ǿ��ֽ��ϴ�.
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
        gameObject.transform.SetParent(PoolingManager.Instance.gameObject.transform);
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
    public static PoolingManager Instance;
    Dictionary<PoolingType, List<ItemStruct>> poolingMap;

    Dictionary<PoolingType, GameObject> prefabsMap;

    const int DefaultPoolSize = 10;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        poolingMap = new Dictionary<PoolingType, List<ItemStruct>>();
        prefabsMap = new Dictionary<PoolingType, GameObject>();
        DontDestroyOnLoad(gameObject);
    }
    

    public void AddInMap(PoolingType name,GameObject prefab)
    {
        if(poolingMap.ContainsKey(name))
        {
            Debug.Log($"{name.ToString()}�� poolingMap�� �̹� �����մϴ�.");return;
        }
        poolingMap.Add(name, new List<ItemStruct>(DefaultPoolSize));
        prefabsMap.Add(name, prefab);
        for(int i=0;i<DefaultPoolSize;i++)
        {
            GameObject go = Instantiate(prefab);
            go.SetActive(false);
            go.transform.SetParent(transform);
            poolingMap[name].Add(new ItemStruct(false,go));
        }
    }

    public ItemStruct GetItem(PoolingType name)
    {
        if (poolingMap.ContainsKey(name) == false)
        {
            Debug.LogError($"{name}�� poolingMap�� �����ϴ�.");return null;
        }
        
        foreach(ItemStruct item in poolingMap[name])
        {
            if (item.isActive == false)
            {
                item.isActive = true;
                return item;
            }
        }

        // ���� ���� ������̶��
        for(int i=0;i<DefaultPoolSize;i++)
        {
            GameObject go = Instantiate(prefabsMap[name]);
            go.transform.SetParent(transform);
            poolingMap[name].Add(new ItemStruct(false, go));
        }

        

        poolingMap[name][^DefaultPoolSize].isActive = true;
        return poolingMap[name][^DefaultPoolSize];
    }

    public void InActiveAllItemByType(PoolingType type)
    {
        poolingMap[type].ForEach(item=>item.ReturnToPool());
    }
}
