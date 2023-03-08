using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager _instance;
    [SerializeField] private SelectionItem selectionItemPrefab;

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
        // get 4 random ints 0-c.equip.count
        List<int> index = new List<int>();
        while (index.Count < 4)
        {
            int temp = Random.Range(0, c._equipment.Count);
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
        
    }
}
