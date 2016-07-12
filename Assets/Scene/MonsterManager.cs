using UnityEngine;
using System.Collections;

public class MonsterManager : MonoBehaviour
{
    public delegate void OnDeathEvent();
    public event OnDeathEvent OnDeath;

    NavMeshAgent agent;
    GameObject player; // 플레이어오브젝트
                       //public GameObject ThisMonster; //이 몬스터
    public BoxCollider colWeapon;

    public Vector3 Dir;

    public float LookDistan; //시야

    //public Ray ray;
    public GameObject effect; // 피격시 이펙트

    public float random; //움직이는 랜덤방향

    Animator aniCon;

    int AttackTpyeONEorTwo; //1번공격인지 2번공격인지

    private Vector3 vecSpawnPos; // 몬스터의 생성위치
    private Vector3 vecMovePos;// 해당 몬스터의 이동지점
    private Vector3 RespawnPosOrigin;

    public float MonsterMaxHP; //몬스터의 HP
    public float MonsterCurrentHP;
    
    public int KillExp; //죽였을시 얻는 경험치
    public int AttackPower; //몬스터의 공격력

    public float AttackColltime = 0.3f; //몬스터가 공격후 플레이어가 대미지 판정을 처리하기까지의 시간
                                        //public int MoveSpeed; // naviMeshAgent에서 관리하므로 필요 없음
    public bool isChase = false;

    public GameObject bullet = null;
    bool isfire = false;
    public float fps = 10;
    public float attackRange = 30f;

    public bool Far;
    private Vector3 vecWeaponColliderSize;
    private Coroutine corFire;
    // Use this for initialization
    void Start()
    {
        vecSpawnPos = transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        aniCon = GetComponent<Animator>();
        vecMovePos = vecSpawnPos;
        StartCoroutine(RunAway(10));
        RespawnPosOrigin = vecSpawnPos;
       
        enabled = true;
        if (colWeapon != null)
            vecWeaponColliderSize = colWeapon.size;
        setEnableWeapon(false);        

        if (Far == true)
        {
            corFire = StartCoroutine(Fire());
        }
    }
    public void setEnableWeapon(bool bEnable)
    {
        if (colWeapon != null)
        {
            colWeapon.enabled = bEnable;
            /*
            if (bEnable)
            {
                colWeapon.size = vecWeaponColliderSize;
            }
            else
                colWeapon.size = Vector3.zero;
            //*/
        }            
    }

    IEnumerator Fire()
    {
        while (true)
        {
            if (isfire == false)
            {
                yield return null;
            }
            else
            {
                Instantiate(bullet, transform.position, transform.rotation);
                yield return new WaitForSeconds(1 / fps);
            }
        }
    }
    IEnumerator RunAway(float time)
    {

        yield return new WaitForSeconds(time);
        vecMovePos = new Vector3(vecSpawnPos.x + Random.Range(-random, random), transform.position.y, vecSpawnPos.z + Random.Range(-random, random));
        StartCoroutine(RunAway(10));
    }

    IEnumerator ItemDrop()
    {
        yield return new WaitForSeconds(1);
    }
    /*
	IEnumerator Dammaged(){

		aniCon.SetBool ("IsDammaged", true);
		//MonsterCurrentHP -=  ; // 플레이어의 공격력만큼 깍음
		yield return new WaitForSeconds (0.01f);
		aniCon.SetBool ("IsDammaged", false);
	}


	IEnumerator Dammaged2(){

		aniCon.SetBool ("IsDammaged2", true);
		//MonsterCurrentHP -=  ; // 플레이어의 공격력만큼 깍음
		yield return new WaitForSeconds (0.01f);
		aniCon.SetBool ("IsDammaged2", false);

	}*/
    void OnTriggerEnter(Collider col)
    {

        if (col.gameObject.tag == ("Weapon"))
        {
            //StartCoroutine (GIGIGIG ());

            AttackTpyeONEorTwo = Random.Range(-10, 10);
            if (AttackTpyeONEorTwo >= 0)
            {
                aniCon.SetTrigger("IsDammagedCast1");
            }
            if (AttackTpyeONEorTwo < 0)
            {
                aniCon.SetTrigger("IsDammagedCast2");
            }

            //aniCon.SetBool ("IsDammaged", true);
            //MonsterCurrentHP -=1 ;
        }
    }

    IEnumerator GIGIGIG()
    { // 타격감을위한경직
        Time.timeScale = 0.15f;
        yield return new WaitForSeconds(0.02f);
        Time.timeScale = 1;
    }


