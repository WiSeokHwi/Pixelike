using UnityEngine;
using UnityEngine.UI;



public class EnemyHealthBar : MonoBehaviour

{
    public Slider slider;
    private IEnemy enemy;  // 공통 인터페이스 사용

    void Start()
    {
        // 부모에서 EnemyCTRL 또는 BossFSMController 둘 중 하나 찾기
        enemy = GetComponentInParent<IEnemy>();
        if (enemy == null)
        {
            Debug.LogError("IEnemy를 상속받는 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        slider.maxValue = enemy.MaxHealth;
    }

    void Update()
    {
        if (enemy != null)
        {
            slider.value = enemy.Health;
        }
    }
}
