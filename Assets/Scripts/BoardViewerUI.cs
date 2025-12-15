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
                {
                    BoardTime.Modifier = 0;
                    Cameraman.BeholdBoard();
                }

                if (Keyboard.current.qKey.wasReleasedThisFrame)
                {
                    BoardTime.Modifier = 1;
                    onLetGo();
                }

                yield return 0;
            }
        }
    }
}
