using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICondition : UIBase
{
    [Header("Condition Setting")]
    public Condition health; //체력
    public Condition stamina; //스테미너
    public Condition hunger; // 임시용
    void Start()
    {
        CharacterManager.Instance.Player.condition.uiCondition = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
