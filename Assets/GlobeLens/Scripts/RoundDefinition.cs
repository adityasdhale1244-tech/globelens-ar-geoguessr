using System;
using UnityEngine;

[Serializable]
public sealed class GeoRound
{
    public string id;
    public string name;
    public string country;
    [TextArea(2, 4)] public string clue;
    public float latitude;
    public float longitude;
    // Texture name loaded from Resources/Panoramas (without extension).
    public string panoramaAsset;
    // Optional prefab name loaded from Resources/Cues (without extension).
    public string cuePrefab;
    public string cueCaption;
    // HTML color string, e.g. #B8E5D0. Empty uses the scene's default ambience.
    public string ambienceHex;
}

[Serializable]
public sealed class GeoRoundCollection
{
    public GeoRound[] rounds;
}
