﻿using UnityEngine;
using System.Collections;
public class PickUpItem : MonoBehaviour
{
	public Item item;
	private Inventory _inventory;
	private GameObject _player;


	void Start()
	{
		_player = GameObject.FindGameObjectWithTag("Player");
		if (_player != null)
			_inventory = GameManager.instance.getPlayerInventory().inventory.GetComponent<Inventory>();
	}

	void Update()
	{
		if (_inventory != null && Input.GetKeyDown(KeyCode.E))
		{
			float distance = Vector3.Distance(this.gameObject.transform.position, _player.transform.position);

			if (distance <= 3)
			{
				bool check = _inventory.checkIfItemAllreadyExist(item.itemID, item.itemValue);
				if (check)
					Destroy(this.gameObject);
				else if (_inventory.ItemsInInventory.Count < (_inventory.width * _inventory.height))
				{
					_inventory.addItemToInventory(item.itemID, item.itemValue);
					_inventory.updateItemList();
					_inventory.stackableSettings();
					Destroy(this.gameObject);
				}

			}
		}
	}

}