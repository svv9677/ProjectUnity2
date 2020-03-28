using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChipUI : MonoBehaviour {

    public Image theImage;

    public void SetColor(Color color) 
    {
        if(this.theImage != null)
        {
            this.theImage.color = color;
        }
    }
}
