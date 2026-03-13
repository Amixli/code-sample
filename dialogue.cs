using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

using UnityEngine;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour
{

    public List<DialogueEntry> entries;
    public UnityEvent onDialogueEnd;

    public void StartDialogue()
    {
        if (entries.Count == 0)
        {
            Debug.Log("Can't show dialogue because it has no entries", this);
            return;
        }
        
        FindObjectOfType<GameManager>().StartDialogue(this);
    }
}

[Serializable]
public class DialogueEntry
{
    public LocalizedString speaker;
    public LocalizedString text;
    //TODO add Sprite speakerImage

    public List<SelectionEntry> selectionOptions;
}

[Serializable]
public class LocalizedString
{
    [TextArea(1,3)]
    [SerializeField] private string englishText;
    [TextArea(1,3)]
    [SerializeField] private string germanText;

    public string GetText(bool german)
    {
        if (german)
        {
            return germanText;
        }
        else
        {
            return englishText;
        }
    }
}

[Serializable]
public class SelectionEntry
{
    public string selectionText;
    public string selectionTextGerman;
    public UnityEvent onSelected;
    public Dialogue nextDialogue;
    
    public string GetText(bool german)
    {
        if (german)
        {
            return selectionTextGerman;
        }
        else
        {
            return selectionText;
        }
    }
}