    // Update is called once per frame
    void Update()
    {
        if (player == null) // 저장 불러오기 시 예외처리
            return;
        Dir = player.transform.position - transform.position;

        if (Vector3.Distance(player.transform.position, transform.position) <= LookDistan)
        {
            isChase = true;
        }
        else
        {
            isChase = false;

            agent.SetDestination(vecMovePos);
            aniCon.SetBool("IsRun", true);
            agent.Resume();
            if (Vector3.Distance(vecMovePos, transform.position) <= 3)
            {
                aniCon.SetBool("IsRun", false);
                aniCon.SetBool("IsIdle", true);

                agent.Stop(); //idle 모션이다시나오게
                //setEnableWeapon(false);
            }
        }

        if (isChase)
        {
            //agent.Stop();
            //aniCon.SetBool ("IsRun", true);
            agent.SetDestination(player.transform.position);
            agent.Resume();

            //if (this.gameObject.tag == "Far") {
            if (Far)
            {
                if (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
                {
                    aniCon.SetBool("IsRun", false);
                    agent.Stop();

                    isfire = true;
                    aniCon.SetBool("IsFarAttack", true);

                    Vector3 vecLookPos = player.transform.position;
                    vecLookPos.y = transform.position.y;
                    transform.LookAt(vecLookPos);


                }
                else
                {
                    aniCon.SetBool("IsRun", true);
                    aniCon.SetBool("IsFarAttack", false);
                    isfire = false;
                }
            }
            else
            {
                if (Vector3.Distance(player.transform.position, transform.position) <= 7)
                {
                    aniCon.SetBool("IsRun", false);
                    agent.Stop();

                    AttackTpyeONEorTwo = Random.Range(-10, 10);
                    if (AttackTpyeONEorTwo >= 0)
                    {
                        aniCon.SetBool("IsAttack2", false);
                        aniCon.SetBool("IsAttack1", true);
                        //setEnableWeapon(true);

                        Vector3 vecLookPos = player.transform.position;
                        vecLookPos.y = transform.position.y;
                        transform.LookAt(vecLookPos);
                        /*
					//NextPattern = Time.time + AttackColltime; // 공격후일정시간뒤에도 그공격범위내에 플레이어가 존재시
						if (Vector3.Distance (player.transform.position, this.transform.position) <= 5.0f &&
							Mathf.Abs (Vector3.Angle (player.transform.forward, ThisMonster.transform.position - player.transform.position)) <= 7.5f) {
							//playerHP -= AttackPower; // player 가 몬스터의 공격력 만큼 대미지를 입음
						} else {
							//player가 대미지 피해 없음
						}*/
                    }
                    if (AttackTpyeONEorTwo < 0)
                    {
                        //Debug.Log (" attack2");
                        aniCon.SetBool("IsAttack1", false);
                        aniCon.SetBool("IsAttack2", true);
                        //setEnableWeapon(true);

                        Vector3 vecLookPos = player.transform.position;
                        vecLookPos.y = transform.position.y;
                        transform.LookAt(vecLookPos);

                    }

                }
                else
                {
                    aniCon.SetBool("IsRun", true);
                    aniCon.SetBool("IsAttack1", false);
                    aniCon.SetBool("IsAttack2", false);
                    //setEnableWeapon(false);
                }
            }
        }
        else
        {
            isChase = false;
            //setEnableWeapon(false);
            isfire = false; 
            aniCon.SetBool("IsFarAttack", false); 
            agent.SetDestination(vecMovePos);
            //agent.SetDestination (this.transform.position.x + Random.Range(-random,random),this.transform.position.y, this.transform.position.z + Random.Range(-random,random));
            agent.Resume();
        }

        if (MonsterCurrentHP <= 0)
        {
            DropRandomItem(); // 아이템 드랍
            GameManager.instance.getPlayerInfo().exp += KillExp; // 플레이어에게 경험치 추가
            setEnableWeapon(false);
            if(corFire != null)
                StopCoroutine(corFire); // 사격 중지            
            agent.Stop();
            aniCon.SetBool("IsIdle", false);
            aniCon.SetBool("IsAttack1", false);
            aniCon.SetBool("IsAttack2", false);
            aniCon.SetBool("IsFarAttack", false);
            aniCon.SetBool("IsRun", false);
            aniCon.SetBool("IsDie", true);
            
            if (OnDeath != null)
                OnDeath();

            enabled = false; // 아이템을 한번만 생성하게
                             //NextPattern = Time.time + patternRegTime;
            Destroy(gameObject, 5f);


            //Instantiate (ThisMonster, LivingZone.transform.position, LivingZone.transform.rotation); // 리스폰
            //enabled = false;
        }
    }
    public void HitToMonster(int damage) // 몬스터가 공격당하면
    {
        MonsterCurrentHP -= damage;
    }

    public void DropRandomItem() // 아이템 드랍
    {
        Item item = GameManager.instance.GetRandomItem();
        GameObject dropItem = Instantiate(item.itemModel, transform.position, transform.rotation) as GameObject;
        dropItem.AddComponent<PickUpItem>();
        dropItem.GetComponent<PickUpItem>().item = item;
    }
}
