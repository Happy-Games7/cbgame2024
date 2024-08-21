using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using System;
using Spine.Unity;
//using static UnityEditor.Progress;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using UnityEngine.Rendering;
using UnityEditor;

public class MonsterController : MonoBehaviour
{
    public enum MonsterType
    {
        Normal,
        Elite,
        Boss,
    }

    public enum MonsterAttackType
    {
        Melee,
        Range,
    }

    public enum SpineAnimState
    {
        Move,
        Attack,
        Death,
    }

    public ParticleSystem boom_Particle;
    public ParticleSystem onDamaged_Particle;

    [SerializeField] UnityEngine.UI.Slider Slider;
    string currentAnimation;
    public SkeletonAnimation skeletonAnimation;
    public SpineAnimState spineAnimState { get; set; } = SpineAnimState.Move;
    public MonsterAttackType monsterAttackType = MonsterAttackType.Melee;
    public MonsterType monsterType = MonsterType.Normal;

    Rigidbody2D rigid;
    Rigidbody2D targetTr; // ������ ���
    PlayerContoller playerContoller;
    CapsuleCollider2D capsuleCollider;
    public GameObject hpBarObj;
    public Transform fireTr;
    public Transform effTr;
    public SpriteRenderer minimapSpot;
    public SpriteRenderer minimapSpot_Boss;

    // 원거리 공격
    float attackRange = 5.65f;
    float attackInterval = 3f;
    float originAttackInterval;
    public float normalTan_dam { get; set; } = 3f;
    public float normalTan_speed { get; set; } = 3f;

    // 근거리 공격
    public float attack_Power { get; set; } = 2f;

    public float burnPower { get; set; }

    public float current_HP { get; set; } = 29f;
    public float max_HP { get; set; } = 29f;    
    public float Boss_max_HP { get; set; } = 1200f;

    public float moveSpeed { get; set; } = 0.92f; // �̵� �ӵ�
    float originMoveSpeed;

    public int monster_score { get; set; } = 1;
    public bool isDie { get; set; } = false;
    bool canBeDamage = true;

    public bool Immun_Shock { get; set; } = false;
    public bool Immun_Freeze { get; set; } = false;
    public bool Immun_Burn { get; set; } = false;
    public bool Immun_Stun { get; set; } = false;
    public bool Immun_Knockback { get; set; } = false;
    public bool Immun_All { get; set; } = false;

    public bool Is_Shock { get; set; } = false; // 2초 동안 대상은 공격 / 이동 불가 상태가 되며, 30% 추가 데미지를 받는다.
    public bool Is_Freeze { get; set; } = false; // 3초 동안 대상은 공격속도 / 이동속도가 80% 감소한다.
    public bool Is_Burn { get; set; } = false; // 1.5초 동안 대상은 0.3초마다 피해량의 3%의 데미지를 받는다.
    public bool Is_Stun { get; set; } = false; // 2초 동안 대상은 공격 / 이동 불가 상태가 된다.
    public bool Is_Knockback { get; set; } = false;
    
    public bool IamBird { get; set; } = false;
    public int PrefabMonNum { get; set; } = 0;
    
    void OnEnable()
    {
        Init();
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        SpineAnimSetting();

        if ((monsterType == MonsterType.Normal || monsterType == MonsterType.Elite)
           && monsterAttackType == MonsterAttackType.Range
           && canRangeAttack
           && !isDie
           && !Is_Knockback
           && !Is_Stun
           && !Is_Shock
           && Vector2.Distance(targetTr.position, rigid.position) <= attackRange) RangeAttack();

        else if (monsterType == MonsterType.Boss           
            && !isDie
            && canBossAttack
            && Vector2.Distance(targetTr.position, rigid.position) <= attackRange) BossAttackStart();
        
    }

    private void FixedUpdate()
    {
        if (!isDie
            && !Is_Knockback
            && !Is_Stun
            && !Is_Shock) Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDie || !canBeDamage) return;

        if (collision.CompareTag("Player_Tan"))
        {
            collision.GetComponent<PlayerNormalTan>().PenetDecrease();
            OnDamaged(collision.GetComponent<PlayerNormalTan>().Attack_Power);
        }

