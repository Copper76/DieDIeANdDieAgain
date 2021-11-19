using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    public GameObject text;

    public GameObject panel;

    public void show()
    {
        if (panel == null)
        {
            panel = new GameObject();
            RectTransform rectTransform = panel.AddComponent<RectTransform>();
            RectTransform parentRectTransform = text.GetComponent<RectTransform>();
            rectTransform.SetParent(parentRectTransform);
            rectTransform.anchoredPosition = rectTransform.parent.localPosition;
            
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1.6f, 1.6f);
            
            rectTransform.sizeDelta = new Vector2(1.0f, 1.0f);

            rectTransform.localPosition = new Vector3(0, 0, 0);
            //rectTransform.localScale = new Vector3(1000,500);
            
            
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color32(255, 54, 255, 65);
            bg.rectTransform.sizeDelta = new Vector2(2.0f, 2.0f);
            panel.layer = LayerMask.NameToLayer("UI");
            bg.maskable = true;
        }
        panel.SetActive(true);
        text.SetActive(true);
    }

    public void hide()
    {
        text.SetActive(false);
        panel.SetActive(false);
    }
}
