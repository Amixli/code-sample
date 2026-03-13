using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class DialogueBox : MonoBehaviour
{
    public static event Action<DialogueBox> ContinuingDialogue; 

    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button selectionButtonPrefab;
    [SerializeField] private Transform selectionParent;

    //[SerializeField] private Image background;
    //[SerializeField] private Sprite thelmaBackgroundSprite;
    // TODO add Image speakerPortrait here

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Sequence slideInOutSequence;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        continueButton.onClick.AddListener(ContinueDialogue);
    }

    public void DisplayDialogueEntry(DialogueEntry dialogueEntry, bool inGerman)
    {
        speakerText.SetText(dialogueEntry.speaker.GetText(inGerman));
        dialogueText.SetText(dialogueEntry.text.GetText(inGerman));

       // switch (dialogueEntry.speaker)
       // {
         //   case "Thelma":
         //       Background.overrideSprite = thelmaBackgroundSprite;
         //      break;
         //   default:
         //       Background.overrideSprite = oldJoeBackgroundSprite;
       // }
         
        ClearSelection();
        TryAddSelections(dialogueEntry.selectionOptions, inGerman);
    }

    private void ShowContinueButton(bool show)
    {
        GameObject continueButtonObject = continueButton.gameObject;
        continueButtonObject.SetActive(show);
    }

    private IEnumerator SelectWithDelayRoutine(GameObject toSelect)
    {
        yield return null; // Waits for one rendering frame
        EventSystem.current.SetSelectedGameObject(toSelect);
    }

    private void ContinueDialogue()
    {
        if (ContinuingDialogue != null)
        {
            ContinuingDialogue.Invoke(this);
        }
    }

    private void ClearSelection()
    {
        foreach (Transform child in selectionParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void TryAddSelections(List<SelectionEntry> selectionOptions, bool inGerman)
    {
        GameObject toSelect = null;

        if (selectionOptions.Count == 0)
        {
            ShowContinueButton(true);
            toSelect = continueButton.gameObject;
        }
        else
        {
            ShowContinueButton(false);

            foreach (SelectionEntry selectionEntry in selectionOptions)
            {
                Button selectionButton = Instantiate(selectionButtonPrefab, selectionParent);
                TextMeshProUGUI buttonText = selectionButton.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.SetText(selectionEntry.GetText(inGerman));
                
                selectionButton.onClick.AddListener(selectionEntry.onSelected.Invoke);

                if (selectionEntry.nextDialogue != null)
                {
                    selectionButton.onClick.AddListener(selectionEntry.nextDialogue.StartDialogue);
                }
                else
                {
                    selectionButton.onClick.AddListener(ContinueDialogue);
                }

                if (toSelect == null)
                {
                    toSelect = selectionButton.gameObject;
                }
            }
        }

        StartCoroutine(SelectWithDelayRoutine(toSelect));
    }

    #region Animations

    public Tween DOShow()
    {
        float height = rectTransform.rect.height;
        
        slideInOutSequence.Kill();
        slideInOutSequence = DOTween.Sequence()
                                    .Append(DOMove(Vector2.zero).From(new Vector2(0f, -height)))
                                    .Join(DOFade(1f).From(0f));
        
        return slideInOutSequence;
    }

    public Tween DOHide()
    {
        float height = rectTransform.rect.height;
        
        slideInOutSequence.Kill();
        slideInOutSequence = DOTween.Sequence()
                                    .Append(DOMove(new Vector2(0f, -height)).From(Vector2.zero))
                                    .Join(DOFade(0f).From(1f));
        
        return slideInOutSequence;
    }

    private TweenerCore<Vector2, Vector2, VectorOptions> DOMove(Vector2 targetPosition)
    {
        return rectTransform.DOAnchorPos(targetPosition, 0.5f).SetEase(Ease.InOutBack);
    }
    private TweenerCore<float, float, FloatOptions> DOFade(float targetAlpha)
    {
        return canvasGroup.DOFade(targetAlpha, 0.5f).SetEase(Ease.InOutSine);
    }

    #endregion
}
