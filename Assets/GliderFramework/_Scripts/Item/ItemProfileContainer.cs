using System.Collections.Generic;
using UnityEngine;

public class ItemProfileContainer : MonoBehaviour
{
    [SerializeField] List<BaseItemProfile> itemProfiles = new List<BaseItemProfile>();
    Dictionary<int, BaseItemProfile> itemProfilesLookupTable = new Dictionary<int, BaseItemProfile>();
    [SerializeField] ItemType itemType;

    private void Awake() 
    {
        if (itemType != ItemType.UNSPECIFIED) RemoveInvalidItems();
        
        foreach (BaseItemProfile item in itemProfiles)
        {
            itemProfilesLookupTable.Add(item.itemID, item);
        }
    }

    private void RemoveInvalidItems()
    {
        for (int i = itemProfiles.Count - 1; i >= 0; i--)
        {
            if (itemProfiles[i].itemType != itemType) itemProfiles.RemoveAt(i);
        }
    }

    public BaseItemProfile GetProfile(int profileID) => itemProfilesLookupTable.GetValueOrDefault(profileID);
}
