using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class CreateItemDataBase:MonoBehaviour
{
	public static ItemDataBaseList asset;                                                  //The List of all Items
	void Start(){
		//asset = createItemDatabase ();
	}

	#if UNITY_EDITOR
	public static ItemDataBaseList createItemDatabase()                                    //creates a new ItemDatabase(new instance)
	{
		print ("ItemDataBaseList play");
		asset = ScriptableObject.CreateInstance<ItemDataBaseList>();                       //of the ScriptableObject InventoryItemList

		AssetDatabase.CreateAsset(asset, "Assets/Resources/ItemDatabase.asset");            //in the Folder Assets/Resources/ItemDatabase.asset
		AssetDatabase.SaveAssets();                                                         //and than saves it there
		asset.itemList.Add(new Item());
		return asset;
	}
	#endif
}
