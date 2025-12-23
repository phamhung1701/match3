using TMPro;
using UnityEngine;

public class FloatingScore : MonoBehaviour
{
    [SerializeField]
    TextMeshPro displayText;

    [SerializeField, Range(0.1f, 1f)]
    float displayDuration = 0.5f;

    [SerializeField, Range(0f, 4f)]
    float riseSpeed = 2f;

    float age;

    PrefabInstancePool<FloatingScore> pool;

    string FormatSmart(float value)
    {
        return Mathf.Approximately(value % 1f, 0f)
            ? ((int)value).ToString()
            : value.ToString("0.##"); 
    }

    public void Show(Vector3 position, float value, Color color)
    {
        FloatingScore instance = pool.GetInstance(this);
        instance.pool = pool;
        instance.displayText.SetText(FormatSmart(value));
        instance.displayText.color = color;
        instance.transform.localPosition = position;
        instance.age = 0f;
    }

    void Update()
    {
        age += Time.deltaTime;
        if (age >= displayDuration)
        {
            pool.Recycle(this);
        }
        else
        {
            Vector3 p = transform.localPosition;
            p.y += riseSpeed * Time.deltaTime;
            transform.localPosition = p;
        }
    }
}