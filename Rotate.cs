using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{ 
    public bool Is_Rotate { get; set; } = false;

    void Update()
    {
        if (Is_Rotate)
            transform.Rotate(Vector3.forward, 550f * Time.deltaTime);
    }
}
