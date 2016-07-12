using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]

public class Item
{
	public string itemName;
	public int itemID;
	public Sprite itemIcon;
	public GameObject itemModel;
	public int itemValue = 1;
	public ItemType itemType;
	public int maxStack = 1;

	[SerializeField]
	public List<ItemAttribute> itemAttributes = new List<ItemAttribute>();   

	public Item(){
	}
	public Item(string name,int id,Sprite icon,GameObject model, ItemType type, int maxStack, List<ItemAttribute> itemAttributes){
		itemName = name;
		itemID = id;
		itemIcon = icon;
		itemModel = model;
		itemType = type;
		this.maxStack = maxStack;
		this.itemAttributes = itemAttributes;
	}
	public Item getCopy()
	{
		return (Item)this.MemberwiseClone();        
	}   
}
