using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ConsumeItem : MonoBehaviour,IPointerDownHandler {
	public Item item;
	public ItemType[] itemTypeOfSlot;

	public static GameObject equipmentSystem;

	public GameObject duplication;
	public static GameObject mainInventory;
	public static GameObject storage;
	public static EquipmentSystem eS;


	void Start () {
		item = GetComponent<ItemOnObject>().item;

		if (GameManager.instance.getPlayerInventory().characterSystem!= null) {
			eS = GameManager.instance.getPlayerInventory().characterSystem.GetComponent<EquipmentSystem>();
			print ("EquipmentSystem");
		}
		    
		if (GameObject.FindGameObjectWithTag ("MainInventory") != null) {
			mainInventory = GameObject.FindGameObjectWithTag ("MainInventory");
			print ("MainInventory");
		}
		if (GameManager.instance.getPlayerInventory().storage != null) {
			storage = GameManager.instance.getPlayerInventory().storage;
			print ("storage");
		}

		if (eS != null)
			itemTypeOfSlot = eS.itemTypeOfSlots;

	}
	public void OnPointerDown(PointerEventData data)
	{
		Inventory inventory = transform.parent.parent.parent.GetComponent<Inventory>();
		bool gearable = false;

			if (data.button == PointerEventData.InputButton.Right) {
			
				for (int i = 0; i < eS.slotsInTotal; i++) {
					//장비 아이템 인가?? 소모성 아이템 인가
				if (itemTypeOfSlot [i].Equals (item.itemType)) {
					 
					//현재 장착 아이템이 0 인가 
					if (eS.transform.GetChild (1).GetChild (i).childCount == 0) {

                        if(item == null)
                            Debug.Log("EQUIP");
						inventory.EquiptItem (item);
							
						this.gameObject.transform.SetParent (eS.transform.GetChild (1).GetChild (i));
						this.gameObject.GetComponent<RectTransform> ().localPosition = Vector3.zero;
							
						eS.gameObject.GetComponent<Inventory> ().updateItemList ();
						inventory.updateItemList ();
						gearable = true;

						break;

					} else {
						inventory.EquiptItem (item);

						Transform temp = eS.transform.GetChild (1).GetChild (i).GetChild (0);
					    
						eS.transform.GetChild (1).GetChild (i).GetChild (0).SetParent(transform.parent);
						temp.GetComponent<RectTransform> ().localPosition = Vector3.zero;

						inventory.UnEquipItem1 (temp.GetComponent<ConsumeItem> ().item);

						this.gameObject.transform.SetParent (eS.transform.GetChild (1).GetChild (i));
						this.gameObject.GetComponent<RectTransform> ().localPosition = Vector3.zero;
					    

						eS.gameObject.GetComponent<Inventory> ().updateItemList ();

						inventory.updateItemList ();

						gearable = true;


					}
					   
				}
				}
			if (!gearable) {

				inventory.ConsumeItem (item);

				item.itemValue--;

				if (item.itemValue <= 0) {
					inventory.deleteItemFromInventory (item);
					Destroy (this.gameObject);                        
				}
					
			}
	    }
	}
	public void consumeIt()
	{
		Inventory inventory = transform.parent.parent.parent.GetComponent<Inventory>();
		bool gearable = false;

			for (int i = 0; i < eS.slotsInTotal; i++) {
				//장비 아이템 인가?? 소모성 아이템 인가
				if (itemTypeOfSlot [i].Equals (item.itemType)) {

					//현재 장착 아이템이 0 인가 
					if (eS.transform.GetChild (1).GetChild (i).childCount == 0) {

						inventory.EquiptItem (item);

						this.gameObject.transform.SetParent (eS.transform.GetChild (1).GetChild (i));
						this.gameObject.GetComponent<RectTransform> ().localPosition = Vector3.zero;

						eS.gameObject.GetComponent<Inventory> ().updateItemList ();
						inventory.updateItemList ();
						gearable = true;

						break;

					} else {
						inventory.EquiptItem (item);

						Transform temp = eS.transform.GetChild (1).GetChild (i).GetChild (0);

						eS.transform.GetChild (1).GetChild (i).GetChild (0).SetParent(transform.parent);
						temp.GetComponent<RectTransform> ().localPosition = Vector3.zero;

						inventory.UnEquipItem1 (temp.GetComponent<ConsumeItem> ().item);

						this.gameObject.transform.SetParent (eS.transform.GetChild (1).GetChild (i));
						this.gameObject.GetComponent<RectTransform> ().localPosition = Vector3.zero;


						eS.gameObject.GetComponent<Inventory> ().updateItemList ();

						inventory.updateItemList ();

						gearable = true;


					}

				}
			}
			if (!gearable) {

				inventory.ConsumeItem (item);

				item.itemValue--;

				if (item.itemValue <= 0) {
					inventory.deleteItemFromInventory (item);
					Destroy (this.gameObject);                        
				}

			}
		
	}

	void Update () {
	
	}
}
