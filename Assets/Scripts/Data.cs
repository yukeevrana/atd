using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public struct TowerData
{
    public float x;
    public float y;
    public bool hasCannon;
}

public struct SpawnedTower
{
    public GameObject Instance;
    public AsyncOperationHandle<GameObject> Handle;
}

public struct CannonBulletData
{
    public float x;
    public float y;
}