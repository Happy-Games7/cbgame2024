using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNormalTan : MonoBehaviour
{   
    public float Speed { get; set; }
    public float Attack_Power { get; set; }    
    Rigidbody2D rigid2d;    
    int penet;
    bool cyclone = false;
    Vector3 dir = Vector3.zero;

    private void Awake()
    {
        rigid2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(cyclone) transform.Translate(dir * 3.5f * Time.deltaTime);
    }

    public void SetDir(Vector3 _dir, bool _cyclone)
    {
        dir = _dir;
        cyclone = _cyclone;
    }

    public void PenetDecrease()
    {
        penet--;
        if (penet < 0) gameObject.SetActive(false);        
    }

    public void Init(float _speed, float _attackPower, int _penet, bool _cyclone = false, Transform target = null)
    {
        if (target == null && _cyclone == false)
        {
            gameObject.SetActive(false);
            return;
        }

        cyclone = _cyclone;
        Speed = _speed;
        Attack_Power = _attackPower;
        penet = _penet;

        if(!cyclone) rigid2d.velocity = Speed * transform.right;
        //rigid2d.velocity = (target.position - transform.position).normalized * Speed;
    }
           
    private void OnBecameInvisible()
    {        
        if(cyclone) Destroy(gameObject);
        else gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (collision.transform.CompareTag("Map"))
        {
            if(cyclone) Destroy(gameObject);
            else gameObject.SetActive(false);
        }                              
    }   
}
