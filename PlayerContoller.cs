using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Text;
using Unity.VisualScripting;
using Spine.Unity;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Random = UnityEngine.Random;
using static System.Net.Mime.MediaTypeNames;
using DG.Tweening;

public class PlayerContoller : MonoBehaviour
{
    public ParticleSystem frozenField;
    public ParticleSystem magicZin;
    public ParticleSystem lvUp_Particle;
    public ParticleSystem coldCounterAttack_Particle;

    public VariableJoystick joy;
    public SpriteRenderer minimapIcon;
    Vector2 minimapDir;
    
    public const float map_Limit_Max_X = 17.2f;
    public const float map_Limit_Min_X = -17.2f;
    public const float map_Limit_Max_Y = 11.5f;
    public const float map_Limit_Min_Y = -12f;
    
    public enum SpineAnimState
    {
        stay,
        move,
        attack,    
        death
    }

    public TextMeshProUGUI text_HpRecovery;
    [SerializeField] UnityEngine.UI.Slider hpBar_Slider;
    public TextMeshProUGUI expText;
    public LayerMask targetLayer;
    public LayerMask monsterTanLayer;
    public PlayerBible playerBible;
    public SpineAnimState spineAnimState { get; set; }
    public SkeletonAnimation skeletonAnim;        
    string currentAnimation;
    Rigidbody2D rigid;    
    Vector2 moveDir;        
    public Transform effTr;

    public float current_HP { get; set; } = 100f;
    public float max_HP { get; set; } = 100f;
    public float moveSpeed { get; set; } = 4.5f; // 최대 이속 5.5 정도로 설정하면 될것 같다
    float originMoveSpeed;
    
    public float retained_EXP { get; set; } = 0;
    public float required_EXP_forLvUp { get; set; }    

    // -----------------------------  [  Skill  ]  ------------------------------------------------


    bool is_ShowingAnotherSkill = false; // 이것이 false가 되어야지만, 대부분의 액티브 스킬이 시전될 수 있다        
    // 0~8 9개가 액티브 스킬, 9~14까지가 패시브 스킬
    public bool[] skill_hadLearn = new bool[15];
    public int[] skill_Lv = new int[15];
    public float[] skill_Dam = new float[10];
    public bool[] skill_canStart = new bool[9];
    float[] skill_coolTime = new float[9];    
    float skill_duration_002_index1 = 5f; // 서먼 위스프
    WaitForSeconds skill_duration_index1_SummonWisp_waitForSec;
    float skill_duration_004_index3 = 5.5f; // 프로즌 필드
    public float Skill_Range_common { get; set; } = 4.8f;
    //public float skill_active_count { get; set; } = 1;
    //public float skill_passive_count { get; set; } = 0;

    // 001. 윈드 애로우 [0]    
    public float normalTan_speed { get; set; } = 6.5f;
    public int bulletCount { get; set; } = 1;
    public int penet { get; set; } = 1;    

    bool canBeDamage = true; // 공격 받기가 가능한지
    public int playerLevel { get; set; } = 1;
    public int itemRange_Lv { get; private set; } = 1; // 안 쓸 예정, 파밍 범위 증가 아이템을 구현하지않았다
    public bool isDie { get; set; } = false;

    // 공격받을 화상 대미지
    public float burnPower { get; set; }

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

    WaitForSeconds waitforsec0002 = new WaitForSeconds(0.002f);
    WaitForSeconds waitForSec08 = new WaitForSeconds(0.8f);
    WaitForSeconds waitForSec05 = new WaitForSeconds(0.5f);

    private void Awake()
    {
        GameData.Instance.playerContoller = this;
    }
    
    void Start()
    {
        Init();                   
    }
    
    void Update()
    {                
        SpinAnimSetting();
        KeyInput();

        if (skill_canStart[0]
            && !Is_Knockback
            && !Is_Shock
            && !Is_Stun) NormalTanAttack();
        
        if (!Is_Knockback 
            && !Is_Shock 
            && !Is_Stun) SkillStart();

        if (isDie)
        {
            StopAllCoroutines();
        }
    }

    private void FixedUpdate()
    {
        if (!isDie)
        {
            if (moveDir == Vector2.zero)
            {
                if (spineAnimState != SpineAnimState.attack) spineAnimState = SpineAnimState.stay;
            }
            else
            {
                if (!Is_Knockback && !Is_Shock && !Is_Stun) Move();
            }
        }        
    }

    #region 상태이상

    public void Mode_All_End()
    {
        Is_Freeze = false;
        Mode_Shock_End();        
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
        Is_Stun = true;
        rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        GameObject stunEff = Instantiate(Resources.Load("Eff/common_CC_stun"), effTr.position, Quaternion.Euler(-75f, 0, 0), effTr) as GameObject;
        Destroy(stunEff, endTime);

        // 대미지 폰트 넣기

        CancelInvoke("Mode_Stun_End");
        Invoke("Mode_Stun_End", endTime);
    }

