using UnityEngine;
using System.Collections;

public class WeaponSetactive : MonoBehaviour
{
    MonsterManager monster;

    // Use this for initialization
    void Start()
    {
        monster = transform.root.GetComponent<MonsterManager>();
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Player")) // 플레이어 타격시
        {
            col.gameObject.GetComponent<Player>().HitToPlayer(monster.AttackPower);            
        }
    }
}
