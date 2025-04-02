using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    private EnemyCTRL enemy;
    
    void Start()
    {
        enemy = GetComponentInParent<EnemyCTRL>();
        slider.maxValue = enemy.maxHealth;
        
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = enemy.Health;
    }
}
