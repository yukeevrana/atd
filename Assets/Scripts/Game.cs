using UnityEngine;

public class Game : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Tower tower = new Tower();
        tower.Create();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
