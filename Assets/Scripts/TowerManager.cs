using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    public float cannonBulletSpeed = 10f;
    public int cannonBulletMaxLifetime = 3000;

    // Списки башен и связанных объектов
    private List<TowerData> towersData;
    private List<SpawnedTower> towerObjs;
    private List<int> towersWithCannon;
    // private int allTowersCount;
    // private int loadedTowersCount;

    // Механика стрельбы из пушки
    private List<SpawnedCannonBullet> bulletObjs;
    private Stack<int> notActiveBulletIds;

    // Пользовательский ввод
    private GameControls controls;
    private List<FireCommand> commandBuffer;

    void Awake()
    {
        towersData = new List<TowerData>();
        towersWithCannon = new List<int>();

        controls = new GameControls();
        commandBuffer = new List<FireCommand>();

        // Изначально задумана одна башня в центре,
        // но создаю 4 для тестирования и демонстрации
        GenerateTowerData(0, 0);
        GenerateTowerData(5, 0);
        GenerateTowerData(0, 5);
        GenerateTowerData(5, 5);

        // allTowersCount = towersData.Count;
        // loadedTowersCount = 0;

        towerObjs = new List<SpawnedTower>();
        for (int i = 0; i < towersData.Count; i++)
            towerObjs.Add(default);

        bulletObjs = new List<SpawnedCannonBullet>();
        notActiveBulletIds = new Stack<int>();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
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

                    // loadedTowersCount++;
                }
            };
        }  
    } 

    void Update()
    {
        UpdateInput();

        ProcessCommands();

        OperateBullets();
    }

    void UpdateInput()
    {
        if (controls.Gameplay.Fire.WasPerformedThisFrame())
        {
            // Получаем позицию курсора
            Vector2 mouseScreenPos = controls.Gameplay.PointerPosition.ReadValue<Vector2>();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector2 targetPos = new Vector2(worldPos.x, worldPos.y);

            // Создаём команду на выстрел в точку курсора 
            // для всех башен, у которых есть пушка
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
        if (tower.Instance == null)
            return; 
        
        // Если нет пульки в пуле, создадим новую на позиции башни и выстрелим
        if (notActiveBulletIds.Count == 0)
        {  
            Addressables.InstantiateAsync("BulletPrefab").Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    SpawnedCannonBullet bullet = new SpawnedCannonBullet
                    {
                        Instance = handle.Result,
                        Handle = handle,
                        lifeTime = 1
                    };

                    bullet.Instance.transform.position = tower.Instance.transform.position;
                    Vector2 dir = (cmd.TargetPosition - (Vector2)bullet.Instance.transform.position).normalized;
                    bullet.Instance.GetComponent<Rigidbody2D>().linearVelocity = dir * cannonBulletSpeed;

                    bulletObjs.Add(bullet);
                }
            };
        }
        // Есть неактивная пулька в пуле, используем её
        else
        {
            int ind = notActiveBulletIds.Pop();

            SpawnedCannonBullet bullet = bulletObjs[ind];

            bullet.Instance.transform.position = tower.Instance.transform.position;
            bullet.lifeTime = 1;
            bullet.Instance.SetActive(true);
            
            Vector2 dir = (cmd.TargetPosition - (Vector2)bullet.Instance.transform.position).normalized;
            bullet.Instance.GetComponent<Rigidbody2D>().linearVelocity = dir * cannonBulletSpeed;

            bulletObjs[ind] = bullet;
        }
    }

    public void OnDestroy()
    {
        if (towerObjs == null)
            return;

        for (int i = 0; i < towerObjs.Count; i++)
            if (towerObjs[i].Handle.IsValid())
                Addressables.ReleaseInstance(towerObjs[i].Handle);

        towerObjs.Clear();

        if (bulletObjs == null)
            return;

        for (int i = 0; i < bulletObjs.Count; i++)
            if (bulletObjs[i].Handle.IsValid())
                Addressables.ReleaseInstance(bulletObjs[i].Handle);

        bulletObjs.Clear();
    }

    int GenerateTowerData(float x, float y)
    {
        TowerData td = new TowerData
        {
            x = x,
            y = y
        };

        towersData.Add(td);
        int ind = towersData.Count - 1;
        towersWithCannon.Add(ind);

        return ind;
    }

    // Здесь у пулек считается время жизни 
    // и отработавшие уходят в пул
    void OperateBullets()
    {
        for (int i = 0; i < bulletObjs.Count; i++)
        {
            SpawnedCannonBullet bullet = bulletObjs[i];

            if (bullet.lifeTime > 0)
            {
                bullet.lifeTime += 1;
            }

            if (bullet.lifeTime > cannonBulletMaxLifetime)
            {
                bullet.Instance.SetActive(false);
                bullet.lifeTime = 0;
                notActiveBulletIds.Push(i);
            }
            
            bulletObjs[i] = bullet;
        }
    }
}