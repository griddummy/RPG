using UnityEngine;
using System.Collections;

public class WeaponCollider : MonoBehaviour {

    Player player;

    void Start()
    {
        player = transform.root.GetComponent<Player>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Monster")) // 몬스터 타격시
        {

            col.transform.root.GetComponent<MonsterManager>().HitToMonster(player.curWeaponDamage);

        }
    }
}
