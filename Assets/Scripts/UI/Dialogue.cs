using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent; //text display of dialogue
    public string[] lines; //lines of dialogue
    public float textSpeed; //speed of text

    private int _index; //index of text in lines

    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        //if mousedown
        if (Input.GetMouseButtonDown(0))
        {
            //if done, go to nextline
            if (textComponent.text == lines[_index])
            {
                NextLine();
            }
            //otherwise finish current line
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[_index];
            }
        }
    }

    private void StartDialogue()
    {
        _index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        //puts in characters based on textspeed
        foreach (char c in lines[_index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void NextLine()
    {
        //if not end, go to next line
        if (_index < lines.Length - 1)
        {
            _index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        //otherwise close dialogue box
        else
        {
            gameObject.SetActive(false);
            //reset
            textComponent.text = string.Empty;
        }
    }
}