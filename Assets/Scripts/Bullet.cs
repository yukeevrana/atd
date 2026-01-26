using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Bullet
{

    // Снаружи будет использоваться пул объектов для экземпляров данного класса.
    // Нужно добавить асинхронный метод создания, который будет также асинхронно вызываться в пуле,
    // чтобы поддерживать нужное количество доступных объектов.
    // Подразумевается, что всегда есть хотя бы несколько уже созданных объектов 
    // (асинхронный метод создания закончил работу).
    // Это не столь необходимо в рамках текущего проекта, но выглядит более правильной реализацией
    // с использованием асинхронных Addressable's.

    // Метод запуска (выстрела) должен запускать физику снаряда,
    // которая дальше реализуется средствами Unity.

    private float from_x = 0;
    private float from_y = 0;
    private float speed = 0;
    private bool onScene = false;
    private GameObject gObject;

    public Bullet()
    {
        Debug.Log("Создается объект Bullet");

        from_x = 0;
        from_y = 0;
        speed = 15f;
        onScene = false;
    }

    // Асинхронное создание, объект появится неактивным
    public async Task Create()
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync("BulletPrefab");

        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            gObject = handle.Result;

            gObject.transform.position = new Vector3(from_x, from_y, 0);

            gObject.SetActive(false);
            onScene = true;
        }
    }

    public void Destroy()
    {
        Addressables.ReleaseInstance(gObject);

        onScene = false;
    }

    public void Hide()
    {
        gObject.SetActive(false);
    }

    public void Shoot(Vector3 from, Vector3 target)
    {
        gObject.SetActive(true);

        from_x = from.x;
        from_y = from.y;

        Vector2 direction = (target - from).normalized;

        // Поворачиваем снаряд "лицом" к цели (для 2D это поворот по оси Z)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gObject.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (gObject.TryGetComponent(out Rigidbody2D rb))
        {
            rb.linearVelocity = direction * 15f;
        }
    }
}