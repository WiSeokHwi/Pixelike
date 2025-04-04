using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{

    private string itemName;
    public AnimationCurve animCurve;
    public float height = 1.5f;
    public float popDuration = 1f;

    private Rigidbody2D rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        itemName = gameObject.name;
        StartCoroutine(PopUp());
    }
    private IEnumerator PopUp()
    {
        Vector2 startPos = rb.position;
        float randomX = startPos.x + Random.Range(-2f, 2f);
        float randomY = startPos.y + Random.Range(-1f, 1f);

        Vector2 targetPos = new Vector2(randomX, randomY);

        float timePassed = 0f;

        while(timePassed < popDuration)
        {
            timePassed += Time.deltaTime;
            float t = timePassed / popDuration;
            float heightOffset = animCurve.Evaluate(t) * height;
            float heightSpeed = Mathf.Lerp(0, height, heightOffset);

            transform.position = (Vector2.Lerp(startPos, targetPos, t) + new Vector2(0, heightSpeed));
            yield return null;
        }

    }
}