        if (collision.CompareTag("Bible"))
        {
            OnDamaged(GameData.Instance.playerContoller.skill_Dam[1]);
            //Debug.Log("회복 전 :" + GameData.Instance.playerContoller.current_HP);
            GameData.Instance.playerContoller.current_HP += (GameData.Instance.playerContoller.skill_Dam[1] * 0.1f) * GameData.Instance.playerContoller.skill_Lv[1];
            if (GameData.Instance.playerContoller.current_HP > GameData.Instance.playerContoller.max_HP)
                GameData.Instance.playerContoller.current_HP = GameData.Instance.playerContoller.max_HP;
            //Debug.Log("회복 후 :" + GameData.Instance.playerContoller.current_HP);

            GameData.Instance.playerContoller.SetHpUpColor();
            GameData.Instance.playerContoller.HpBarUpdate();
        }

        if(collision.CompareTag("cyclone"))
        {
            OnDamaged(GameData.Instance.playerContoller.skill_Dam[6], true);            
        }
    }

    public void OnDamaged(float dam, bool cyclone = false)
    {
        if (isDie || !canBeDamage) return;

        canBeDamage = false;

        if (Is_Shock) dam *= 1.3f;
        
        onDamaged_Particle.Play(); 
        
        current_HP -= dam;
        HpBarUpdate();

        skeletonAnimation.skeleton.SetColor(new Color(0.8f, 0, 0, 1f));

        if(!cyclone) Mode_Knockback(targetTr.position);
        else Mode_Knockback(targetTr.position, 7.5f);
        
        Invoke("Recovery", 0.2f);
        Invoke("CanBeDamageTrue", 0.3f);
        if (current_HP <= 0 && !isDie) Die();        
    }

    void Recovery()
    {
        SetColorOrigin();
        rigid.velocity = Vector2.zero;
        Is_Knockback = false;
    }

    void CanBeDamageTrue()
    {
        canBeDamage = true;
    }

    void SetColorOrigin()
    {
        skeletonAnimation.skeleton.SetColor(new Color(1f, 1f, 1f));
    }

    void Die()
    {
        // 효과음 
        if (monsterType == MonsterType.Boss) SoundManager.Instance.PlaySe("ME002");
        else if (monsterType == MonsterType.Normal || monsterType == MonsterType.Elite) SoundManager.Instance.PlaySe("ME001");


        if (monsterType == MonsterType.Boss) GameData.Instance.uiManager.bossHpBar_Slider.gameObject.SetActive(false);

        isDie = true;

        if (effTr.childCount > 0)
        {
            for (int i = 0; i < effTr.childCount; i++)
            {                
                Destroy(effTr.GetChild(i).gameObject);
                //tr.gameObject.SetActive(false);
            }
        }

        shadow.enabled = false;

        Mode_All_End();
        capsuleCollider.enabled = false;
        hpBarObj.SetActive(false);
        spineAnimState = SpineAnimState.Death;

        RigidFreezePosition(false);
        attackInterval = originAttackInterval;        

        Invoke("ItemDrop", 0.3f);
        Invoke("SetActiveFalse", 1f);
    }

    void SetActiveFalse()
    {        
        if (PrefabMonNum == 537)
        {
            GameData.Instance.uiManager.bossHpbarObj1.SetActive(false);
            GameData.Instance.gameManager.Boss1_Die = true;
        }
        else if (PrefabMonNum == 557)
        {
            GameData.Instance.uiManager.bossHpbarObj2.SetActive(false);
            GameData.Instance.gameManager.Boss2_Die = true;
        }
        else if (PrefabMonNum == 513)
        {
            GameData.Instance.uiManager.bossHpbarObj3.SetActive(false);
            GameData.Instance.gameManager.Boss3_Die = true;
        }

        gameObject.SetActive(false);
    }

    public void BurnDamaged()
    {
        current_HP -= burnPower;
        HpBarUpdate();
        skeletonAnimation.skeleton.SetColor(new Color(0.8f, 0, 0, 1f));
        Invoke("SetColorOrigin", 0.15f);
        if (current_HP <= 0 && !isDie) Die();
    }

    void Init()
    {
        targetTr = GameData.Instance.playerContoller.GetComponent<Rigidbody2D>();
        playerContoller = GameData.Instance.playerContoller.GetComponent<PlayerContoller>();
        
        originAttackInterval = attackInterval;
        RigidFreezePosition(false);

        capsuleCollider.enabled = true;
        rigid.velocity = Vector2.zero;

        canRangeAttack = true;
        canBossAttack = true;
        canBeDamage = true;

        isDie = false;
        spineAnimState = SpineAnimState.Move;
        hpBarObj.SetActive(true);
        Mode_All_End();

        if (effTr.childCount > 0)
        {
            for (int i = 0; i < effTr.childCount; i++)
            {                
                Destroy(effTr.GetChild(i).gameObject);
                //tr.gameObject.SetActive(false);
            }
        }
    }

    SpriteRenderer shadow;
    private GameObject tempMonObj;    

    public void InitSetting(int _monNum, MonsterType mt, MonsterAttackType mat)
    {
        PrefabMonNum = _monNum;
        monsterType = mt;
        monsterAttackType = mat;

        // 미니맵 관련 설정
        switch (mt)
        {
            case MonsterType.Normal:
                minimapSpot_Boss.gameObject.SetActive(false);
                minimapSpot.gameObject.SetActive(true);
                minimapSpot.color = Color.red;
                minimapSpot.transform.localScale = new Vector3(5f, 5f, 5f);
                minimapSpot.sortingOrder = 0;
                break;
            case MonsterType.Elite:
                minimapSpot_Boss.gameObject.SetActive(false);
                minimapSpot.gameObject.SetActive(true);
                minimapSpot.color = Color.red;
                minimapSpot.transform.localScale = new Vector3(6.5f, 6.5f, 6.5f);
                minimapSpot.sortingOrder = 1;
                break;
            case MonsterType.Boss:
                minimapSpot_Boss.gameObject.SetActive(true);
                minimapSpot.gameObject.SetActive(false);                
                minimapSpot_Boss.transform.localScale = new Vector3(7.5f, 7.5f, 7.5f);
                minimapSpot_Boss.sortingOrder = 2;
                break;
        }
       
        // 체력, 이속, 공격력 셋팅 부분
        switch (_monNum)
        {
            case 3: // 3 칼을 든 고블린 (체력 200, 공격력 10, 이속 2)
                max_HP = 100f;
                current_HP = max_HP;
                HpBarUpdate();
                attack_Power = 6f;
                moveSpeed = 1.5f;
                monsterType = mt;
                break;
            case 4: // 4 궁수 고블린  (체력 250, 공격력 12, 이속 1.8)  ** 원거리
                max_HP = 70f;
                current_HP = max_HP;
                HpBarUpdate();
                attack_Power = 5f;
                moveSpeed = 1.3f;
                monsterType = mt;
                break;
            case 9: // 9 버섯 (체력 100, 공격력 5, 이속 1)
                max_HP = 80f;
                current_HP = max_HP;
                HpBarUpdate();
                attack_Power = 4f;
                moveSpeed = 1.4f;
                monsterType = mt;
                break;
            case 10: // 10 화난 버섯   (체력 250, 공격력 15, 이속 3)
                max_HP = 90f;
                current_HP = max_HP;
                HpBarUpdate();
                attack_Power = 5f;
                moveSpeed = 1.45f;
                monsterType = mt;
                break;
            case 43: // 43 마법사 고블린 (체력 350, 공격력 32, 이속 1.8)  ** 원거리
                max_HP = 80f;
                current_HP = max_HP;
                HpBarUpdate();
                attack_Power = 7f;
                moveSpeed = 1.35f;
                monsterType = mt;
                break;
            case 51: // 51 노란 새  (체력 200, 공격력 10, 이속 4)
                max_HP = 40f;
                current_HP = max_HP;
                HpBarUpdate();
                attack_Power = 1.9f;
                moveSpeed = 2.9f;
                monsterType = mt;
                break; 
            case 52: // 52 빨간 새 (체력 200, 공격력 10, 이속 4)
                max_HP = 35f;
                current_HP = max_HP;
                HpBarUpdate();
                attack_Power = 1.7f;
                moveSpeed = 2.9f;
                monsterType = mt;
                break;
            case 53: // 53 파리 (체력 200, 공격력 10, 이속 4)
                max_HP = 29f;
                current_HP = max_HP;
                HpBarUpdate();
                attack_Power = 1.5f;
                moveSpeed = 2.9f;
                monsterType = mt;
                break;
            case 55: // 55 박쥐  (체력 100, 공격력 5, 이속 4)
                max_HP = 45f;
                current_HP = max_HP;
                HpBarUpdate();
                attack_Power = 1.9f;
                moveSpeed = 2.9f;
                monsterType = mt;
                break;
            case 112: // 112 분홍 슬라임 (체력 30미만, 공격력 2, 이속 0.9)
                max_HP = 29f;
                current_HP = max_HP;
                HpBarUpdate();
                attack_Power = 2f;
                moveSpeed = 0.95f;
                monsterType = mt;
                break;
            case 116: // 116 초록 슬라임 (체력 30미만, 원거리 공격력 2, 근거리 공격력도 같은 수치로 셋팅, 이속 0.8)   ** 원거리
                max_HP = 69f;
                current_HP = max_HP;
                HpBarUpdate();
                attack_Power = 3f;
                moveSpeed = 1.29f;
                monsterType = mt;
                break;
            case 126: // 126 뿔 달린 버섯   (체력 350, 공격력 25, 이속 3)
                max_HP = 99f;
                current_HP = max_HP;
                HpBarUpdate();
                attack_Power = 4.5f;
                moveSpeed = 1.66f;
                monsterType = mt;
                break;
            case 138: // 138 창 고블린 (체력 300, 공격력 12, 이속 2.5)
                max_HP = 120f;
                current_HP = max_HP;
                HpBarUpdate();
                attack_Power = 5f;
                moveSpeed = 1.7f;
                monsterType = mt;
                break;
            case 140: // 140 늑대 탄 창 고블린  (체력 450, 공격력 22, 이속 3.8)
                max_HP = 140f;
                current_HP = max_HP;
                HpBarUpdate();
                attack_Power = 6f;
                moveSpeed = 1.9f;
                monsterType = mt;
                break;
            case 513: // 513 외계생명체 (최종 보스, 체력4000, 공격력 25, 이속 3)  ** 원거리
                Boss_max_HP = 1500f;
                current_HP = Boss_max_HP;
                HpBarUpdate();
                attack_Power = 10f;
                moveSpeed = 2.8f;
                monsterType = mt;
                break;
            case 537: // 537 슬라임 왕 (첫번째 보스, 체력2000, 원거리공격력 15, 이속 2)  ** 원거리
                Boss_max_HP = 600f;
                current_HP = Boss_max_HP;
                HpBarUpdate();
                attack_Power = 8f;
                moveSpeed = 2.5f;
                monsterType = mt;
                break;
            case 538: // 538 버서커 고블린 (보물상자 떨구는 엘리트, 체력1500, 공격력 25, 이속 3)
                max_HP = 350f;
                current_HP = max_HP;
                HpBarUpdate();
                attack_Power = 7f;
                moveSpeed = 2.1f;
                monsterType = mt;
                break;
            case 557: // 557 늑대 탄 왕고블린 (두번째 보스, 체력3000, 공격력30, 이속4)
                Boss_max_HP = 650f;
                current_HP = Boss_max_HP;
                HpBarUpdate();
                attack_Power = 9f;
                moveSpeed = 2.65f;
                monsterType = mt;
                break;
        }

        originMoveSpeed = moveSpeed;        

        if (_monNum == 51
            || _monNum == 52
            || _monNum == 53
            || _monNum == 55) IamBird = true;
        else IamBird = false;

        if(IamBird) capsuleCollider.isTrigger = true;
        else capsuleCollider.isTrigger = false;

        float _sizeSpine = 0.1f;
        Destroy(tempMonObj);

        string _cardStarStr = "Monster_JSS/MO_" + _monNum.ToString("000");

        // 게임 오브젝트 생성
        tempMonObj =
            Instantiate(Resources.Load(_cardStarStr), Vector3.zero, Quaternion.identity, transform
        ) as GameObject;

        skeletonAnimation = tempMonObj.transform.GetComponent<SkeletonAnimation>();

        skeletonAnimation.state.SetAnimation(0, "MV", true);

        tempMonObj.transform.localPosition = Vector3.zero + Vector3.down * 0.35f;        
        tempMonObj.transform.localScale = new Vector3(_sizeSpine, _sizeSpine, _sizeSpine);

        shadow = tempMonObj.GetComponentInChildren<SpriteRenderer>();
        shadow.enabled = true;
    }  

    #region 상태이상

    public void Mode_All_End()
    {
        Mode_Shock_End();
        Is_Freeze = false;
        Mode_Burn_End();
        Mode_Stun_End();
        Mode_Knockback_End();
    }
    
    public void Mode_Stun(float endTime = 3f)
    {
        if (Immun_All || Immun_Stun)
        {
            // 이뮨 상태일때 처리
            // 대미지 폰트 넣기
            return;
        }

        RigidFreezePosition(true);

        Is_Stun = true;
        
        //rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        GameObject stunEff = Instantiate(Resources.Load("Eff/common_CC_stun"), effTr.position, Quaternion.Euler(-75f, 0, 0), effTr) as GameObject;
        Destroy(stunEff, endTime);

        // 대미지 폰트 넣기

        CancelInvoke("Mode_Stun_End");
        Invoke("Mode_Stun_End", endTime);
    }

    void Mode_Stun_End()
    {
        Is_Stun = false;
        RigidFreezePosition(false);
        //rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Mode_Knockback(Vector2 attackerPos, float knockBack_Power = 3f)
    {
        if (Immun_All || Immun_Knockback)
        {
            // 이뮨 상태일때 처리
            // 대미지 폰트 넣기
            return;
        }

        Is_Knockback = true;

        if (monsterType == MonsterType.Boss) knockBack_Power = 0.3f;
        
        Vector2 dir = (rigid.position - attackerPos).normalized;
        rigid.AddForce(dir * knockBack_Power, ForceMode2D.Impulse);

        CancelInvoke("Mode_Knockback_End");
        Invoke("Mode_Knockback_End", 0.2f);
    }

    void Mode_Knockback_End()
    {
        Is_Knockback = false;
        rigid.velocity = Vector2.zero;                    
    }

    public void Mode_Burn(float endTime, float dam) // 보통 1.5초
    {
        if (Immun_All || Immun_Burn)
        {
            // 이뮨 상태 알려주는 폰트 넣기
            return;
        }
        
        burnPower = dam * 0.03f;

        if (!Is_Burn)
        {
            InvokeRepeating("Mode_BurnDamage", 0.3f, 0.3f);
        }

        Is_Burn = true;

        GameObject burnEff = Instantiate(Resources.Load("Eff/common_CC_burn"), effTr.position + Vector3.down * 0.35f, Quaternion.Euler(-45f, 0, 0), effTr) as GameObject;
        Destroy(burnEff, endTime);
               
        CancelInvoke("Mode_Burn_End");        
        Invoke("Mode_Burn_End", endTime);
    }

    void Mode_BurnDamage()
    {
        if (Is_Burn)
        {
            BurnDamaged();
        }
    }

    void Mode_Burn_End()
    {
        CancelInvoke("Mode_BurnDamage");
        Is_Burn = false;
    }    
    
    public void Mode_Freeze(float endTime = 3f)
    {
        if (Immun_All || Immun_Freeze)
        {
            return;
        }
        
        if (!Is_Freeze)
        {
            moveSpeed *= 0.2f;
            // 공격속도 감소 효과도 적용해야 한다
            attackInterval *= 1.8f;
        }
        
        Is_Freeze = true;

        GameObject freezeEff = Instantiate(Resources.Load("Eff/common_damage effect_ice"), effTr.position + Vector3.down * 1f, Quaternion.Euler(-45f, 0, 0), effTr) as GameObject;
        Destroy(freezeEff, endTime);        
        CancelInvoke("Mode_Freeze_End");
        Invoke("Mode_Freeze_End", endTime);
    }

    void Mode_Freeze_End()
    {
        Is_Freeze = false;
        moveSpeed = originMoveSpeed;
        attackInterval = originAttackInterval;
    }
    
    // 넉백, 스턴, 쇼크 3가지일때는 이동, 공격 불가하게 설정
    // 프리즈일때는 이동속도, 공격속도 감소되게 설정

    public void Mode_Shock(float endTime = 1.5f)
    {
        if(Immun_All || Immun_Shock)
        {
            return;
        }

        RigidFreezePosition(true);
        Is_Shock = true;
               
        GameObject shockEff = Instantiate(Resources.Load("Eff/common_CC_shock"), effTr.position + Vector3.down * 0.35f, Quaternion.identity, effTr) as GameObject;
        Destroy(shockEff, endTime);        
        CancelInvoke("Mode_Shock_End");
        Invoke("Mode_Shock_End", endTime);
    }

    void Mode_Shock_End()
    {
        Is_Shock = false;
        RigidFreezePosition(false);        
    }

    public void RigidFreezePosition(bool rock)
    {
        if(rock) rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        else rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    #endregion
           
    bool canRangeAttack = true;
    SpineAnimState originSpinAnimState;

    void RangeAttack()
    {
        canRangeAttack = false;
        
        originSpinAnimState = spineAnimState;
        spineAnimState = SpineAnimState.Attack;
        Invoke("SpinAnimToMove", 0.5f);

        GameObject tan = GameData.Instance.poolManager.Get(1);
        tan.transform.position = transform.position;
       
        Vector3 dir = GameData.Instance.playerContoller.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        tan.transform.localRotation = Quaternion.Euler(0, 0, angle);

        switch (PrefabMonNum)
        {
            case 4:
                tan.GetComponent<MonsterTan>().Init(normalTan_speed, 10f);
                break;
            case 43:
                tan.GetComponent<MonsterTan>().Init(normalTan_speed, 15f);
                break;
            case 116:
                tan.GetComponent<MonsterTan>().Init(normalTan_speed, 4f);
                break;
            default:
                tan.GetComponent<MonsterTan>().Init(normalTan_speed, normalTan_dam);
                break;
        }

        CancelInvoke("CoolTime_RangeAttack");
        Invoke("CoolTime_RangeAttack", attackInterval);
    }
  
    void CoolTime_RangeAttack()
    {        
        canRangeAttack = true;        
    }

    private void Move()
    {
        if (isDie || Is_Knockback || Is_Stun || Is_Shock) return;
        
        if(monsterAttackType == MonsterAttackType.Melee)
        {
            //if (!isDie) spineAnimState = SpineAnimState.Move;
            Vector2 dir = (targetTr.position - rigid.position).normalized;
            Vector2 moveVec = dir * moveSpeed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + moveVec);
        }
        else if(monsterAttackType == MonsterAttackType.Range)
        {
            if (Vector2.Distance(targetTr.position, rigid.position) > attackRange - 1.2f)
            {
                //if (!isDie) spineAnimState = SpineAnimState.Move;
                Vector2 dir = (targetTr.position - rigid.position).normalized;
                Vector2 moveVec = dir * moveSpeed * Time.fixedDeltaTime;
                rigid.MovePosition(rigid.position + moveVec);
            }           
        }

        if (targetTr.position.x > transform.position.x) 
            skeletonAnimation.transform.localScale = new Vector3(-0.1f, 0.1f, 0.1f);
        else skeletonAnimation.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        if(monsterAttackType == MonsterAttackType.Melee) 
            if (Vector2.Distance(targetTr.position, transform.position) < 0.1f) MeleeAttack();
    }

    void MeleeAttack()
    {
        if(!isDie) spineAnimState = SpineAnimState.Attack;
        Invoke("SpinAnimToMove", 0.5f);
    }    

    void SpineAnimSetting()
    {        
        switch (spineAnimState)
        {
            case SpineAnimState.Move:
                if(currentAnimation != "MV" && !isDie && spineAnimState != SpineAnimState.Attack)
                {
                    skeletonAnimation.state.SetAnimation(0, "MV", true);
                    currentAnimation = "MV";
                }
                break;           
            case SpineAnimState.Attack:
                if(currentAnimation != "AK" && !isDie)
                {
                    skeletonAnimation.state.SetAnimation(0, "AK", false);
                    currentAnimation = "AK";
                }
                break;
            case SpineAnimState.Death:
                if (currentAnimation != "DT")
                {
                    skeletonAnimation.state.SetAnimation(0, "DT", false);
                    currentAnimation = "DT";
                }
                break;                    
        }
    }
                                        
    void HpBarUpdate()
    {
        if (PrefabMonNum == 537)
        {
            Slider.value = current_HP / Boss_max_HP;
            GameData.Instance.uiManager.bossHpBar_Slider.value = current_HP / Boss_max_HP;
        }
        else if (PrefabMonNum == 557)
        {
            Slider.value = current_HP / Boss_max_HP;
            GameData.Instance.uiManager.bossHpBar2_Slider.value = current_HP / Boss_max_HP;
        }
        else if (PrefabMonNum == 513)
        {
            Slider.value = current_HP / Boss_max_HP;
            GameData.Instance.uiManager.bossHpBar3_Slider.value = current_HP / Boss_max_HP;
        }
        else
            Slider.value = current_HP / max_HP;
    }

    void ItemDrop()
    {
        // 경험치 보석 드랍
        GameObject exp = GameData.Instance.poolManager.Get(4);                            
        exp.transform.position = transform.position;
        Item expIt = exp.GetComponent<Item>();
        expIt.ExpItemInit(monsterType);
        
        // 특수 아이템 드랍
        if (monsterType == MonsterType.Elite && PrefabMonNum == 538)
        {
            GameObject treasure = GameData.Instance.poolManager.Get(4);
            treasure.transform.position = transform.position;
            Item treasureIt = treasure.GetComponent<Item>();
            treasureIt.TreasureItemInit();            
        }
        else if(monsterType == MonsterType.Normal)
        {
            if(Item.canDropBoom)
            {                
                GameObject boom = GameData.Instance.poolManager.Get(4);
                boom.transform.position = transform.position;
                Item boomIt = boom.GetComponent<Item>();
                boomIt.BoomItemInit();               
            }
            else if(Item.canDropMagnetic)
            {
                GameObject magnetic = GameData.Instance.poolManager.Get(4);
                magnetic.transform.position = transform.position;
                Item magneticIt = magnetic.GetComponent<Item>();
                magneticIt.MagneticItemInit();                
            }
            else if (Item.canDropPotion)
            {
                GameObject potion = GameData.Instance.poolManager.Get(4);
                potion.transform.position = transform.position;
                Item potionIt = potion.GetComponent<Item>();
                potionIt.PotionItemInit();                
            }
        }
    }
    
    bool canBossAttack = true;
    int patternNum = 1;    

    void BossAttackStart()
    {
        if (!canBossAttack) return;
        
        canBossAttack = false;

        // test --------------------------------------------
        //BossAttackPattern_1_Circle();
        //StartCoroutine(BossAttackPattern_2_Spin());
        //StartCoroutine(BossAttackPattern_3_ShotGun());
        //BossAttackPattern_4_FanShape();

        switch (PrefabMonNum)
        {
            case 537:
                BossAttackPattern_1_Circle();                
                break;
            case 557:
                StartCoroutine(BossAttackPattern_2_Spin());
                break;
            case 513:                
                if(patternNum == 1)
                {
                    StartCoroutine(BossAttackPattern_2_Spin());                    
                    patternNum = 2;
                }
                else if(patternNum == 2)
                {
                    BossAttackPattern_1_Circle();
                    patternNum = 3;                    
                }
                else if (patternNum == 3)
                {                    
                    BossAttackPattern_4_FanShape();
                    patternNum = 4;
                }
                else if (patternNum == 4)
                {
                    StartCoroutine(BossAttackPattern_3_ShotGun());
                    patternNum = 1;
                }
                break;                                                    
        }
    }

    void BossAttackPattern_1_Circle() // 1번째 보스
    {
        originSpinAnimState = spineAnimState;
        spineAnimState = SpineAnimState.Attack;
        for (int i = 0; i < 360; i += 24)
        {
            GameObject tan = GameData.Instance.poolManager.Get(2);           
            tan.transform.SetLocalPositionAndRotation(transform.position, Quaternion.Euler(0f, 0f, i));
            tan.GetComponent<MonsterTan>().Init(normalTan_speed * 1.2f, 8f);
        }

        Invoke("SpinAnimToMove", 0.5f);
        CancelInvoke("CoolTime_BossAttackPattern");
        Invoke("CoolTime_BossAttackPattern", 2.7f);        
    }

    public Rotate rt;
    WaitForSeconds waitForSecond005 = new WaitForSeconds(0.05f);

    IEnumerator BossAttackPattern_2_Spin() // 2번째 보스
    {
        originSpinAnimState = spineAnimState;
        spineAnimState = SpineAnimState.Attack;
        Invoke("SpinAnimToMove", 0.5f);
        fireTr.localRotation = Quaternion.identity;
        rt.Is_Rotate = true;
                
        for (int i = 0; i < 50; i++)
        {
            yield return waitForSecond005;

            GameObject tan = GameData.Instance.poolManager.Get(2);
            tan.transform.SetLocalPositionAndRotation(fireTr.position, fireTr.localRotation);
            tan.GetComponent<MonsterTan>().Init(normalTan_speed * 1.4f, 9f);
        }
        
        rt.Is_Rotate = false;
        fireTr.localRotation = Quaternion.identity;
        
        CancelInvoke("CoolTime_BossAttackPattern");
        Invoke("CoolTime_BossAttackPattern", 2.4f);
    }

    WaitForSeconds waitForSecond015 = new WaitForSeconds(0.19f);

    IEnumerator BossAttackPattern_3_ShotGun() // 3번째 보스
    {
        originSpinAnimState = spineAnimState;
        spineAnimState = SpineAnimState.Attack;
        Invoke("SpinAnimToMove", 0.5f);

        for (int i = 0; i < 3; i++)
        {
            GameObject tan = GameData.Instance.poolManager.Get(2);            
            Vector3 dir = GameData.Instance.playerContoller.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            tan.transform.position = transform.position;
            tan.transform.localRotation = Quaternion.Euler(0, 0, angle);
            tan.GetComponent<MonsterTan>().Init(normalTan_speed * 1.2f, 10f, true, 3);
            
            GameObject tan2 = GameData.Instance.poolManager.Get(2);
            tan2.transform.position = transform.position;
            tan2.transform.localRotation = Quaternion.Euler(0, 0, angle + Random.Range(-40f, 40f));
            tan2.GetComponent<MonsterTan>().Init(normalTan_speed * 1.2f, 10f, true, 3);
            
            GameObject tan3 = GameData.Instance.poolManager.Get(2);
            tan3.transform.position = transform.position;
            tan3.transform.localRotation = Quaternion.Euler(0, 0, angle - Random.Range(-40f, 40f));
            tan3.GetComponent<MonsterTan>().Init(normalTan_speed * 1.2f, 10f, true, 3);
            
            GameObject tan4 = GameData.Instance.poolManager.Get(2);
            tan4.transform.position = transform.position;
            tan4.transform.localRotation = Quaternion.Euler(0, 0, angle - Random.Range(-40f, 40f));
            tan4.GetComponent<MonsterTan>().Init(normalTan_speed * 1.2f, 10f, true, 3);

            yield return waitForSecond015;
        }

        CancelInvoke("CoolTime_BossAttackPattern");
        Invoke("CoolTime_BossAttackPattern", 1.1f);
    }
    
    void BossAttackPattern_4_FanShape() // 3번째 보스
    {
        originSpinAnimState = spineAnimState;
        spineAnimState = SpineAnimState.Attack;

        GameObject tan = GameData.Instance.poolManager.Get(2);
        tan.transform.position = transform.position;
        Vector3 dir = GameData.Instance.playerContoller.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;                
        tan.transform.localRotation = Quaternion.Euler(0, 0, angle);
        tan.GetComponent<MonsterTan>().Init(normalTan_speed * 1.4f, 10f, true, 3);
        
        GameObject[] tanArr = new GameObject[6];
        float minusAngle = angle;
        float plusAngle = angle;

        for (int i = 0; i < 3; i++)
        {
            plusAngle += 18.5f;
            tanArr[i] = GameData.Instance.poolManager.Get(2);
            tanArr[i].transform.SetLocalPositionAndRotation(transform.position, Quaternion.Euler(0, 0, plusAngle));
            tanArr[i].GetComponent<MonsterTan>().Init(normalTan_speed * 1.4f, 10f, true, 3);
        }

        for (int i = 3; i < 6; i++)
        {
            minusAngle -= 18.5f;
            tanArr[i] = GameData.Instance.poolManager.Get(2);
            tanArr[i].transform.SetLocalPositionAndRotation(transform.position, Quaternion.Euler(0, 0, minusAngle));
            tanArr[i].GetComponent<MonsterTan>().Init(normalTan_speed * 1.4f, 10f, true, 3);
        }

        Invoke("SpinAnimToMove", 0.5f);
        CancelInvoke("CoolTime_BossAttackPattern");
        Invoke("CoolTime_BossAttackPattern", 1.1f);      
    }    

    void CoolTime_BossAttackPattern()
    {        
        canBossAttack = true;        
    }

    void SpinAnimToMove()
    {
        if (!isDie) spineAnimState = SpineAnimState.Move;
    }

    public void Boom()
    {     
        boom_Particle.Play();        
        canBeDamage = true;
        OnDamaged(9999f);
    }
}
