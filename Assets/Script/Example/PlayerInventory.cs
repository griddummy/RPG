using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerInventory : MonoBehaviour {
	public GameObject inventory;
	public GameObject characterSystem;
	public GameObject storage;
    public GameObject hotbar;
	private Inventory mainInventory;
	private Inventory characterSystemInventory;
	private Inventory storageInventory;
    private Inventory hotbarInventory;
	private InputManager inputManagerDatabase;
    /*
	float maxHealth = 100;
	float maxMana = 100;
	float maxDamage = 0;
	float maxArmor = 0;

	public float currentHealth = 60;
	float currentMana = 100;
	float currentDamage = 0;
	float currentArmor = 0;
    */
    private Player player;

	// Use this for initialization
    void Awake()
    {
        if (inputManagerDatabase == null)
            inputManagerDatabase = (InputManager)Resources.Load("InputManager");
        if (inventory != null)
            mainInventory = inventory.GetComponent<Inventory>();
        if (characterSystem != null)
            characterSystemInventory = characterSystem.GetComponent<Inventory>();
        if (storage != null)
            storageInventory = storage.GetComponent<Inventory>();
        if (hotbar != null)
            hotbarInventory = hotbar.GetComponent<Inventory>();
        player = GameManager.instance.getPlayerInfo();
    }
	void Start () {
        
		
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(inputManagerDatabase.CharacterSystemKeyCode))
		{
			if (!characterSystem.activeSelf)
			{
			   characterSystemInventory.openInventory();
				//characterSystemInventory.
			}
			else
			{
				characterSystemInventory.closeInventory();
			}
		}

		if (Input.GetKeyDown(inputManagerDatabase.InventoryKeyCode))
		{
			print ("key press" + inputManagerDatabase.InventoryKeyCode);

			if (!inventory.activeSelf)
			{
				mainInventory.openInventory();
			}
			else
			{
				mainInventory.closeInventory();
			}
		}

		if (Input.GetKeyDown(inputManagerDatabase.StorageKeyCode))
		{
			if (!storage.activeSelf)
				storageInventory.openInventory();
			else
			{
				storageInventory.closeInventory();
			}
		}
	}
	public void OnEnable()
	{
		Inventory.ItemEquip += OnGearItem;
		Inventory.ItemConsumed += OnConsumeItem;
		Inventory.UnEquipItem += OnUnEquipItem;

		Inventory.ItemEquip += EquipWeapon;
		Inventory.UnEquipItem += UnEquipWeapon;
	}

	public void OnDisable()
	{
		Inventory.ItemEquip -= OnGearItem;
		Inventory.ItemConsumed -= OnConsumeItem;
		Inventory.UnEquipItem -= OnUnEquipItem;

		Inventory.UnEquipItem -= UnEquipWeapon;
		Inventory.ItemEquip -= EquipWeapon;
	}

	void EquipWeapon(Item item)
	{
		if (item.itemType == ItemType.Weapon)
		{
			//add the weapon if you unequip the weapon
		}
	}

	void UnEquipWeapon(Item item)
	{
		if (item.itemType == ItemType.Weapon)
		{
			//delete the weapon if you unequip the weapon
		}
	}

    // 아이템 소비
	public void OnConsumeItem(Item item)
	{
        
        for (int i = 0; i < item.itemAttributes.Count; i++)
		{
			if (item.itemAttributes[i].attributeName == "Health")
			{
                /*
				if ((currentHealth + item.itemAttributes[i].attributeValue) > maxHealth)
					currentHealth = maxHealth;
				else
					currentHealth += item.itemAttributes[i].attributeValue;
                */
                
                player.hp += item.itemAttributes[i].attributeValue; // 피 회복
            }
			if (item.itemAttributes[i].attributeName == "Mana")
			{
                /*
				if ((currentMana + item.itemAttributes[i].attributeValue) > maxMana)
					currentMana = maxMana;
				else
					currentMana += item.itemAttributes[i].attributeValue;
                    */
                player.sp += item.itemAttributes[i].attributeValue; // Sp 회복
            }
			if (item.itemAttributes[i].attributeName == "Armor") // 아머 스탯은 없음
			{
                /*
				if ((currentArmor + item.itemAttributes[i].attributeValue) > maxArmor)
					currentArmor = maxArmor;
				else
					currentArmor += item.itemAttributes[i].attributeValue;
                */
			}
			if (item.itemAttributes[i].attributeName == "Damage") // 데미지 버프는 없음
			{
                /*
				if ((currentDamage + item.itemAttributes[i].attributeValue) > maxDamage)
					currentDamage = maxDamage;
				else
					currentDamage += item.itemAttributes[i].attributeValue;
                */
			}
		}
		//if (HPMANACanvas != null)
		//{
		//    UpdateManaBar();
		//    UpdateHPBar();
		//}
	}

	public void OnGearItem(Item item)
	{
		for (int i = 0; i < item.itemAttributes.Count; i++)
		{
            if (item.itemAttributes[i].attributeName == "Health")
            {
                //maxHealth += item.itemAttributes[i].attributeValue;
                player.hpMaxBuff += item.itemAttributes[i].attributeValue;
            }
			if (item.itemAttributes[i].attributeName == "Mana")
            {
                //maxMana += item.itemAttributes[i].attributeValue;
                player.spMaxBuff += item.itemAttributes[i].attributeValue;
            }

            if (item.itemAttributes[i].attributeName == "Armor")
            {
                //maxArmor += item.itemAttributes[i].attributeValue;
                player.hpMaxBuff += item.itemAttributes[i].attributeValue;
            }
			if (item.itemAttributes[i].attributeName == "Damage")
            {
                //maxDamage += item.itemAttributes[i].attributeValue;
                player.attackPowerBuff += item.itemAttributes[i].attributeValue;
            }
		}
		//if (HPMANACanvas != null)
		//{
		//    UpdateManaBar();
		//    UpdateHPBar();
		//}
	}

	public void OnUnEquipItem(Item item)
	{
		for (int i = 0; i < item.itemAttributes.Count; i++)
		{
			if (item.itemAttributes[i].attributeName == "Health")
            {
                //maxHealth -= item.itemAttributes[i].attributeValue;
                player.hpMaxBuff -= item.itemAttributes[i].attributeValue;
            }				
			if (item.itemAttributes[i].attributeName == "Mana")
            {
                //maxMana -= item.itemAttributes[i].attributeValue;
                player.spMaxBuff -= item.itemAttributes[i].attributeValue;
            }				
			if (item.itemAttributes[i].attributeName == "Armor")
            {
                //maxArmor -= item.itemAttributes[i].attributeValue;
                player.hpMaxBuff -= item.itemAttributes[i].attributeValue;
            }				
			if (item.itemAttributes[i].attributeName == "Damage")
            {
                //maxDamage -= item.itemAttributes[i].attributeValue;
                player.attackPowerBuff -= item.itemAttributes[i].attributeValue;
            }				
		}
	}
    public List<Item> GetInventoryItems()
    {
        return new List<Item>(mainInventory.ItemsInInventory);
    }
    public List<Item> GetCharacterSystemInventoryItems()
    {
        return new List<Item>(characterSystemInventory.ItemsInInventory);
    }
    public List<Item> GetStorageInventoryItems()
    {
        return new List<Item>(storageInventory.ItemsInInventory);
    }
    public List<Item> GetHotBarItems()
    {
        return new List<Item>(hotbarInventory.ItemsInInventory);
    }
    public void InitInventoryItems(List<Item> list)
    {
        mainInventory.ItemsInInventory = new List<Item>(list);
        mainInventory.addAllItemsToInventory();
    }
    public void InitStorageItems(List<Item> list)
    {
        storageInventory.ItemsInInventory = new List<Item>(list);
        storageInventory.addAllItemsToInventory();
    }
    public void InitHotbarItems(List<Item> list)
    {
        hotbarInventory.ItemsInInventory = new List<Item>(list);
        hotbarInventory.addAllItemsToInventory();
    }
    public void InitCharSystemItems(List<Item> list)
    {
        characterSystemInventory.ItemsInInventory = new List<Item>(list);
        characterSystemInventory.addAllItemsToInventory();
        foreach(Item item in list)
        {
            characterSystemInventory.EquiptItem(item);
        }        
    }
}