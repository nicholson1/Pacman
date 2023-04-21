using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using ImportantStuff;
using UnityEngine;

public class EquipmentModelManager : MonoBehaviour
{
    [SerializeField] private GameObject[] HeadModels;
    [SerializeField] private GameObject[] ChestModels;
    [SerializeField] private GameObject[] ShoulderModels;
    [SerializeField] private GameObject[] GloveModels;
    [SerializeField] private GameObject[] BootsModels;
    
    [SerializeField] private GameObject[] Faces;
    [SerializeField] private GameObject[] Hair;

    [SerializeField] private GameObject[] LeftHandWep;
    [SerializeField] private GameObject[] RightHandWep;

    // [SerializeField] private GameObject[] Hammers;
    // [SerializeField] private GameObject[] Swords;
    // [SerializeField] private GameObject[] Axes;
    // [SerializeField] private GameObject[] Sheilds;
    // [SerializeField] private GameObject[] Wands;
    // [SerializeField] private GameObject[] Orbs;
    // [SerializeField] private GameObject[] Daggers;

    private int rightHandIndex = 0;
    private int LeftHandIndex = 0;


    private int headIndex = 0;
    private int shoulderIndex = 0;
    private int chestIndex = 0;
    private int gloveIndex = 0;
    private int bootIndex = 0;
    private int faceIndex = 0;
    private int hairIndex = 0;

    private bool showHelm = true;
    

    public void RandomCharacter()
    {
        faceIndex = Random.Range(0, Faces.Length);
        Faces[0].SetActive(false);
        Faces[faceIndex].SetActive(true);
        
        hairIndex = Random.Range(0, Hair.Length);
        Hair[0].SetActive(false);
        Hair[hairIndex].SetActive(true);
        
        // 1/4 chance they done show helm
        int roll = Random.Range(0, 4);
        if (roll == 0)
        {
            showHelm = false;
        }


    }

    public void UpdateHead()
    {
        HeadModels[headIndex].SetActive(!HeadModels[headIndex].activeSelf);
        
        Hair[hairIndex].SetActive(!Hair[hairIndex].activeSelf);
    }


    public void UpdateSlot(Equipment equipment,  bool remove = false)
    {
        int newIndex = equipment.modelIndex;

        if (remove)
        {
            newIndex = 0;
        }
        switch (equipment.slot)
        {
            case Equipment.Slot.Head:

                if (showHelm == false)
                {
                    HeadModels[headIndex].SetActive(false);
                    Hair[hairIndex].SetActive(true);
                    break;
                }

                if (newIndex > 10 || newIndex == 0)
                {
                    Hair[hairIndex].SetActive(true);
                }
                else
                {
                    Hair[hairIndex].SetActive(false);
                }
                // need to deal with hair
                HeadModels[headIndex].SetActive(false);
                headIndex = newIndex;
                HeadModels[headIndex].SetActive(true);
                break;
            case Equipment.Slot.Shoulders:
                ShoulderModels[shoulderIndex].SetActive(false);
                shoulderIndex = newIndex;
                ShoulderModels[shoulderIndex].SetActive(true);
                break;
            case Equipment.Slot.Chest:
                ChestModels[chestIndex].SetActive(false);
                chestIndex = newIndex;
                ChestModels[chestIndex].SetActive(true);
                break;
            case Equipment.Slot.Gloves:
                GloveModels[gloveIndex].SetActive(false);
                gloveIndex = newIndex;
                GloveModels[gloveIndex].SetActive(true);
                break;
            case Equipment.Slot.Boots:
                BootsModels[bootIndex].SetActive(false);
                bootIndex = newIndex;
                BootsModels[bootIndex].SetActive(true);
                break;

        }
    }

    public void UpdateWeapon(Equipment w1, Equipment w2)
    {
        
        
        Weapon wep1 = (Weapon)w1;
        Weapon wep2 = (Weapon)w2;

        int newIndex1 = 0;
        int newIndex2 = 0;
        
        
        //figured out which one is in which hand
        
        // put the hammer in hand 1
        // put the shield in hand 2
        if (wep2 != null)
        {
            if (wep2.spellType1 == Weapon.SpellTypes.Hammer2 || wep2.spellType1 == Weapon.SpellTypes.Hammer3 || wep2.spellType1 == Weapon.SpellTypes.Axe3)
            {
                (wep1, wep2) = (wep2, wep1);
                //Debug.Log("put hammer in right hand");
                //Debug.Log(wep1 + " " + wep2);
            }

            //newIndex1 = wep1.modelIndex;
        }

        if (wep1 != null)
        {
            
            if (wep1.spellType1 == Weapon.SpellTypes.Shield2 || wep1.spellType1 == Weapon.SpellTypes.Shield3)
            {
                (wep1, wep2) = (wep2, wep1);

            }
            //newIndex2 = wep2.modelIndex;

        }

        if (wep2 != null)
        {
            newIndex2 = wep2.modelIndex;
            Debug.Log(wep2.name + " " + newIndex2);


        }

        if (wep1 != null)
        {
            newIndex1 = wep1.modelIndex;
            Debug.Log(wep1.name + " " + newIndex1);


        }

        
        LeftHandWep[LeftHandIndex].SetActive(false);
        RightHandWep[rightHandIndex].SetActive(false);

        LeftHandIndex = newIndex2;
        rightHandIndex = newIndex1;
        
        LeftHandWep[LeftHandIndex].SetActive(true);
        RightHandWep[rightHandIndex].SetActive(true);

    }
    
    public void FaceButton(int direction)
    {
        Faces[faceIndex].SetActive(false);
        faceIndex += direction;
        if (faceIndex > Faces.Length - 1 )
        {
            faceIndex = 0;
        }else if (faceIndex < 0)
        {
            faceIndex = Faces.Length - 1;
        }
        Faces[faceIndex].SetActive(true);
    }
    
    public void HairButton(int direction)
    {
        Hair[hairIndex].SetActive(false);
        hairIndex += direction;
        if (hairIndex > Hair.Length - 1 )
        {
            hairIndex = 0;
        }else if (hairIndex < 0)
        {
            hairIndex = Hair.Length - 1;
        }
        Hair[hairIndex].SetActive(true);
    }
    
    
}
