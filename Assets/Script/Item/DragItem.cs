using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler{

	private Vector2 pointerOffset;
	private RectTransform rectTransform;
	private RectTransform rectTransformSlot;
	private CanvasGroup canvasGroup;
	private GameObject oldSlot;
	private Inventory inventory;
	private Transform draggedItemBox;


	void Start () {
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
		rectTransformSlot = GameObject.FindGameObjectWithTag("DraggingItem").GetComponent<RectTransform>();
		inventory = transform.parent.parent.parent.GetComponent<Inventory>();
		draggedItemBox = GameObject.FindGameObjectWithTag("DraggingItem").transform;
	}
	

	void Update () {
	
	}
	public void OnDrag(PointerEventData data){
		if (rectTransform == null)
			return;

		if (data.button == PointerEventData.InputButton.Left)
		{
			rectTransform.SetAsLastSibling();
			transform.SetParent(draggedItemBox);
			Vector2 localPointerPosition;
			canvasGroup.blocksRaycasts = false;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransformSlot, Input.mousePosition, data.pressEventCamera, out localPointerPosition))
			{
				rectTransform.localPosition = localPointerPosition - pointerOffset;

			}
		}

		inventory.updateItemList ();

	}
	public void OnPointerDown(PointerEventData data)
	{
		if (data.button == PointerEventData.InputButton.Left)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out pointerOffset);
            oldSlot = transform.parent.gameObject;
        }
		inventory.updateItemList ();
	}
	public void createDuplication(GameObject Item)
	{
		Item item = Item.GetComponent<ItemOnObject>().item;
		GameObject duplication = GameObject.FindGameObjectWithTag("MainInventory").GetComponent<Inventory>().addItemToInventory(item.itemID, item.itemValue);
		duplication.transform.parent.parent.parent.GetComponent<Inventory>().stackableSettings();

	}
	public void OnEndDrag(PointerEventData data){

		if (data.button == PointerEventData.InputButton.Left) {

			canvasGroup.blocksRaycasts = true;
			Transform newSlot = null;//새로운 슬롯

			if (data.pointerEnter != null)
				newSlot = data.pointerEnter.transform;

			if (newSlot != null) {//새로운 슬롯이 null이 아니라면

				GameObject firstItemGameObject = this.gameObject;//item object
				GameObject secondItemGameObject = newSlot.parent.gameObject;//새로운 슬롯의 아이템 object
				RectTransform firstItemRectTransform = this.gameObject.GetComponent<RectTransform>();//기존 item의 rectTransform
				RectTransform secondItemRectTransform = newSlot.parent.GetComponent<RectTransform>();//새로운 item의 rectTransform
				Item firstItem = rectTransform.GetComponent<ItemOnObject>().item;//기존 item
				Item secondItem = new Item();//새로운 item

				GameObject Inventory;

				if (newSlot.parent.GetComponent<ItemOnObject> () != null) { //새로운 슬롯에 item이 있다면
					secondItem = newSlot.parent.GetComponent<ItemOnObject> ().item; //새로운 item에 넣는다.
					Inventory = secondItemRectTransform.parent.parent.parent.gameObject;
				} else {
					Inventory = secondItemRectTransform.parent.gameObject;
				}

				//get some informations about the two items
				bool sameItem = firstItem.itemName == secondItem.itemName;  //기존 item과 새로운 item이 이름이 같다면
				bool sameItemRerferenced = firstItem.Equals(secondItem);  //기존 item과 새로운 item이 같은걸 참조 한다면
				bool secondItemStack = false;
				bool firstItemStack = false;
				if (sameItem)//같은 아이템이라면
				{
					firstItemStack = firstItem.itemValue < firstItem.maxStack;//첫번째 item의 itemvalue가 maxStack보다 작다면
					secondItemStack = secondItem.itemValue < secondItem.maxStack;//두번째 item의 itemvalue가 maxStack보 작다면 
				}
				//inventory나 storage로 drag한다면
				if ( Inventory.GetComponent<EquipmentSystem> () == null) {
					
					int newSlotChildCount = newSlot.transform.parent.childCount;
					bool isOnSlot = newSlot.transform.parent.GetChild (0).tag == "ItemIcon";//item icon이 있다면

					if (newSlotChildCount != 0 && isOnSlot) {

						bool fitsIntoStack = false;
						if (sameItem)//같은 아이템이라면
							fitsIntoStack = (firstItem.itemValue + secondItem.itemValue) <= firstItem.maxStack;
						//두개 더한 값이 max값보다 적다면

						if (inventory.stackable && sameItem && firstItemStack && secondItemStack) {//누적할 수 있다면
							
							if (fitsIntoStack && !sameItemRerferenced) {//같은 아이템을 참조하는게 아니라면
								secondItem.itemValue = firstItem.itemValue + secondItem.itemValue;//두째 아이템에 누적
								secondItemGameObject.transform.SetParent (newSlot.parent.parent);//두번째 아이템의 슬롯에 세팅
								Destroy (firstItemGameObject);//첫번째 아이템 파괴
								secondItemRectTransform.localPosition = Vector3.zero;//두번째 아이템 위치 세팅

							} else { //두개 더한게 맥스 갑을 넘는다면
								
								int rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack;//나머지 값 

								//한 아이템의 스텍에 가득 채우고 다른 아이템의 스텍에 나머지 저장
								if (!fitsIntoStack && rest > 0) {
									firstItem.itemValue = firstItem.maxStack;
									secondItem.itemValue = rest;

									firstItemGameObject.transform.SetParent (secondItemGameObject.transform.parent);
									secondItemGameObject.transform.SetParent (oldSlot.transform);

									firstItemRectTransform.localPosition = Vector3.zero;
									secondItemRectTransform.localPosition = Vector3.zero;
								}
							}

						}
						//누적할수 없다면
						else {
							
							int rest = 0;
							if (sameItem)
								rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack;


							if (!fitsIntoStack && rest > 0) {
								secondItem.itemValue = firstItem.maxStack;
								firstItem.itemValue = rest;

								firstItemGameObject.transform.SetParent (secondItemGameObject.transform.parent);
								secondItemGameObject.transform.SetParent (oldSlot.transform);

								firstItemRectTransform.localPosition = Vector3.zero;
								secondItemRectTransform.localPosition = Vector3.zero;
							}

							else if (!fitsIntoStack && rest == 0) {
								
								if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem> () != null && firstItem.itemType == secondItem.itemType) {
									newSlot.transform.parent.parent.parent.parent.GetComponent<Inventory> ().UnEquipItem1 (firstItem);
									oldSlot.transform.parent.parent.GetComponent<Inventory> ().EquiptItem (secondItem);

									firstItemGameObject.transform.SetParent (secondItemGameObject.transform.parent);
									secondItemGameObject.transform.SetParent (oldSlot.transform);
									secondItemRectTransform.localPosition = Vector3.zero;
									firstItemRectTransform.localPosition = Vector3.zero;

								}
								                                    
								else if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem> () != null && firstItem.itemType != secondItem.itemType) {
									firstItemGameObject.transform.SetParent (oldSlot.transform);
									firstItemRectTransform.localPosition = Vector3.zero;
								}

								else if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem> () == null) {
									firstItemGameObject.transform.SetParent (secondItemGameObject.transform.parent);
									secondItemGameObject.transform.SetParent (oldSlot.transform);
									secondItemRectTransform.localPosition = Vector3.zero;
									firstItemRectTransform.localPosition = Vector3.zero;
								}
							}

						}

					} else {//빈 슬롯으로 드래그 할때....
						firstItemGameObject.transform.SetParent(newSlot.transform);
						firstItemRectTransform.localPosition = Vector3.zero;

						if (newSlot.transform.parent.parent.GetComponent<EquipmentSystem>() == null && oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null)
							oldSlot.transform.parent.parent.GetComponent<Inventory>().UnEquipItem1(firstItem);
					}
				}

				if (Inventory.GetComponent<EquipmentSystem>() != null)
				{
					ItemType[] itemTypeOfSlots = GameObject.FindGameObjectWithTag("EquipmentSystem").GetComponent<EquipmentSystem>().itemTypeOfSlots;
					int newSlotChildCount = newSlot.transform.parent.childCount;
					bool isOnSlot = newSlot.transform.parent.GetChild(0).tag == "ItemIcon";
					bool sameItemType = firstItem.itemType == secondItem.itemType;
					bool fromHot = oldSlot.transform.parent.parent.GetComponent<HotBar>() != null;


					if (newSlotChildCount != 0 && isOnSlot)
					{
						
						if (sameItemType && !sameItemRerferenced) 
						{
							Transform temp1 = secondItemGameObject.transform.parent.parent.parent;
							Transform temp2 = oldSlot.transform.parent.parent;                            

							firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
							secondItemGameObject.transform.SetParent(oldSlot.transform);
							secondItemRectTransform.localPosition = Vector3.zero;
							firstItemRectTransform.localPosition = Vector3.zero;

							if (fromHot)
								createDuplication(secondItemGameObject);

						}

						else
						{
							firstItemGameObject.transform.SetParent(oldSlot.transform);
							firstItemRectTransform.localPosition = Vector3.zero;

							if (fromHot)
								createDuplication(firstItemGameObject);
						}

					}

					else
					{
						for (int i = 0; i < newSlot.parent.childCount; i++)
						{
							if (newSlot.Equals(newSlot.parent.GetChild(i)))
							{
								
								if (itemTypeOfSlots[i] == transform.GetComponent<ItemOnObject>().item.itemType)
								{
									transform.SetParent(newSlot);
									rectTransform.localPosition = Vector3.zero;

									if (!oldSlot.transform.parent.parent.Equals(newSlot.transform.parent.parent))
										Inventory.GetComponent<Inventory>().EquiptItem(firstItem);

								}

								else
								{
									transform.SetParent(oldSlot.transform);
									rectTransform.localPosition = Vector3.zero;
									if (fromHot)
										createDuplication(firstItemGameObject);
								}
							}
						}
					}

				}
				Inventory.GetComponent<Inventory> ().updateItemList();

			} else {//슬롯이 아니라 다른 곳에 드래그 하면
				GameObject dropItem = (GameObject)Instantiate(GetComponent<ItemOnObject>().item.itemModel);
				dropItem.AddComponent<PickUpItem>();
				dropItem.GetComponent<PickUpItem>().item = this.gameObject.GetComponent<ItemOnObject>().item;               
				dropItem.transform.localPosition = GameObject.FindGameObjectWithTag("Player").transform.localPosition;
				inventory.updateItemList();
				if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null)
					inventory.GetComponent<Inventory>().UnEquipItem1(dropItem.GetComponent<PickUpItem>().item);
				Destroy(this.gameObject);



			}



		}
		inventory.updateItemList ();

	}

}