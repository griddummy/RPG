using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StorageSystem : MonoBehaviour {

	[SerializeField]
	public int slotsInTotal;

	//[SerializeField]
	//public ItemType[] itemTypeOfSlots = new ItemType[999];

	//[SerializeField]
	//public GameObject inventory;

	//[SerializeField]
	//public List<Item> storageItems = new List<Item>();

	//[SerializeField]
	//private ItemDataBaseList itemDatabase;

	//private InputManager inputManagerDatabase;

	//GameObject player;

	//Inventory inv;

	//public int itemAmount;

	//public void addItemToStorage(int id, int value)
	//{
		//Item item = itemDatabase.getItemByID(id);
	//	item.itemValue = value;
	//	storageItems.Add(item);
	//}
	// Use this for initialization

	void Start () {
		/*if (inputManagerDatabase == null)
			inputManagerDatabase = (InputManager)Resources.Load("InputManager");
		
		player = GameObject.FindGameObjectWithTag("Player");
		inv = inventory.GetComponent<Inventory>();
		ItemDataBaseList inventoryItemList = (ItemDataBaseList)Resources.Load("ItemDatabase");

		int creatingItemsForChest = 1;

		int randomItemAmount = Random.Range(1, itemAmount);

		while (creatingItemsForChest < randomItemAmount)
		{
			int randomItemNumber = Random.Range(0, inventoryItemList.itemList.Count - 1);
			//int raffle = Random.Range(1, 100);

			//if (raffle <= inventoryItemList.itemList[randomItemNumber].rarity)
			//{
				//int randomValue = Random.Range(1, inventoryItemList.itemList[randomItemNumber].getCopy().maxStack);
				Item item = inventoryItemList.itemList[randomItemNumber].getCopy();
				//item.itemValue = randomValue;
				storageItems.Add(item);
				creatingItemsForChest++;
			    
			//}
		}
		*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void getSlotsInTotal()
	{
		Inventory inv = GetComponent<Inventory>();
		slotsInTotal = inv.width * inv.height;
	}
}
