using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerBible : MonoBehaviour
{
    public Transform playerTr;
    public GameObject biblePrefab;    
    float rotateSpeed = 290f;        
    int bibleCount = 5;

    // Start is called before the first frame update
    void Start()
    {
        Batch();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.gameObject.activeSelf)
        {
            transform.position = playerTr.position;
            transform.Rotate(Vector3.back, rotateSpeed * Time.deltaTime);
        }        
    }

    void Batch()
    {
        for (int i = 0; i < bibleCount; i++)
        {
            Transform bibleTr;

            if (i < transform.childCount)
            {
                bibleTr = transform.GetChild(i);
            }
            else
            {
                bibleTr = Instantiate(biblePrefab, transform).transform;
                //bibleTr = Instantiate(biblePrefab).transform;
                //bibleTr.parent = transform;
            }

            bibleTr.localPosition = Vector3.zero;
            bibleTr.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * i / bibleCount;
            bibleTr.Rotate(rotVec);
            bibleTr.Translate(bibleTr.up * 2.65f, Space.World);
        }
    }

    public void LevelUp()
    {     
        Batch();
    }


}
