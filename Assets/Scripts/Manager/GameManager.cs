using Define.DesignPatterns;
using Define.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : SingleTon<GameManager>
{
    List<Charactor> myCharactors = new List<Charactor>();
    //추후 세이브 기능을 위해 게임내 자원, 캐릭터 정보 세이브파일 저장에 유의

    /// <summary>
    /// 임시함수
    /// </summary>
    public void Setcharactors()
    {
        myCharactors.Add(new Charactor("BlackSlime", "BlackSlime", 10, 10, 10, 10, 10));
    }
}
