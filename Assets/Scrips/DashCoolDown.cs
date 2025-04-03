using UnityEngine;
using UnityEngine.UI;
public class DashCoolDown : MonoBehaviour
{
    public Slider slider;
    private Player player;

    void Start()
    {
        player = GetComponentInParent<Player>();
        slider.maxValue = player.dashCoolTime;

    }

    // Update is called once per frame
    void Update()
    {
        slider.value = player.dashingTime;
    }
}
