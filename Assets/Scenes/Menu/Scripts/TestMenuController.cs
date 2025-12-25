using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestMenuController : MonoBehaviour
{
    [SerializeField] private ButtonObject[] buttons;

    private IEnumerator Start()
    {
        var currentIndex = 0;
        buttons[currentIndex].Select(new Vector2Int(0, 0));

        while (true)
        {
            var thisIndex = currentIndex;

            while (thisIndex == currentIndex)
            {
                if (Keyboard.current.wKey.wasPressedThisFrame)
                    thisIndex--;
                if (Keyboard.current.sKey.wasPressedThisFrame)
                    thisIndex++;

                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                    buttons[currentIndex].Click();

                Debug.Log(thisIndex);

                thisIndex = Mathf.Clamp(thisIndex, 0, buttons.Length - 1);

                yield return 0;
            }

            buttons[currentIndex].Deselect();
            buttons[thisIndex].Select(new Vector2Int(0, thisIndex - currentIndex));
            currentIndex = thisIndex;
        }
    }
}
