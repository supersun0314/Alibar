using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class UIMapping : MonoBehaviour
{
    private static UIMapping instance;
    public static UIMapping Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("UIMapping");
                instance = go.AddComponent<UIMapping>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    [System.Serializable]
    public class UIMappingData
    {
        public string prefabName;
        public UIType type;
        public UILayer layer;
        [Tooltip("预制体在Resources文件夹下的完整路径，例如：UI/Prefabs/MainMenu")]
        public string prefabPath;
    }

    [SerializeField]
    private List<UIMappingData> uiMappings = new List<UIMappingData>();

    private Dictionary<string, UIType> nameToTypeMap = new Dictionary<string, UIType>();
    private Dictionary<string, UILayer> nameToLayerMap = new Dictionary<string, UILayer>();
    private Dictionary<UIType, string> typeToNameMap = new Dictionary<UIType, string>();
    private Dictionary<UIType, string> typeToPathMap = new Dictionary<UIType, string>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeMappings();
    }

    private void InitializeMappings()
    {
        nameToTypeMap.Clear();
        nameToLayerMap.Clear();
        typeToNameMap.Clear();
        typeToPathMap.Clear();

        foreach (var mapping in uiMappings)
        {
            if (string.IsNullOrEmpty(mapping.prefabName))
            {
                Debug.LogError("Found UI mapping with empty prefab name!");
                continue;
            }

            if (string.IsNullOrEmpty(mapping.prefabPath))
            {
                Debug.LogError($"Prefab path is empty for UI: {mapping.prefabName}");
                continue;
            }

            nameToTypeMap[mapping.prefabName] = mapping.type;
            nameToLayerMap[mapping.prefabName] = mapping.layer;
            typeToNameMap[mapping.type] = mapping.prefabName;
            typeToPathMap[mapping.type] = mapping.prefabPath;
        }
    }

    public List<UIManager.UIPrefabData> GetUIPrefabDataList()
    {
        List<UIManager.UIPrefabData> prefabDataList = new List<UIManager.UIPrefabData>();

        foreach (var mapping in uiMappings)
        {
            if (string.IsNullOrEmpty(mapping.prefabPath))
            {
                Debug.LogError($"Prefab path is empty for UI: {mapping.prefabName}");
                continue;
            }

            GameObject prefab = Resources.Load<GameObject>(mapping.prefabPath);
            if (prefab == null)
            {
                Debug.LogError($"Failed to load UI prefab at path: {mapping.prefabPath}\n" +
                             $"Make sure the prefab exists in the Resources folder and the path is correct.\n" +
                             $"Current mapping: Name={mapping.prefabName}, Type={mapping.type}, Layer={mapping.layer}");
                continue;
            }

            // 检查预制体是否包含UIBase组件
            UIBase uiBase = prefab.GetComponent<UIBase>();
            if (uiBase == null)
            {
                Debug.LogError($"UI prefab {mapping.prefabName} does not have UIBase component!");
                continue;
            }

            prefabDataList.Add(new UIManager.UIPrefabData
            {
                type = mapping.type,
                prefab = prefab,
                layer = mapping.layer
            });

            Debug.Log($"Successfully loaded UI prefab: {mapping.prefabName} from path: {mapping.prefabPath}");
        }

        if (prefabDataList.Count == 0)
        {
            Debug.LogError("No UI prefabs were loaded successfully! Please check your UI mappings and prefab paths.");
        }
        else
        {
            Debug.Log($"Successfully loaded {prefabDataList.Count} UI prefabs.");
        }

        return prefabDataList;
    }

    // 用于在编辑器中验证预制体路径
    public void ValidatePrefabPaths()
    {
        foreach (var mapping in uiMappings)
        {
            if (string.IsNullOrEmpty(mapping.prefabPath))
            {
                Debug.LogError($"Empty prefab path for UI: {mapping.prefabName}");
                continue;
            }

            GameObject prefab = Resources.Load<GameObject>(mapping.prefabPath);
            if (prefab == null)
            {
                Debug.LogError($"Cannot find prefab at path: {mapping.prefabPath} for UI: {mapping.prefabName}");
            }
            else
            {
                Debug.Log($"Valid prefab found at path: {mapping.prefabPath} for UI: {mapping.prefabName}");
            }
        }
    }

    // 用于在编辑器中检查Resources文件夹结构
    public void CheckResourcesStructure()
    {
        string resourcesPath = Path.Combine(Application.dataPath, "Resources");
        if (!Directory.Exists(resourcesPath))
        {
            Debug.LogError("Resources folder not found in Assets directory!");
            return;
        }

        Debug.Log("Resources folder structure:");
        PrintDirectoryStructure(resourcesPath, 0);
    }

    private void PrintDirectoryStructure(string path, int indent)
    {
        string indentStr = new string(' ', indent * 2);
        Debug.Log($"{indentStr}Directory: {Path.GetFileName(path)}");

        // 打印文件
        foreach (string file in Directory.GetFiles(path))
        {
            if (file.EndsWith(".prefab"))
            {
                Debug.Log($"{indentStr}  File: {Path.GetFileName(file)}");
            }
        }

        // 递归打印子目录
        foreach (string dir in Directory.GetDirectories(path))
        {
            PrintDirectoryStructure(dir, indent + 1);
        }
    }
}