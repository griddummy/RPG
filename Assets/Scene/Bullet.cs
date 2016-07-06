﻿using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float speed;
	public Rigidbody rigid;
	//public int BulletDamage;
	//public GameObject bullet;

	public GameObject Player;

	// Use this for initialization
	void Start () {
		//bullet = GameObject.FindGameObjectWithTag("bullet");
		rigid = GetComponent<Rigidbody> ();
		rigid.velocity = transform.forward * speed;
        Player = GameObject.FindGameObjectWithTag("Player");
    }


	// Update is called once per frame

	void OnTriggerEnter(Collider coll) {
		if (coll.CompareTag ("Player")) {
			//playerhp를 깍음
			//PlayerhP -= BulletDamage;
		}
			


	}

	void Update () {
		//if(bullet == null)
		//rigid.velocity += new Vector3 (0f, -0.3f, 0f);
		//if (Vector3.Distance (Player.transform.position, this.gameObject.transform.position) >= 100.0f) {
			Destroy (gameObject, 3f);
		//}
	}

}

