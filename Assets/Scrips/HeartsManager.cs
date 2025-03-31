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
            
        
        // 기존 하트 삭제
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // 새로운 하트 생성
            int childCount = transform.childCount;

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
