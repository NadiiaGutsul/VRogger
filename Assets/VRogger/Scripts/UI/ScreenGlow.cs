using UnityEngine;
using UnityEngine.UI;

public class ScreenGlow : MonoBehaviour
{
    private Color baseColor = new Color(0.75f, 0.87f, 0.395f);
    [SerializeField] private float glowSpeed = 2f;
    [SerializeField] private float minIntensity = 0.8f;
    [SerializeField] private float maxIntensity = 1.2f;
    
    
    private Image panelImage;
    private float timer = 0f;

    private void Awake()
    {
        panelImage = GetComponent<Image>();
    }

    private void Start()
    {
        if (panelImage)
        {
            panelImage.color = baseColor;
        }
    }

    private void Update()
    {
        if (!panelImage) return;
        AnimateGlow();
    }
    
    private void AnimateGlow()
    {
        timer += Time.deltaTime * glowSpeed;

        float intensity = Mathf.Lerp(minIntensity, maxIntensity,
            (Mathf.Sin(timer) + 1f) / 2f);

        Color glowColor = baseColor * intensity;
        glowColor.a = 1f;
        panelImage.color = glowColor;
    }
}
