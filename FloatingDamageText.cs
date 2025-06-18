using TMPro;
using UnityEngine;

public class FloatingDamageText : MonoBehaviour
{
    [SerializeField] float life = 1.5f;
    [SerializeField] float riseSpeed = 1f;

    private TextMeshProUGUI tmp;
    private Color startColor;
    private float elapsed;   // nesnenin kendi ömrünü takip etmek için

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        startColor = tmp.color;
        Destroy(gameObject, life);
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // Yukarý doðru kaydýr
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // Alfa’yý ömrü boyunca 1 -> 0 arasýnda azalt
        float t = 1 - (elapsed / life);
        tmp.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Clamp01(t));
    }
}
