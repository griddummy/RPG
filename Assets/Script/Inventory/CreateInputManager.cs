using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateInputManager :MonoBehaviour
{

	public static InputManager asset;
	void Start(){

	//	asset = createInputManager ();
	}
		
	#if UNITY_EDITOR
	public static InputManager createInputManager()
	{
		asset = ScriptableObject.CreateInstance<InputManager>();

		AssetDatabase.CreateAsset(asset, "Assets/Resources/InputManager.asset");
		AssetDatabase.SaveAssets();
		return asset;
	}
	#endif

}