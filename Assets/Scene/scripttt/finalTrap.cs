﻿using UnityEngine;
using System.Collections;

public class finalTrap : MonoBehaviour {

	GameObject player;
	//GameObject Thisistrap;

	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag ("Player");
		//Thisistrap = GameObject.FindGameObjectWithTag ("trap");
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Player") {
			Destroy (gameObject);
		}
	}


	// Update is called once per frame
	void Update () {

	}
}