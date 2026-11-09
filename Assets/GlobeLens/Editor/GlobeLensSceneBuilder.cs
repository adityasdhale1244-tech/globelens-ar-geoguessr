using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Creates a desktop-playable demo scene shell. Add Meta XR hand-ray input before Quest validation.</summary>
public static class GlobeLensSceneBuilder
{
    private const string ScenePath = "Assets/GlobeLens/Scenes/GlobeLensDemo.unity";
    private static TMP_FontAsset font;

    [MenuItem("GlobeLens/Create Demo Scene")]
    public static void CreateScene()
    {
        font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font == null)
        {
            EditorUtility.DisplayDialog("TextMeshPro required", "Import TextMeshPro Essentials from Window > TextMeshPro > Import TMP Essential Resources, then run this command again.", "OK");
            return;
        }

        Directory.CreateDirectory("Assets/GlobeLens/Scenes");
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject cameraObject = new GameObject("Demo Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 1.6f, -4.5f);
        cameraObject.transform.rotation = Quaternion.identity;
        cameraObject.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        cameraObject.GetComponent<Camera>().backgroundColor = new Color(.035f, .07f, .06f);

        GameObject systems = new GameObject("Game Systems");
        RoundCatalog catalog = systems.AddComponent<RoundCatalog>();
        GeoGuessrGameManager manager = systems.AddComponent<GeoGuessrGameManager>();
        PanoramaDisplay panorama = systems.AddComponent<PanoramaDisplay>();
        RoundEnvironmentController environment = systems.AddComponent<RoundEnvironmentController>();
        Light key = new GameObject("Soft Key Light", typeof(Light)).GetComponent<Light>();
        key.transform.SetParent(systems.transform);
        key.type = LightType.Directional;
        key.intensity = .55f;
        key.transform.rotation = Quaternion.Euler(35f, -25f, 0f);
        SetObject(environment, "keyLight", key);

        GameObject canvasObject = new GameObject("Game UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = cameraObject.GetComponent<Camera>();
        canvasObject.transform.position = new Vector3(0f, 1.45f, -1.8f);
        canvasObject.transform.localScale = Vector3.one * .0012f;
        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1600f, 900f);
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.scaleFactor = 1f;

        GameObject eventObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        eventObject.GetComponent<StandaloneInputModule>().forceModuleActive = true;

        GameObject backdrop = Panel("Evergreen Backplate", canvasObject.transform, new Vector2(0, 0), new Vector2(1600, 900), new Color(.035f, .075f, .06f, .96f));
        GameObject header = Panel("Header", backdrop.transform, new Vector2(0, 390), new Vector2(1460, 90), new Color(.07f, .13f, .105f, 1f));
        Text("GLOBE LENS   /   FIELD NOTES", header.transform, new Vector2(-490, 0), new Vector2(600, 60), 30, new Color(.72f, .95f, .72f));
        TMP_Text roundText = Text("ROUND 01 / 03", header.transform, new Vector2(300, 0), new Vector2(260, 55), 25, Color.white);
        TMP_Text scoreText = Text("0 PTS", header.transform, new Vector2(590, 0), new Vector2(190, 55), 25, new Color(.75f, 1f, .55f));
        TMP_Text timerText = Text("30", header.transform, new Vector2(710, 0), new Vector2(100, 55), 28, Color.white);

        GameObject clueCard = Panel("Clue Card", backdrop.transform, new Vector2(-430, 205), new Vector2(620, 190), new Color(.08f, .15f, .12f, .98f));
        Text("FIELD CLUE", clueCard.transform, new Vector2(0, 55), new Vector2(520, 38), 20, new Color(.72f, .95f, .72f));
        TMP_Text clueText = Text("Read the clues and place your best guess.", clueCard.transform, new Vector2(0, -25), new Vector2(540, 110), 27, Color.white);
        clueText.alignment = TextAlignmentOptions.Center;

        GameObject mapCard = Panel("World Map Console", backdrop.transform, new Vector2(340, -45), new Vector2(700, 460), new Color(.055f, .11f, .09f, 1f));
        TMP_Text mapTitle = Text("NORTH-UP WORLD MAP  ·  POINT + PINCH", mapCard.transform, new Vector2(0, 192), new Vector2(650, 38), 17, new Color(.75f, 1f, .55f));
        mapTitle.alignment = TextAlignmentOptions.Center;
        GameObject mapObject = new GameObject("World Map", typeof(RectTransform), typeof(CanvasRenderer), typeof(WorldMapGraphic), typeof(MapGuessInput));
        mapObject.transform.SetParent(mapCard.transform, false);
        RectTransform mapRect = mapObject.GetComponent<RectTransform>();
        mapRect.sizeDelta = new Vector2(650, 330);
        mapRect.anchoredPosition = new Vector2(0, -14);
        WorldMapGraphic worldGraphic = mapObject.GetComponent<WorldMapGraphic>();
        worldGraphic.color = Color.white;

        RectTransform preview = Marker("Preview Pin", mapRect, new Color(.55f, .9f, .85f, .4f), 25f);
        RectTransform guessMarker = Marker("Guess Pin", mapRect, new Color(.72f, 1f, .35f, 1f), 34f);
        RectTransform answerMarker = Marker("Answer Pin", mapRect, new Color(.45f, .85f, 1f, 1f), 34f);
        RectTransform connector = ImageRect("Guess to Answer Line", mapRect, new Color(.45f, .85f, 1f, .8f), new Vector2(4, 4));
        connector.gameObject.SetActive(false);
        MapGuessInput mapInput = mapObject.GetComponent<MapGuessInput>();
        SetObject(mapInput, "gameManager", manager);
        SetObject(mapInput, "mapRect", mapRect);
        SetObject(mapInput, "previewMarker", preview);
        SetObject(mapInput, "guessMarker", guessMarker);
        SetObject(mapInput, "answerMarker", answerMarker);
        SetObject(mapInput, "connector", connector);

        TMP_Text statusText = Text("Point at the map · pinch to place your pin.", backdrop.transform, new Vector2(-400, -195), new Vector2(650, 55), 22, Color.white);
        TMP_Text revealText = Text("", backdrop.transform, new Vector2(-400, -265), new Vector2(650, 80), 22, new Color(.75f, 1f, .55f));
        TMP_Text cueCaption = Text("", backdrop.transform, new Vector2(-420, 100), new Vector2(590, 55), 20, new Color(.85f, .88f, .78f));
        GameObject startPanel = Panel("Start Panel", backdrop.transform, Vector2.zero, new Vector2(500, 300), new Color(.03f, .08f, .06f, .96f));
        Text("GLOBELENS QUEST", startPanel.transform, new Vector2(0, 65), new Vector2(460, 75), 38, new Color(.75f, 1f, .55f));
        Text("Look · Pin · Reveal", startPanel.transform, new Vector2(0, 5), new Vector2(440, 45), 24, Color.white);
        Button("START JOURNEY", startPanel.transform, new Vector2(0, -75), manager, "StartGame");

        GameObject gamePanel = backdrop;
        GameObject revealPanel = Panel("Reveal Panel", backdrop.transform, new Vector2(-400, -320), new Vector2(600, 110), new Color(.08f, .15f, .12f, .98f));
        TMP_Text revealLabel = Text("LOCATION REVEALED", revealPanel.transform, Vector2.zero, new Vector2(550, 80), 23, Color.white);
        GameObject finalPanel = Panel("Final Panel", backdrop.transform, Vector2.zero, new Vector2(540, 320), new Color(.03f, .08f, .06f, .98f));
        Text("JOURNEY COMPLETE", finalPanel.transform, new Vector2(0, 55), new Vector2(500, 65), 34, new Color(.75f, 1f, .55f));
        Text("Your final score", finalPanel.transform, new Vector2(0, 0), new Vector2(450, 45), 24, Color.white);
        Button("PLAY AGAIN", finalPanel.transform, new Vector2(0, -75), manager, "Replay");

        Button("LOCK IN GUESS", backdrop.transform, new Vector2(340, -360), manager, "SubmitGuess");
        Button("NEXT LOCATION", backdrop.transform, new Vector2(650, -360), manager, "NextRound");
        Button("PAUSE", backdrop.transform, new Vector2(700, 390), manager, "TogglePause");

        SetObject(manager, "roundCatalog", catalog);
        GeoGuessrHUD hud = systems.AddComponent<GeoGuessrHUD>();
        SetObject(hud, "gameManager", manager);
        SetObject(hud, "mapInput", mapInput);
        SetObject(hud, "panoramaDisplay", panorama);
        SetObject(hud, "environmentController", environment);
        SetObject(hud, "roundText", roundText);
        SetObject(hud, "scoreText", scoreText);
        SetObject(hud, "timerText", timerText);
        SetObject(hud, "clueText", clueText);
        SetObject(hud, "statusText", statusText);
        SetObject(hud, "revealText", revealLabel);
        SetObject(hud, "cueCaptionText", cueCaption);
        SetObject(hud, "startPanel", startPanel);
        SetObject(hud, "gamePanel", gamePanel);
        SetObject(hud, "revealPanel", revealPanel);
        SetObject(hud, "finalPanel", finalPanel);
        SetObject(hud, "startButton", startPanel.GetComponentInChildren<Button>());
        SetObject(hud, "submitButton", FindButton("LOCK IN GUESS"));
        SetObject(hud, "nextButton", FindButton("NEXT LOCATION"));
        SetObject(hud, "pauseButton", FindButton("PAUSE"));
        SetObject(hud, "replayButton", finalPanel.GetComponentInChildren<Button>());
        SetObject(hud, "pauseButtonText", FindButton("PAUSE").GetComponentInChildren<TMP_Text>());

        SetActive(finalPanel, false);
        SetActive(revealPanel, false);
        SetActive(gamePanel, false);
        SetActive(startPanel, true);

        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Scene created", "Created " + ScenePath + ". This is an editor demo scene shell; configure Meta XR hand-ray UI input and test on Quest before submission.", "OK");
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
    }

