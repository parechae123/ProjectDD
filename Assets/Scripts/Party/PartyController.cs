using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyController : MonoBehaviour
{
    // Start is called before the first frame update
    KeyCode forwardKey;
    KeyCode backwardKey;
    public float moveSpeed;
    
    void Awake()
    {
        DistSort();
        forwardKey = forwardKey == null || forwardKey == KeyCode.None ? KeyCode.D : forwardKey;
        backwardKey = backwardKey == null || backwardKey == KeyCode.None ? KeyCode.A : backwardKey;
    }

    public void SetSprites()
    {

    }

    /// <summary>
    /// 캐릭터 거리 정렬
    /// </summary>
    public void DistSort()
    {
        int n = transform.childCount;
        float dist = 1.5f;
        float startPos = (n - 1) * dist / 2;

        for (int i = 0; i < n; i++)
        {
            transform.GetChild(i).localPosition = new Vector3(startPos - (i * dist), 0, 0);
        }

    }
    public void Update()
    {
        if (Input.GetKey(forwardKey)) transform.position += (Vector3.right * moveSpeed) * Time.deltaTime;
        if (Input.GetKey(backwardKey)) transform.position += (Vector3.left * moveSpeed) * Time.deltaTime;
    }
}
