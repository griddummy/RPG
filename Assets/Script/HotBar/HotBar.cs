using UnityEngine;
using System.Collections;

public class HotBar : MonoBehaviour {

	[SerializeField]
	public KeyCode[] keyCodesForSlots = new KeyCode[9];
	[SerializeField]
	public int slotsInTotal;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < slotsInTotal; i++)
		{
			if (Input.GetKeyDown(keyCodesForSlots[i]))
			{
				if (transform.GetChild(1).GetChild(i).childCount != 0)
				{
					transform.GetChild(1).GetChild(i).GetChild(0).GetComponent<ConsumeItem>().consumeIt();
				}
			}
		}
	}
	public int getSlotsInTotal()
	{
		Inventory inv = GetComponent<Inventory>();
		return slotsInTotal = inv.width * inv.height;
	}
}
