using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject Player;
    public int playerHealth;
    public float playerDamage;
    public int playerMaxHealth;
    public GameObject spawnPosition;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ���� �� �ı� �ȵǰ�
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }
        Instantiate(Player, spawnPosition.transform.position , Quaternion.identity);

        
        playerMaxHealth = 3;
        playerHealth = playerMaxHealth;
        playerDamage = 3f;
    }
}