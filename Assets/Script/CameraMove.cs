using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

	Transform tr;
    void Start()
    {
        //Player 테그를 가진 게임 오브젝트의 좌표값을 tr 변수에 저장한다.
        tr = GameObject.FindWithTag("Player").transform;
    }
	void LateUpdate () {
        if(tr != null)
		    transform.position = tr.position;
	}
}



