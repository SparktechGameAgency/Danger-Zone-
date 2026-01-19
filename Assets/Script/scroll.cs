using UnityEngine;

public class ScrollLockYElastic : MonoBehaviour
{
    [Header("Clamp Y Range")]
    public float maxY = 1250f;    // Top boundary
    public float minY = -1700f;   // Bottom boundary

    [Header("Elastic Settings")]
    public float bounceSpeed = 10f;   // How fast it returns
    public float elasticity = 0.3f;   // Smoothness of bounce (0.1 = soft, 1 = snappy)

    private RectTransform rectTransform;
    private Vector3 targetPos;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();


        Vector3 startPos = rectTransform.anchoredPosition;
        startPos.y = minY;
        rectTransform.anchoredPosition = startPos;
    }

    void LateUpdate()
    {
        Vector3 pos = rectTransform.anchoredPosition;


        if (pos.y <= maxY && pos.y >= minY)
        {
            targetPos = pos;
        }
        else
        {

            float clampedY = Mathf.Clamp(pos.y, minY, maxY);
            targetPos = new Vector3(pos.x, clampedY, pos.z);
        }


        rectTransform.anchoredPosition = Vector3.Lerp(
            rectTransform.anchoredPosition,
            targetPos,
            Time.deltaTime * bounceSpeed
        );




    }
}


