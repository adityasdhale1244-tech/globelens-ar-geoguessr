using System;
using UnityEngine;

public sealed class RoundCatalog : MonoBehaviour
{
    [SerializeField] private TextAsset roundsJson;

    public GeoRound[] LoadRounds()
    {
        TextAsset source = roundsJson != null ? roundsJson : Resources.Load<TextAsset>("rounds");
        if (source == null)
            throw new InvalidOperationException("Assign rounds.json or place it at Assets/Resources/rounds.json.");

        GeoRoundCollection collection = JsonUtility.FromJson<GeoRoundCollection>(source.text);
        if (collection == null || collection.rounds == null || collection.rounds.Length == 0)
            throw new InvalidOperationException("rounds.json must contain a non-empty 'rounds' array.");

        foreach (GeoRound round in collection.rounds)
        {
            if (round == null || string.IsNullOrWhiteSpace(round.id) ||
                string.IsNullOrWhiteSpace(round.name) || string.IsNullOrWhiteSpace(round.clue) ||
                round.latitude < -90f || round.latitude > 90f ||
                round.longitude < -180f || round.longitude > 180f)
                throw new InvalidOperationException("Every round needs an id, name, clue, and valid coordinates.");
        }

        return collection.rounds;
    }
}
