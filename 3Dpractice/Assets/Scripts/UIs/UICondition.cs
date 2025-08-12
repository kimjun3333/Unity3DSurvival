using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICondition : UIBase
{
    [Header("Condition Setting")]
    public Condition health; //ü��
    public Condition stamina; //���׹̳�
    public Condition hunger; // �ӽÿ�
    void Start()
    {
        CharacterManager.Instance.Player.condition.uiCondition = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
