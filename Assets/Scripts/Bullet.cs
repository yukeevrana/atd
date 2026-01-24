using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Bullet
{
    private float x = 0;
    private float y = 0;
    private bool onScene = false;
    private GameObject gObject;

    public Bullet()
    {
        Debug.Log("Создается объект Bullet");

        x = 0;
        y = 0;
        onScene = false;
    }

    public void Create()
    {
        Addressables.InstantiateAsync("BulletPrefab").Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                gObject = handle.Result;

                gObject.transform.position = new Vector3(x, y, 0);

                onScene = true;
            }
        };
     }

    public void Destroy()
    {
        Addressables.ReleaseInstance(gObject);

        onScene = false;
    }

    public void Hide()
    {
        gObject.SetActive(false);
        onScene = false;
    }

    public void CreateAndShoot(Vector2 from, Vector3 target)
    {
        x = from.x;
        y = from.y;

        Create(); // TODO async
    }
}