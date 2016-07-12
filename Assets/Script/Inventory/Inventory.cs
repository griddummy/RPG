using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//dddddddddddddddddddddddddddddddddddddddddd

public class Inventory : MonoBehaviour {

	[SerializeField]
	private GameObject SlotContainer;

	[SerializeField]
	public List<Item> ItemsInInventory = new List<Item>();

	[SerializeField]
	public int height;

	[SerializeField]
	public int width;

	[SerializeField]
	private GameObject prefabItem;

	private InputManager inputManagerDatabase;

	[SerializeField]
	private ItemDataBaseList itemDatabase;

	//GameObject player;

	public int itemAmount;
	public bool stackable = true;

	[SerializeField]
	private int positionNumberX = 16;
	[SerializeField]
	private int positionNumberY = 17;


	public delegate void ItemDelegate(Item item);
	public static event ItemDelegate ItemConsumed;
	public static event ItemDelegate ItemEquip;
	public static event ItemDelegate UnEquipItem;

	// Use this for initialization
    void Awake()
    {
        if (transform.GetComponent<HotBar>() == null)
            this.gameObject.SetActive(false);

        inputManagerDatabase = (InputManager)Resources.Load("InputManager");
        itemDatabase = (ItemDataBaseList)Resources.Load("ItemDatabase");

        setImportantVariables();
        GetPrefab();

        setDefaultSettings();

        updateItemList();
    }
	void Start () {
		


		
	}
	

