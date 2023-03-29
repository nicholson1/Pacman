using System;
using System.Collections;
using System.Collections.Generic;
using ImportantStuff;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionItem : MonoBehaviour
{
    public Equipment item;
    [SerializeField] private CombatEntity myCharacter;
    
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI rarity;
    [SerializeField] private TextMeshProUGUI slot;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI[] stats;

    [SerializeField] private Button equip;
    [SerializeField] private Button  inventory;
    [SerializeField] private ToolTip _toolTip;
    [SerializeField] private Image icon;

    [SerializeField] private ToolTip[] SpellToolTips;

    public void InitializeSelectionItem(Equipment e)
    {

        
        myCharacter = CombatController._instance.Player.GetComponent<CombatEntity>();
        
        
        item = e;
        title.text = e.name;
        if (e.slot != Equipment.Slot.OneHander)
        {
            slot.text = e.slot.ToString();
        }
        else
        {
            Weapon x = (Weapon)e;
            slot.text = "Weapon: " +GetWeaponType(x.spellType1);
            
        }
        SetRarityText(e.stats[Equipment.Stats.Rarity], e);

        icon.sprite = e.icon;
        title.color = rarity.color;

        for (int i = 0; i <= stats.Length -1; i++)
        {
            stats[i].text = String.Empty;
            SpellToolTips[i].gameObject.SetActive(false);

        }

        int count = 0;
        foreach (var kvp in e.stats)
        {
            if (kvp.Key != Equipment.Stats.Rarity && kvp.Key != Equipment.Stats.ItemLevel)
            {
                stats[count].text = kvp.Key + ": " + kvp.Value;
                count += 1;
                
            }
        }
        

        if (e.isWeapon)
        {
            Weapon x = (Weapon) e;
            if (x.spellType1 != Weapon.SpellTypes.None)
            {
                stats[count].text = x.scalingInfo1[0].ToString();
                stats[count].color = rarity.color;
                
                
                SpellToolTip(x.spellType1,x, count);

                // activate tool tip on stats[count]
                
                count += 1;
            }
            if (x.spellType2 != Weapon.SpellTypes.None)
            {
                stats[count].text = x.scalingInfo1[0].ToString();
                stats[count].color = rarity.color;
                // activate tool tip on stats[count]
                
                SpellToolTip(x.spellType1,x, count);

                count += 1;
            }
        }
        
        _toolTip.iLvl = e.stats[Equipment.Stats.ItemLevel].ToString();
        _toolTip.rarity = e.stats[Equipment.Stats.Rarity];
        _toolTip.Cost = "";
        _toolTip.Title = e.name;
        foreach (var stat in stats)
        {
            _toolTip.Message += stat.text + "\n";
        }

       



    }
    
    private void SetRarityText(int r, Equipment e)
    {
        switch (r)
        {
            case 0:
                rarity.text = "Common";
                rarity.color = ToolTipManager._instance.rarityColors[0];
                break;
            case 1:
                rarity.text = "Uncommon";
                rarity.color = ToolTipManager._instance.rarityColors[1];

                break;
            case 2:
                rarity.text = "Rare";
                rarity.color = ToolTipManager._instance.rarityColors[2];

                break;
            case 3:
                rarity.text = "Epic";
                rarity.color = ToolTipManager._instance.rarityColors[3];

                break;
            case -1 :
                rarity.text = "";
                break;
            
        }
       rarity.text += " (lvl " + e.stats[Equipment.Stats.ItemLevel]+")";
    }
    
    // public void UpdateToolTipWeapon(Weapon.SpellTypes s, Weapon w)
    // {
    //
    //    
    //     List<int> power = TheSpellBook._instance.GetPowerValues(s, w, myCharacter);
    //
    //     List<List<object>> DataTable = DataReader._instance.GetWeaponScalingTable();
    //
    //     _toolTip.Title = DataTable[(int)s][0].ToString();;
    //     _toolTip.Message = AdjustDescriptionValues(DataTable[(int)s][3].ToString(), power[1], power[0]);
    //     _toolTip.Cost = DataTable[(int)s][2].ToString();
    //     
    //     //iLVL
    //     int a;
    //     w.stats.TryGetValue(Equipment.Stats.ItemLevel, out a);
    //     _toolTip.iLvl = a.ToString();
    //     //Rarity
    //     int r;
    //     w.stats.TryGetValue(Equipment.Stats.Rarity, out r);
    //     _toolTip.rarity = r;
    //     
    //     
    //     
    //     
    // }
    public string AdjustDescriptionValues(string message, int turns, float amount)
    {
        message = message.Replace("$", turns.ToString());
        message = message.Replace("@", amount.ToString());
        message = message.Replace("#", (Mathf.RoundToInt(amount/4)*4).ToString());
        

        return message;

    }

    public void AddToInventory()
    {
        EquipmentManager._instance.AddItemToInventoryFromSelection(item, this);
    }

    public void EquipedFromSelection()
    {
        EquipmentManager._instance.EquipItemFromSelection(item, this);
    }

    public void RemoveSelection()
    {
        SelectionManager._instance.SelectionMade(this);
        //Destroy(gameObject);
    }

    public void SpellToolTip(Weapon.SpellTypes s, Weapon w, int index)
    {
        
        SpellToolTips[index].gameObject.SetActive(true);
        List<List<object>> DataTable = DataReader._instance.GetWeaponScalingTable();
        List<int> power = TheSpellBook._instance.GetPowerValues(s, w, myCharacter);

        //Debug.Log(w.name + "--------------");


        //tt.enabled = true;


        SpellToolTips[index].Title = DataTable[(int)s][0].ToString();;
        SpellToolTips[index].Message = AdjustDescriptionValues(DataTable[(int)s][3].ToString(), power[1], power[0]);
        SpellToolTips[index].Cost = DataTable[(int)s][2].ToString();
        
        //iLVL
        int a;
        w.stats.TryGetValue(Equipment.Stats.ItemLevel, out a);
        SpellToolTips[index].iLvl = a.ToString();
        //Rarity
        int r;
        w.stats.TryGetValue(Equipment.Stats.Rarity, out r);
        SpellToolTips[index].rarity = r;
        
        //Debug.Log("we did the things?");

        
    }

    private string GetWeaponType(Weapon.SpellTypes spell)
    {
        string name = "";
        switch (spell)
        {
            case Weapon.SpellTypes.Dagger1:
                name = "Dagger";
                break;
            case Weapon.SpellTypes.Dagger2:
                name = "Dagger";
                break;
            case Weapon.SpellTypes.Shield1:
                name = "Shield";
                break;
            case Weapon.SpellTypes.Shield2:
                name = "Shield";
                break;
            case Weapon.SpellTypes.Sword1:
                name = "Sword";
                break;
            case Weapon.SpellTypes.Sword2:
                name = "Sword";
                break;
            case Weapon.SpellTypes.Axe1:
                name = "Axe";
                break;
            case Weapon.SpellTypes.Axe2:
                name = "Axe";
                break;
            case Weapon.SpellTypes.Hammer1:
                name = "Hammer";
                break;
            case Weapon.SpellTypes.Hammer2:
                name = "Hammer";
                break;
            case Weapon.SpellTypes.Nature1:
                name = "Nature";
                break;
            case Weapon.SpellTypes.Nature2:
                name = "Nature";
                break;
            case Weapon.SpellTypes.Nature3:
                name = "Nature";
                break;
            case Weapon.SpellTypes.Nature4:
                name = "Nature";
                break;
            case Weapon.SpellTypes.Fire1:
                name = "Fire";
                break;
            case Weapon.SpellTypes.Fire2:
                name = "Fire";
                break;
            case Weapon.SpellTypes.Fire3:
                name = "Fire";
                break;
            case Weapon.SpellTypes.Fire4:
                name = "Fire";
                break;
            case Weapon.SpellTypes.Ice1:
                name = "Ice";
                break;
            case Weapon.SpellTypes.Ice2:
                name = "Ice";
                break;
            case Weapon.SpellTypes.Ice3:
                name = "Ice";
                break;
            case Weapon.SpellTypes.Ice4:
                name = "Ice";
                break;
            case Weapon.SpellTypes.Blood1:
                name = "Blood";
                break;
            case Weapon.SpellTypes.Blood2:
                name = "Blood";
                break;
            case Weapon.SpellTypes.Blood3:
                name = "Blood";
                break;
            case Weapon.SpellTypes.Blood4:
                name = "Blood";
                break;
            case Weapon.SpellTypes.Shadow1:
                name = "Shadow";
                break;
            case Weapon.SpellTypes.Shadow2:
                name = "Shadow";
                break;
            case Weapon.SpellTypes.Shadow3:
                name = "Shadow";
                break;
            case Weapon.SpellTypes.Shadow4:
                name = "Shadow";
                break;
            }

        return name;

    }








}