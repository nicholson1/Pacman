using System;
using System.Collections;
using System.Collections.Generic;
using ImportantStuff;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private SelectionItem selectionItemPrefab;

    
    public int selectionsLeft = 2;
    public static SelectionManager _instance;
    public MultiImageButton SkipButton;

    private bool startingSelections = true;

    private int startingSelectionCount = 4;
    [SerializeField] private EquipmentCreator EC;
    [SerializeField] private GameObject BeginAdventureButton;
    [SerializeField] private GameObject inventoryButton;
    [SerializeField] private GameObject selectionScreen;
    [SerializeField] private TextMeshProUGUI selectionText;


    [SerializeField] public LootButtonManager LootManager;
    [SerializeField] public Image Background;


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
    
    public void RandomSelectionFromEquipment(Character c)
    {
        SkipButton.gameObject.SetActive(true);
        // get 4 random ints 0-c.equip.count
        List<int> index = new List<int>();
        // force a spell or weapon that has not been selected
        int forcedWep = Random.Range(c._equipment.Count - 4, c._equipment.Count);

        index.Add(forcedWep);
        
        // fill the rest with 3 random
        while (index.Count < 4)
        {
            int temp = Random.Range(0, c._equipment.Count);

            if (c._equipment[temp].canBeLoot == false)
            {
                continue;
            }
            
            if (!index.Contains(temp))
            {
                index.Add(temp);
            }
        }
        
        
        foreach (var i in index)
        {
            SelectionItem item = Instantiate(selectionItemPrefab, this.transform);
            item.InitializeSelectionItem(c._equipment[i]);
        }

        StartCoroutine(FadeImage(.75f));
    }

    public void SelectionsFromList(List<Equipment> equipments)
    {
        SkipButton.gameObject.SetActive(true);
        foreach (var i in equipments)
        {
            SelectionItem item = Instantiate(selectionItemPrefab, this.transform);
            item.InitializeSelectionItem(i);
        }
        StartCoroutine(FadeImage(.75f));

    }

    public void SelectionMade(SelectionItem si)
    {
        //todo pool these
        
        si.DisableButtons();
        selectionsLeft -= 1;
        if (selectionsLeft <= 0)
        {
            SelectionItem[] selectionItems = GetComponentsInChildren<SelectionItem>();
            foreach (var i in selectionItems)
            {
                i.DisableButtons();
            }
        }
    }

    public void ClearSelections()
    {
        
        
        SelectionItem[] selectionItems = GetComponentsInChildren<SelectionItem>();
        foreach (var si in selectionItems)
        {
            if (si.isFlipping)
            {
                return;
            }
        }
        //selectionsLeft = 10;
        for (int i = selectionItems.Length -1; i >= 0; i--)
        {
            Destroy(selectionItems[i].gameObject);
        }
        

        if (selectionsLeft == 2)
        {
           // UIController._instance.ToggleInventoryUI();

        }
        selectionsLeft = 2;
        SkipButton.gameObject.SetActive(false);


        //UIController._instance.ToggleLootUI();
        selectionScreen.SetActive(false);
        //CombatController._instance.NextCombatButton.gameObject.SetActive(true);
        StartCoroutine(FadeImage(0f));

    }

    public void CreateEquipmentListsStart()
    {
        List<List<Equipment>> equipments = new List<List<Equipment>>();

        List<Equipment> selection1 = new List<Equipment>();
        List<Equipment> selection2 = new List<Equipment>();

        List<Equipment> selection3 = new List<Equipment>();
        List<Equipment> selection4 = new List<Equipment>();

        int level = CombatController._instance.Player._level;
        selection1.Add(EC.CreateRandomWeaponWithSpell(level, Weapon.SpellTypes.Shield2));
        // present 4 spells
        int spellCount = 1;
        while (spellCount < 4)
        {
            Equipment eq = EC.CreateRandomWeapon(level, false);

            if (spellCount == 3)
            {
                if (!HasDamageSpell(selection1))
                {
                    eq = EC.CreateWeapon(level, 0, Equipment.Slot.OneHander,
                        (Weapon.SpellTypes)GetRandomDamagePhysicalSpellInt());
                }
            }

            Weapon w = (Weapon)eq;

            bool hasSpell = false;
            foreach (var equipment in selection1)
            {
                Weapon wep = (Weapon)equipment;
                if (wep.spellType1 == w.spellType1)
                {
                    hasSpell = true;
                }
            }

            if (hasSpell == false)
            {
                selection1.Add(eq);
                spellCount += 1;
            }
        }

        equipments.Add(selection1);
            ///////////////////////////////////////////////////////////////////////////////////

            spellCount = 0;
            selectionText.text = "Selection (2/4)";
            while (spellCount < 4)
            {
                Equipment eq = EC.CreateRandomSpellScroll(level);

                if (spellCount == 3)
                {
                    if (!HasDamageSpell(selection2))
                    {
                        eq = EC.CreateSpellScroll(level, 0, (Weapon.SpellTypes)GetRandomDamageSpellInt());
                    }
                }

                Weapon w = (Weapon)eq;

                bool hasSpell = false;
                foreach (var equipment in selection2)
                {
                    Weapon wep = (Weapon)equipment;
                    if (wep.spellType1 == w.spellType1)
                    {
                        hasSpell = true;
                    }
                }

                if (hasSpell == false)
                {
                    selection2.Add(eq);
                    spellCount += 1;
                }
            }

            equipments.Add(selection2);
            ///////////////////////////////////////////////////////////////////////////////////
            selection3.Add(EC.CreateArmor(level, Equipment.Slot.Head));
            selection3.Add(EC.CreateArmor(level, Equipment.Slot.Shoulders));
            selection3.Add(EC.CreateArmor(level, Equipment.Slot.Chest));
            selection3.Add(EC.CreateArmor(level, (Equipment.Slot)Random.Range(0, 6)));
            equipments.Add(selection3);
            
            ///////////////////////////////////////////////////////////////////////////////////
            selection4.Add(EC.CreateArmor(level, Equipment.Slot.Gloves));
            selection4.Add(EC.CreateArmor(level, Equipment.Slot.Legs));
            selection4.Add(EC.CreateArmor(level, Equipment.Slot.Boots));
            selection4.Add(EC.CreateArmor(level, (Equipment.Slot)Random.Range(0, 6)));
            equipments.Add(selection4);

            foreach (var VARIABLE in equipments)
            {
                foreach (var v in VARIABLE)
                {
                    Debug.Log(v.name);
                }
            }


            LootManager.SetLootButtons(equipments, new List<int>(){25});
    }

    
    public void RandomSelectionBegging()
    {
        //get level from character
        int level = CombatController._instance.Player._level;
        if (!startingSelections)
        {
            return;
        }
        List<Equipment> equipments = new List<Equipment>();

        if (startingSelectionCount == 4)
        {
            // present 4 weapons, 1 must be a blocking shield
            UIController._instance.ToggleInventoryUI(1);
            selectionScreen.gameObject.SetActive(true);
            selectionText.text = "Selection (1/4)";
            inventoryButton.gameObject.SetActive(true);
            BeginAdventureButton.SetActive(false);
            equipments.Add(EC.CreateRandomWeaponWithSpell(level, Weapon.SpellTypes.Shield2));
            // present 4 spells
            int spellCount = 1;
            while (spellCount < 4)
            {
                Equipment eq = EC.CreateRandomWeapon(level, false);

                if (spellCount == 3)
                {
                    if (!HasDamageSpell(equipments))
                    {
                        eq = EC.CreateWeapon(level, 0, Equipment.Slot.OneHander,(Weapon.SpellTypes)GetRandomDamagePhysicalSpellInt());
                    }
                }
                
                Weapon w = (Weapon)eq;

                bool hasSpell = false;
                foreach (var equipment in equipments)
                {
                    Weapon wep = (Weapon)equipment;
                    if (wep.spellType1 == w.spellType1)
                    {
                        hasSpell = true;
                    }
                }

                if (hasSpell == false)
                {
                    equipments.Add(eq);
                    spellCount += 1;
                    //Debug.Log(spellCount);
                }

            }

        }

        else if (startingSelectionCount == 3)
        {
            // present 4 spells
            int spellCount = 0;
            selectionText.text = "Selection (2/4)";
            while (spellCount < 4)
            {
                Equipment eq = EC.CreateRandomSpellScroll(level);

                if (spellCount == 3)
                {
                    if (!HasDamageSpell(equipments))
                    {
                        eq = EC.CreateSpellScroll(level, 0, (Weapon.SpellTypes)GetRandomDamageSpellInt());
                    }
                }
                Weapon w = (Weapon)eq;

                bool hasSpell = false;
                foreach (var equipment in equipments)
                {
                    Weapon wep = (Weapon)equipment;
                    if (wep.spellType1 == w.spellType1)
                    {
                        hasSpell = true;
                    }
                }

                if (hasSpell == false)
                {
                    equipments.Add(eq);
                    spellCount += 1;
                }

            }
            //equipments.Add(EC.CreateRandomSpellScroll(level));
            //equipments.Add(EC.CreateRandomSpellScroll(level));

            
        }
        else if (startingSelectionCount == 2)
        {
            //head, shoulder, chest, random
            selectionText.text = "Selection (3/4)";
            equipments.Add(EC.CreateArmor(level, Equipment.Slot.Head));
            equipments.Add(EC.CreateArmor(level, Equipment.Slot.Shoulders));
            equipments.Add(EC.CreateArmor(level, Equipment.Slot.Chest));
            equipments.Add(EC.CreateArmor(level, (Equipment.Slot)Random.Range(0,6)));


        }
        else if (startingSelectionCount == 1)
        {
            //gloves, legs, boots, random
            selectionText.text = "Selection (4/4)";
            equipments.Add(EC.CreateArmor(level, Equipment.Slot.Gloves));
            equipments.Add(EC.CreateArmor(level, Equipment.Slot.Legs));
            equipments.Add(EC.CreateArmor(level, Equipment.Slot.Boots));
            equipments.Add(EC.CreateArmor(level, (Equipment.Slot)Random.Range(0,6)));
        }
        
        //SkipButton.gameObject.SetActive(true);
        // get 4 random ints 0-c.equip.count
        
        
        
        foreach (var e in equipments)
        {
            SelectionItem item = Instantiate(selectionItemPrefab, this.transform);
            item.InitializeSelectionItem(e);
        }

        startingSelectionCount -= 1;

        if (startingSelectionCount == 0)
        {
            startingSelections = false;
        }
        
        

    }
    
    public float fadeDuration = 1f;
    IEnumerator FadeImage(float targetAlpha)
    {
            
        Background.gameObject.SetActive(true);
        
        // Set the initial alpha value 
        float startingAlpha = Background.color.a;

        Color startColor = Background.color;
        Color endColor = startColor;
        endColor.a = targetAlpha;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // Calculate the current alpha value based on the elapsed time
            float alpha = Mathf.Lerp(startingAlpha, targetAlpha, elapsedTime / fadeDuration);

            // Set the alpha value of the image
            Background.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the alpha value is exactly 1 at the end
        Background.color = endColor;
        if (targetAlpha == 0)
        {
            Background.gameObject.SetActive(false);
        }
    }

    bool HasDamageSpell(List<Equipment> equipments)
    {
        bool hasDamage = false;
        foreach (var eq in equipments)
        {
            Weapon wep = (Weapon)eq;
            // get the spell
            int spellIndex = (int)wep.spellType1;
            List<List<object>> scaling = DataReader._instance.GetWeaponScalingTable();
            IList abilities = (IList)scaling[(int)spellIndex][4];

            if (abilities.Contains(0) || abilities.Contains(1))
            {
                hasDamage = true;
            }
        }

        return hasDamage;
    }

    int GetRandomDamageSpellInt()
    {
        int[] damageSpells = new[] { 17, 18, 21, 22, 23, 27, 28, 30, 31, 37 };
        return damageSpells[Random.Range(0, damageSpells.Length)];
    }
    int GetRandomDamagePhysicalSpellInt()
    {
        int[] damageSpells = new[] { 0,1,2,3,6,7,8,9,10,11,12,13,14 };
        return damageSpells[Random.Range(0, damageSpells.Length)];
    }
}
