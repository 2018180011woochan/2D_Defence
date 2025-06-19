using UnityEngine;
using UnityEngine.EventSystems;

public class CloseOnClickOutside : MonoBehaviour
{
    public RectTransform boardRect;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && boardRect.gameObject.activeSelf)
        {

            if (EventSystem.current.IsPointerOverGameObject()) return;


            if (!RectTransformUtility.RectangleContainsScreenPoint(
                  boardRect, Input.mousePosition, null))
            {

                boardRect.gameObject.SetActive(false);
            }
        }
    }
}