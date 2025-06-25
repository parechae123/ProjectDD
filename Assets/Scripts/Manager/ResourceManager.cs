using Define.DesignPatterns;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class ResourceManager : SingleTon<ResourceManager>
{
    private SpriteAtlas sprites;
    public SpriteAtlas GetSprite 
    { 
        get
        {
            
            if (!isLoadAble<SpriteAtlas>(sprites))
            {
                Debug.LogError("ManagerException : ResourceManager의 sprite Instance가 비어있습니다. 호출 순서,Adressable key값 혹은 코드상의 키값을 확인해주세요");
                LoadAsync<SpriteAtlas>("spritePackage", (obj) => { sprites = obj; }, true);
            }
            return sprites;
        }
    }
    private bool isLoadAble<T>(T instance) { return instance != null; }
    //메니저 인스턴스 생성시 실행되는 함수
    public override void Init()
    {
        base.Init();
        if(!isLoadAble<SpriteAtlas>(sprites)) LoadAsync<SpriteAtlas>("spritePackage", (obj) => { sprites = obj; }, true);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">리턴타입</typeparam>
    /// <param name="key">타겟 키값</param>
    /// <param name="callback">(obj)=>{targetInstance = obj}</param>
    public void LoadAsync<T>(string key, Action<T> callback, bool isCaching = false)
    {
        if (key.Contains(".sprite"))
        {
            key = $"{key}[{key.Replace(".sprite", "")}]";
        }
        AsyncOperationHandle<T> infoAsyncOP = Addressables.LoadAssetAsync<T>(key);
        infoAsyncOP.Completed += (op) =>
        {

            callback?.Invoke(infoAsyncOP.Result);
            if(!isCaching)Addressables.Release(infoAsyncOP);
        };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">리턴타입</typeparam>
    /// <param name="label">타겟 키값</param>
    /// <param name="callback">(obj)=>{targetInstance = obj}</param>
    public void LoadAsyncAll<T>(string label, Action<(string, T)[]> callback, bool isCaching = false)
    {
        var labelKeys = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        //label의 T타입인 오브젝트들의 키값을 가져온다
        labelKeys.WaitForCompletion();
        //resource를 전부 load할때까지 대기

        Debug.Log(labelKeys.Result);
        if (labelKeys.Result.Count == 0) { Debug.LogError($"{label}라벨이 비어있습니다."); callback.Invoke(null); }//해당하는 키가 없을경우 null을 리턴

        int doneCount = 0;

        (string, T)[] tempT = new (string, T)[labelKeys.Result.Count];
        for (int i = 0; i < tempT.Length; i++)
        {
            int curIndex = i;
            string curKey = labelKeys.Result[i].PrimaryKey; //콜백을 동시에 실행시 클로저 이슈가 생길 수 있기에 분리(다른 루프의 스택메모리를 참조하는 현상)
            LoadAsync<T>(labelKeys.Result[i].PrimaryKey, (result) =>
            {
                tempT[curIndex].Item1 = curKey;
                tempT[curIndex].Item2 = result;
                doneCount++;
                if (doneCount == labelKeys.Result.Count)
                {
                    callback?.Invoke(tempT);
                    if (!isCaching) Addressables.Release(labelKeys);
                }
            }, isCaching);
        }
    }
}
