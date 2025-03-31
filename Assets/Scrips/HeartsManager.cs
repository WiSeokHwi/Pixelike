using UnityEngine;
using UnityEngine.UI;
public class HeartsManager : MonoBehaviour
{
    public Player Player;



    public int childCount;

    public GameObject fullHeart;
    public GameObject emptyHeart;

    void Update()
    {
        {
            UpdateHearts();
        }
    }

        void UpdateHearts()
        {
            
        
        // ���� ��Ʈ ����
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // ���ο� ��Ʈ ����
            int childCount = transform.childCount;

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
