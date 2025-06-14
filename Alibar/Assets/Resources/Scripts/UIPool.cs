using UnityEngine;
using System.Collections.Generic;

public class UIPool : MonoBehaviour
{
    private static UIPool instance;
    public static UIPool Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("UIPool");
                instance = go.AddComponent<UIPool>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private Dictionary<UIType, Queue<GameObject>> poolDictionary = new Dictionary<UIType, Queue<GameObject>>();
    private Dictionary<UIType, GameObject> prefabDictionary = new Dictionary<UIType, GameObject>();
    private Dictionary<UIType, UILayer> layerDictionary = new Dictionary<UIType, UILayer>();

    private const int MAX_POOL_SIZE = 5; // 每个UI类型最大缓存数量

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void InitializePool(List<UIManager.UIPrefabData> prefabDataList)
    {
        foreach (var data in prefabDataList)
        {
            prefabDictionary[data.type] = data.prefab;
            layerDictionary[data.type] = data.layer;
            poolDictionary[data.type] = new Queue<GameObject>();
        }
    }

    public GameObject GetUI(UIType type, Transform parent)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            Debug.LogError($"UI type {type} not found in pool!");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[type];
        GameObject uiObject;

        if (pool.Count > 0)
        {
            uiObject = pool.Dequeue();
            uiObject.SetActive(true);
        }
        else
        {
            uiObject = Instantiate(prefabDictionary[type], parent);
        }

        return uiObject;
    }

    public void ReturnUI(UIType type, GameObject uiObject)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            Debug.LogError($"UI type {type} not found in pool!");
            return;
        }

        Queue<GameObject> pool = poolDictionary[type];

        // 如果池中对象超过最大数量，直接销毁
        if (pool.Count >= MAX_POOL_SIZE)
        {
            Destroy(uiObject);
            return;
        }

        uiObject.SetActive(false);
        uiObject.transform.SetParent(transform);
        pool.Enqueue(uiObject);
    }

    public void ClearPool()
    {
        foreach (var pool in poolDictionary.Values)
        {
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                Destroy(obj);
            }
        }
    }
} 