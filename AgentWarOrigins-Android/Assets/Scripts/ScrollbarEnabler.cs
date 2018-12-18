using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarEnabler : MonoBehaviour
{
    [SerializeField]
    RectTransform container;
    [SerializeField]
    RectTransform content;
    [SerializeField]
    Scrollbar scrollbar;

    bool enableScrollbar = false;

    void Update()
    {
        if (enableScrollbar != scrollbar.gameObject.activeSelf)
            scrollbar.gameObject.SetActive(enableScrollbar);
    }

    void OnRectTransformDimensionsChange()
    {
        enableScrollbar = container.rect.height < content.rect.height;
    }
}