using UnityEngine;
using System.Collections;

public class EquipmentSystem : MonoBehaviour {

	[SerializeField]
	public int slotsInTotal = 8;
	[SerializeField]
	public ItemType[] itemTypeOfSlots = new ItemType[8];

	// Use this for initialization
	void Start () {

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
