using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private string scoreFormat = "{0}";
    [Space]
    [SerializeField]private GameHandler gameHandler;

    private void Start() => gameHandler = gameHandler != null ? gameHandler : FindObjectOfType<GameHandler>();
    private void OnEnable() => gameHandler.AddScoreListener(Refresh);
    private void OnDisable() => gameHandler.RemoveScoreListener(Refresh);

    public void Refresh(int newScore)
    {
        scoreText.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        scoreText.transform.DOKill();
        scoreText.transform.DOScale(1, .5f);

        scoreText.text = string.Format(scoreFormat, newScore);
    }
}
