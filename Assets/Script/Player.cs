using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    public delegate void OnChangedHpEvent(int Hp, int MaxHp);
    public event OnChangedHpEvent OnChangedHP;

    public delegate void OnChangedSpEvent(int Sp, int MaxSp);
    public event OnChangedSpEvent OnChangedSP;

    public delegate void OnChangedExpEvent(int Exp, int MaxExp);
    public event OnChangedExpEvent OnChangedExp;

    public delegate void OnChangedLevelEvent(int level);
    public event OnChangedLevelEvent OnChangedLevel;

    public delegate void OnDeathEvent();
    public event OnDeathEvent OnDeath;

    private int m_Hp;                 // 현재 HP
    private int m_HpMaxBuff;          // Buff HP (장비로 인한 추가 HP)
    private int m_HpRegenerateBuff;   // 분당 회복력 버프

    private int m_Sp;                 // spellPower, 현재 SP
    private int m_SpMaxBuff;            // Buff sp (장비로 인한 추가 sp)
    private int m_SpRegenerateBuff;   // 분당 회복력 버프

    private int m_AttackPowerBuff;    // 공격력 버프

    private int m_Level = 1;          // 현재 레벨 , 기본값 1
    private int m_Experience;         // 현재 경험치

    private Job m_Job;                // 현재 직업 정보(스킬, 레벨별 스탯)
    private Skill m_curSkill;           // 현재 사용 가능한 메인스킬
    private GameManager gm;

    // playerController

    private float speed = 0f;                    //현재 이동속도
    public float runspeed = 7f;                 //달리는 이동속도
    public float jumplevel = 20f;               //점프레벨
    public float gravity = 50f;                 //중력제어
    public float turnspeed = 100f;              //player의 회전속도

    public Collider boxCol;                     //BoxCollider 
    public GameObject Smoke;                    //이동시 먼지흩날리는효과
    public Transform transformWeapon;

    private CharacterController controller;     //캐릭터 컨트롤러
    private Vector3 lookDirection;              //input에 의해 정해진 현재방향
    private Vector3 jumPing;                    //jump를 위한 vector3할당

    //현재 animation상태 저장
    protected Animator anim;

    private AnimatorStateInfo currentBaseState;

    private float xx;
    private float zz;

    public int curWeaponDamage;

    //layer.state를 hash로 변수정의
    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int runState = Animator.StringToHash("Base Layer.Run");
    static int jumpState = Animator.StringToHash("Base Layer.Jump");
    static int damageState = Animator.StringToHash("Base Layer.Damage");
    static int deadState = Animator.StringToHash("Base Layer.Dead");
    static int skill1State = Animator.StringToHash("Base Layer.Skill1");
    static int skill2State = Animator.StringToHash("Base Layer.Skill2");
    static int skill3State = Animator.StringToHash("Base Layer.Skill3");

    static int JumpAttack = Animator.StringToHash("Base Layer.JumpAttack");
    static int combo1State = Animator.StringToHash("Base Layer.Combo1");
    static int combo2State = Animator.StringToHash("Base Layer.Combo2");
    static int combo3State = Animator.StringToHash("Base Layer.Combo3");

    // 프로퍼티(속성)
    public int hp // 현재 Hp를 제어.  get,set
    {
        get
        {
            return m_Hp;
        }
        set
        {
            if (value < 0) // 0보다 작을 수 없고
                value = 0;
            else if (value > hpMax) // Max보다 클 수 없다.
                value = hpMax;
            int preHp = m_Hp;
            m_Hp = value;
            if (preHp != m_Hp)
            {
                if (OnChangedHP != null)
                    OnChangedHP(hp, hpMax);
                if(m_Hp <= 0)
                {
                    anim.SetBool("Dead", true);
                    if (OnDeath != null)
                        OnDeath();
                }
            }
        }
    }

    public int hpMax // Max HP를 얻는다 (기본 MaxHP + Buff MaxHP ) get   
    {
        get
        {
            return getLevelInfo().maxHP + hpMaxBuff;
        }
    }

    public int hpMaxBuff // 아이템으로 인한 Max Hp 증가량 제어 get, set
    {
        get
        {
            return m_HpMaxBuff;
        }
        set
        {
            int preHpMaxBuff = m_HpMaxBuff;
            m_HpMaxBuff = value;
            if (preHpMaxBuff != m_HpMaxBuff)
            {
                if (OnChangedHP != null)
                    OnChangedHP(hp, hpMax);
            }
        }
    }

    public int hpRegenerate // HP 분당 회복량을 얻는다. (기본 + 버프) get        
    {
        get
        {
            return getLevelInfo().hpRegenerate + m_HpRegenerateBuff;
        }
    }

    public int hpRegenerateBuff  // 아이템으로 인한 HP 분당 회복량을 제어 get, set
    {
        get
        {
            return m_HpRegenerateBuff;
        }
        set
        {
            m_HpRegenerateBuff = value;
        }
    }

    public int sp // 현재 Sp를 제어 get,set
    {
        get
        {
            return m_Sp;
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            else if (value > spMax)
            {
                value = spMax;
            }
            int preSp = m_Sp;
            m_Sp = value;
            if (preSp != m_Sp)
            {
                if (OnChangedSP != null)
                    OnChangedSP(m_Sp, spMax);
            }

        }
    }

    public int spMax // 현재 Max SP를 얻는다.( 기본 Max SP + Buff Max SP) get
    {
        get
        {
            return getLevelInfo().maxSP + spMaxBuff;
        }
    }

    public int spMaxBuff // 아이템으로 인한 Max Sp 증가량 제어 get, set
    {
        get
        {
            return m_SpMaxBuff;
        }
        set
        {
            int preSpMaxBuff = m_SpMaxBuff;
            m_SpMaxBuff = value;
            if (preSpMaxBuff != m_SpMaxBuff)
            {
                if (OnChangedSP != null)
                    OnChangedSP(m_Sp, spMax);
            }

        }
    }

    public int spRegenerate // SP 분당 회복량을 얻는다. get
    {
        get
        {
            return getLevelInfo().spRegenerate + m_SpRegenerateBuff;
        }
    }

    public int spRegenerateBuff // 아이템으로 인한 SP 분당 회복량 제어 get, set
    {
        get
        {
            return m_SpRegenerateBuff;
        }
        set
        {
            m_SpRegenerateBuff = value;
        }
    }

    public int attackPower      // 전체 공격력을 얻는다 (기본 + 버프) get
    {
        get
        {
            return getLevelInfo().attackPower + m_AttackPowerBuff;
        }
    }

    public int baseAttackPower  // 기본 공격력을 얻는다. get
    {
        get
        {
            return getLevelInfo().attackPower;
        }
    }

    public int attackPowerBuff  // 공격력 버프량(아이템으로 인한)을 제어 get, set
    {
        get
        {
            return m_AttackPowerBuff;
        }

        set
        {
            m_AttackPowerBuff = value;
        }
    }

    public int level //현재 레벨을 제어. get, set
    {
        get
        {
            return m_Level;
        }
        set
        {
            if (value > m_Job.maxLevel)
                value = m_Job.maxLevel;
            int preLevel = m_Level;
            m_Level = value;
            changedLevel();
            if (preLevel != m_Level)
            {
                if (OnChangedLevel != null)
                    OnChangedLevel(m_Level);
            }
        }
    }

    public int exp // 현재 경험치 제어. get, set
    {
        get
        {
            return m_Experience;
        }
        set
        {
            if (level >= m_Job.maxLevel)
            {
                value = 0;
            }
            int preExp = m_Experience;
            m_Experience = value;
            calculateExp();
            if (preExp != m_Experience)
            {
                if (OnChangedExp != null)
                {
                    OnChangedExp(m_Experience, expRequired);
                }
            }
        }
    }

    public int expRequired // 현재 레벨 필요 경험치 Get
    {
        get
        {
            return getLevelInfo().requiredExp;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // Scene이 변경되어도 삭제하지 않음.
        gm = GameManager.instance;
        m_Job = gm.getCurrentJobInfo();
        m_Hp = hpMax;
        m_Sp = spMax;
        setMainSkill();
        boxCol.enabled = false;                                 // 충돌체 상시off
        controller = GetComponent<CharacterController>();       // controller 정의
        anim = GetComponent<Animator>();                        // meacanim   정의
    }
    void Start()
    {
        StartCoroutine(Regen());
    }
    void Update()
    {
        StateCheck();
        if(currentBaseState.fullPathHash != deadState)
            keyBoardInput();        
        LookUpdate();
        currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
    }

    private Stat getLevelInfo()
    {
        return m_Job.getStatInfo(m_Level);
    }

    public void LevelUp() // 레벨업
    {
        if (level >= m_Job.maxLevel)
        {
            exp = 0;
        }
        else
            level++;

        // 레벨업 Effect 실행

    }
    private void changedLevel()
    {
        // 현재 Hp, Sp 회복
        hp = hpMax;
        sp = spMax;

        // 레벨에 따른 메인스킬 변경
        setMainSkill();
    }

    private void setMainSkill()
    {
        // 레벨에 따른 메인스킬 변경
        m_curSkill = m_Job.getSkill(level);
    }

    private void calculateExp() // 현재 경험치 계산. 레벨업에 영향
    {
        while (m_Experience >= expRequired) // 현재 경험치 >= 필요경험치
        {
            m_Experience -= expRequired; // 새로운 현재 경험치 = 현재 경험치 - 필요경험치 
            LevelUp(); // 레벨업 한번
        }
    }

    void keyBoardInput()
    {
        xx = Input.GetAxis("Vertical"); //전진&후진
        zz = Input.GetAxis("Horizontal");   //좌우

        //Z키를 받을 때 공격
        if (Input.GetKeyDown(KeyCode.Z))
        {
            anim.SetTrigger("Combo1");
        }

        //x키를 받을 때  점프
        if (Input.GetKeyDown(KeyCode.X) && controller.isGrounded)
        {
            jumPing.y = jumplevel;
            anim.SetTrigger("Jump");
        }

        //c키를 받을 때 스킬사용
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (sp >= m_curSkill.requiredSp)
            {
                sp -= m_curSkill.requiredSp;
                // 스킬 애니메이션 결정                  
                anim.SetTrigger(m_curSkill.skillAnimationName);

                // 스킬 이펙트
                if (m_curSkill.effect != null)
                {
                    Instantiate(m_curSkill.effect, transformWeapon.position, Quaternion.identity);
                }
            }
        }

        //Up Down left Rightkey를 입력받을 시 이동 및 meacanim
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) ||
            Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            anim.SetBool("Idle", false);
            anim.SetBool("Run", true);
            lookDirection = xx * Vector3.forward + zz * Vector3.right;
            speed = runspeed;

            //LookDirection의 방향&거리로 Transform을 이동
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        //controller내부 함수를 이용하여 중력구현
        gravity_time();
        controller.Move(jumPing * Time.deltaTime);
    }

    //중력처리
    void gravity_time()
    {
        jumPing -= new Vector3(0, gravity * Time.deltaTime, 0);
    }

    //캐릭터 회전
    void LookUpdate()
    {
        //From(Player) -> LookDirection방향으로 회전
        if (!lookDirection.Equals(Vector3.zero))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                Quaternion.LookRotation(lookDirection),
                turnspeed);
        }
    }

    void StateCheck()
    {

        //Idle상태일때 모두 초기화
        if (currentBaseState.fullPathHash == idleState)
        {
            anim.SetBool("Run", false);
            anim.SetBool("Dead", false);
            anim.SetBool("JumpAttack", false);            
            //anim.ResetTrigger("Damage");
            anim.ResetTrigger("Skill1");
            anim.ResetTrigger("Jump");
            anim.ResetTrigger("Combo1");
            anim.ResetTrigger("Combo2");
            anim.ResetTrigger("Combo3");
            boxCol.enabled = false;
            curWeaponDamage = 0;
        }
        //죽었을때 전부초기화
        if (currentBaseState.fullPathHash == deadState)
        {
            anim.SetBool("Run", false);
            anim.SetBool("Dead", false);
            anim.SetBool("JumpAttack", false);
            anim.ResetTrigger("Damage");
            anim.ResetTrigger("Skill1");
            anim.ResetTrigger("Jump");
            anim.ResetTrigger("Combo1");
            anim.ResetTrigger("Combo2");
            anim.ResetTrigger("Combo3");
            curWeaponDamage = 0;
        }
        //달리기 상태(smoke 생성) & 정지상태
        if (currentBaseState.fullPathHash == runState)
        {
            Instantiate(Smoke, transform.position, Quaternion.identity);
            if (xx == 0 && zz == 0)
            {
                speed = 0f;
                anim.SetBool("Run", false);
                anim.SetBool("Idle", true);
            }
            curWeaponDamage = 0;
        }

        //점프공격
        if (currentBaseState.fullPathHash == jumpState && Input.GetKey(KeyCode.Z))
        {
            anim.SetBool("Run", false);
            anim.SetBool("Idle", false);
            anim.ResetTrigger("Jump");
            anim.SetBool("JumpAttack", true);
            anim.ResetTrigger("Combo1");
        }

        //~콤보2
        if (currentBaseState.fullPathHash == combo1State && Input.GetKey(KeyCode.Z))
        {
            anim.SetTrigger("Combo2");
        }
        //~콤보3
        if (currentBaseState.fullPathHash == combo2State && Input.GetKey(KeyCode.Z))
        {
            anim.SetTrigger("Combo3");
        }
        //콤보공격중 
        if (currentBaseState.fullPathHash == combo1State || currentBaseState.fullPathHash == combo2State)
        {
            curWeaponDamage = attackPower;
            boxCol.enabled = true;
            Invoke("BoxBox", 1.7f);

        }
        //콤보3. 모션시간이 길어서 별도의 조건처리를 하였음
        if (currentBaseState.fullPathHash == combo3State)
        {
            curWeaponDamage = attackPower;
            boxCol.enabled = true;
        }

        //스킬공격 - none effect
        if (currentBaseState.fullPathHash == skill1State)
        {
            curWeaponDamage = m_curSkill.getSkillDamage(attackPower);
            boxCol.enabled = true;
        }

        //스킬2(add effect)
        if (currentBaseState.fullPathHash == skill2State)
        {
            curWeaponDamage = m_curSkill.getSkillDamage(attackPower);
            boxCol.enabled = true;
        }

        //스킬3(add effect)
        if (currentBaseState.fullPathHash == skill3State)
        {
            curWeaponDamage = m_curSkill.getSkillDamage(attackPower);
            boxCol.enabled = true;
        }

        //점프공격
        if (currentBaseState.fullPathHash == JumpAttack)
        {
            curWeaponDamage = attackPower;
            boxCol.enabled = true;
        }

        if (currentBaseState.fullPathHash == damageState)
        {
            anim.SetBool("Idle", true);
        }
    }

    //invoke사용을 위한 메서드
    void BoxBox()
    {
        boxCol.enabled = false;
    }


    // 플레이어 몸체의 충돌
    void OnTriggerEnter(Collider col)
    {
        //죽음
        if (hp <= 0)
        {
            anim.SetBool("Dead", true);
        }

        //피격시
        if (col.gameObject.tag == "Bullet" || col.gameObject.tag == "bullet")
        {
            hp -= 0; // monsterWeapon
            anim.SetTrigger("Damage");
        }
    }

    // weapon의 충돌체에서 호출됨
    public void HitToPlayer(int damage)
    {
        hp -= damage;
        anim.SetBool("Idle", false);
        anim.SetTrigger("Damage");
    }

    IEnumerator Regen()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (hp > 0)
            {
                hp += hpRegenerate;
                sp += spRegenerate;
            }
        }
    }
}

