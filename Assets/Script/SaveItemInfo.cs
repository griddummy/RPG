using UnityEngine;
using System.Collections;
[System.Serializable]
public class SaveItemInfo
{
    public int itemID;      // 고유 ID
    public int itemValue;   // 아이템 갯수

    public Item getItem(ItemDataBaseList db)
    {        
        foreach(Item item in db.itemList)
        {
            if(item.itemID == itemID)
            {
                Item newItem = item.getCopy();
                newItem.itemValue = itemValue;
                return newItem;
            }
        }
        return null;
    }
}