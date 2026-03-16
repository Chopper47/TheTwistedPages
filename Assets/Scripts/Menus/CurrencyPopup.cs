using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite[] possibleIcons;

    [SerializeField] private float lifeTime = 1.5f;
    [SerializeField] private float moveSpeed = 50.0f;
    [SerializeField] private float fadeDelay = 0.5f;

    private float elapsedTime = 0f;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (canvasGroup != null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    public void Setup(int amount)
    {
        if (text != null)
        {
            text.text = amount.ToString();
        }

        if (iconImage != null && possibleIcons != null && possibleIcons.Length > 0)
        {
            int randomIndex = Random.Range(0, possibleIcons.Length);
            iconImage.sprite = possibleIcons[randomIndex];
        }

        canvasGroup.alpha = 1.0f;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        if (elapsedTime > fadeDelay)
        {
            float fadeTime = lifeTime - fadeDelay;
            float t = (elapsedTime - fadeDelay) / fadeTime;
            canvasGroup.alpha = Mathf.Lerp(1.0f, 0f, t);
        }

        if (elapsedTime >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}