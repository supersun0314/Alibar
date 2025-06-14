using UnityEngine;
using System.Collections.Generic;

public enum UIType
{
    MainMenu,
    Settings,
    Inventory,
    Shop,
    // Add more UI types as needed
}

public enum UILayer
{
    TOP,    // 最上层UI，如弹窗、提示等
    MIDDLE, // 中间层UI，如主界面、设置等
    BOTTOM  // 底层UI，如背景、HUD等
}

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("UIManager");
                instance = go.AddComponent<UIManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    [System.Serializable]
    public class UIPrefabData
    {
        public UIType type;
        public GameObject prefab;
        public UILayer layer;
    }

    private List<UIPrefabData> uiPrefabs = new List<UIPrefabData>();
    private Dictionary<UIType, GameObject> activeUIs = new Dictionary<UIType, GameObject>();
    
    // UI层级根节点
    private Transform topLayerRoot;
    private Transform middleLayerRoot;
    private Transform bottomLayerRoot;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // 创建UI层级根节点
        CreateUILayerRoots();

        // 从UIMapping获取UI预制体数据
        uiPrefabs = UIMapping.Instance.GetUIPrefabDataList();

        // 初始化对象池
        UIPool.Instance.InitializePool(uiPrefabs);
    }

    private void CreateUILayerRoots()
    {
        // 创建Canvas作为UI根节点
        GameObject canvasObj = new GameObject("UICanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        canvasObj.transform.SetParent(transform);

        // 创建三个层级根节点
        topLayerRoot = CreateLayerRoot("TopLayer", canvasObj.transform);
        middleLayerRoot = CreateLayerRoot("MiddleLayer", canvasObj.transform);
        bottomLayerRoot = CreateLayerRoot("BottomLayer", canvasObj.transform);

        // 设置层级顺序（从下到上）
        SetLayerOrder(topLayerRoot, 2);
        SetLayerOrder(middleLayerRoot, 1);
        SetLayerOrder(bottomLayerRoot, 0);
    }

    private Transform CreateLayerRoot(string name, Transform parent)
    {
        GameObject layerObj = new GameObject(name);
        layerObj.transform.SetParent(parent, false);
        RectTransform rectTransform = layerObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        return layerObj.transform;
    }

    private void SetLayerOrder(Transform layerRoot, int order)
    {
        Canvas canvas = layerRoot.gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = order;
    }

    public void OpenUI(UIType type)
    {
        if (activeUIs.ContainsKey(type))
        {
            Debug.LogWarning($"UI {type} is already open!");
            return;
        }

        UIPrefabData prefabData = uiPrefabs.Find(x => x.type == type);
        if (prefabData == null)
        {
            Debug.LogError($"No prefab found for UI type: {type}");
            return;
        }

        // 根据层级选择父节点
        Transform parentTransform = GetLayerRoot(prefabData.layer);
        
        // 从对象池获取UI
        GameObject uiInstance = UIPool.Instance.GetUI(type, parentTransform);
        if (uiInstance != null)
        {
            activeUIs.Add(type, uiInstance);
        }
    }

    private Transform GetLayerRoot(UILayer layer)
    {
        switch (layer)
        {
            case UILayer.TOP:
                return topLayerRoot;
            case UILayer.MIDDLE:
                return middleLayerRoot;
            case UILayer.BOTTOM:
                return bottomLayerRoot;
            default:
                return middleLayerRoot;
        }
    }

    public void CloseUI(UIType type)
    {
        if (!activeUIs.ContainsKey(type))
        {
            Debug.LogWarning($"UI {type} is not open!");
            return;
        }

        // 将UI返回对象池
        UIPool.Instance.ReturnUI(type, activeUIs[type]);
        activeUIs.Remove(type);
    }

    public bool IsUIOpen(UIType type)
    {
        return activeUIs.ContainsKey(type);
    }

    public void CloseAllUI()
    {
        foreach (var ui in activeUIs.Values)
        {
            UIPool.Instance.ReturnUI(ui.GetComponent<UIBase>()?.Type ?? UIType.MainMenu, ui);
        }
        activeUIs.Clear();
    }

    private void OnDestroy()
    {
        // 清理对象池
        if (UIPool.Instance != null)
        {
            UIPool.Instance.ClearPool();
        }
    }
} 