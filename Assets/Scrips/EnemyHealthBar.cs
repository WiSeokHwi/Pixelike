using UnityEngine;
using UnityEngine.UI;



public class EnemyHealthBar : MonoBehaviour

{
    public Slider slider;
    private IEnemy enemy;  // ���� �������̽� ���

    void Start()
    {
        // �θ𿡼� EnemyCTRL �Ǵ� BossFSMController �� �� �ϳ� ã��
        enemy = GetComponentInParent<IEnemy>();
        if (enemy == null)
        {
            Debug.LogError("IEnemy�� ��ӹ޴� ������Ʈ�� ã�� �� �����ϴ�.");
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
