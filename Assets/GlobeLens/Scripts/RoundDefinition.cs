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
}

[Serializable]
public sealed class GeoRoundCollection
{
    public GeoRound[] rounds;
}