    void Mode_Stun_End()
    {
        Is_Stun = false;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
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

    public void Mode_Burn(float endTime, float dam)
    {
        if (Immun_All || Immun_Burn)
        {
            // 이뮨 상태 알려주는 폰트 넣기
            return;
        }
        Is_Burn = true;
        GameObject burnEff = Instantiate(Resources.Load("Eff/common_CC_burn"), effTr.position + Vector3.down * 0.35f, Quaternion.Euler(-45f, 0, 0), effTr) as GameObject;
        Destroy(burnEff, endTime);
        burnPower = dam * 0.03f;
        CancelInvoke("Mode_Burn_End");
        InvokeRepeating("Mode_BurnDamage", 0.3f, 0.3f);
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
   
    public void Mode_Freeze(float endTime)
    {
        if (Immun_All || Immun_Freeze)
        {
            return;
        }
        Is_Freeze = true;
        moveSpeed *= 0.2f;
        // 공격속도 감소 효과도 적용해야 한다
        GameObject freezeEff = Instantiate(Resources.Load("Eff/common_damage effect_ice"), effTr.position + Vector3.down * 0.35f, Quaternion.Euler(-45f, 0, 0), effTr) as GameObject;
        Destroy(freezeEff, endTime);
        CancelInvoke("Mode_Freeze_End");
        Invoke("Mode_Freeze_End", endTime);
    }

    void Mode_Freeze_End()
    {
        Is_Freeze = false;
        moveSpeed = originMoveSpeed;
    }
    
    // 넉백, 스턴, 쇼크 3가지일때는 공격 불가하게 설정
    // 프리즈일때는 공격속도 감소되게 설정

    public void Mode_Shock(float endTime)
    {
        if (Immun_All || Immun_Shock)
        {
            return;
        }
        Is_Shock = true;
        rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        GameObject shockEff = Instantiate(Resources.Load("Eff/common_CC_shock"), effTr.position + Vector3.down * 0.35f, Quaternion.identity, effTr) as GameObject;
        Destroy(shockEff, endTime);
        CancelInvoke("Mode_Shock_End");
        Invoke("Mode_Shock_End", endTime);
    }

    void Mode_Shock_End()
    {
        Is_Shock = false;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    #endregion    

    void KeyInput()
    {
        moveDir.x = joy.Horizontal;
        moveDir.y = joy.Vertical;
        minimapDir.x = joy.Horizontal;
        minimapDir.y = joy.Vertical;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            moveDir.y = 1;
            minimapDir.y = 1;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveDir.x = -1;
            minimapDir.x = -1;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            moveDir.y = -1;
            minimapDir.y = -1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveDir.x = 1;
            minimapDir.x = 1;
        }

        //moveDir.x = Input.GetAxisRaw("Horizontal");        
        if ((rigid.position.x >= map_Limit_Max_X && moveDir.x == 1) || (rigid.position.x <= map_Limit_Min_X && moveDir.x == -1)) moveDir.x = 0;

        //moveDir.y = Input.GetAxisRaw("Vertical");        
        if ((rigid.position.y >= map_Limit_Max_Y && moveDir.y == 1) || (rigid.position.y <= map_Limit_Min_Y && moveDir.y == -1)) moveDir.y = 0;

        if (minimapDir.x == 1 && minimapDir.y == 0)
        {
            minimapIcon.flipX = false;
            minimapIcon.transform.rotation = Quaternion.identity;
        }
        else if (minimapDir.x == 1 && minimapDir.y == 1)
        {
            minimapIcon.flipX = false;
            minimapIcon.transform.rotation = Quaternion.Euler(0, 0, 45f);
        }
        else if (minimapDir.x == -1 && minimapDir.y == 0)
        {
            minimapIcon.flipX = true;
            minimapIcon.transform.rotation = Quaternion.identity;
        }
        else if (minimapDir.x == -1 && minimapDir.y == 1)
        {
            minimapIcon.flipX = true;
            minimapIcon.transform.rotation = Quaternion.Euler(0, 0, -45f);
        }
        else if (minimapDir.y == 1 && minimapDir.x == 0)
        {
            minimapIcon.flipX = false;
            minimapIcon.transform.rotation = Quaternion.Euler(0, 0, 90f);
        }
        else if (minimapDir.y == -1 && minimapDir.x == 1)
        {
            minimapIcon.flipX = false;
            minimapIcon.transform.rotation = Quaternion.Euler(0, 0, -45f);
        }
        else if (minimapDir.y == -1 && minimapDir.x == 0)
        {
            minimapIcon.flipX = false;
            minimapIcon.transform.rotation = Quaternion.Euler(0, 0, -90f);
        }
        else if (minimapDir.y == -1 && minimapDir.x == -1)
        {
            minimapIcon.flipX = true;
            minimapIcon.transform.rotation = Quaternion.Euler(0, 0, 45f);
        }
    }

    List<int> skillsToStartList = new List<int>();
    
    void SkillStart()
    {
        if (skill_hadLearn[1] && skill_canStart[1] && !skillsToStartList.Contains(1))
        {
            skillsToStartList.Add(1);
        }
        else if (skill_hadLearn[2] && skill_canStart[2] && !skillsToStartList.Contains(2))
        {
            skillsToStartList.Add(2);
        }
        else if (skill_hadLearn[3] && skill_canStart[3] && !skillsToStartList.Contains(3))
        {
            skillsToStartList.Add(3);
        }
        else if(skill_hadLearn[4] && skill_canStart[4] && !skillsToStartList.Contains(4))
        {
            skillsToStartList.Add(4);
        }
        else if (skill_hadLearn[5] && skill_canStart[5] && !skillsToStartList.Contains(5))
        {
            skillsToStartList.Add(5);
        }
        else if (skill_hadLearn[6] && skill_canStart[6] && !skillsToStartList.Contains(6))
        {
            skillsToStartList.Add(6);
        }
        else if (skill_hadLearn[7] && skill_canStart[7] && !skillsToStartList.Contains(7))
        {
            skillsToStartList.Add(7);
        }
        else if (skill_hadLearn[8] && skill_canStart[8] && !skillsToStartList.Contains(8))
        {
            skillsToStartList.Add(8);
        }

        if (skillsToStartList.Count > 0)
        {
            if (!is_ShowingAnotherSkill)
            {
                is_ShowingAnotherSkill = true;
                ShowSkill(skillsToStartList[0]);
                skillsToStartList.RemoveAt(0);
            }
        }
    }    
  
    void ShowSkill(int skill_index)
    {
        skill_canStart[skill_index] = false;
                        
        switch (skill_index)
        {
            case 1:
                RaycastHit2D hit1 = Physics2D.CircleCast(transform.position, Skill_Range_common, Vector2.zero, 0, targetLayer);
                if (hit1)
                {
                    MagicZinFirst(skill_index);
                }
                else
                {
                    SummonWisp_index1_End();
                    Is_ShowingAnotherSkillFalse();
                }                
                break;
            case 2:                                
                RaycastHit2D hit2 = Physics2D.CircleCast(transform.position, Skill_Range_common, Vector2.zero, 0, targetLayer);
                if (hit2)
                {
                    MagicZinFirst(skill_index);
                }
                else
                {
                    LightningBolt003_index2_End();
                    Is_ShowingAnotherSkillFalse();
                }

                break;
            case 3:
                RaycastHit2D hit3 = Physics2D.CircleCast(transform.position, Skill_Range_common, Vector2.zero, 0, targetLayer);
                if (hit3)
                {
                    MagicZinFirst(skill_index);
                }
                else
                {                   
                    FrozenField004_index3_End();
                    Is_ShowingAnotherSkillFalse();
                }                
                break;
            case 4:
                RaycastHit2D hit4 = Physics2D.CircleCast(transform.position, Skill_Range_common, Vector2.zero, 0, targetLayer);
                if (hit4)
                {
                    MagicZinFirst(skill_index);
                }
                else
                {
                    FireNova_index4_End();
                    Is_ShowingAnotherSkillFalse();
                }
                break;
            case 5:
                RaycastHit2D hit5 = Physics2D.CircleCast(transform.position, Skill_Range_common, Vector2.zero, 0, targetLayer);
                if (hit5)
                {
                    MagicZinFirst(skill_index);
                }
                else
                {
                    EarthQuake_index5_End();
                    Is_ShowingAnotherSkillFalse();
                }
                break;
            case 6:
                RaycastHit2D hit6 = Physics2D.CircleCast(transform.position, 5.5f, Vector2.zero, 0, targetLayer);
                if (hit6)
                {
                    MagicZinFirst(skill_index);
                }
                else
                {
                    Cyclone_index6_End();
                    Is_ShowingAnotherSkillFalse();
                }
                break;
            case 7:
                RaycastHit2D hit7 = Physics2D.CircleCast(transform.position, 5.5f, Vector2.zero, 0, targetLayer);
                if (hit7)
                {
                    MagicZinFirst(skill_index);
                }
                else
                {
                    Meteor_index7_End();
                    Is_ShowingAnotherSkillFalse();
                }
                break;
            case 8:
                RaycastHit2D hit8 = Physics2D.CircleCast(transform.position, 5.5f, Vector2.zero, 0, targetLayer);
                if (hit8)
                {
                    MagicZinFirst(skill_index);
                }
                else
                {
                    ShockWave_index8_End();
                    Is_ShowingAnotherSkillFalse();
                }
                break;
        }
    }

    void MagicZinFirst(int skill_index)
    {
        //float delay = 0.8f;
        magicZin.Play();

        // 효과음 
        SoundManager.Instance.PlaySe("SE001");

        switch (skill_index)
        {
            case 1:
                StartCoroutine(SummonWisp_index1());
                break;
            case 2:
                StartCoroutine(LightningBolt003_index2());
                break;
            case 3:
                StartCoroutine(FrozenField004_index3());
                break;
            case 4:
                StartCoroutine(FireNova_index4());
                break;
            case 5:
                StartCoroutine(EarthQuake_index5());
                break;
            case 6:
                StartCoroutine(Cyclone_index6());
                break;
            case 7:
                StartCoroutine(Meteor_index7());
                break;
            case 8:
                StartCoroutine(ShockWave_index8());
                break;
        }               
    }
    
    IEnumerator ShockWave_index8()
    {
        yield return waitForSec08;
        
        RaycastHit2D hit8 = Physics2D.CircleCast(transform.position, 5.5f, Vector2.zero, 0, targetLayer);
        if (hit8)
        {
            GameObject go = Instantiate(Resources.Load("Skill/skill006_double impact"), hit8.transform.position, Quaternion.identity) as GameObject;
            Destroy(go, 2f);
            
            spineAnimState = PlayerContoller.SpineAnimState.attack;
            CancelInvoke("SetPlayerStateStay");
            Invoke("SetPlayerStateStay", 0.4f);

            yield return waitForSec05;

            SoundManager.Instance.PlaySe("earthQuake");

            RaycastHit2D[] targets = Physics2D.CircleCastAll(hit8.transform.position, 4.3f, Vector2.zero, 0, targetLayer);
            foreach (RaycastHit2D target in targets)
            {
                if (target)
                {
                    MonsterController mc = target.transform.GetComponent<MonsterController>();
                    mc.OnDamaged(skill_Dam[8]);
                    mc.Mode_Stun();
                }
            }

            // 총알 제거
            //RaycastHit2D hitMonsterTan = Physics2D.CircleCast(hit8.transform.position, 4.3f, Vector2.zero, 0, monsterTanLayer);
            //if (hitMonsterTan)
            //{
            //    RaycastHit2D[] monsterTantargets = Physics2D.CircleCastAll(hit8.transform.position, 4.3f, Vector2.zero, 0, monsterTanLayer);
            //    foreach (RaycastHit2D target in monsterTantargets)
            //    {
            //        if (target)
            //        {
            //            GameObject tanExplosion = Instantiate(Resources.Load("Bullet/basic_TanExplosion06"), target.transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
            //            Destroy(tanExplosion, 0.6f);                       

            //            target.transform.gameObject.SetActive(false);
            //        }
            //    }
            //}            

            yield return waitForSec05;

            Is_ShowingAnotherSkillFalse();
            Invoke("ShockWave_index8_End", skill_coolTime[8]);
        }
        else
        {
            Is_ShowingAnotherSkillFalse();
            ShockWave_index8_End();
        }                
    }

    void ShockWave_index8_End()
    {
        skill_canStart[8] = true;
    }    

    IEnumerator Meteor_index7()
    {
        yield return waitForSec08;
        
        RaycastHit2D hit7 = Physics2D.CircleCast(transform.position, 5.5f, Vector2.zero, 0, targetLayer);
        if (hit7)
        {
            GameObject go = Instantiate(Resources.Load("Skill/skill205_meteor"), hit7.transform.position, Quaternion.identity) as GameObject;
            Destroy(go, 2f);

            spineAnimState = PlayerContoller.SpineAnimState.attack;
            CancelInvoke("SetPlayerStateStay");
            Invoke("SetPlayerStateStay", 0.4f);

            yield return waitForSec05;

            // 효과음 
            SoundManager.Instance.PlaySe("burn");

            RaycastHit2D[] targets = Physics2D.CircleCastAll(hit7.transform.position, 4.3f, Vector2.zero, 0, targetLayer);
            foreach (RaycastHit2D target in targets)
            {
                if (target)
                {
                    MonsterController mc = target.transform.GetComponent<MonsterController>();
                    mc.OnDamaged(skill_Dam[7]);
                    mc.Mode_Burn(1.5f, skill_Dam[7]);
                }
            }

            // 총알 제거
            //RaycastHit2D hitMonsterTan = Physics2D.CircleCast(hit7.transform.position, 4.3f, Vector2.zero, 0, monsterTanLayer);
            //if (hitMonsterTan)
            //{
            //    RaycastHit2D[] monsterTantargets = Physics2D.CircleCastAll(hit7.transform.position, 4.3f, Vector2.zero, 0, monsterTanLayer);
            //    foreach (RaycastHit2D target in monsterTantargets)
            //    {
            //        if (target)
            //        {
            //            GameObject tanExplosion = Instantiate(Resources.Load("Bullet/basic_TanExplosion06"), target.transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
            //            Destroy(tanExplosion, 0.6f);                      

            //            target.transform.gameObject.SetActive(false);
            //        }
            //    }
            //}

            yield return waitForSec05;

            Is_ShowingAnotherSkillFalse();
            Invoke("Meteor_index7_End", skill_coolTime[7]);
        }
        else
        {
            Is_ShowingAnotherSkillFalse();
            Meteor_index7_End();
        }                
    }

    void Meteor_index7_End()
    {
        skill_canStart[7] = true;
    }    

    IEnumerator Cyclone_index6()
    {
        yield return waitForSec08;
        
        RaycastHit2D hit6 = Physics2D.CircleCast(transform.position, 5.5f, Vector2.zero, 0, targetLayer);
        if (hit6)
        {
            SoundManager.Instance.PlaySe("frozenField");
            GameObject go = Instantiate(Resources.Load("Skill/cyclone"), transform) as GameObject;
            go.transform.position = transform.position;
            Vector3 dir = (hit6.transform.position - transform.position).normalized;
            go.GetComponent<PlayerNormalTan>().SetDir(dir, true);

            spineAnimState = PlayerContoller.SpineAnimState.attack;
            CancelInvoke("SetPlayerStateStay");
            Invoke("SetPlayerStateStay", 0.4f);

            yield return waitForSec05;

            Is_ShowingAnotherSkillFalse();
            Invoke("Cyclone_index6_End", skill_coolTime[6]);
        }
        else
        {
            Is_ShowingAnotherSkillFalse();
            Cyclone_index6_End();
        }        
    }

    void Cyclone_index6_End()
    {
        skill_canStart[6] = true;
    }

    IEnumerator EarthQuake_index5()
    {
        yield return waitForSec08;
        
        RaycastHit2D hit8 = Physics2D.CircleCast(transform.position, Skill_Range_common, Vector2.zero, 0, targetLayer);
        if (hit8)
        {
            GameObject go = Instantiate(Resources.Load("Skill/skill206_earth quake"), transform.position, Quaternion.identity) as GameObject;
            Destroy(go, 2.7f);

            SoundManager.Instance.PlaySe("earthQuake");

            spineAnimState = PlayerContoller.SpineAnimState.attack;
            CancelInvoke("SetPlayerStateStay");
            Invoke("SetPlayerStateStay", 0.4f);

            RaycastHit2D[] targets = Physics2D.CircleCastAll(transform.position, 6f, Vector2.zero, 0, targetLayer);
            foreach (RaycastHit2D target in targets)
            {
                if (target)
                {
                    MonsterController mc = target.transform.GetComponent<MonsterController>();
                    mc.OnDamaged(skill_Dam[5]);
                    mc.Mode_Stun(3f);
                }
            }

            // 총알 제거
            //RaycastHit2D hitMonsterTan = Physics2D.CircleCast(transform.position, 6f, Vector2.zero, 0, monsterTanLayer);
            //if (hitMonsterTan)
            //{
            //    RaycastHit2D[] monsterTantargets = Physics2D.CircleCastAll(transform.position, 6f, Vector2.zero, 0, monsterTanLayer);
            //    foreach (RaycastHit2D target in monsterTantargets)
            //    {
            //        if (target)
            //        {
            //            GameObject tanExplosion = Instantiate(Resources.Load("Bullet/basic_TanExplosion06"), target.transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
            //            Destroy(tanExplosion, 0.6f);                      

            //            target.transform.gameObject.SetActive(false);
            //        }
            //    }
            //}            

            yield return waitForSec05;

            Is_ShowingAnotherSkillFalse();
            Invoke("EarthQuake_index5_End", skill_coolTime[5]);
        }
        else
        {
            Is_ShowingAnotherSkillFalse();
            EarthQuake_index5_End();
        }                
    }

    void EarthQuake_index5_End()
    {
        skill_canStart[5] = true;
    }    

    IEnumerator FireNova_index4()
    {
        yield return waitForSec08;
        
        RaycastHit2D hit8 = Physics2D.CircleCast(transform.position, Skill_Range_common, Vector2.zero, 0, targetLayer);
        if (hit8)
        {
            GameObject go = Instantiate(Resources.Load("Eff/common_DoubleNovaFire"), transform.position, Quaternion.Euler(45, 0, 0)) as GameObject;
            Destroy(go, 3f);

            // 효과음 
            SoundManager.Instance.PlaySe("burn");

            spineAnimState = PlayerContoller.SpineAnimState.attack;
            CancelInvoke("SetPlayerStateStay");
            Invoke("SetPlayerStateStay", 0.4f);

            RaycastHit2D[] targets = Physics2D.CircleCastAll(transform.position, 6f, Vector2.zero, 0, targetLayer);
            foreach (RaycastHit2D target in targets)
            {
                if (target)
                {
                    MonsterController mc = target.transform.GetComponent<MonsterController>();
                    mc.OnDamaged(skill_Dam[4]);
                    mc.Mode_Burn(1.5f, skill_Dam[4]);
                }
            }

            // 총알 제거
            //RaycastHit2D hitMonsterTan = Physics2D.CircleCast(transform.position, 6f, Vector2.zero, 0, monsterTanLayer);
            //if (hitMonsterTan)
            //{
            //    RaycastHit2D[] monsterTantargets = Physics2D.CircleCastAll(transform.position, 6f, Vector2.zero, 0, monsterTanLayer);
            //    foreach (RaycastHit2D target in monsterTantargets)
            //    {
            //        if (target)
            //        {
            //            GameObject tanExplosion = Instantiate(Resources.Load("Bullet/basic_TanExplosion06"), target.transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
            //            Destroy(tanExplosion, 0.6f);
                      
            //            target.transform.gameObject.SetActive(false);
            //        }
            //    }
            //}            

            yield return waitForSec05;

            Is_ShowingAnotherSkillFalse();
            Invoke("FireNova_index4_End", skill_coolTime[4]);
        }
        else
        {
            Is_ShowingAnotherSkillFalse();
            FireNova_index4_End();
        }        
    }   

    void FireNova_index4_End()
    {
        skill_canStart[4] = true;
    }

    IEnumerator SummonWisp_index1()
    {
        yield return waitForSec08;

        SoundManager.Instance.PlaySe("frozenField");

        spineAnimState = PlayerContoller.SpineAnimState.attack;
        CancelInvoke("SetPlayerStateStay");
        Invoke("SetPlayerStateStay", 0.4f);

        playerBible.gameObject.SetActive(true);
        yield return skill_duration_index1_SummonWisp_waitForSec;
        playerBible.gameObject.SetActive(false);
        
        Is_ShowingAnotherSkillFalse();
        Invoke("SummonWisp_index1_End", skill_coolTime[1]);
    }

    void SummonWisp_index1_End()
    {
        skill_canStart[1] = true;
    }

    IEnumerator FrozenField004_index3()
    {
        yield return waitForSec08;

        frozenField.Play();        
        
        spineAnimState = PlayerContoller.SpineAnimState.attack;
        CancelInvoke("SetPlayerStateStay");
        Invoke("SetPlayerStateStay", 0.4f);

        float time = 0;
        while (time <= skill_duration_004_index3)
        {
            RaycastHit2D[] targets = Physics2D.CircleCastAll(transform.position, 3.5f, Vector2.zero, 0, targetLayer);
                                    
            foreach (RaycastHit2D target in targets)
            {
                if (target)
                {
                    SoundManager.Instance.PlaySe("frostimpact");
                    MonsterController mc = target.transform.GetComponent<MonsterController>();
                    mc.OnDamaged(skill_Dam[3]);
                    mc.Mode_Freeze(3f);
                }
            }

            // 총알 얼리기
            RaycastHit2D hitMonsterTan = Physics2D.CircleCast(transform.position, 3.5f, Vector2.zero, 0, monsterTanLayer);
            if (hitMonsterTan)
            {
                RaycastHit2D[] monsterTantargets = Physics2D.CircleCastAll(transform.position, 3.5f, Vector2.zero, 0, monsterTanLayer);
                foreach (RaycastHit2D target in monsterTantargets)
                {
                    if (target)
                    {
                        MonsterTan mt = target.transform.GetComponent<MonsterTan>();
                        mt.Mode_Freeze(3f);
                    }
                }
            }            

            yield return waitForSec05;
            time += 0.5f;            
        }

        if (time >= skill_duration_004_index3)
        {
            frozenField.Stop();
            Is_ShowingAnotherSkillFalse();
            Invoke("FrozenField004_index3_End", skill_coolTime[3]);            
        }        
    }

    void FrozenField004_index3_End()
    {                
        skill_canStart[3] = true;
    }
    
    IEnumerator LightningBolt003_index2()
    {
        yield return waitForSec08;
        
        RaycastHit2D hit8 = Physics2D.CircleCast(transform.position, Skill_Range_common, Vector2.zero, 0, targetLayer);
        if (hit8)
        {            
            GameObject swordEff = Instantiate(Resources.Load("Skill/skill003_lightning sword"), transform.position, transform.rotation) as GameObject;
            swordEff.transform.parent = GameData.Instance.gameManager.transform;
            Destroy(swordEff, 0.6f);

            spineAnimState = PlayerContoller.SpineAnimState.attack;
            CancelInvoke("SetPlayerStateStay");
            Invoke("SetPlayerStateStay", 0.4f);

            yield return new WaitForSeconds(0.25f);

            SoundManager.Instance.PlaySe("lightning");

            RaycastHit2D[] targets = Physics2D.CircleCastAll(transform.position, 6f, Vector2.zero, 0, targetLayer);
            foreach (RaycastHit2D target in targets)
            {
                GameObject lightningEff = Instantiate(Resources.Load("Skill/skill003_lightning strike blue"), target.transform.position, target.transform.rotation) as GameObject;
                lightningEff.transform.parent = GameData.Instance.gameManager.transform;
                Destroy(lightningEff, 1.7f);
                if (target)
                {
                    MonsterController mc = target.transform.GetComponent<MonsterController>();
                    mc.OnDamaged(skill_Dam[2]);
                    mc.Mode_Shock(1.5f);
                }
                // 감전 효과 추가
                yield return waitforsec0002;
            }

            Is_ShowingAnotherSkillFalse();
            Invoke("LightningBolt003_index2_End", skill_coolTime[2]);
        }
        else
        {
            Is_ShowingAnotherSkillFalse();
            LightningBolt003_index2_End();
        }                
    }

    void LightningBolt003_index2_End()
    {
        skill_canStart[2] = true;
    }

    void Is_ShowingAnotherSkillFalse()
    {
        is_ShowingAnotherSkill = false;
    }

    RaycastHit2D[] targets;
    Transform nearestTargetTr;
    float scanRange = 5.85f;    

    void NormalTanAttack()
    {
        skill_canStart[0] = false;
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTargetTr = GetNearest();
        Fire();

        CancelInvoke("CoolTime_NormalTan");
        Invoke("CoolTime_NormalTan", skill_coolTime[0]);        
    }

    void CoolTime_NormalTan()
    {
        skill_canStart[0] = true;
    }
   
    Transform GetNearest()
    {
        Transform result = null;
        float dist = 10f;

        foreach (RaycastHit2D target in targets)
        {
            Vector2 playerPos = transform.position;
            Vector2 targetPos = target.transform.position;
            float curDist = Vector2.Distance(playerPos, targetPos);
            if (curDist < dist)
            {
                dist = curDist;
                result = target.transform;
            }
        }

        return result;
    }

    float[] bulletAngle = new float[5];

    void Fire()
    {
        if (isDie) return;
        if (Is_Stun || Is_Knockback || Is_Shock) return;
        if (!nearestTargetTr) return;

        // 효과음 
        //SoundManager.Instance.PlaySe("PE003");

        spineAnimState = PlayerContoller.SpineAnimState.attack;
        CancelInvoke("SetPlayerStateStay");
        Invoke("SetPlayerStateStay", 0.4f);
        
        Vector3 dir = nearestTargetTr.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        bulletAngle[0] = angle;
        bulletAngle[1] = angle + 13f;
        bulletAngle[2] = angle - 13f;
        bulletAngle[3] = angle + 26f;
        bulletAngle[4] = angle - 26f;        

        for (int i = 0; i < bulletCount; i++)
        {
            GameObject tan = GameData.Instance.poolManager.Get(3);
            tan.transform.position = transform.position;
            tan.transform.localRotation = Quaternion.Euler(0f, 0f, bulletAngle[i]);
            tan.GetComponent<PlayerNormalTan>().Init(normalTan_speed, skill_Dam[0], penet, false, nearestTargetTr);           
        }
    }    
    
    void SetPlayerStateStay()
    {
        if (!GameData.Instance.playerContoller.isDie)
            GameData.Instance.playerContoller.spineAnimState = PlayerContoller.SpineAnimState.stay;
    }            

    void SpinAnimSetting()
    {        
        switch (spineAnimState)
        {
            case SpineAnimState.stay:
                if(currentAnimation != "idle" && !isDie)
                {
                    skeletonAnim.state.SetAnimation(0, "idle", true);
                    currentAnimation = "idle";
                }                
                break;
            case SpineAnimState.move:
                if(currentAnimation != "run" && !isDie && !Is_Knockback && !Is_Shock && !Is_Stun)
                {
                    skeletonAnim.state.SetAnimation(0, "run", true);
                    currentAnimation = "run";
                }                
                break;
            case SpineAnimState.attack:
                if(currentAnimation != "attack" && !isDie)
                {
                    skeletonAnim.state.SetAnimation(0, "attack", false);
                    currentAnimation = "attack";
                }                
                break;
            case SpineAnimState.death:
                if(currentAnimation != "dead")
                {
                    skeletonAnim.state.SetAnimation(0, "dead", false);
                    currentAnimation = "dead";
                }                
                break;           
        }
    }
          
    void Move()
    {
        spineAnimState = SpineAnimState.move;
        Vector2 moveVec = moveDir.normalized * moveSpeed * Time.fixedDeltaTime;

        rigid.MovePosition(rigid.position + moveVec);

        // Flip
        if (moveDir.x != 0)
        {
            if (moveDir.x > 0) skeletonAnim.transform.localScale = new Vector3(-0.08f, 0.08f, 0.08f);
            else skeletonAnim.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        }
    }

    void Init()
    {
        canBeDamage = true;
        originMoveSpeed = moveSpeed;
        isDie = false;
        spineAnimState = SpineAnimState.stay;
        rigid = GetComponent<Rigidbody2D>();
        FullHp();
        required_EXP_forLvUp = Hero_LvExp(playerLevel);
        ExpBarUpdate();
        GameData.Instance.uiManager.playerLevelText.text = $"Lv. {playerLevel}";

        // 스킬 초기화
        skill_hadLearn[0] = true;
        for (int i = 1; i < skill_hadLearn.Length; i++)
        {
            skill_hadLearn[i] = false;
        }

        skill_Lv[0] = 1;
        for (int i = 1; i < skill_Lv.Length; i++)
        {
            skill_Lv[i] = 0;
        }

        /*
            001. 윈드 애로우 [0]
            002. 서먼 위스프 [1]
            003. 라이트닝 볼트 [2]
            004. 프로즌 필드 [3]
            005. 파이어 노바 [4]
            006. 어스퀘이크 [5]
            201. 사이클론 [6]
            205. 메테오 [7]
            206. 쇼크웨이브 [8]
        */

        skill_coolTime[0] = 0.8f;
        skill_coolTime[1] = 12.7f; 
        skill_coolTime[2] = 11.5f;
        skill_coolTime[3] = 10.5f;
        skill_coolTime[4] = 12.3f; 
        skill_coolTime[5] = 12.5f; 
        skill_coolTime[6] = 10.3f; 
        skill_coolTime[7] = 11.3f; 
        skill_coolTime[8] = 11.1f;

        skill_duration_index1_SummonWisp_waitForSec = new WaitForSeconds(skill_duration_002_index1);

        skill_Dam[0] = 29.1f;
        skill_Dam[1] = 35f;  
        skill_Dam[2] = 15f; 
        skill_Dam[3] = 15f; 
        skill_Dam[4] = 20f;
        skill_Dam[5] = 15f;
        skill_Dam[6] = 10f;
        skill_Dam[7] = 20f;
        skill_Dam[8] = 15f;
        skill_Dam[9] = 25f; // 패시브 스킬 : 냉기 반격

        skill_canStart[0] = true;
        for (int i = 1; i < skill_canStart.Length; i++)
        {
            skill_canStart[i] = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDie || !canBeDamage) return;
        
        if (collision.CompareTag("Monster"))
        {            
            MonsterController mc = collision.gameObject.GetComponent<MonsterController>();

            if (skill_hadLearn[14])
            {
                mc.OnDamaged(skill_Lv[14] * skill_Dam[9]);
                mc.Mode_Freeze();
                coldCounterAttack_Particle.Play();
                //GameObject go = Instantiate(Resources.Load("Eff/FrostExplosionNormal"), transform) as GameObject;
                //Destroy(go, 1.1f);
            }

            if (skill_hadLearn[12])
                if (Random.Range(0, 100) < skill_Lv[12] * 12) return;

            if (!mc.Is_Freeze) OnDamaged(mc.attack_Power);                        
        }

        if (collision.CompareTag("Monster_Tan"))
        {
            GameObject tanExplosion = Instantiate(Resources.Load("Bullet/basic_TanExplosion06"), transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
            Destroy(tanExplosion, 0.6f);
            
            collision.gameObject.SetActive(false);

            if (skill_hadLearn[12])
                if (Random.Range(0, 100) < skill_Lv[12] * 12) return;

            OnDamaged(collision.gameObject.GetComponent<MonsterTan>().dam);
        }     
    }

    public void OnDamaged(float dam)
    {                
        if (isDie || !canBeDamage) return;

        canBeDamage = false;
        skeletonAnim.skeleton.SetColor(new Color(0.8f, 0, 0, 1f));

        Invoke("Recovery", 0.2f);
        
        Invoke("CanBeDamageTrue", 0.4f);

        if (Is_Shock) dam *= 1.3f;
        if (skill_hadLearn[11]) dam -= (dam * (skill_Lv[11] * 0.1f));

        current_HP -= dam;
        HpBarUpdate();

        if (current_HP <= 0 && !isDie) 
        {            
            isDie = true;
            playerBible.gameObject.SetActive(false);
            spineAnimState = SpineAnimState.death;
            Invoke("Die", 3f);
        }            
    }

    public void BurnDamaged()
    {
        if (isDie) return;        
        skeletonAnim.skeleton.SetColor(new Color(0.8f, 0, 0, 1f));
        Invoke("Recovery", 0.15f);
        current_HP -= burnPower;
        HpBarUpdate();
        if (current_HP <= 0 && !isDie)
        {
            isDie = true;
            spineAnimState = SpineAnimState.death;
            Invoke("Die", 3f);
        }
    }

    void CanBeDamageTrue()
    {
        canBeDamage = true;
    }

    void Recovery()
    {
        //rigid.velocity = Vector2.zero;        
        skeletonAnim.skeleton.SetColor(new Color(1f, 1f, 1f, 1f));
    }

    public void HpBarUpdate()
    {
        hpBar_Slider.value = current_HP / max_HP;
    }

    void FullHp()
    {
        current_HP = max_HP;        
        HpBarUpdate();
    }

    #region 아이템
    public void EatMagnetic()
    {
        // 효과음 
        SoundManager.Instance.PlaySe("PE013");

        Item[] items = GameData.Instance.poolManager.GetComponentsInChildren<Item>();
        foreach (Item item in items)
        {
            if (item.gameObject.activeSelf)
            {
                if(item.itemType == Item.ItemType.ExpGemSmall || item.itemType == Item.ItemType.ExpGemMedium || item.itemType == Item.ItemType.ExpGemLarge)
                {
                    item.MoveExpGem = true;                                
                }
            }
        }        
    }
    
    public void EatBoom()
    {
        // 효과음 
        SoundManager.Instance.PlaySe("PE013");
               
        MonsterController[] mcs = GameData.Instance.poolManager.GetComponentsInChildren<MonsterController>();

        if(mcs.Length > 0) SoundManager.Instance.PlaySe("earthQuake");

        foreach (MonsterController mc in mcs)
        {
            if(mc.monsterType == MonsterController.MonsterType.Normal && mc.gameObject.activeInHierarchy) mc.Boom();
        }
    }

    public void EatPotion()
    {
        // 효과음 
        SoundManager.Instance.PlaySe("PE014");
        
        current_HP += max_HP * 0.25f;
        if (current_HP > max_HP) current_HP = max_HP;
        HpBarUpdate();

        SetHpUpColor();

        //text_HpRecovery.gameObject.SetActive(true);
        text_HpRecovery.alpha = 1f;

        CancelInvoke("HpRecoveryText_SetActiveFalse");
        Invoke("HpRecoveryText_SetActiveFalse", 1.5f);
    }   
    
    public void SetHpUpColor()
    {
        skeletonAnim.skeleton.SetColor(new Color(0, 1f, 0, 1f));
        Invoke("Recovery", 0.12f);       
    }

    void HpRecoveryText_SetActiveFalse()
    {
        text_HpRecovery.DOFade(0, 1.5f);
        //text_HpRecovery.gameObject.SetActive(false);
    }

    public void EatTreasure()
    {
        // 효과음 
        SoundManager.Instance.PlaySe("IE004");
        GameData.Instance.uiManager.Show_Skill_Select(true);
    }
    #endregion

    void ExpBarUpdate()
    {
        float expRatio = retained_EXP / required_EXP_forLvUp;
        GameData.Instance.uiManager.expBar_Slider.value = expRatio;        
        expText.text = $"Exp: {Mathf.Floor(expRatio * 100)}%";
    }

    public void ExpUp(float exp)
    {
        if(isDie) return;

        // 효과음 
        SoundManager.Instance.PlaySe("PE012");

        retained_EXP += exp;
        ExpBarUpdate();
        if (retained_EXP >= required_EXP_forLvUp)
        {
            LevelUp();            
        }
    }

    public void LevelUp()
    {
        // 효과음 
        SoundManager.Instance.PlaySe("IE006");

        retained_EXP -= required_EXP_forLvUp;
        ExpBarUpdate();

        playerLevel++;
        GameData.Instance.uiManager.playerLevelText.text = $"Lv. {playerLevel}";
        required_EXP_forLvUp = Hero_LvExp(playerLevel);
        ExpBarUpdate();
                               
        max_HP += 5f;
        current_HP += 5f;
        //current_HP = max_HP;
        HpBarUpdate();

        GameData.Instance.uiManager.Show_Skill_Select(true);        
    }

    public void SkillSelect(int skillNum)
    {
        switch (skillNum)
        {
            case 0:
                Skill_Up_Index0_WindArrow_001();
                break;
            case 1:
                Skill_Up_Index1_SummonWisp_002();
                break;
            case 2:
                Skill_Up_Index2_LightningBolt_003();
                break;
            case 3:
                Skill_Up_Index3_FrozenField_004();
                break;
            case 4:
                Skill_Up_Index4_FireNova_005();
                break;
            case 5:
                Skill_Up_Index5_EarthQuake_006();
                break;
            case 6:
                Skill_Up_Index6_Cyclone_201();
                break;
            case 7:
                Skill_Up_Index7_Meteor_205();
                break;
            case 8:
                Skill_Up_Index8_ShockWave_206();
                break;
            case 9:
                Skill_Up_Index9_PassiveSkill_MaxHpUp_102();
                break;
            case 10:
                Skill_Up_Index10_PssiveSkill_IncreaseAttackPower_103();
                break;
            case 11:
                Skill_Up_Index11_PassiveSkill_DefenseUp_104();
                break;
            case 12:
                Skill_Up_Index12_PassiveSkill_MoveSpeedDexUp_105();
                break;
            case 13:
                Skill_Up_Index13_PassiveSkill_DecreaseSkillCoolTime_M102();
                break;
            case 14:
                Skill_Up_Index14_PassiveSkill_ColdCounterAttack_M053();
                break;
        }
      
        if (retained_EXP >= required_EXP_forLvUp)
        {
            LevelUp();
        }
        else if (retained_EXP < required_EXP_forLvUp)
        {
            moveDir = Vector2.zero;
            joy.transform.localScale = Vector3.one;            
            moveDir = Vector2.zero;
            Time.timeScale = 1f;
        }        
    }   
   
    public void Die()
    {                               
        GameData.Instance.uiManager.Show_GameOver(true);
    }

    public float Hero_LvExp(int heroLv)
    {
        float heroLv_EXP = 0;

        // 영웅 레벨업_데이터 정리							
        switch (heroLv)
        {
            case 1: heroLv_EXP = 7f; break; //	다음 레벨까지 경험치량
            case 2: heroLv_EXP = 12f; break;    //	다음 레벨까지 경험치량
            case 3: heroLv_EXP = 18f; break;    //	다음 레벨까지 경험치량
            case 4: heroLv_EXP = 25f; break;    //	다음 레벨까지 경험치량
            case 5: heroLv_EXP = 33f; break;    //	다음 레벨까지 경험치량
            case 6: heroLv_EXP = 42f; break;    //	다음 레벨까지 경험치량
            case 7: heroLv_EXP = 52f; break;    //	다음 레벨까지 경험치량
            case 8: heroLv_EXP = 63f; break;    //	다음 레벨까지 경험치량
            case 9: heroLv_EXP = 75f; break;    //	다음 레벨까지 경험치량
            case 10: heroLv_EXP = 88f; break;   //	다음 레벨까지 경험치량
            case 11: heroLv_EXP = 102f; break;  //	다음 레벨까지 경험치량
            case 12: heroLv_EXP = 117f; break;  //	다음 레벨까지 경험치량
            case 13: heroLv_EXP = 133f; break;  //	다음 레벨까지 경험치량
            case 14: heroLv_EXP = 150f; break;  //	다음 레벨까지 경험치량
            case 15: heroLv_EXP = 168f; break;  //	다음 레벨까지 경험치량
            case 16: heroLv_EXP = 187f; break;  //	다음 레벨까지 경험치량
            case 17: heroLv_EXP = 207f; break;  //	다음 레벨까지 경험치량
            case 18: heroLv_EXP = 228f; break;  //	다음 레벨까지 경험치량
            case 19: heroLv_EXP = 250f; break;  //	다음 레벨까지 경험치량
            case 20: heroLv_EXP = 273f; break;  //	다음 레벨까지 경험치량
            case 21: heroLv_EXP = 297f; break;  //	다음 레벨까지 경험치량
            case 22: heroLv_EXP = 322f; break;  //	다음 레벨까지 경험치량
            case 23: heroLv_EXP = 348f; break;  //	다음 레벨까지 경험치량
            case 24: heroLv_EXP = 375f; break;  //	다음 레벨까지 경험치량
            case 25: heroLv_EXP = 403f; break;  //	다음 레벨까지 경험치량
            case 26: heroLv_EXP = 432f; break;  //	다음 레벨까지 경험치량
            case 27: heroLv_EXP = 462f; break;  //	다음 레벨까지 경험치량
            case 28: heroLv_EXP = 493f; break;  //	다음 레벨까지 경험치량
            case 29: heroLv_EXP = 525f; break;  //	다음 레벨까지 경험치량
            case 30: heroLv_EXP = 558f; break;  //	다음 레벨까지 경험치량
            case 31: heroLv_EXP = 592f; break;  //	다음 레벨까지 경험치량
            case 32: heroLv_EXP = 627f; break;  //	다음 레벨까지 경험치량
            case 33: heroLv_EXP = 663f; break;  //	다음 레벨까지 경험치량
            case 34: heroLv_EXP = 700f; break;  //	다음 레벨까지 경험치량
            case 35: heroLv_EXP = 738f; break;  //	다음 레벨까지 경험치량
            case 36: heroLv_EXP = 777f; break;  //	다음 레벨까지 경험치량
            case 37: heroLv_EXP = 817f; break;  //	다음 레벨까지 경험치량
            case 38: heroLv_EXP = 858f; break;  //	다음 레벨까지 경험치량
            case 39: heroLv_EXP = 900f; break;  //	다음 레벨까지 경험치량
            case 40: heroLv_EXP = 943f; break;  //	다음 레벨까지 경험치량
            case 41: heroLv_EXP = 987f; break;  //	다음 레벨까지 경험치량
            case 42: heroLv_EXP = 1032f; break; //	다음 레벨까지 경험치량
            case 43: heroLv_EXP = 1078f; break; //	다음 레벨까지 경험치량
            case 44: heroLv_EXP = 1125f; break; //	다음 레벨까지 경험치량
            case 45: heroLv_EXP = 1173f; break; //	다음 레벨까지 경험치량
            case 46: heroLv_EXP = 1222f; break; //	다음 레벨까지 경험치량
            case 47: heroLv_EXP = 1272f; break; //	다음 레벨까지 경험치량
            case 48: heroLv_EXP = 1323f; break; //	다음 레벨까지 경험치량
            case 49: heroLv_EXP = 1375f; break; //	다음 레벨까지 경험치량
        }

        return heroLv_EXP;
    }

    void FreezeAura(float time)
    {
        //if (Vector3.Distance(GameData.Instance.playerContoller.transform.position, transform.position) <= 11.8f)
        {
            GameData.Instance.playerContoller.Mode_Freeze(time);
        }
    }

    void BurnAura(float time)
    {
        //if (Vector3.Distance(GameData.Instance.playerContoller.transform.position, transform.position) <= 11.8f)
        {
            GameData.Instance.playerContoller.Mode_Burn(time, 100f);
        }
    }

    void StunAura(float time)
    {
        //if (Vector3.Distance(GameData.Instance.playerContoller.transform.position, transform.position) <= 11.8f)
        {
            GameData.Instance.playerContoller.Mode_Stun(time);
        }
    }

    void ShockAura(float time)
    {
        //if (Vector3.Distance(GameData.Instance.playerContoller.transform.position, transform.position) <= 11.8f)
        {
            GameData.Instance.playerContoller.Mode_Shock(time);
        }
    }           

    // 스킬 선택창에서 클릭하면 호출되는 함수들

    void Skill_Up_Index0_WindArrow_001()
    {
        if (!skill_hadLearn[0]) skill_hadLearn[0] = true;
        skill_Lv[0]++;
        skill_Dam[0] += 3f;
                
        penet++;
        normalTan_speed += 0.5f;

        if (bulletCount < 5) bulletCount++;

        DecreaseWindArrowCoolTime();
    }

    void DecreaseWindArrowCoolTime()
    {
        if (skill_coolTime[0] > 0.5f)
        {
            skill_coolTime[0] -= 0.1f;
            if (skill_coolTime[0] < 0.5f) skill_coolTime[0] = 0.5f;
        }
    }

    void Skill_Up_Index1_SummonWisp_002()
    {
        if (!skill_hadLearn[1]) skill_hadLearn[1] = true;
        skill_Lv[1]++;        
        skill_Dam[1] += 8f;

        skill_coolTime[1] -= 0.5f;

        skill_duration_002_index1 += 0.5f;
        skill_duration_index1_SummonWisp_waitForSec = new WaitForSeconds(skill_duration_002_index1);        
    }

    void Skill_Up_Index2_LightningBolt_003()
    {
        if (!skill_hadLearn[2]) skill_hadLearn[2] = true;
        skill_Lv[2]++;
        skill_Dam[2] += 7f;

        skill_coolTime[2] -= 1f;
    }

    void Skill_Up_Index3_FrozenField_004()
    {
        if (!skill_hadLearn[3]) skill_hadLearn[3] = true;
        skill_Lv[3]++;
        skill_Dam[3] += 6f;

        skill_coolTime[3] -= 0.5f;
        skill_duration_004_index3 += 0.5f;
    }

    void Skill_Up_Index4_FireNova_005()
    {
        if (!skill_hadLearn[4]) skill_hadLearn[4] = true;
        skill_Lv[4]++;
        skill_Dam[4] += 9f;
        skill_coolTime[4] -= 1f;
    }

    void Skill_Up_Index5_EarthQuake_006()
    {
        if (!skill_hadLearn[5]) skill_hadLearn[5] = true;
        skill_Lv[5]++;
        skill_Dam[5] += 5f;
        skill_coolTime[5] -= 1f;
    }

    void Skill_Up_Index6_Cyclone_201()
    {
        if (!skill_hadLearn[6]) skill_hadLearn[6] = true;
        skill_Lv[6]++;
        skill_Dam[6] += 7f;
        skill_coolTime[6] -= 1f;
    }

    void Skill_Up_Index7_Meteor_205()
    {
        if (!skill_hadLearn[7]) skill_hadLearn[7] = true;
        skill_Lv[7]++;
        skill_Dam[7] += 8f;
        skill_coolTime[7] -= 1f;
    }

    void Skill_Up_Index8_ShockWave_206()
    {
        if (!skill_hadLearn[8]) skill_hadLearn[8] = true;
        skill_Lv[8]++;
        skill_Dam[8] += 6f;
        skill_coolTime[8] -= 1f;
    }

    void Skill_Up_Index9_PassiveSkill_MaxHpUp_102()
    {
        if (!skill_hadLearn[9]) skill_hadLearn[9] = true;
        skill_Lv[9]++;

        max_HP += (max_HP * 0.2f);
        FullHp();
    }

    void Skill_Up_Index10_PssiveSkill_IncreaseAttackPower_103()
    {
        if (!skill_hadLearn[10]) skill_hadLearn[10] = true;
        skill_Lv[10]++;

        for (int i = 0; i < skill_Dam.Length; i++)
        {
            skill_Dam[i] += (skill_Dam[i] * 0.2f);
        }
    }

    void Skill_Up_Index11_PassiveSkill_DefenseUp_104()
    {
        if (!skill_hadLearn[11]) skill_hadLearn[11] = true;
        skill_Lv[11]++;
    }

    void Skill_Up_Index12_PassiveSkill_MoveSpeedDexUp_105()
    {
        if (!skill_hadLearn[12]) skill_hadLearn[12] = true;
        skill_Lv[12]++;
        moveSpeed += 0.2f;
    }

    void Skill_Up_Index13_PassiveSkill_DecreaseSkillCoolTime_M102()
    {
        if (!skill_hadLearn[13]) skill_hadLearn[13] = true;
        skill_Lv[13]++;

        DecreaseWindArrowCoolTime();

        for (int i = 1; i < skill_coolTime.Length; i++)
        {
            skill_coolTime[i] -= (skill_coolTime[i] * 0.05f);
        }
    }

    void Skill_Up_Index14_PassiveSkill_ColdCounterAttack_M053()
    {
        if (!skill_hadLearn[14]) skill_hadLearn[14] = true;
        skill_Lv[14]++;
    }
}