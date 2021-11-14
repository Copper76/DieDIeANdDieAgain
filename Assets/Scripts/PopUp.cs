using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    public GameObject text;

    public void show()
    {
        text.SetActive(true);
    }

    public void hide()
    {
        text.SetActive(false);
    }
}
