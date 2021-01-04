using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour
{
    public RectTransform buttonMask;
    public RectTransform toggleMask;
    public Transform tempParent;
    public Transform[] shapeButtons;
    public Transform[] toggleButtons;
    public RectTransform buttonPanel;
    public RectTransform togglePanel;
    private bool panelOpened;
    private bool panelStateOpen = false;
    private bool panelOpeningA = false;
    public float openTimeA = 0.5f;
    public float openTimeB = 0.5f;
    private Coroutine changingPanelCo;
    private Coroutine changingButtonsCo;
    private Coroutine changingTogglesCo;
    private Vector3 maskPosOpen;
    private Vector3 maskPosClosed;
    private Vector3 maskTogPosOpen;
    private Vector3 maskTogPosClosed;
    private int lastButton = 0;
    public ShapeTracker shapeScript;

    public GameObject tog_TreeSilhouettes;
    public GameObject tog_Particles;
    public ColorRandomizer colorScript;
    private bool[] toggleStates = new bool[]{ true, true, true, true };
    public Color buttonUpColor;
    public Color buttonDownColor;

    private void Start()
    {
        SetMenusClosed();

        // Start with hex shape
        shapeButtons[0].GetComponent<Button>().interactable = false;
        lastButton = 0;

        // Set toggle buttons to down state colors
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            var buttonColors = toggleButtons[i].GetComponent<Button>().colors;
            buttonColors.normalColor = buttonDownColor;
            buttonColors.selectedColor = buttonDownColor;
            toggleButtons[i].GetComponent<Button>().colors = buttonColors;
        }
    }

    private void SetMenusClosed()
    {
        // Start with menu panels closed
        buttonPanel.localScale = new Vector3(0.25f, 0, 1);
        togglePanel.localScale = new Vector3(0, 1, 1);
        maskPosOpen = buttonMask.anchoredPosition;
        maskPosClosed = new Vector3(maskPosOpen.x - buttonMask.rect.width, maskPosOpen.y, maskPosOpen.z);
        maskTogPosOpen = toggleMask.anchoredPosition;
        maskTogPosClosed = new Vector3(maskTogPosOpen.x - toggleMask.rect.width, maskTogPosOpen.y, maskTogPosOpen.z);

        // Unparent
        for (int i = 0; i < shapeButtons.Length; i++)
        {
            shapeButtons[i].SetParent(tempParent);
        }
        // Move mask
        buttonMask.anchoredPosition = maskPosClosed;
        // Reparent
        for (int i = 0; i < shapeButtons.Length; i++)
        {
            shapeButtons[i].SetParent(buttonMask.transform);
        }

        // Unparent
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            toggleButtons[i].SetParent(tempParent);
        }
        // Move mask
        toggleMask.anchoredPosition = maskTogPosClosed;
        // Reparent
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            toggleButtons[i].SetParent(toggleMask.transform);
        }
    }

    public void MenuButtonPressed()
    {
        if (changingPanelCo != null) StopCoroutine(changingPanelCo);
        panelStateOpen = !panelStateOpen;
        changingPanelCo = StartCoroutine(ChangingPanel(panelStateOpen));
    }

    public void ShapeButtonPressed(int whichButton)
    {
        shapeButtons[lastButton].GetComponent<Button>().interactable = true;
        shapeButtons[whichButton].GetComponent<Button>().interactable = false;
        lastButton = whichButton;

        shapeScript.SwitchShape(whichButton);
    }

    public void ToggleButtonPressed(int whichButton)
    {
        toggleStates[whichButton] = !toggleStates[whichButton];

        var buttonColors = toggleButtons[whichButton].GetComponent<Button>().colors;
        buttonColors.normalColor = toggleStates[whichButton] ? buttonDownColor : buttonUpColor;
        buttonColors.selectedColor = toggleStates[whichButton] ? buttonDownColor : buttonUpColor;
        toggleButtons[whichButton].GetComponent<Button>().colors = buttonColors;

        switch (whichButton)
        { 
            // Tree silhouette
            case 0:
                tog_TreeSilhouettes.SetActive(toggleStates[0]);
                break;
            // Atmospheric Particles
            case 1:
                tog_Particles.SetActive(toggleStates[1]);
                break;
            // Click Burst
            case 2:
                colorScript.particleBurstOnClick = toggleStates[2];
                break;
            // Random Colors
            case 3:
                colorScript.randomColors = toggleStates[3];
                break;
        }
    }

    public void ExitButtonPressed()
    {
        Application.Quit();
    }

    IEnumerator ChangingPanel(bool opening)
    {
        Vector3 panelStartScale = buttonPanel.localScale;
        Vector3 panelStageOne = panelOpeningA ? panelStartScale : new Vector3(0.25f, 1, 1);
        Vector3 panelStageTwo = opening ? Vector3.one : new Vector3 (0.25f, 0, 1);
        Vector3 sidePanelStartScale = togglePanel.localScale;
        Vector3 sidePanelEndScale = opening ? Vector3.one : new Vector3(0, 1, 1);
        bool ranButtonChange = false;

        float t = 0;

        // If in the middle of first stage opening, skip first stage
        if (panelOpeningA) t = 1;
           
        panelOpeningA = true;

        // Lower panel
        while (t < 1)
        {
            t += Mathf.Clamp(Time.deltaTime / openTimeA, 0, 1);
            float tEased = Mathf.SmoothStep(0, 1, t);

            if (!opening && !ranButtonChange)
            {
                if (changingButtonsCo != null) StopCoroutine(changingButtonsCo);
                changingButtonsCo = StartCoroutine(MaskingButtons(false));
                if (changingTogglesCo != null) StopCoroutine(changingTogglesCo);
                changingTogglesCo = StartCoroutine(MaskingToggles(false));
                ranButtonChange = true;
            }
            buttonPanel.localScale = Vector3.Lerp(panelStartScale, panelStageOne, tEased);
            if (opening) togglePanel.localScale = Vector3.Lerp(sidePanelStartScale, sidePanelEndScale, tEased);
            yield return null;
        }

        panelOpeningA = false;

        t = 0;
        // Open panel
        while (t < 1)
        {
            t = Mathf.Clamp(t + (Time.deltaTime / openTimeB), 0, 1);
            float tEased = Mathf.SmoothStep(0, 1, t);

            if (t > 0.5f && opening && !ranButtonChange)
            {
                if (changingButtonsCo != null) StopCoroutine(changingButtonsCo);
                changingButtonsCo = StartCoroutine(MaskingButtons(true));
                if (changingTogglesCo != null) StopCoroutine(changingTogglesCo);
                changingTogglesCo = StartCoroutine(MaskingToggles(true));
                ranButtonChange = true;
            }

            buttonPanel.localScale = Vector3.Lerp(panelStageOne, panelStageTwo, tEased);
            if (!opening) togglePanel.localScale = Vector3.Lerp(sidePanelStartScale, sidePanelEndScale, tEased);
            yield return null;
        }
    }

    IEnumerator MaskingButtons(bool opening)
    {
        Vector3 maskStartPos = buttonMask.anchoredPosition;
        Vector3 maskEndPos = opening ? maskPosOpen : maskPosClosed;

        float t = 0;

        while (t < 1)
        {
            t = Mathf.Clamp(t + (Time.deltaTime / (openTimeB / 2)), 0, 1);
            float tEased = Mathf.SmoothStep(0, 1, t);

            Vector3[] startPos = new Vector3[shapeButtons.Length];
            // Unparent
            for (int i = 0; i < shapeButtons.Length; i++)
            {
                shapeButtons[i].SetParent (tempParent);
            }
            // Move mask
            buttonMask.anchoredPosition = Vector3.Lerp(maskStartPos, maskEndPos, tEased);
            // Reparent
            for (int i = 0; i < shapeButtons.Length; i++)
            {
                shapeButtons[i].SetParent (buttonMask.transform);
            }

            yield return null;
        }
    }
    IEnumerator MaskingToggles(bool opening)
    {
        Vector3 maskTogStartPos = toggleMask.anchoredPosition;
        Vector3 maskTogEndPos = opening ? maskTogPosOpen : maskTogPosClosed;

        float t = 0;

        while (t < 1)
        {
            t = Mathf.Clamp(t + (Time.deltaTime / (openTimeB / 2)), 0, 1);
            float tEased = Mathf.SmoothStep(0, 1, t);

            Vector3[] startPos = new Vector3[toggleButtons.Length];
            // Unparent
            for (int i = 0; i < toggleButtons.Length; i++)
            {
                toggleButtons[i].SetParent(tempParent);
            }
            // Move mask
            toggleMask.anchoredPosition = Vector3.Lerp(maskTogStartPos, maskTogEndPos, tEased);
            // Reparent
            for (int i = 0; i < shapeButtons.Length; i++)
            {
                toggleButtons[i].SetParent(toggleMask.transform);
            }

            yield return null;
        }
    }
}
