using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class UIBuilder : EditorWindow
{
    [MenuItem("Tools/Fix Existing UI Sizes")]
    public static void FixExistingUI()
    {
        // 1. Find Background (lifebar)
        GameObject bgObj = GameObject.Find("Background");
        if (bgObj != null)
        {
            RectTransform bgRt = bgObj.GetComponent<RectTransform>();
            if (bgRt != null)
            {
                Undo.RecordObject(bgRt, "Adjust Background Size");
                bgRt.sizeDelta = new Vector2(bgRt.sizeDelta.x, 35f); // Height to 35
                Debug.Log("UIBuilder: Adjusted Background (lifebar) height to 35.");
                
                // Adjust child (fill bar)
                if (bgRt.childCount > 0)
                {
                    RectTransform fillRt = bgRt.GetChild(0).GetComponent<RectTransform>();
                    if (fillRt != null)
                    {
                        Undo.RecordObject(fillRt, "Adjust Fill Anchors");
                        fillRt.anchorMin = Vector2.zero;
                        fillRt.anchorMax = Vector2.one;
                        fillRt.anchoredPosition = Vector2.zero;
                        fillRt.sizeDelta = Vector2.zero;
                        // Leave a 4px padding around the fill bar
                        fillRt.offsetMin = new Vector2(4f, 4f);
                        fillRt.offsetMax = new Vector2(-4f, -4f);
                        Debug.Log("UIBuilder: Set lifebar fill to stretch automatically with padding.");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("UIBuilder: Could not find 'Background' GameObject in the scene.");
        }

        // 2. Find Selected Spell Image (named "Image")
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Transform imgTransform = canvas.transform.Find("Image");
            if (imgTransform != null)
            {
                RectTransform imgRt = imgTransform.GetComponent<RectTransform>();
                if (imgRt != null)
                {
                    Undo.RecordObject(imgRt, "Adjust Spell Image Size");
                    imgRt.localScale = Vector3.one; // Reset scale to 1
                    imgRt.sizeDelta = new Vector2(70f, 70f); // Size to 70x70
                    imgRt.anchoredPosition = new Vector2(-60f, 60f); // Offset from Bottom-Right
                    Debug.Log("UIBuilder: Adjusted Selected Spell Image size to 70x70 and reset scale.");
                }
            }
            else
            {
                Debug.LogWarning("UIBuilder: Could not find 'Image' (spell icon) under Canvas.");
            }
        }
        
        EditorUtility.DisplayDialog("UI Adjuster", "¡La barra de vida y el icono de hechizo han sido redimensionados con éxito!", "Aceptar");
    }

    [MenuItem("Tools/Build Game UI")]
    public static void BuildUI()
    {
        // 1. Find or create GameManager in the active scene
        GameManager gameManager = Object.FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            GameObject gmObj = new GameObject("GameManager");
            gameManager = gmObj.AddComponent<GameManager>();
            Undo.RegisterCreatedObjectUndo(gmObj, "Create GameManager");
            Debug.Log("UIBuilder: Created GameManager GameObject.");
        }

        // 2. Find or create Canvas
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        GameObject canvasObj;
        if (canvas == null)
        {
            canvasObj = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // Adjust canvas scaler for standard scale-with-screen-size
            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");
            Debug.Log("UIBuilder: Created Canvas GameObject.");
        }
        else
        {
            canvasObj = canvas.gameObject;
        }

        // 3. Ensure EventSystem exists
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
            Undo.RegisterCreatedObjectUndo(eventSystemObj, "Create EventSystem");
            Debug.Log("UIBuilder: Created EventSystem GameObject.");
        }

        // 4. Delete old panels under Canvas to avoid duplicates
        Transform canvasTransform = canvasObj.transform;
        string[] panelNames = { "MainMenuPanel", "PausePanel", "GameOverPanel", "VictoryPanel" };
        foreach (string panelName in panelNames)
        {
            Transform existing = canvasTransform.Find(panelName);
            if (existing != null)
            {
                Undo.DestroyObjectImmediate(existing.gameObject);
            }
        }

        // 5. Create HUD Panel if it doesn't exist
        Transform hudTransform = canvasTransform.Find("HUDPanel");
        GameObject hudPanel;
        if (hudTransform == null)
        {
            hudPanel = new GameObject("HUDPanel", typeof(RectTransform));
            hudPanel.transform.SetParent(canvasTransform, false);
            RectTransform rt = hudPanel.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            Undo.RegisterCreatedObjectUndo(hudPanel, "Create HUDPanel");
        }
        else
        {
            hudPanel = hudTransform.gameObject;
        }

        // Colors for Panels (sleek dark colors)
        Color menuColor = new Color(0.08f, 0.09f, 0.12f, 1.0f);        // Dark slate blue-gray
        Color pauseColor = new Color(0.05f, 0.05f, 0.05f, 0.85f);       // Translucent black
        Color gameOverColor = new Color(0.18f, 0.02f, 0.02f, 0.88f);    // Translucent dark red
        Color victoryColor = new Color(0.02f, 0.18f, 0.08f, 0.88f);     // Translucent dark green/emerald

        // 6. Create Main Menu Panel
        GameObject mainMenuPanel = CreatePanel("MainMenuPanel", canvasTransform, menuColor);
        CreateText("MENÚ PRINCIPAL", mainMenuPanel.transform, 48f, new Vector2(0f, 200f), Color.white);
        CreateButton("Jugar", mainMenuPanel.transform, new Vector2(0f, 20f), gameManager, "StartGame");
        CreateButton("Salir", mainMenuPanel.transform, new Vector2(0f, -60f), gameManager, "QuitGame");

        // 7. Create Pause Panel
        GameObject pausePanel = CreatePanel("PausePanel", canvasTransform, pauseColor);
        CreateText("JUEGO EN PAUSA", pausePanel.transform, 48f, new Vector2(0f, 200f), Color.white);
        CreateButton("Reanudar", pausePanel.transform, new Vector2(0f, 40f), gameManager, "ResumeGame");
        CreateButton("Reiniciar", pausePanel.transform, new Vector2(0f, -20f), gameManager, "RestartGame");
        CreateButton("Menú Principal", pausePanel.transform, new Vector2(0f, -80f), gameManager, "LoadMainMenu");

        // 8. Create Game Over Panel
        GameObject gameOverPanel = CreatePanel("GameOverPanel", canvasTransform, gameOverColor);
        CreateText("JUEGO TERMINADO", gameOverPanel.transform, 48f, new Vector2(0f, 200f), Color.red);
        CreateButton("Intentar de Nuevo", gameOverPanel.transform, new Vector2(0f, 20f), gameManager, "RestartGame");
        CreateButton("Menú Principal", gameOverPanel.transform, new Vector2(0f, -60f), gameManager, "LoadMainMenu");

        // 9. Create Victory Panel
        GameObject victoryPanel = CreatePanel("VictoryPanel", canvasTransform, victoryColor);
        CreateText("¡VICTORIA!", victoryPanel.transform, 56f, new Vector2(0f, 200f), Color.yellow);
        CreateButton("Volver a Jugar", victoryPanel.transform, new Vector2(0f, 20f), gameManager, "RestartGame");
        CreateButton("Menú Principal", victoryPanel.transform, new Vector2(0f, -60f), gameManager, "LoadMainMenu");

        // 10. Link Panels to GameManager
        gameManager.mainMenuPanel = mainMenuPanel;
        gameManager.hudPanel = hudPanel;
        gameManager.pausePanel = pausePanel;
        gameManager.gameOverPanel = gameOverPanel;
        gameManager.victoryPanel = victoryPanel;

        // Save changes in the scene
        EditorUtility.SetDirty(gameManager);
        
        Debug.Log("UIBuilder: Successfully built and wired all UI Panels!");
        EditorUtility.DisplayDialog("UI Builder", "¡Las interfaces de Menú Principal, Pausa, Derrota y Victoria han sido construidas y conectadas al GameManager con éxito!", "Aceptar");
    }

    private static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(parent, false);
        
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        
        Image img = panel.GetComponent<Image>();
        img.color = color;
        
        Undo.RegisterCreatedObjectUndo(panel, "Create " + name);
        return panel;
    }

    private static void CreateText(string textContent, Transform parent, float fontSize, Vector2 anchoredPos, Color color)
    {
        GameObject textObj = new GameObject("TitleText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(parent, false);
        
        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = textContent;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        
        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(600f, 100f);
        
        Undo.RegisterCreatedObjectUndo(textObj, "Create Text");
    }

    private static void CreateButton(string label, Transform parent, Vector2 anchoredPos, GameManager gm, string methodName)
    {
        GameObject btnObj = new GameObject("Button_" + label, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(parent, false);
        
        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(250f, 50f);
        
        Image img = btnObj.GetComponent<Image>();
        img.color = new Color(0.18f, 0.20f, 0.25f, 0.95f); // Sleek modern dark button
        
        // Add rounded effect outline if possible, or just default block
        Button btn = btnObj.GetComponent<Button>();
        
        // Button hover states / transition colors
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.18f, 0.20f, 0.25f, 0.95f);
        colors.highlightedColor = new Color(0.28f, 0.32f, 0.40f, 1.0f);
        colors.pressedColor = new Color(0.12f, 0.14f, 0.18f, 1.0f);
        btn.colors = colors;

        // Create Text child
        GameObject txtObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        txtObj.transform.SetParent(btnObj.transform, false);
        
        TextMeshProUGUI tmp = txtObj.GetComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 18f;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        
        RectTransform txtRt = txtObj.GetComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.sizeDelta = Vector2.zero;
        
        // Wire up onClick via UnityEventTools for persistent listener in editor
        System.Reflection.MethodInfo method = typeof(GameManager).GetMethod(methodName);
        if (method != null)
        {
            System.Delegate action = System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), gm, method);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, (UnityEngine.Events.UnityAction)action);
        }
        
        Undo.RegisterCreatedObjectUndo(btnObj, "Create Button " + label);
    }
}
