using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Overlay tutorial that sits on top of the live GameScene UI. Built programmatically so no
// scene or prefab wiring is required (mirrors GameManager.EnsurePrototypeUI). Two modes:
//   - Splash: launched from SplashScreen; Close returns to SplashScreen.
//   - Encounter: opened mid-game; Close just hides the overlay (battle state preserved).
//
// Callouts anchor to live HUD elements via TutorialCallout so they follow whatever the
// Canvas Scaler does at different resolutions.
public class TutorialPanel : MonoBehaviour
{
    public enum PanelMode { Splash, Encounter }
    private enum ViewKind { Core, Archetype }

    private Canvas sceneCanvas;
    private GameManager gm;
    private GameObject overlayRoot;
    private GameObject corePage;
    private GameObject archetypePage;
    private Button toggleViewButton;
    private TextMeshProUGUI toggleViewLabel;
    private Button hudTutorialButton;
    private TextMeshProUGUI hudTutorialButtonLabel;

    private PanelMode mode;
    private ViewKind view;
    private MasterGameManager.PrototypeMode archetypeMode;

    public static TutorialPanel Create(Canvas canvas, GameManager gameManager, MasterGameManager.PrototypeMode archetype, PanelMode panelMode)
    {
        GameObject host = new GameObject("TutorialPanel");
        host.transform.SetParent(canvas.transform, false);
        RectTransform hostRt = host.AddComponent<RectTransform>();
        hostRt.anchorMin = Vector2.zero;
        hostRt.anchorMax = Vector2.one;
        hostRt.sizeDelta = Vector2.zero;
        hostRt.anchoredPosition = Vector2.zero;

        TutorialPanel panel = host.AddComponent<TutorialPanel>();
        panel.sceneCanvas = canvas;
        panel.gm = gameManager;
        panel.mode = panelMode;
        panel.archetypeMode = archetype;
        panel.Build();
        panel.AutoOpen();
        return panel;
    }

    private void Build()
    {
        overlayRoot = BuildOverlayRoot(transform);
        corePage = BuildCorePage(overlayRoot.transform);
        archetypePage = BuildArchetypePage(overlayRoot.transform, archetypeMode);

        toggleViewButton = BuildBottomButton(overlayRoot.transform, "View Core Tutorial", new Vector2(-190, 20));
        toggleViewButton.onClick.AddListener(ToggleView);
        toggleViewLabel = toggleViewButton.GetComponentInChildren<TextMeshProUGUI>();

        hudTutorialButton = BuildHudTutorialButton(transform);
        hudTutorialButton.onClick.AddListener(OnHudButtonClicked);
        hudTutorialButtonLabel = hudTutorialButton.GetComponentInChildren<TextMeshProUGUI>();

        BuildHudExitButton(transform);
    }

    public void AutoOpen()
    {
        bool splash = mode == PanelMode.Splash;
        bool coreLoopEncounter = mode == PanelMode.Encounter
            && archetypeMode == MasterGameManager.PrototypeMode.CoreLoop;

        view = (splash || coreLoopEncounter) ? ViewKind.Core : ViewKind.Archetype;
        toggleViewButton.gameObject.SetActive(!splash && !coreLoopEncounter);

        overlayRoot.SetActive(true);
        ApplyView();
        SetHudLabel(true);
    }

    public void OpenHud()
    {
        if (mode == PanelMode.Encounter
            && archetypeMode != MasterGameManager.PrototypeMode.CoreLoop)
        {
            view = ViewKind.Archetype;
        }
        overlayRoot.SetActive(true);
        ApplyView();
        SetHudLabel(true);
    }

    public void Close()
    {
        if (mode == PanelMode.Splash)
        {
            MasterGameManager.isTutorialLaunch = false;
            SceneManager.LoadScene("SplashScreen");
            return;
        }
        overlayRoot.SetActive(false);
        SetHudLabel(false);
    }

    public void ToggleView()
    {
        if (mode == PanelMode.Splash) { return; }
        if (archetypeMode == MasterGameManager.PrototypeMode.CoreLoop) { return; }
        view = view == ViewKind.Core ? ViewKind.Archetype : ViewKind.Core;
        ApplyView();
    }

    private void OnHudButtonClicked()
    {
        if (overlayRoot.activeSelf) { Close(); } else { OpenHud(); }
    }

