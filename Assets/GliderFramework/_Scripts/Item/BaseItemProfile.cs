using UnityEngine;

[CreateAssetMenu(fileName = "BaseItemProfile", menuName = "GliderFramework/BaseItemProfile", order = 10)]
public class BaseItemProfile : ScriptableObject
{
    public int itemID;
    public string itemName;
    [TextArea(2, 6)] public string itemDescription;
    public int itemCost;
    public ItemType itemType;
}


public enum ItemType
{
    UNSPECIFIED = 0,
    WEAPON = 1000,
    UTILITY = 2000
}
