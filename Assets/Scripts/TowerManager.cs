using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    private List<TowerData> towersData;
    private List<SpawnedTower> towerObjs;
    private int allTowersCount;
    private int loadedTowersCount;

    private List<int> towersWithCannon;

    private List<FireCommand> commandBuffer;

    void Awake()
    {
        towersData = new List<TowerData>();
        towersWithCannon = new List<int>();
        
        commandBuffer = new List<FireCommand>();

        GenerateTowerData(0, 0);
        GenerateTowerData(5, 0);
        GenerateTowerData(0, 5);
        GenerateTowerData(5, 5);

        allTowersCount = towersData.Count;
        loadedTowersCount = 0;

        towerObjs = new List<SpawnedTower>();
        for (int i = 0; i < towersData.Count; i++)
            towerObjs.Add(default);
    }

    void Start()
    {
        for (int i = 0; i < towersData.Count; i++)
        {
            // Нужно зафиксировать значение i для коллбэка
            int ind = i;

            Addressables.InstantiateAsync("TowerPrefab").Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    SpawnedTower tower = new SpawnedTower
                    {
                        Instance = handle.Result,
                        Handle = handle
                    };

                    tower.Instance.transform.position = new Vector3(towersData[ind].x, towersData[ind].y, 0);

                    towerObjs[ind] = tower;

                    loadedTowersCount++;
                }
            };
        }  
    } 

    void Update()
    {
        UpdateInput();

        ProcessCommands();
    }

    void UpdateInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 targetPos = new Vector2(worldPos.x, worldPos.y);

            foreach (int towerIndex in towersWithCannon)
            {
                var cmd = new FireCommand
                {
                    TowerIndex = towerIndex,
                    TargetPosition = targetPos
                };

                commandBuffer.Add(cmd);
            }
        }
    }
    
    void ProcessCommands()
    {
        if (commandBuffer.Count == 0) return;

        foreach (var cmd in commandBuffer)
        {
            FireBullet(cmd);
        }

        commandBuffer.Clear();
    }

    void FireBullet(FireCommand cmd)
    {
        SpawnedTower tower = towerObjs[cmd.TowerIndex];
        if (tower.Instance == null) return; 
        
        Addressables.InstantiateAsync("BulletPrefab").Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject bullet = handle.Result;
                bullet.transform.position = tower.Instance.transform.position;
                
                Vector2 dir = (cmd.TargetPosition - (Vector2)bullet.transform.position).normalized;
                
                bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * 10f;
            }
        };
    }

    public void OnDestroy()
    {
        if (towerObjs == null)
            return;

        for (int i = 0; i < towerObjs.Count; i++)
            if (towerObjs[i].Handle.IsValid())
                Addressables.ReleaseInstance(towerObjs[i].Handle);

        towerObjs.Clear();
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
        int ind = towersData.Count - 1;

        towersWithCannon.Add(ind);

        return ind;
    }
}