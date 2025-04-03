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
            Player.OnHealthChanged += UpdateHearts; // 이벤트 구독
        }
        UpdateHearts();
    }
    void OnDestroy()
    {
        if (Player != null)
        {
            Player.OnHealthChanged -= UpdateHearts; // 구독 해제 (메모리 누수 방지)
        }
    }

    void UpdateHearts()
        {
            
        
        // 기존 하트 삭제
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }



            for (int i = 0; i < Player.MaxHealth; i++)
            {
                if (i < Player.Health)
                {
                    // 체력이 남아있는 부분은 꽉 찬 하트
                    Instantiate(fullHeart, transform);
                }
                else
                {
                    // 체력이 없는 부분은 빈 하트
                    Instantiate(emptyHeart, transform);
                }
            }
        }
    }
