using System;
using System.Collections;
using System.Collections.Generic;
using ImportantStuff;
using UnityEngine;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
    private string _name;
    public int _level;
    public int _experience;
    public int _currentHealth;
    public int _maxHealth;
    public int _maxEnergy;
    public int _currentEnergy;

    public bool isPlayerCharacter;
    
    private List<Equipment> _equipment;
    private List<Weapon> _weapons = new List<Weapon>();
    private List<Weapon> _spellScrolls = new List<Weapon>();

    public List<(CombatEntity.BuffTypes, int, float)> Buffs = new List<(CombatEntity.BuffTypes, int, float)>();
    public List<(CombatEntity.DeBuffTypes, int, float)> DeBuffs = new List<(CombatEntity.DeBuffTypes, int, float)>();



    Dictionary<Equipment.Stats, int> _stats;
    private List<Equipment> _inventory;

    private EquipmentCreator EC;

    [SerializeField] public CombatEntity _combatEntity;

    public static event Action<Character,int, int> UpdateBlock; 
    public (Weapon.SpellTypes, Weapon.SpellTypes, Weapon, Weapon) GetWeaponSpells()
    {
        //spell 1, spell2, weapon1, weapon2
        (Weapon.SpellTypes, Weapon.SpellTypes, Weapon, Weapon) spells = (Weapon.SpellTypes.None, Weapon.SpellTypes.None, null, null);


        switch (_weapons.Count)
        {
            case 0:
                break;
            case 1:
                if (_weapons[0].slot == Equipment.Slot.TwoHander)
                {
                    spells.Item1 = _weapons[0].GetSpellTypes().Item1;
                    spells.Item2 = _weapons[0].GetSpellTypes().Item2;
                    spells.Item3 = _weapons[0];
                    spells.Item4 = _weapons[0];
                }
                else
                {
                    spells.Item1 = _weapons[0].GetSpellTypes().Item1;
                    spells.Item3 = _weapons[0];
                }
                break;
            case 2:
                spells.Item1 = _weapons[0].GetSpellTypes().Item1;
                spells.Item3 = _weapons[0];
                spells.Item2 = _weapons[1].GetSpellTypes().Item1;
                spells.Item4 = _weapons[1];

                break;
                
                
        }

        return spells;
    }

    public (Weapon.SpellTypes, Weapon.SpellTypes, Weapon, Weapon) GetScollSpells()
    {
        (Weapon.SpellTypes, Weapon.SpellTypes, Weapon, Weapon) spells = (Weapon.SpellTypes.None, Weapon.SpellTypes.None, null, null);
        switch (_spellScrolls.Count)
        {
            case 0:
                break;
            case 1:
                spells.Item1 = _spellScrolls[0].GetSpellTypes().Item1;
                break;
            case 2:
                spells.Item1 = _spellScrolls[0].GetSpellTypes().Item1;
                spells.Item3 = _spellScrolls[0];
                spells.Item2 = _spellScrolls[1].GetSpellTypes().Item1;
                spells.Item4 = _spellScrolls[1];

                break;
                
                
        }

        return spells;
    }

    public Dictionary<Equipment.Stats, int> GetStats()
    {
        return _stats;
    }
    
    private void Start()
    {
        EC = FindObjectOfType<EquipmentCreator>();
        CombatTrigger.TriggerCombat += ActivateCombatEntity;
        CombatTrigger.EndCombat += DeactivateCombatEntity;
        
        CombatEntity.GetHitWithAttack += GetHitWithAttack;
        CombatEntity.GetHitWithBuff += GetHitWithBuff;
        CombatEntity.GetHitWithDeBuff += GetHitWithDeBuff;


        CombatEntity.GetHealed += GetHealed;

        
        //todo base it off of level
        _equipment = EC.CreateAllEquipment(10);
        //_weapons = EC.CreateAllWeapons(10);
        //_spellScrolls = EC.CreateAllSpellScrolls(10);
        UpdateStats();
        _currentHealth = _maxHealth;

        if (isPlayerCharacter)
        {
            _weapons.Add(EC.CreateWeapon(5,1,Equipment.Slot.OneHander, Weapon.SpellTypes.Blood3));
            _weapons.Add(EC.CreateWeapon(5,2,Equipment.Slot.OneHander, Weapon.SpellTypes.Blood4));
            _spellScrolls.Add(EC.CreateSpellScroll(5,1,Weapon.SpellTypes.Nature1));
            _spellScrolls.Add(EC.CreateSpellScroll(5,1,Weapon.SpellTypes.Axe2));
        }
        else
        {
            _weapons = EC.CreateAllWeapons(1);
            _spellScrolls = EC.CreateAllSpellScrolls(1);
            // _weapons.Add(EC.CreateWeapon(5,1,Equipment.Slot.OneHander, Weapon.SpellTypes.Nature4));
            // _weapons.Add(EC.CreateWeapon(5,2,Equipment.Slot.OneHander, Weapon.SpellTypes.Ice2));
            // _spellScrolls.Add(EC.CreateSpellScroll(5,1,Weapon.SpellTypes.Axe2));
            // _spellScrolls.Add(EC.CreateSpellScroll(5,1,Weapon.SpellTypes.Fire2));
        }
    }
    
    private void OnDestroy()
    {
        CombatTrigger.TriggerCombat -= ActivateCombatEntity;
        CombatTrigger.EndCombat -= DeactivateCombatEntity;
        CombatEntity.GetHitWithAttack -= GetHitWithAttack;
        CombatEntity.GetHealed -= GetHealed;
        
        CombatEntity.GetHitWithBuff -= GetHitWithBuff;
        CombatEntity.GetHitWithDeBuff -= GetHitWithDeBuff;
    }

    private void GetHitWithBuff(Character c, CombatEntity.BuffTypes buff, int turns, float amount)
    {
        if(c != this)
            return;

        int i = GetIndexOfBuff(buff);
        switch (buff)
        {
            case CombatEntity.BuffTypes.Block:
                if (i == -1)
                {
                    Buffs.Add((buff,turns,amount));
                    UpdateBlock(this, Mathf.RoundToInt(amount), Mathf.RoundToInt(amount));

                }
                else
                {
                    Buffs[i] = (buff,turns, amount + Buffs[i].Item3);
                    UpdateBlock(this, Mathf.RoundToInt(Buffs[i].Item3), Mathf.RoundToInt(amount));
                
                    if (Buffs[i].Item3 <=0)
                    {
                        Buffs.RemoveAt(i);
                    }
                

                }
                break;
            case CombatEntity.BuffTypes.Rejuvenate:
                Buffs.Add((buff,turns,amount));
                break;
            case CombatEntity.BuffTypes.Thorns:
                Buffs.Add((buff,turns,amount));
                break;
            case CombatEntity.BuffTypes.Immortal:
                if (i == -1)
                {
                    Buffs.Add((buff,turns,amount));
                }
                else
                {
                    Buffs[i] = (buff,turns + Buffs[i].Item2, amount);
                }
                break;
            case CombatEntity.BuffTypes.Invulnerable:
                if (i == -1)
                {
                    Buffs.Add((buff,turns,amount));
                }
                else
                {
                    Buffs[i] = (buff,turns + Buffs[i].Item2, amount);
                }
                break;
            case CombatEntity.BuffTypes.Momentum:
                if (i == -1)
                {
                    Buffs.Add((buff,turns,amount));
                }
                else
                {
                    if (amount > Buffs[i].Item3)
                    {
                        Buffs[i] = (buff,turns + Buffs[i].Item2, amount);
                    }
                    else
                    {
                        Buffs[i] = (buff,turns + Buffs[i].Item2, Buffs[i].Item3);

                    }
                }
                break;
        }
        
        
        


    }

   

    public int GetIndexOfBuff(CombatEntity.BuffTypes buff)
    {
        int i = -1;
        for (int j = 0; j < Buffs.Count; j++)
        {
            if (Buffs[j].Item1 == buff)
            {
                i = j;
                break;
            }
        }

        return i;
    }
    public int GetIndexOfDebuff(CombatEntity.DeBuffTypes debuff)
    {
        int i = -1;
        for (int j = 0; j < DeBuffs.Count; j++)
        {
            if (DeBuffs[j].Item1 == debuff)
            {
                i = j;
                break;
            }
        }

        return i;
    }
    
    private void GetHitWithDeBuff(Character c, CombatEntity.DeBuffTypes deBuff, int turns, float amount)
    {
        if(c != this)
            return;

        int i = GetIndexOfDebuff(deBuff);
        switch (deBuff)
        {
            case CombatEntity.DeBuffTypes.Bleed:
                DeBuffs.Add((deBuff,turns,amount));
                break;
            case CombatEntity.DeBuffTypes.Burn:
                DeBuffs.Add((deBuff,turns,amount));
                break;
            case CombatEntity.DeBuffTypes.Chilled:
                //check if we already have it
                if (i != -1)
                {
                    DeBuffs[i] = (deBuff, DeBuffs[i].Item2 + turns, amount);
                }
                else
                {
                    DeBuffs.Add((deBuff,turns,amount));

                }
                break;
            case CombatEntity.DeBuffTypes.Weakened:
                //check if we already have it
                if (i != -1)
                {
                    DeBuffs[i] = (deBuff, DeBuffs[i].Item2 + 1, amount);
                }
                else
                {
                    DeBuffs.Add((deBuff,turns,amount));

                }
                break;
            case CombatEntity.DeBuffTypes.Exposed:
                //check if we already have it
                if (i != -1)
                {
                    DeBuffs[i] = (deBuff, DeBuffs[i].Item2 + 1, amount);
                }
                else
                {
                    DeBuffs.Add((deBuff,turns,amount));

                }
                break;
            case CombatEntity.DeBuffTypes.Wound:
                //check if we already have it
                if (i != -1)
                {
                    DeBuffs[i] = (deBuff, DeBuffs[i].Item2 + 1, amount);
                }
                else
                {
                    DeBuffs.Add((deBuff,turns,amount));

                }
                break;

        }
        
        
        
    }

    private void GetHealed(Character c, int healAmount)
    {
        if(c != this)
            return;

        _currentHealth += healAmount;
        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
    }

    private void GetHitWithAttack(Character c, CombatEntity.AbilityTypes abilityTypes, int amount, int reduction = 0)
    {
        if(c != this)
            return;
        // take damage
        // possibly die
        _currentHealth -= amount;
        if (_currentHealth <= 0)
        {
            // die
            _currentHealth = 0;
            Debug.Log("DEFEAT");
        }
    }

    private void ActivateCombatEntity(Character player, Character enemy)
    {
        if (player == this)
        {
            _combatEntity.enabled = true;
            _combatEntity.Target = enemy._combatEntity;
        }
        else if (enemy == this)
        {
            _combatEntity.enabled = true;
            _combatEntity.Target = player._combatEntity;
            
            //activate friends and set their target to the player
        }
        
    }
    private void DeactivateCombatEntity()
    {
        if (_combatEntity.enabled == true)
        {
            _combatEntity.enabled = false;
        }
        
    }


    private void UpdateStats()
    {
        _stats = new Dictionary<Equipment.Stats, int>();

        foreach (Equipment e in _equipment)
        {
            foreach (var stat in e.stats)
            {
                if (_stats.ContainsKey(stat.Key))
                {
                    _stats[stat.Key] += stat.Value;
                }
                else
                {
                    _stats.Add(stat.Key, stat.Value);
                }
            }
        }
        
        // max health = 50 * level + 50 + hp from stats
        SetMaxHealth();
        
        
        PrettyPrintStats();
    }

    private void SetMaxHealth()
    {
        int hp = 50 * _level + 50;
        int hpFromStats = 0;
        _stats.TryGetValue(Equipment.Stats.Health, out hpFromStats);
        hp += hpFromStats;
        _maxHealth = hp;
        
    }
    
    private void PrettyPrintStats()
    {
        string Output = "";
        Output += name + "\n";
        Output += GetWeaponSpellsNames();
        //Output += "Level: " + level + "\n";
        foreach (KeyValuePair<Equipment.Stats,int> kvp in _stats)
        {
            Output += kvp.Key.ToString() + ": " + kvp.Value + "\n";

        }
        
        Debug.Log(Output);
    }
    
    
    public string GetWeaponSpellsNames()
    {

        string printString = "Spells:\n";

        switch (_weapons.Count)
        {
            case 0:
                break;
            case 1:
                if (_weapons[0].slot == Equipment.Slot.TwoHander)
                {
                    printString += _weapons[0].GetSpellTypes().Item1 +"\n";
                    printString += _weapons[0].GetSpellTypes().Item2 +"\n";

                    
                }
                else
                {
                    printString += _weapons[0].GetSpellTypes().Item1+"\n";
                }
                break;
            case 2:
                printString += _weapons[0].GetSpellTypes().Item1 +"\n";
                printString += _weapons[1].GetSpellTypes().Item1+"\n";
                break;
                
                
        }

        return printString;
    }
    
    
    
    

    
}
