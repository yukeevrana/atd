using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    private List<TowerData> towersData;
    private List<GameObject> towerObjs;
    private int allTowersCount;
    private int loadedTowersCount;

    void Awake()
    {
        towersData = new List<TowerData>();
        towerObjs = new List<GameObject>();

        GenerateTowerData(0, 0);

        allTowersCount = 1;
        loadedTowersCount = 0;
    }

    void Start()
    {
        for (int i = 0; i < towersData.Count; i++)
        {
            // Нужно зафиксировать значение i для коллбэка
            int ind = i;
            // Нужно зарезервировать место для нового объекта
            towerObjs.Add(null);

            Addressables.InstantiateAsync("TowerPrefab").Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    towerObjs.Insert(ind, handle.Result);

                    towerObjs[ind].transform.position = new Vector3(towersData[ind].x, towersData[ind].y, 0);

                    loadedTowersCount++;
                }
            };
        }  
    } 

    public void OnDestroy()
    {
        for (int i = 0; i < towerObjs.Count; i++)
        {
            if (towerObjs[i] is not null)
                Addressables.ReleaseInstance(towerObjs[i]);
        }
    }

    int GenerateTowerData(float x, float y)
    {
        TowerData td = new TowerData
        {
            x = x,
            y = y,
            hasCannon = true
        };

        towersData.Add(td);
        return towersData.Count - 1;
    }
}