    private void ApplyView()
    {
        bool onCore = view == ViewKind.Core;
        corePage.SetActive(onCore);
        archetypePage.SetActive(!onCore);
        if (toggleViewLabel != null)
        {
            toggleViewLabel.text = onCore ? "View Encounter Tutorial" : "View Core Tutorial";
        }
    }

    private void SetHudLabel(bool open)
    {
        if (hudTutorialButtonLabel != null)
        {
            hudTutorialButtonLabel.text = open ? "Close" : "Tutorial";
        }
    }

    // ---------- UI construction ----------

    private GameObject BuildOverlayRoot(Transform parent)
    {
        GameObject root = new GameObject("TutorialOverlay");
        root.transform.SetParent(parent, false);
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        // Own sort order so we render above WinPanel / GameOverPanel / DeckView.
        Canvas overlay = root.AddComponent<Canvas>();
        overlay.overrideSorting = true;
        overlay.sortingOrder = 100;
        root.AddComponent<GraphicRaycaster>();

        // Raycast-blocking semi-transparent backdrop (prevents swiping cards through).
        GameObject backdrop = new GameObject("Backdrop");
        backdrop.transform.SetParent(root.transform, false);
        RectTransform bdRt = backdrop.AddComponent<RectTransform>();
        bdRt.anchorMin = Vector2.zero;
        bdRt.anchorMax = Vector2.one;
        bdRt.sizeDelta = Vector2.zero;
        bdRt.anchoredPosition = Vector2.zero;
        Image bdImg = backdrop.AddComponent<Image>();
        bdImg.color = new Color(0f, 0f, 0f, 0.55f);

        return root;
    }

    private GameObject BuildCorePage(Transform parent)
    {
        GameObject page = BuildPageContainer(parent, "CorePage");
        AddTitle(page, "Core Loop — How to play", yOffset: -5);

        RectTransform enemyStatus = gm != null && gm.enemyStatusText != null ? gm.enemyStatusText.rectTransform : null;
        RectTransform viewDeck = FindUiRect("ViewDeckButton");

        // Drop zones + card are 3D world-space GameObjects, not UI — project via Camera.
        // The active card comes from GameManager (the scene "Card" placeholder was destroyed
        // in GameManager.Start() and replaced with a deck card whose GameObject name differs).
        Transform leftZone = FindWorldTransform("DropZoneLeft");
        Transform rightZone = FindWorldTransform("DropZoneRight");
        Transform card = gm != null && gm.currentCard != null ? gm.currentCard.transform : null;

        // Put HP labels INSIDE the drop zones (plenty of empty space) with up-arrows
        // so the player's eye traces from the arrow inside the zone up to the HP text.
        AddWorldCallout(page, "↑ Your HP", leftZone, new Vector2(0, 160), width: 220, fontSize: 22);
        AddWorldCallout(page, "← LEFT\nplays the card's LEFT action", leftZone, new Vector2(0, 40), width: 260, fontSize: 22);

        AddWorldCallout(page, "↑ Enemy HP", rightZone, new Vector2(0, 160), width: 220, fontSize: 22);
        AddWorldCallout(page, "RIGHT →\nplays the card's RIGHT action", rightZone, new Vector2(0, 40), width: 260, fontSize: 22);

        // Enemy Intent text sits top-right — put callout BELOW it and NUDGED LEFT so the
        // 320-wide box doesn't clip the right edge of the canvas (Enemy Status is anchored
        // right-of-center at x=1155 on a 1280-wide canvas; centered 320px would span to
        // x=1315, clipping by 35px).
        AddCallout(page, "↑ Enemy Intent\nacts when the countdown hits 0",
            enemyStatus, new Vector2(-60, -60), width: 320, fontSize: 20);

        // Current card callout sits well above the card (card is at world-Y ≈ 5; 220px
        // clears the card art and sits just under the title row).
        AddWorldCallout(page, "Current Card ↓ — drag left or right", card, new Vector2(0, 160), width: 420, fontSize: 22);

        // View Deck button is at bottom-center; callout to its LEFT (below-would-clip).
        AddCallout(page, "View Deck ↑\ninspect deck + discard",
            viewDeck, new Vector2(0, -40), width: 260, fontSize: 20);

        // Basics strip — sits below the drop zones, above the View Deck row.
        AddCallout(page, "Attack deals damage to enemy HP.\nShield blocks enemy attacks — all shield is lost after the enemy acts.",
            null, new Vector2(0, -165), width: 720, fontSize: 20, centerOnScreen: true);

        return page;
    }

