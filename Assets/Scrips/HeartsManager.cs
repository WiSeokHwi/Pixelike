using UnityEngine;
using UnityEngine.UI;
public class HeartsManager : MonoBehaviour
{
    public Player Player;



    public int childCount;

    public GameObject fullHeart;
    public GameObject emptyHeart;

    void Start()
    {
        if (Player != null)
        {
            Player.OnHealthChanged += UpdateHearts; // �̺�Ʈ ����
        }
        UpdateHearts();
    }
    void OnDestroy()
    {
        if (Player != null)
        {
            Player.OnHealthChanged -= UpdateHearts; // ���� ���� (�޸� ���� ����)
        }
    }

    void UpdateHearts()
        {
            
        
        // ���� ��Ʈ ����
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }



            for (int i = 0; i < Player.MaxHealth; i++)
            {
                if (i < Player.Health)
                {
                    // ü���� �����ִ� �κ��� �� �� ��Ʈ
                    Instantiate(fullHeart, transform);
                }
                else
                {
                    // ü���� ���� �κ��� �� ��Ʈ
                    Instantiate(emptyHeart, transform);
                }
            }
        }
    }
