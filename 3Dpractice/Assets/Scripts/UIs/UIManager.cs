using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CheckUIList //�߰��� UI üũ��
{
    [Header("Checking UI")]
    public UIType uiType;
    public LayerType layerType;
    public GameObject prefab;
}
public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if( _instance == null )
            {
                _instance = new GameObject("UIManager").AddComponent<UIManager>();
            }
            return _instance;
        }
    }


    [Header("UI Position")]
    public Transform topArea;
    public Transform middleArea;
    public Transform bottomArea;

    private Dictionary<UIType, GameObject> uiPrefabDict;
    private Dictionary<UIType, GameObject> instantiateUI = new();

    [SerializeField] private List<CheckUIList> checkUILists = new(); //üũ��

    private void Awake()
    {
        if(_instance == null) //�̱���
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        LoadUIPrefabFromResources();
        CheckUIInfo();

        foreach(UIType type in uiPrefabDict.Keys)
        {
            CreateUI(type);
        }
    }


    void LoadUIPrefabFromResources() //Resources�� �ִ� UI�������� Dictionary�� �߰�
    {
        uiPrefabDict = new();

        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("UIPrefabs"); //UIPrefabs�� Resources ������ ������

        foreach(var prefab in loadedPrefabs)
        {
            UIBase uiBase = prefab.GetComponent<UIBase>(); //prefab�� �ִ� UIBase ������Ʈ�� ������
            if( uiBase == null )
            {
                Debug.Log($"{prefab.name}�� UIBase ������Ʈ�� �����ϴ�. Ȯ�ο��");
                continue;
            }

            if(uiPrefabDict.ContainsKey(uiBase.uiType))
            {
                Debug.LogError($"�ߺ��� UIType {uiBase.uiType}�� �߰ߵǾ� �����մϴ�.");
                continue;
            }

            uiPrefabDict.Add(uiBase.uiType, prefab); // UIBase�� �ְ� ������ Key(UiType)�� ���� ��ųʸ��� uiprefabDic�� �߰�
        }

        Debug.Log($"UI ������ {uiPrefabDict.Count}�� ��ϿϷ�");
    }

    public GameObject CreateUI(UIType uiType) //��ϵ� UIType�� �ش�� �������� �ν��Ͻ��� �����ϴ� �Լ�
    {
        if(!uiPrefabDict.TryGetValue(uiType, out GameObject prefab)) //���޹��� uiType�� �ش��ϴ� �������� ��ϵǾ��ִ��� Ȯ��
        {
            Debug.Log($"UIType {uiType}�� �ش��ϴ� �������� �����ϴ�.");
            return null;
        }

        GameObject instance = Instantiate(prefab); //�������� �ν��Ͻ�ȭ(Scene�� ����)�Ѵ�.

        UIBase uiBase = instance.GetComponent<UIBase>();
        Transform parent;

        switch(uiBase.layerType)
        {
            case LayerType.Top:
                parent = topArea;
                break;

            case LayerType.Middle:
                parent = middleArea;
                break;

            case LayerType.Bottom:
                parent = bottomArea;
                break;

            default:
                parent = bottomArea;
                break;
        }

        instance.transform.SetParent(parent, false); // instance�� �θ� ������ ���� parent�� ����. False�� ���� ��ǥ�� �ƴ� *������ǥ*�� �����ϰڴٴ� �ǹ�

        if(!instantiateUI.ContainsKey(uiType))
        {
            instantiateUI.Add(uiType, instance);
        }
        else
        {
            Debug.LogError($"{uiType} UI�� �̹� �����Ǿ� �ֽ��ϴ�.");
        }

        return instance;
    }

    public void CheckUIInfo() //������
    {
        checkUILists.Clear();
        foreach(var list in uiPrefabDict)
        {
            var prefab = list.Value;
            var uiBase = prefab.GetComponent<UIBase>();

            LayerType layer = uiBase != null ? uiBase.layerType : LayerType.Bottom;

            checkUILists.Add(new CheckUIList
            {
                uiType = list.Key,
                prefab = prefab,
                layerType = layer
            });
        }
    }
}