	void Update () {
	
	}
	public void openInventory(){
		this.gameObject.SetActive(true);

	}
	public void closeInventory()
	{
		this.gameObject.SetActive(false);

	}
	void setImportantVariables(){
		SlotContainer = transform.GetChild(1).gameObject;

	}
	public void updateItemList()
	{
		ItemsInInventory.Clear();

		for (int i = 0; i < SlotContainer.transform.childCount; i++)
		{
			Transform trans = SlotContainer.transform.GetChild(i);
			if (trans.childCount != 0)
			{
				ItemsInInventory.Add(trans.GetChild(0).GetComponent<ItemOnObject>().item);
				//print (trans.GetChild (0).GetComponent<ItemOnObject> ().item.itemName);
			}
		}

	}
	public void setDefaultSettings(){
		if (GetComponent<EquipmentSystem> () == null && GetComponent<HotBar> () == null && GetComponent<StorageSystem>()== null) {
			height = 4;
			width = 4;

            //random item
            //RandomItem();
            //addAllItemsToInventory();

        } else if (GetComponent<HotBar> () != null) {
			height = 1;
			width = 9;

		} else if (GetComponent<EquipmentSystem> () != null) {
			print ("EEEEEEeeeeeeeee");
			height = 4;
			width = 2;

		} else if (GetComponent <StorageSystem> () != null) {
			height = 5;
			width = 5;
		}
	}
    public void setRandomItemTest()
    {
        if (GetComponent<EquipmentSystem>() == null && GetComponent<HotBar>() == null && GetComponent<StorageSystem>() == null)
        {
            RandomItem();
            addAllItemsToInventory();
        }
    }
	public void GetPrefab(){
		if (prefabItem == null)
			prefabItem = Resources.Load("Prefabs/Item") as GameObject;
	}
	public void RandomItem(){
        
        int creatingItemsForChest = 1;

		int ItemAmount = height * width;

		while (creatingItemsForChest <= ItemAmount) {
			int randomItemNumber = Random.Range (0, itemDatabase.itemList.Count - 1);

			Item item = itemDatabase.itemList [randomItemNumber].getCopy ();

			int randomValue = Random.Range(1, itemDatabase.itemList[randomItemNumber].getCopy().maxStack);
			item.itemValue = randomValue;

			ItemsInInventory.Add (item);
			creatingItemsForChest++;
		}
	}
	public void addAllItemsToInventory()
	{
		for (int k = 0; k < ItemsInInventory.Count; k++)
		{
			for (int i = 0; i < SlotContainer.transform.childCount; i++)
			{
				if (SlotContainer.transform.GetChild(i).childCount == 0)
				{
					GameObject item = (GameObject)Instantiate(prefabItem);

					item.GetComponent<ItemOnObject>().item = ItemsInInventory[k];
					item.transform.SetParent(SlotContainer.transform.GetChild(i));

					item.GetComponent<RectTransform>().localPosition = Vector3.zero;
					item.transform.GetChild(0).GetComponent<Image>().sprite = ItemsInInventory[k].itemIcon;

					break;
				}
			}
		}
		stackableSettings();
	}
	public void stackableSettings()
	{
		for (int i = 0; i < SlotContainer.transform.childCount; i++)
		{
			if (SlotContainer.transform.GetChild(i).childCount > 0)
			{
				ItemOnObject item = SlotContainer.transform.GetChild(i).GetChild(0).GetComponent<ItemOnObject>();
				if (item.item.maxStack > 1)
				{
					RectTransform textRectTransform = SlotContainer.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<RectTransform>();
					Text text = SlotContainer.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>();
					text.text = "" + item.item.itemValue;
					text.enabled = stackable;
					textRectTransform.localPosition = new Vector3(positionNumberX, positionNumberY, 0);
				}
				else
				{
					Text text = SlotContainer.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>();
					text.enabled = false;
				}
			}
		}

	}
	public bool checkIfItemAllreadyExist(int itemID, int itemValue)
	{
		updateItemList();
		int stack;
		for (int i = 0; i < ItemsInInventory.Count; i++)
		{
			if (ItemsInInventory[i].itemID == itemID)
			{
				stack = ItemsInInventory[i].itemValue + itemValue;
				if (stack <= ItemsInInventory[i].maxStack)
				{
					ItemsInInventory[i].itemValue = stack;
					GameObject temp = getItemGameObject(ItemsInInventory[i]);
					if (temp != null && temp.GetComponent<ConsumeItem>().duplication != null)
						temp.GetComponent<ConsumeItem>().duplication.GetComponent<ItemOnObject>().item.itemValue = stack;
					return true;
				}
			}
		}
		return false;
	}
	public GameObject getItemGameObject(Item item)
	{
		for (int k = 0; k < SlotContainer.transform.childCount; k++)
		{
			if (SlotContainer.transform.GetChild(k).childCount != 0)
			{
				GameObject itemGameObject = SlotContainer.transform.GetChild(k).GetChild(0).gameObject;
				Item itemObject = itemGameObject.GetComponent<ItemOnObject>().item;
				if (itemObject.Equals(item))
				{
					return itemGameObject;
				}
			}
		}
		return null;
	}
	public void addItemToInventory(int id)
	{
		for (int i = 0; i < SlotContainer.transform.childCount; i++)
		{
			if (SlotContainer.transform.GetChild(i).childCount == 0)
			{
				GameObject item = (GameObject)Instantiate(prefabItem);
				item.GetComponent<ItemOnObject>().item = itemDatabase.getItemByID(id);
				item.transform.SetParent(SlotContainer.transform.GetChild(i));
				item.GetComponent<RectTransform>().localPosition = Vector3.zero;
				item.transform.GetChild(0).GetComponent<Image>().sprite = item.GetComponent<ItemOnObject>().item.itemIcon;

				break;
			}
		}

		stackableSettings();
		updateItemList();

	}
	public GameObject addItemToInventory(int id, int value)
	{
		for (int i = 0; i < SlotContainer.transform.childCount; i++)
		{
			if (SlotContainer.transform.GetChild(i).childCount == 0)
			{
				GameObject item = (GameObject)Instantiate(prefabItem);
				ItemOnObject itemOnObject = item.GetComponent<ItemOnObject>();
				itemOnObject.item = itemDatabase.getItemByID(id);

				if (itemOnObject.item.itemValue <= itemOnObject.item.maxStack && value <= itemOnObject.item.maxStack)
					itemOnObject.item.itemValue = value;
				else
					itemOnObject.item.itemValue = 1;

				item.transform.SetParent(SlotContainer.transform.GetChild(i));
				item.GetComponent<RectTransform>().localPosition = Vector3.zero;
				item.transform.GetChild(0).GetComponent<Image>().sprite = itemOnObject.item.itemIcon;

				return item;
			}
		}

		stackableSettings();
		updateItemList();
		return null;

	}
	public void deleteItemFromInventory(Item item)
	{
		for (int i = 0; i < ItemsInInventory.Count; i++)
		{
			if (item.Equals(ItemsInInventory[i]))
				ItemsInInventory.RemoveAt(i);
		}
	}
	public void ConsumeItem(Item item)
	{
		if (ItemConsumed != null)
			ItemConsumed(item);
	}

	public void EquiptItem(Item item)
	{
		if (ItemEquip != null)
			ItemEquip(item);
	}

	public void UnEquipItem1(Item item)
	{
		if (UnEquipItem != null)
			UnEquipItem(item);
	}
}
