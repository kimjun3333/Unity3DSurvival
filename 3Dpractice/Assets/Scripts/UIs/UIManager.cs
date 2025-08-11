using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CheckUIList //추가된 UI 체크용
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

    [SerializeField] private List<CheckUIList> checkUILists = new(); //체크용

    private void Awake()
    {
        if(_instance == null) //싱글톤
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


    void LoadUIPrefabFromResources() //Resources에 있는 UI프리팹을 Dictionary에 추가
    {
        uiPrefabDict = new();

        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("UIPrefabs"); //UIPrefabs의 Resources 파일을 가져옴

        foreach(var prefab in loadedPrefabs)
        {
            UIBase uiBase = prefab.GetComponent<UIBase>(); //prefab에 있는 UIBase 컴포넌트를 가져옴
            if( uiBase == null )
            {
                Debug.Log($"{prefab.name}에 UIBase 컴포넌트가 없습니다. 확인요망");
                continue;
            }

            if(uiPrefabDict.ContainsKey(uiBase.uiType))
            {
                Debug.LogError($"중복된 UIType {uiBase.uiType}이 발견되어 무시합니다.");
                continue;
            }

            uiPrefabDict.Add(uiBase.uiType, prefab); // UIBase가 있고 고유의 Key(UiType)를 가진 딕셔너리를 uiprefabDic에 추가
        }

        Debug.Log($"UI 프리팹 {uiPrefabDict.Count}개 등록완료");
    }

    public GameObject CreateUI(UIType uiType) //등록된 UIType에 해당된 프리팹을 인스턴스로 생성하는 함수
    {
        if(!uiPrefabDict.TryGetValue(uiType, out GameObject prefab)) //전달받은 uiType에 해당하는 프리팹이 등록되어있는지 확인
        {
            Debug.Log($"UIType {uiType}에 해당하는 프리팹이 없습니다.");
            return null;
        }

        GameObject instance = Instantiate(prefab); //프리팹을 인스턴스화(Scene에 생성)한다.

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

        instance.transform.SetParent(parent, false); // instance의 부모를 위에서 정한 parent로 설정. False는 월드 좌표가 아닌 *로컬좌표*로 유지하겠다는 의미

        if(!instantiateUI.ContainsKey(uiType))
        {
            instantiateUI.Add(uiType, instance);
        }
        else
        {
            Debug.LogError($"{uiType} UI가 이미 생성되어 있습니다.");
        }

        return instance;
    }

    public void CheckUIInfo() //디버깅용
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
