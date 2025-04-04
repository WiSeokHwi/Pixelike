using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private Player player;
    public TextMeshProUGUI AttackPower;

    void Start()
    {
        player = FindAnyObjectByType <Player>();

        
    }

    // Update is called once per frame
    void Update()
    {
        AttackPower.text = player.Damage.ToString("F2");
    }
}
