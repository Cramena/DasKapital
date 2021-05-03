using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverTextTest : MonoBehaviour
{
    public TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, Camera.main);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex]; // Get the information about the link
            // Do something based on what link ID or Link Text is encountered...
            print($"Key is: {linkInfo.GetLinkID()}");
            // if (linkInfo.GetLinkID() == "Test")
            // {
            //     print("Display popup");
            // }
        }
    }
}
