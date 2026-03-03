using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public struct TowerData
{
    public float x;
    public float y;
}

public struct SpawnedTower
{
    public GameObject Instance;
    public AsyncOperationHandle<GameObject> Handle;
}

public struct SpawnedCannonBullet
{
    public GameObject Instance;
    public AsyncOperationHandle<GameObject> Handle;
    public Vector3 velocity;
    public int life_time;
    public float radius;
    public float damage;
}

public struct SpawnedSquare
{
    public GameObject Instance;
    public AsyncOperationHandle<GameObject> Handle;
    public Vector3 velocity;
    public float radius;
    public float hit_points;
}