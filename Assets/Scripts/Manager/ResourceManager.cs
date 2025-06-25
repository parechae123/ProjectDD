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
                Debug.LogError("ManagerException : ResourceManager�� sprite Instance�� ����ֽ��ϴ�. ȣ�� ����,Adressable key�� Ȥ�� �ڵ���� Ű���� Ȯ�����ּ���");
                LoadAsync<SpriteAtlas>("spritePackage", (obj) => { sprites = obj; }, true);
            }
            return sprites;
        }
    }
    private bool isLoadAble<T>(T instance) { return instance != null; }
    //�޴��� �ν��Ͻ� ������ ����Ǵ� �Լ�
    public override void Init()
    {
        base.Init();
        if(!isLoadAble<SpriteAtlas>(sprites)) LoadAsync<SpriteAtlas>("spritePackage", (obj) => { sprites = obj; }, true);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">����Ÿ��</typeparam>
    /// <param name="key">Ÿ�� Ű��</param>
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
    /// <typeparam name="T">����Ÿ��</typeparam>
    /// <param name="label">Ÿ�� Ű��</param>
    /// <param name="callback">(obj)=>{targetInstance = obj}</param>
    public void LoadAsyncAll<T>(string label, Action<(string, T)[]> callback, bool isCaching = false)
    {
        var labelKeys = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        //label�� TŸ���� ������Ʈ���� Ű���� �����´�
        labelKeys.WaitForCompletion();
        //resource�� ���� load�Ҷ����� ���

        Debug.Log(labelKeys.Result);
        if (labelKeys.Result.Count == 0) { Debug.LogError($"{label}���� ����ֽ��ϴ�."); callback.Invoke(null); }//�ش��ϴ� Ű�� ������� null�� ����

        int doneCount = 0;

        (string, T)[] tempT = new (string, T)[labelKeys.Result.Count];
        for (int i = 0; i < tempT.Length; i++)
        {
            int curIndex = i;
            string curKey = labelKeys.Result[i].PrimaryKey; //�ݹ��� ���ÿ� ����� Ŭ���� �̽��� ���� �� �ֱ⿡ �и�(�ٸ� ������ ���ø޸𸮸� �����ϴ� ����)
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
