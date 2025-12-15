using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardViewerUI : MonoBehaviour
{
    public void WaitForZoomOutButton(Action onLetGo)
    {
        StopAllCoroutines();

        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            while (true)
            {
                if (Keyboard.current.qKey.wasPressedThisFrame)
                    Cameraman.ZoomOut();

                if (Keyboard.current.eKey.wasPressedThisFrame)
                    onLetGo();

                yield return 0;
            }
        }
    }
}
