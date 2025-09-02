using System;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Globalization;
using UnityEngine.SceneManagement;

public class GameStartController : MonoBehaviour
{
    public static GameStartController Instance;
    
    [Header("Game Start")]
    public RectTransform panel;
    public RectTransform button;
    public ArcJumpCurve2D arcJumpCurve2D;

    [Header("Game Lose")] 
    public CanvasGroup looseBackground;  // фон лучше через CanvasGroup
    public RectTransform loosePanel;
    public RectTransform restartButton;
    public TextMeshProUGUI oldMetersText;
    public TextMeshProUGUI metersText;   // текст, куда выводим финальный результат

    private void Awake()
    {
        Instance = this;
    }

    public void StartGame()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(button.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));

        seq.OnComplete(() =>
        {
            panel.gameObject.SetActive(false);
            arcJumpCurve2D.StartGame();
        });
    }

    public void LoseGameController()
    {
        float finalValue = ParseMeters(oldMetersText.text);

        // создаём последовательность
        Sequence seq = DOTween.Sequence();

        // 1. убираем старый текст (alpha = 0 и scale вниз)
        seq.Append(oldMetersText.DOFade(0f, 0.3f));
        seq.Join(oldMetersText.rectTransform.DOScale(0.7f, 0.3f));

        // 2. включаем фон (alpha = 1)
        looseBackground.alpha = 0;
        looseBackground.gameObject.SetActive(true);
        seq.Append(looseBackground.DOFade(1f, 0.4f));

        // 3. показываем панель
        loosePanel.localScale = Vector3.zero;
        seq.Append(loosePanel.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

        // 4. считаем текст от 0 до finalValue
        metersText.text = "RECORD 0m";
        float displayedValue = 0f;
        seq.Append(DOTween.To(() => displayedValue, x =>
        {
            displayedValue = x;
            metersText.text = "RECORD " + displayedValue.ToString("F2", CultureInfo.InvariantCulture) + "m";
        }, finalValue, 1f).SetEase(Ease.Linear));

        // 5. показываем restartButton
        restartButton.localScale = Vector3.zero;
        seq.Append(restartButton.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private float ParseMeters(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0f;

        // убираем "m" и парсим
        string numeric = text.Replace("m", "").Trim();

        if (float.TryParse(numeric, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            return result;

        return 0f;
    }
}
