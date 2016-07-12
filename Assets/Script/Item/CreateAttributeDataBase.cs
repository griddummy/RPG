using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateAttributeDataBase : MonoBehaviour
{

	public static ItemAttributeList asset;                                                  //The List of all Items
	void Start(){
		//asset = createItemAttributeDataBase();
	}
	#if UNITY_EDITOR
	public static ItemAttributeList createItemAttributeDataBase()                                    //creates a new ItemDatabase(new instance)
	{
		print ("ItemAttributeList");
		asset = ScriptableObject.CreateInstance<ItemAttributeList>();                       //of the ScriptableObject InventoryItemList

		AssetDatabase.CreateAsset(asset, "Assets/Resources/AttributeDatabase.asset");            //in the Folder Assets/Resources/ItemDatabase.asset
		AssetDatabase.SaveAssets();                                                         //and than saves it there        
		return asset;
	}
	#endif

}
