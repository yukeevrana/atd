using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class CannonManager : MonoBehaviour
{
    private List<TowerData> towersWithCannons;

    void Awake()
    {
        // towersData = new List<TowerData>();

        // allTowersCount = 1;
        // loadedTowersCount = 0;
    }

    void Start()
    {
        // for (int i = 0; i < towersData.Count; i++)
        // {
        //     // Нужно зафиксировать значение i для коллбэка
        //     int ind = i;
        //     // Нужно зарезервировать место для нового объекта
        //     towerObjs.Add(null);

        //     Addressables.InstantiateAsync("TowerPrefab").Completed += (handle) =>
        //     {
        //         if (handle.Status == AsyncOperationStatus.Succeeded)
        //         {
        //             towerObjs.Insert(ind, handle.Result);

        //             towerObjs[ind].transform.position = new Vector3(towersData[ind].x, towersData[ind].y, 0);

        //             loadedTowersCount++;
        //         }
        //     };
        // }  
    } 

    public void OnDestroy()
    {
    }
}