using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))] // Adicionado para garantir o AudioSource
public class DeliberationPapyrusManager : MonoBehaviour
{
    [Header("Prefabs & References")]
    public GameObject papyrusPrefab;
    public Transform stackContainer;
    public Button cycleArrowButton;

    [Header("Stack Positioning")]
    public Vector2 offsetPerItem = new Vector2(50f, 50f);
    public float animationDuration = 0.4f;
    public float slideDownDistance = 600f;
    public float slideRightDistance = 400f;

    [Header("Drag Settings")]
    public float dragScale = 0.8f;
    public Vector2 dragOffset = new Vector2(-100f, 0f);

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip slideSound; // Som do papel a arrastar

    private List<GameObject> activePapyruses = new List<GameObject>();
    private bool isAnimating = false;

    private void Awake()
    {
        // Garante que o AudioSource é atribuído
        if (audioSource == null) 
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        if (cycleArrowButton != null)
        {
            cycleArrowButton.onClick.AddListener(CycleStack);
            cycleArrowButton.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (cycleArrowButton != null)
        {
            cycleArrowButton.onClick.RemoveListener(CycleStack);
        }
    }

    public void ClearPapyruses()
    {
        foreach (var p in activePapyruses)
        {
            if (p != null) Destroy(p);
        }
        activePapyruses.Clear();
        
        if (cycleArrowButton != null)
        {
            cycleArrowButton.gameObject.SetActive(false);
        }
    }

    public void SpawnPapyruses()
    {
        // Clear existing
        ClearPapyruses();

        if (GameManager.Instance == null || GameManager.Instance.currentLevel == null) return;

        var npcs = GameManager.Instance.currentLevel.npcsInLevel;
        
        // Spawn one papyrus for each NPC
        for (int i = 0; i < npcs.Length; i++)
        {
            GameObject papyrusObj = Instantiate(papyrusPrefab, stackContainer);
            
            // Disable components that might interfere since this is a static prefab
            // We must use GetComponentsInChildren because they might be attached to child objects!
            var movers = papyrusObj.GetComponentsInChildren<PapyrusMover>(true);
            foreach (var m in movers) Destroy(m);
            
            var uis = papyrusObj.GetComponentsInChildren<PapyrusUI>(true);
            foreach (var u in uis) Destroy(u);

            // Populate the data
            TextMeshProUGUI[] texts = papyrusObj.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var txt in texts)
            {
                // Force text to be visible (in case the prefab was saved mid-animation or scripts hid it)
                txt.maxVisibleCharacters = int.MaxValue;
                
                if (txt.name.ToLower().Contains("name") || txt.name.ToLower().Contains("header"))
                {
                    txt.text = npcs[i].npcName;
                }
                else
                {
                    txt.color = Color.black; // Ensure text is black (PapyrusUI did this)
                    if (GameManager.Instance.SavedCluesByNPC.ContainsKey(npcs[i].npcName))
                    {
                        var clues = GameManager.Instance.SavedCluesByNPC[npcs[i].npcName];
                        txt.text = "";
                        foreach(var clue in clues)
                        {
                            txt.text += $"- {clue}\n\n";
                        }
                    }
                    else 
                    {
                        txt.text = "No clues recorded.";
                    }
                }
            }

            DeliberationPapyrusDrag dragger = papyrusObj.AddComponent<DeliberationPapyrusDrag>();
            dragger.Initialize(this, npcs[i]);

            activePapyruses.Add(papyrusObj);
        }

        UpdateStackPositions(false);
        
        if (cycleArrowButton != null && activePapyruses.Count > 1)
        {
            cycleArrowButton.gameObject.SetActive(true);
            cycleArrowButton.transform.SetAsLastSibling(); // ensure arrow is on top
        }
    }

    private void UpdateStackPositions(bool animated, bool ignoreLast = false)
    {
        // The first element in the list is the FRONT of the stack (index 0).
        // The last element is the BACK of the stack.
        // We render the back elements first so they are behind in the UI.
        for (int i = activePapyruses.Count - 1; i >= 0; i--)
        {
            activePapyruses[i].transform.SetAsLastSibling(); // Unity rendering order
            
            if (ignoreLast && i == activePapyruses.Count - 1)
                continue; // Skip animating the last one so we can custom animate it

            RectTransform rt = activePapyruses[i].GetComponent<RectTransform>();
            Vector2 targetPos = new Vector2(offsetPerItem.x * i, offsetPerItem.y * i);
            
            if (animated)
            {
                StartCoroutine(AnimateToPosition(rt, targetPos));
            }
            else
            {
                rt.anchoredPosition = targetPos;
            }
        }
        
        // Keep the button completely in front
        if (cycleArrowButton != null) cycleArrowButton.transform.SetAsLastSibling();
    }

    private IEnumerator AnimateToPosition(RectTransform rt, Vector2 targetPos)
    {
        Vector2 startPos = rt.anchoredPosition;
        float elapsed = 0;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / animationDuration);
            yield return null;
        }
        rt.anchoredPosition = targetPos;
    }

    private void CycleStack()
    {
        if (isAnimating || activePapyruses.Count <= 1) return;
        StartCoroutine(CycleAnimationSequence());
    }

    private IEnumerator CycleAnimationSequence()
    {
        isAnimating = true;

        // Toca o som de arrastar o papel no momento em que a animação começa
        if (audioSource != null && slideSound != null)
        {
            audioSource.clip = slideSound;
            audioSource.loop = true; // Mantém o som a tocar enquanto a animação durar
            audioSource.Play();
        }

        GameObject frontPapyrus = activePapyruses[0];
        RectTransform rt = frontPapyrus.GetComponent<RectTransform>();
        
        Vector2 startPos = rt.anchoredPosition;
        Vector2 rightPos = new Vector2(startPos.x + slideRightDistance, startPos.y);

        // 1. Slide out to the right
        float elapsed = 0;
        while (elapsed < animationDuration / 2)
        {
            elapsed += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(startPos, rightPos, elapsed / (animationDuration / 2));
            yield return null;
        }

        // Move to the back of the list logically
        activePapyruses.RemoveAt(0);
        activePapyruses.Add(frontPapyrus);
        
        // Update siblings to visually put it in the back
        UpdateStackPositions(true, true); 

        // 2. Slide into the back position strictly horizontally
        Vector2 finalPos = new Vector2(offsetPerItem.x * (activePapyruses.Count - 1), offsetPerItem.y * (activePapyruses.Count - 1));
        Vector2 backRightPos = new Vector2(finalPos.x + slideRightDistance, finalPos.y);
        
        rt.anchoredPosition = backRightPos;
        
        elapsed = 0;
        while (elapsed < animationDuration / 2)
        {
            elapsed += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(backRightPos, finalPos, elapsed / (animationDuration / 2));
            yield return null;
        }
        
        rt.anchoredPosition = finalPos;

        // Para o som assim que o papel chegar ao destino
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        isAnimating = false;
    }

    public bool IsFrontPapyrus(GameObject papyrus)
    {
        return activePapyruses.Count > 0 && activePapyruses[0] == papyrus;
    }

    public bool IsAnimating
    {
        get { return isAnimating; }
    }

    public void ForceUpdatePositions()
    {
        UpdateStackPositions(false);
    }

    public float GetDragScale()
    {
        return dragScale;
    }

    public Vector2 GetDragOffset()
    {
        return dragOffset;
    }
}