    private GameObject BuildArchetypePage(Transform parent, MasterGameManager.PrototypeMode m)
    {
        GameObject page = BuildPageContainer(parent, "ArchetypePage_" + m);

        RectTransform statusRt = gm != null && gm.prototypeStatusText != null ? gm.prototypeStatusText.rectTransform : null;
        RectTransform endTurnRt = gm != null && gm.endTurnButton != null ? gm.endTurnButton.GetComponent<RectTransform>() : null;
        RectTransform enemyStatus = gm != null && gm.enemyStatusText != null ? gm.enemyStatusText.rectTransform : null;

        switch (m)
        {
            case MasterGameManager.PrototypeMode.CoreLoop:
                AddTitle(page, "Core Encounter", yOffset: -5);
                AddCallout(page, "This encounter uses only the core loop — no extra twists.",
                    null, new Vector2(0, -200), width: 640, fontSize: 26, centerOnScreen: true);
                break;

            case MasterGameManager.PrototypeMode.EnragedBoss:
                AddTitle(page, "Enraged Boss — beware the rage meter", yOffset: -5);
                AddCallout(page, "↑ Rage fills as you play cards this turn",
                    statusRt, new Vector2(0, -100), width: 340, fontSize: 22);
                AddCallout(page, "5 cards in a turn → the boss INTERRUPTS and hits harder.\nUse Brace to yield before rage fills.",
                    null, new Vector2(0, -200), width: 600, fontSize: 22, centerOnScreen: true);
                AddCallout(page, "↑ Brace — end your turn voluntarily",
                    endTurnRt, new Vector2(120, -30), width: 320, fontSize: 20);
                break;

            case MasterGameManager.PrototypeMode.RequeueEnemy:
                AddTitle(page, "Requeue Enemy — the deck reshuffles against you", yOffset: -5);
                AddCallout(page, "When the enemy acts, your discard slams back into the deck:\n← LEFT swipes go to the TOP (you see them again soon)\n→ RIGHT swipes go to the BOTTOM (pushed away)",
                    null, new Vector2(0, -200), width: 720, fontSize: 22, centerOnScreen: true);
                break;

            case MasterGameManager.PrototypeMode.Infiltrator:
                AddTitle(page, "Infiltrator — traps in the deck", yOffset: -5);
                AddCallout(page, "The enemy's intent is compound ↑\nWill attack AND plant a trap when the countdown fires",
                    enemyStatus, new Vector2(-60, -50), width: 360, fontSize: 20);
                AddCallout(page, "Playing a trap (either side) hits you for 5 damage, reduced by shield.\nTraps accumulate across intent cycles.",
                    null, new Vector2(0, -200), width: 640, fontSize: 22, centerOnScreen: true);
                break;

            case MasterGameManager.PrototypeMode.Mirror:
                AddTitle(page, "Mirror — rhythm over variety", yOffset: -5);
                AddCallout(page, "↑ Chain tracks consecutive same-direction swipes",
                    statusRt, new Vector2(0, -70), width: 360, fontSize: 22);
                AddCallout(page, "3 in a row → +5 Shield bonus.\nBreak an established chain → enemy's next attack gains +3 damage.",
                    null, new Vector2(0, -200), width: 640, fontSize: 22, centerOnScreen: true);
                break;

            default:
                AddTitle(page, m.ToString(), yOffset: -5);
                AddCallout(page, "Tutorial content coming soon.",
                    null, new Vector2(0, -200), width: 480, fontSize: 24, centerOnScreen: true);
                break;
        }
        return page;
    }

    private GameObject BuildPageContainer(Transform parent, string name)
    {
        GameObject page = new GameObject(name);
        page.transform.SetParent(parent, false);
        RectTransform rt = page.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        return page;
    }

    private void AddTitle(GameObject parent, string text, float yOffset)
    {
        GameObject obj = new GameObject("Title");
        obj.transform.SetParent(parent.transform, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = new Vector2(0, yOffset);
        rt.sizeDelta = new Vector2(900, 60);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 29;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1f, 0.95f, 0.6f, 1f);
        tmp.fontStyle = FontStyles.Bold;
    }

