using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    
    public float speed;
    public Rigidbody rigid;
    public int BulletDamage = 1; //피격시플레이어가입는공격력
    //public GameObject bullet;
    public float DesTime;
    private Player player;
    //public GameObject effect;

    // Use this for initialization
    void Start()
    {
        //bullet = GameObject.FindGameObjectWithTag("bullet");
        rigid = GetComponent<Rigidbody>();
        rigid.velocity = transform.forward * speed;
        
        player = GameManager.instance.getPlayerInfo();
    }    

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == ("Player"))
        {
            player.HitToPlayer(BulletDamage);
            //playerhp를 깍음
            //PlayerhP -= BulletDamage;
            //Instantiate(effect, transform.position, transform.rotation);
        }
    }

    void Update()
    {
        Destroy(gameObject, DesTime);
    }
}

