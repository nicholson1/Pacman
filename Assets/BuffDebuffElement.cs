using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffDebuffElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnCounter;
    [SerializeField] private Image icon;
    [SerializeField] private ToolTip toolTip;

    public bool isDebuff = false;
    public int _turns = 0;
    private float _amount = 0;
    private CombatEntity.DeBuffTypes _debuff;
    private CombatEntity.BuffTypes _buff;

    public void InitializeDisplay(CombatEntity.DeBuffTypes deBuff, int turns, float amount)
    {
        _turns = turns;
        _amount = amount;
        turnCounter.text = turns.ToString();
        icon.sprite =TheSpellBook._instance.GetSprite(deBuff);
        toolTip.iLvl = "";
        toolTip.Title = deBuff.ToString();
        toolTip.Message = TheSpellBook._instance.GetDesc(deBuff);
        toolTip.Message = AdjustDescriptionValues(TheSpellBook._instance.GetDesc(deBuff), turns, amount);
        toolTip.rarity = -1;
        isDebuff = true;
    }

    public void InitializeDisplay(CombatEntity.BuffTypes buff, int turns, float amount)
    {
        turnCounter.text = turns.ToString();
        icon.sprite =TheSpellBook._instance.GetSprite(buff);


    }

    public void UpdateValues()
    {
        _turns -= 1;
        if (isDebuff)
        {
            toolTip.Message = AdjustDescriptionValues(TheSpellBook._instance.GetDesc(_debuff), _turns, _amount);
        }
        else
        {
            toolTip.Message = AdjustDescriptionValues(TheSpellBook._instance.GetDesc(_buff), _turns, _amount);

        }
        turnCounter.text = _turns.ToString();
        
    }

    public string AdjustDescriptionValues(string message, int turns, float amount)
    {
        message = message.Replace("$", turns.ToString());
        message = message.Replace("@", amount.ToString());

        return message;

    }
}