    // Adds a callout that tracks a UI RectTransform (or falls back to centered on screen
    // if target is null and centerOnScreen is true — used for archetype summary text).
    private void AddCallout(GameObject parent, string text, RectTransform target, Vector2 pixelOffset, float width, int fontSize, bool centerOnScreen = false)
    {
        GameObject obj = BuildCalloutBase(parent, text, width, fontSize);
        RectTransform rt = obj.GetComponent<RectTransform>();

        if (target != null)
        {
            TutorialCallout follow = obj.AddComponent<TutorialCallout>();
            follow.uiTarget = target;
            follow.pixelOffset = pixelOffset;
        }
        else if (centerOnScreen)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pixelOffset;
        }
    }

    private void AddWorldCallout(GameObject parent, string text, Transform target, Vector2 pixelOffset, float width, int fontSize)
    {
        GameObject obj = BuildCalloutBase(parent, text, width, fontSize);
        if (target != null)
        {
            TutorialCallout follow = obj.AddComponent<TutorialCallout>();
            follow.worldTarget = target;
            follow.worldCamera = Camera.main;
            follow.pixelOffset = pixelOffset;
        }
        else
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pixelOffset;
        }
    }

    private GameObject BuildCalloutBase(GameObject parent, string text, float width, int fontSize)
    {
        GameObject obj = new GameObject("Callout");
        obj.transform.SetParent(parent.transform, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(width, 100);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        return obj;
    }

    // Bottom-right anchored to sit next to the HUD Tutorial/Close button. Caller passes
    // an anchoredPosition relative to the parent's bottom-right corner.
    private Button BuildBottomButton(Transform parent, string label, Vector2 anchoredPos)
    {
        GameObject obj = new GameObject("ToggleViewButton");
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.pivot = new Vector2(1, 0);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(220, 44);
        Image img = obj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.5f, 0.8f, 1f);
        Button btn = obj.AddComponent<Button>();
        btn.targetGraphic = img;
        AddLabel(obj, label, 18, Color.white);
        return btn;
    }

    // HUD Tutorial button sits OUTSIDE the overlayRoot on its own Canvas with a higher
    // sortingOrder so it remains visible and clickable even while the backdrop is
    // showing — clicking it while the overlay is open acts as Close.
    private Button BuildHudTutorialButton(Transform parent)
    {
        GameObject obj = new GameObject("TutorialHudButton");
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.pivot = new Vector2(1, 0);
        rt.anchoredPosition = new Vector2(-20, 20);
        rt.sizeDelta = new Vector2(160, 44);

        Canvas c = obj.AddComponent<Canvas>();
        c.overrideSorting = true;
        c.sortingOrder = 101;
        obj.AddComponent<GraphicRaycaster>();

        Image img = obj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.5f, 0.8f, 1f);
        Button btn = obj.AddComponent<Button>();
        btn.targetGraphic = img;
        AddLabel(obj, "Tutorial", 20, Color.white);
        return btn;
    }

    // Mirrors the HUD Tutorial button on the bottom-LEFT. Returns to PrototypeSelectScene
    // and destroys MasterGameManager so the next run starts fresh (same semantics as
    // RetryButton.BackToMenu / Restart.RestartGame).
    private Button BuildHudExitButton(Transform parent)
    {
        GameObject obj = new GameObject("ExitHudButton");
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0, 0);
        rt.anchoredPosition = new Vector2(20, 20);
        rt.sizeDelta = new Vector2(160, 44);

        Canvas c = obj.AddComponent<Canvas>();
        c.overrideSorting = true;
        c.sortingOrder = 101;
        obj.AddComponent<GraphicRaycaster>();

        Image img = obj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.5f, 0.8f, 1f);
        Button btn = obj.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(OnExitButtonClicked);
        AddLabel(obj, "Exit", 20, Color.white);
        return btn;
    }

    private void OnExitButtonClicked()
    {
        if (MasterGameManager.instance != null)
        {
            Destroy(MasterGameManager.instance.gameObject);
        }
        MasterGameManager.isTutorialLaunch = false;
        SceneManager.LoadScene("PrototypeSelectScene");
    }

    private void AddLabel(GameObject parent, string label, int fontSize, Color color)
    {
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(parent.transform, false);
        RectTransform rt = textObj.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color;
    }

    // ---------- Scene lookup helpers ----------

    private static RectTransform FindUiRect(string name)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) { return null; }
        return go.GetComponent<RectTransform>();
    }

    private static Transform FindWorldTransform(string name)
    {
        GameObject go = GameObject.Find(name);
        return go != null ? go.transform : null;
    }
}
