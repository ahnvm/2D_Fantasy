using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    [SerializeField]private Renderer objectRenderer;
    private bool isVisible = true;

    void Start()
    {
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            isVisible = !isVisible;
            objectRenderer.enabled = isVisible;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
