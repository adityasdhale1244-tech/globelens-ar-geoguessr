using System;
using UnityEngine;

public enum GeoGamePhase
{
    Ready,
    Playing,
    Paused,
    Revealed,
    Complete
}

public sealed class GeoGameSnapshot
{
    public GeoGamePhase phase;
    public int roundIndex;
    public int roundCount;
    public int score;
    public float secondsLeft;
    public bool hasGuess;
    public float guessLatitude;
    public float guessLongitude;
    public string clue;
    public string panoramaAsset;
    public string revealedLocation;
    public float distanceKm;
    public int roundPoints;
    public bool timedOut;
}

public sealed class GeoGuessrGameManager : MonoBehaviour
{
    [SerializeField] private RoundCatalog roundCatalog;
    [SerializeField, Min(5f)] private float secondsPerRound = 20f;

    public event Action<GeoGameSnapshot> StateChanged;

    private GeoRound[] rounds;
    private int roundIndex;
    private int score;
    private float secondsLeft;
    private bool hasGuess;
    private float guessLatitude;
    private float guessLongitude;
    private float distanceKm;
    private int roundPoints;
    private bool timedOut;
    private GeoGamePhase phase = GeoGamePhase.Ready;

    private void Start()
    {
        if (roundCatalog == null)
        {
            Debug.LogError("Assign a RoundCatalog to GeoGuessrGameManager.", this);
            enabled = false;
            return;
        }

        try
        {
            rounds = roundCatalog.LoadRounds();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, this);
            enabled = false;
            return;
        }

        Publish();
    }

    private void Update()
    {
        if (phase != GeoGamePhase.Playing)
            return;

        secondsLeft = Mathf.Max(0f, secondsLeft - Time.deltaTime);
        if (secondsLeft <= 0f)
            ResolveRound(true);
        else
            Publish();
    }

    public void StartGame()
    {
        if (rounds == null || rounds.Length == 0) return;
        roundIndex = 0;
        score = 0;
        BeginRound();
    }

    public void TogglePause()
    {
        if (phase == GeoGamePhase.Playing) phase = GeoGamePhase.Paused;
        else if (phase == GeoGamePhase.Paused) phase = GeoGamePhase.Playing;
        else return;
        Publish();
    }

    public bool PlaceGuess(float latitude, float longitude)
    {
        if (phase != GeoGamePhase.Playing ||
            float.IsNaN(latitude) || float.IsNaN(longitude) ||
            latitude < -90f || latitude > 90f || longitude < -180f || longitude > 180f)
            return false;

        hasGuess = true;
        guessLatitude = latitude;
        guessLongitude = longitude;
        Publish();
        return true;
    }

    public void SubmitGuess()
    {
        if (phase != GeoGamePhase.Playing) return;
        if (!hasGuess)
        {
            Publish();
            return;
        }
        ResolveRound(false);
    }

    public void NextRound()
    {
        if (phase != GeoGamePhase.Revealed) return;
        if (roundIndex + 1 >= rounds.Length)
        {
            phase = GeoGamePhase.Complete;
            Publish();
            return;
        }
        roundIndex++;
        BeginRound();
    }

    public void Replay()
    {
        if (rounds == null || rounds.Length == 0) return;
        roundIndex = 0;
        score = 0;
        BeginRound();
    }

    public GeoGameSnapshot GetSnapshot()
    {
        GeoRound round = rounds != null && roundIndex < rounds.Length ? rounds[roundIndex] : null;
        bool reveal = phase == GeoGamePhase.Revealed || phase == GeoGamePhase.Complete;
        return new GeoGameSnapshot
        {
            phase = phase,
            roundIndex = roundIndex,
            roundCount = rounds == null ? 0 : rounds.Length,
            score = score,
            secondsLeft = secondsLeft,
            hasGuess = hasGuess,
            guessLatitude = guessLatitude,
            guessLongitude = guessLongitude,
            clue = round == null ? string.Empty : round.clue,
            panoramaAsset = round == null ? string.Empty : round.panoramaAsset,
            revealedLocation = reveal && round != null ? round.name + ", " + round.country : string.Empty,
            distanceKm = distanceKm,
            roundPoints = roundPoints,
            timedOut = timedOut
        };
    }

    private void BeginRound()
    {
        phase = GeoGamePhase.Playing;
        secondsLeft = secondsPerRound;
        hasGuess = false;
        guessLatitude = 0f;
        guessLongitude = 0f;
        distanceKm = 0f;
        roundPoints = 0;
        timedOut = false;
        Publish();
    }

    private void ResolveRound(bool expired)
    {
        timedOut = expired;
        roundPoints = 0;
        distanceKm = 0f;

        if (hasGuess)
        {
            GeoRound answer = rounds[roundIndex];
            distanceKm = DistanceKm(guessLatitude, guessLongitude, answer.latitude, answer.longitude);
            roundPoints = Mathf.RoundToInt(5000f * Mathf.Exp(-distanceKm / 1800f));
            score += roundPoints;
        }

        phase = GeoGamePhase.Revealed;
        Publish();
    }

    private static float DistanceKm(float lat1, float lon1, float lat2, float lon2)
    {
        const float earthRadiusKm = 6371f;
        float dLat = Mathf.Deg2Rad * (lat2 - lat1);
        float dLon = Mathf.Deg2Rad * (lon2 - lon1);
        float a = Mathf.Sin(dLat / 2f) * Mathf.Sin(dLat / 2f) +
                  Mathf.Cos(Mathf.Deg2Rad * lat1) * Mathf.Cos(Mathf.Deg2Rad * lat2) *
                  Mathf.Sin(dLon / 2f) * Mathf.Sin(dLon / 2f);
        return earthRadiusKm * 2f * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1f - a));
    }

    private void Publish()
    {
        StateChanged?.Invoke(GetSnapshot());
    }
}
