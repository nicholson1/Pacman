using System;
using System.Collections;
using System.Collections.Generic;
using ImportantStuff;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] public Character c;
    
    public static EquipmentManager _instance;

    public TextMeshProUGUI levelText;

    [SerializeField] private DragItem _dragItemPrefab;
    [SerializeField] private InventorySlot[] InventorySlots;
    [SerializeField] private Transform inventoryTransform;
    //[SerializeField] private TextMeshProUGUI stats;
    public static event Action<ErrorMessageManager.Errors> InventoryNotifications;

    [SerializeField] private StatDisplay[] _statDisplays;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        Character.UpdateStatsEvent += UpdateStats;
    }
    private void OnDestroy()
    {
        Character.UpdateStatsEvent -= UpdateStats;
    }

    private void UpdateStats(Character c)
    {
        foreach (var kvp in c.GetStats())
        {
            if (kvp.Key != Equipment.Stats.Rarity && kvp.Key != Equipment.Stats.ItemLevel )
            {
                _statDisplays[((int)kvp.Key)-2].UpdateValues(kvp.Key, kvp.Value);
            }
        }

        levelText.text = "Level: " + c._level;
    }

    // private void UpdateStats(Character c)
    // {
    //     if (!c.isPlayerCharacter)
    //     {
    //         return;
    //     }
    //
    //     stats.text = "Stats:\n";
    //     stats.text += "Level: " + c._level+ "\n";
    //     stats.text += "Max HP: " + c._maxHealth + "\n";
    //
    //     foreach (var kvp in c.GetStats())
    //     {
    //         if (kvp.Key != Equipment.Stats.Rarity)
    //         {
    //             stats.text += kvp.Key + ": " + kvp.Value +"\n";
    //
    //         }
    //     }
    // }

    public void EquipItemFromSelection(Equipment e, SelectionItem si)
    {
        
        // loop through char inventory
        bool equippedSlot = false;
        foreach (var invSlot in InventorySlots)
        {
            //find the slot that has the item
            if (invSlot.Slot == e.slot)
            {
                if (invSlot.Item == null )
                {
                    DragItem di = Instantiate(_dragItemPrefab, inventoryTransform);
                    di.InitializeDragItem(e, invSlot);
                    //Debug.Log(c.GetStats()[Equipment.Stats.CritChance]);
                    c._equipment.Add(e);
                    if (e.isWeapon)
                    {
                        Weapon x = (Weapon) e;
                        if (x.slot == Equipment.Slot.Scroll)
                        {
                            c._spellScrolls.Add(x);
                        }
                        else
                        {
                            c._weapons.Add(x);
                        }
                    }
                    c.UpdateStats();
                    //Debug.Log(c.GetStats()[Equipment.Stats.CritChance]);

                    
                    si.RemoveSelection();
                    return;
                }
                
                InventorySlot slot = null;
                // check if we have an empty, if we do save that one
                for (int i = 10; i < InventorySlots.Length; i++)
                {
                    if (InventorySlots[i].Item == null)
                    {
                        slot = InventorySlots[i];
                        break;
                    }
                }

                if (slot == null)
                {
                    InventoryNotifications(ErrorMessageManager.Errors.InventoryFull);
                    return;
                }
                
                else
                {
                    //move current equiped item to inventory
                    //if weapon check slot + 1

                    if (e.isWeapon)
                    {
                        if (InventorySlots[Array.IndexOf(InventorySlots, invSlot) + 1].Item == null)
                        {
                            DragItem wep = Instantiate(_dragItemPrefab, inventoryTransform);
                            wep.InitializeDragItem(e, InventorySlots[Array.IndexOf(InventorySlots, invSlot) + 1]);
                            c._equipment.Add(e);
                            if (e.isWeapon)
                            {
                                Weapon x = (Weapon) e;
                                if (x.slot == Equipment.Slot.Scroll)
                                {
                                    c._spellScrolls.Add(x);
                                }
                                else
                                {
                                    c._weapons.Add(x);
                                }
                            }
                            c.UpdateStats();
                    
                            si.RemoveSelection();
                            return;
                        }
                    }

                    
                    DragItem equiped = invSlot.Item;
                    equiped.currentLocation = slot;
                    equiped._rectTransform.anchoredPosition = slot._rt.anchoredPosition;
                    equiped.currentLocation.Item = equiped;
                    slot.LabelCheck();
                    UnEquipItem(invSlot.Item.e);
                    //c._equipment.Remove(equiped.e);
                    
                    
                    
                    DragItem di = Instantiate(_dragItemPrefab, inventoryTransform);
                    di.InitializeDragItem(e, invSlot);
                    c._equipment.Add(e);
                    if (e.isWeapon)
                    {
                        Weapon eq = (Weapon)equiped.e;
                        Weapon x = (Weapon) e;
                        if (x.slot == Equipment.Slot.Scroll)
                        {
                            c._spellScrolls.Add(x);
                            c._spellScrolls.Remove(eq);
                        }
                        else
                        {
                            c._weapons.Add(x);
                            c._spellScrolls.Remove(eq);

                        }
                    }
                    c.UpdateStats();
                    
                    si.RemoveSelection();
                    return;

                }

                
            }
        }

        if (equippedSlot == false)
        {
            // no item in same slot
            c._equipment.Add(e);
            c._inventory.Remove(e);
            if (e.isWeapon)
            {
                Weapon x = (Weapon) e;
                if (x.slot == Equipment.Slot.Scroll)
                {
                    c._spellScrolls.Add(x);
                }
                else
                {
                    c._weapons.Add(x);
                }
            }
        }
        c.UpdateStats();

    }

    public void UnEquipItem(Equipment e)
    {
        c._equipment.Remove(e);
        if (!c._inventory.Contains(e))
        {
            c._inventory.Add(e);
        }

        if (e.isWeapon)
        {
            Weapon x = (Weapon) e;
            if (x.slot == Equipment.Slot.Scroll)
            {
                c._spellScrolls.Remove(x);
            }
            else
            {
                c._weapons.Remove(x);
            }
        }
        
        c.UpdateStats();

    }

    public void EquipFromInventory(Equipment e)
    {
        c._inventory.Remove(e);
        if (!c._equipment.Contains(e))
        {
            c._equipment.Add(e);
            if (e.isWeapon)
            {
                Weapon x = (Weapon) e;
                if (x.slot == Equipment.Slot.Scroll)
                {
                    c._spellScrolls.Add(x);
                }
                else
                {
                    c._weapons.Add(x);
                }
            }

        }
        c.UpdateStats();

    }
    public void DropItem(Equipment e)
    {
        c._inventory.Remove(e);
        c._equipment.Remove(e);
        
        if (e.isWeapon)
        {
            Weapon x = (Weapon) e;
            if (x.slot == Equipment.Slot.Scroll)
            {
                c._spellScrolls.Remove(x);
            }
            else
            {
                c._weapons.Remove(x);
            }
        }
        
        c.UpdateStats();
        
    }

    public void AddItemToInventoryFromSelection(Equipment e, SelectionItem si)
    {
        InventorySlot slot = null;
        // check if we have an empty, if we do save that one
        for (int i = 10; i < InventorySlots.Length; i++)
        {
            if (InventorySlots[i].Item == null)
            {
                slot = InventorySlots[i];
                break;
            }
        }

        if (slot == null)
        {
            InventoryNotifications(ErrorMessageManager.Errors.InventoryFull);
            return;
        }
        else
        {
            DragItem di = Instantiate(_dragItemPrefab, inventoryTransform);
            di.InitializeDragItem(e, slot);
            c._inventory.Add(e);
            
            si.RemoveSelection();

        }
    }

    private bool hasInitialized = false;

    public void InitializeEquipmentAndInventoryItems()
    {
        if (hasInitialized)
        {
            return;
        }
        bool placedWep = false;
        bool placedScroll = false;
        
        foreach (var e in c._equipment)
        {
            InventorySlot currentSlot = null;
            if (!e.isWeapon)
            {
                // figure out which slot
                currentSlot = InventorySlots[(int)e.slot];
            }
            else
            {
                if (e.slot == Equipment.Slot.Scroll)
                {
                    if (!placedScroll)
                    {
                        currentSlot = InventorySlots[8];
                        placedScroll = true;
                    }
                    else
                    {
                        currentSlot = InventorySlots[9];

                    }
                }
                else // if we wep
                {
                    if (!placedWep)
                    {
                        currentSlot = InventorySlots[6];
                        placedWep = true;
                    }
                    else
                    {
                        currentSlot = InventorySlots[7];
                    }
                    
                }
               
            }

            DragItem di = Instantiate(_dragItemPrefab, inventoryTransform);
            di.InitializeDragItem(e, currentSlot);

        }

        hasInitialized = true;
    }
    
    
}
