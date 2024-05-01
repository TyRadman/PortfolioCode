using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemName
{
    startingMedicine,
    startingKey,
    Battery,
    key,
    mainKey,
    energyDrink,
    none,
    FlashLight,
    HealthPotion,
    SanityPotion
}

public class ItemsManager : MonoBehaviour
{
    public static ItemsManager Instance;

    [System.Serializable]
    public class ItemSet
    {
        public ItemName ItemTag;
        List<I_ObservedObject> Items;
    }

    public List<ItemSet> TheItems;
    public Dictionary<ItemName, List<i_ObjectiveItem>> Items = new Dictionary<ItemName, List<i_ObjectiveItem>>();

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerItemWithTag(ItemName _tag)
    {
        if(!Items.ContainsKey(_tag))
        {
            print($"Item {_tag} wasn't created by items manager");
            return;
        }

        Items[_tag].ForEach(i => i.IsObjective = true);
    }
}