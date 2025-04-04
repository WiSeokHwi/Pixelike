using UnityEngine;

public class Swaponer : MonoBehaviour
{
    public GameObject Player;
    private GameObject playerInstance;
    void Awake()
    {
        playerInstance = Instantiate(Player, new Vector3(0, 0, 0), Quaternion.identity);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
