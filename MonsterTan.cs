using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class MonsterTan : MonoBehaviour
{      
    float speed = 3f;
    public float dam { get; set; } = 2f;    
    Rigidbody2D rigid;
    CircleCollider2D circleColl;
    bool bossTan = false;
    int bossNum = 0;

    private void OnEnable()
    {
        TanSetting();
    }

    public void Awake()
    {
        TanSetting();
    }

    void TanSetting()
    {
        rigid = GetComponent<Rigidbody2D>();
        circleColl = GetComponent<CircleCollider2D>();
        rigid.velocity = Vector2.zero;
        isFreeze = false;
        circleColl.enabled = true;
    }

    public void Init(float _speed, float _dam, bool _bossTan = false, int _bossNum = 0)
    {             
        speed = _speed;
        dam = _dam;

        bossTan = _bossTan;
        bossNum = _bossNum;

        rigid.velocity = speed * transform.right;
        //rigid.velocity = (GameData.Instance.playerContoller.transform.position - transform.position).normalized * speed;
    }
                         
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            if (bossTan && bossNum == 3) // 보스 대미지 설정
            {
                GameData.Instance.playerContoller.Mode_Burn(1.5f, 20f);
            }

            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }

        if (other.CompareTag("Bible"))
        {
            GameObject go = Instantiate(Resources.Load("Bullet/basic_TanExplosion06"), transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
            Destroy(go, 0.6f);           

            gameObject.SetActive(false);
        }
       
        if (other.CompareTag("Map")) gameObject.SetActive(false);
    }

    private void OnBecameInvisible() 
    {
        gameObject.SetActive(false);
    }

    bool isFreeze = false;
    public void Mode_Freeze(float endTime)
    {
        if (isFreeze) return;
        isFreeze = true;
        circleColl.enabled = false;
        GameObject freezeEff = Instantiate(Resources.Load("Eff/common_damage effect_ice"), transform.position + Vector3.down * 0.35f, Quaternion.Euler(-45f, 0, 0)) as GameObject;
        Destroy(freezeEff, endTime);
        rigid.velocity = Vector2.zero;
        Invoke("SetActiveFalse", 2.5f);
    }

    public void SetActiveFalse()
    {
        GameObject tanExplosion = Instantiate(Resources.Load("Bullet/basic_TanExplosion06"), transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
        Destroy(tanExplosion, 0.6f);
        
        isFreeze = false;
        circleColl.enabled = true;
        gameObject.SetActive(false);
    }
}
