using System;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Globalization;
using UnityEngine.SceneManagement;

public class GameStartController : MonoBehaviour
{
    public static GameStartController Instance;

    public AudioClip music;
    public AudioLowPassFilter filter;
    
    [Header("Game Start")]
    public RectTransform panel;
    public RectTransform button;
    public ArcJumpCurve2D arcJumpCurve2D;

    [Header("Game Lose")] 
    public CanvasGroup looseBackground;
    public RectTransform loosePanel;
    public RectTransform reviveButton;   // 🔹 новая кнопка
    public RectTransform restartButton;
    public TextMeshProUGUI recordText;   // новый TMP для рекорда
    public TextMeshProUGUI currentText;  // новый TMP для текущих метров

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

        filter.cutoffFrequency = 22000;
        MusicController.Instance._audioSourceMusic.clip = music;
        MusicController.Instance._audioSourceMusic.Play();
    }

    public void LoseGameController()
    {
        filter.cutoffFrequency = 300;
        
        float currentValue = MetersCount.Instance.GetCurrentMeters();
        float bestRecord = PlayerPrefs.GetFloat("BestRecord", 0f);

        // если перебили рекорд → сохранить
        if (currentValue > bestRecord)
        {
            bestRecord = currentValue;
            PlayerPrefs.SetFloat("BestRecord", bestRecord);
            PlayerPrefs.Save();
        }

        // обновляем тексты
        recordText.text = "RECORD " + bestRecord.ToString("F2", CultureInfo.InvariantCulture) + "m";
        currentText.text = "CURRENT 0.00m";

        // создаём последовательность
        Sequence seq = DOTween.Sequence();

        // 1. фон
        looseBackground.alpha = 0;
        looseBackground.gameObject.SetActive(true);
        seq.Append(looseBackground.DOFade(1f, 0.4f));

        // 2. панель
        loosePanel.localScale = Vector3.zero;
        seq.Append(loosePanel.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

        // 3. показать recordText
        recordText.alpha = 0;
        recordText.rectTransform.localScale = Vector3.zero;
        seq.Append(recordText.DOFade(1f, 0.3f));
        seq.Join(recordText.rectTransform.DOScale(1f, 0.3f).SetEase(Ease.OutBack));

        // 4. анимация currentText от 0 до currentValue
        float displayedValue = 0f;
        currentText.alpha = 1f;
        currentText.rectTransform.localScale = Vector3.zero;
        seq.Append(currentText.rectTransform.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
        seq.Append(DOTween.To(() => displayedValue, x =>
        {
            displayedValue = x;
            currentText.text = "CURRENT " + displayedValue.ToString("F2", CultureInfo.InvariantCulture) + "m";
        }, currentValue, 1f).SetEase(Ease.Linear));

        // 5. reviveButton
        reviveButton.localScale = Vector3.zero;
        seq.Append(reviveButton.DOScale(1f, 0.4f).SetEase(Ease.OutBack));

        // 6. restartButton
        restartButton.localScale = Vector3.zero;
        seq.Append(restartButton.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