    private static GameObject Panel(string name, Transform parent, Vector2 position, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        go.GetComponent<Image>().color = color;
        return go;
    }

    private static TMP_Text Text(string name, Transform parent, Vector2 position, Vector2 size, float fontSize, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        TMP_Text text = go.GetComponent<TMP_Text>();
        text.font = font;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = true;
        return text;
    }

    private static RectTransform Marker(string name, Transform parent, Color color, float size)
    {
        RectTransform rect = ImageRect(name, parent, color, Vector2.one * size);
        Image image = rect.GetComponent<Image>();
        image.raycastTarget = false;
        return rect;
    }

    private static RectTransform ImageRect(string name, Transform parent, Color color, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.sizeDelta = size;
        Image image = go.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return rect;
    }

    private static Button Button(string label, Transform parent, Vector2 position, Object target, string method)
    {
        GameObject go = Panel(label, parent, position, new Vector2(250, 62), new Color(.72f, .95f, .55f, 1f));
        Button button = go.AddComponent<Button>();
        Text(label + " Label", go.transform, Vector2.zero, new Vector2(230, 52), 20, new Color(.04f, .09f, .06f));
        button.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
        {
            if (target is GeoGuessrGameManager manager)
            {
                var methodInfo = typeof(GeoGuessrGameManager).GetMethod(method, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                methodInfo?.Invoke(manager, null);
            }
        }));
        return button;
    }

    private static Button FindButton(string label)
    {
        foreach (Button button in Resources.FindObjectsOfTypeAll<Button>())
            if (button.gameObject.name == label) return button;
        return null;
    }

    private static void SetObject(Object component, string property, Object value)
    {
        if (component == null) return;
        SerializedObject serialized = new SerializedObject(component);
        SerializedProperty field = serialized.FindProperty(property);
        if (field == null) return;
        field.objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetActive(GameObject target, bool value)
    {
        if (target != null) target.SetActive(value);
    }
}
