using Define.DesignPatterns;
using Define.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : SingleTon<GameManager>
{
    List<Charactor> myCharactors = new List<Charactor>();
    //���� ���̺� ����� ���� ���ӳ� �ڿ�, ĳ���� ���� ���̺����� ���忡 ����

    /// <summary>
    /// �ӽ��Լ�
    /// </summary>
    public void Setcharactors()
    {
        myCharactors.Add(new Charactor("BlackSlime", "BlackSlime", 10, 10, 10, 10, 10));
    }
}
