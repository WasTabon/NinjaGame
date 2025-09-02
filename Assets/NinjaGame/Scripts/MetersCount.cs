using UnityEngine;
using TMPro;
using System.Globalization; // обязательно!

public class MetersCount : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Transform player;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI metersText;

    private float startY;
    private float maxY;
    private float metersTraveled;

    private string cachedText;

    private void Start()
    {
        if (player == null)
            player = transform;

        startY = player.position.y;
        maxY = startY;
        UpdateText(0f);
    }

    private void Update()
    {
        float currentY = player.position.y;

        if (currentY > maxY)
        {
            maxY = currentY;
            metersTraveled = maxY - startY;

            UpdateText(metersTraveled);
        }
    }

    private void UpdateText(float value)
    {
        string newText = value.ToString("F2", CultureInfo.InvariantCulture) + "m";

        if (newText != cachedText)
        {
            metersText.text = newText;
            cachedText = newText;
        }
    }
}