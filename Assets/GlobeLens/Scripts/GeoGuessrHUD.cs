using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GeoGuessrHUD : MonoBehaviour
{
    [Header("Game")]
    [SerializeField] private GeoGuessrGameManager gameManager;
    [SerializeField] private MapGuessInput mapInput;

    [Header("Text")]
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text clueText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text revealText;

    [Header("Panels and buttons")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject revealPanel;
    [SerializeField] private GameObject finalPanel;
    [SerializeField] private Button startButton;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private TMP_Text pauseButtonText;

    private void OnEnable()
    {
        if (gameManager != null) gameManager.StateChanged += Render;
        if (startButton != null) startButton.onClick.AddListener(StartGame);
        if (submitButton != null) submitButton.onClick.AddListener(SubmitGuess);
        if (nextButton != null) nextButton.onClick.AddListener(NextRound);
        if (pauseButton != null) pauseButton.onClick.AddListener(TogglePause);
        if (replayButton != null) replayButton.onClick.AddListener(Replay);
    }

    private void Start()
    {
        if (gameManager != null) Render(gameManager.GetSnapshot());
    }

    private void OnDisable()
    {
        if (gameManager != null) gameManager.StateChanged -= Render;
        if (startButton != null) startButton.onClick.RemoveListener(StartGame);
        if (submitButton != null) submitButton.onClick.RemoveListener(SubmitGuess);
        if (nextButton != null) nextButton.onClick.RemoveListener(NextRound);
        if (pauseButton != null) pauseButton.onClick.RemoveListener(TogglePause);
        if (replayButton != null) replayButton.onClick.RemoveListener(Replay);
    }

    private void StartGame() => gameManager.StartGame();
    private void SubmitGuess() => gameManager.SubmitGuess();
    private void NextRound() => gameManager.NextRound();
    private void TogglePause() => gameManager.TogglePause();
    private void Replay() => gameManager.Replay();

    private void Render(GeoGameSnapshot state)
    {
        if (roundText != null) roundText.text = state.roundCount == 0 ? "" : $"ROUND {Mathf.Min(state.roundIndex + 1, state.roundCount):00} / {state.roundCount:00}";
        if (scoreText != null) scoreText.text = $"{state.score:N0} PTS";
        if (timerText != null) timerText.text = state.phase == GeoGamePhase.Paused ? "PAUSED" : $"{Mathf.CeilToInt(state.secondsLeft):00}";
        if (clueText != null) clueText.text = state.clue;

        bool ready = state.phase == GeoGamePhase.Ready;
        bool complete = state.phase == GeoGamePhase.Complete;
        bool revealed = state.phase == GeoGamePhase.Revealed;
        bool playing = state.phase == GeoGamePhase.Playing;
        bool paused = state.phase == GeoGamePhase.Paused;

        SetActive(startPanel, ready);
        SetActive(gamePanel, !ready && !complete);
        SetActive(revealPanel, revealed);
        SetActive(finalPanel, complete);

        if (submitButton != null) submitButton.interactable = playing && state.hasGuess;
        if (nextButton != null) nextButton.interactable = revealed;
        if (pauseButton != null) pauseButton.interactable = playing || paused;
        if (pauseButtonText != null) pauseButtonText.text = paused ? "RESUME" : "PAUSE";
        if (replayButton != null) replayButton.interactable = complete;

        if (statusText != null)
        {
            if (ready) statusText.text = "A quick journey around the world. No controller required.";
            else if (paused) statusText.text = "Paused — take a breath, then resume.";
            else if (playing) statusText.text = state.hasGuess ? "Pin placed. Lock in your guess." : "Look around, then tap the map to guess.";
            else if (revealed) statusText.text = state.timedOut && !state.hasGuess ? "Time is up — no points this round." : $"{state.distanceKm:N0} km away · +{state.roundPoints:N0} points";
            else statusText.text = $"Journey complete · {state.score:N0} points";
        }

        if (revealText != null && revealed)
        {
            string distance = state.timedOut && !state.hasGuess ? "No guess placed" : $"{state.distanceKm:N0} km from your guess";
            revealText.text = $"{state.revealedLocation}\n{distance}\n+{state.roundPoints:N0} points";
        }

        if (revealed && mapInput != null) mapInput.enabled = false;
        else if (playing && mapInput != null) mapInput.enabled = true;
        if ((ready || complete) && mapInput != null) mapInput.ClearMarker();
    }

    private static void SetActive(GameObject target, bool value)
    {
        if (target != null && target.activeSelf != value) target.SetActive(value);
    }
}
