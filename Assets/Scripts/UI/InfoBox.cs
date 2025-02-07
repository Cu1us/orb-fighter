using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoBox : MonoBehaviour, IPointerClickHandler, IDeselectHandler
{
    protected static InfoBox Instance;

    [SerializeField] RectTransform Container;
    [SerializeField] TextMeshProUGUI Title;
    [SerializeField] TextMeshProUGUI Description;
    bool isVisible;
    public static Guid currentBoxGuid { get; private set; }

    void Awake()
    {
        if (!Instance) Instance = this;
    }

    public static Guid Show(string title, string description, Vector2 anchoredPosition, Vector2 pivot)
    {
        if (!Instance) return Guid.Empty;
        currentBoxGuid = Guid.NewGuid();
        SetText(title, description);
        SetPosition(anchoredPosition, pivot);
        SetVisible(true);
        MarkSelected();
        return currentBoxGuid;
    }
    public static Guid Show(string title, string description, Vector2 anchoredPosition)
    {
        if (!Instance) return Guid.Empty;
        currentBoxGuid = Guid.NewGuid();
        SetText(title, description);
        SetPosition(anchoredPosition, Vector2.one);
        SetVisible(true);
        MarkSelected();
        return currentBoxGuid;
    }
    public static bool RemoveIfGuidMatches(Guid guid)
    {
        if (currentBoxGuid == guid)
        {
            SetVisible(false);
            return true;
        }
        return false;
    }

    public static void SetVisible(bool visible) { if (Instance) Instance.SetVisibleThis(visible); }
    private void SetVisibleThis(bool visible)
    {
        Container.gameObject.SetActive(visible);
        isVisible = visible;
    }

    public static void SetPosition(Vector2 anchoredPosition, Vector2 pivot) { if (Instance) Instance.SetPositionThis(anchoredPosition, pivot); }
    void SetPositionThis(Vector2 anchoredPosition, Vector2 pivot)
    {
        Container.pivot = pivot;
        Container.anchoredPosition = anchoredPosition;
    }

    public static void SetText(string title, string description) { if (Instance) Instance.SetTextThis(title, description); }
    private void SetTextThis(string title, string description)
    {
        Title.text = title;
        Description.text = description;
    }

    public static void MarkSelected() { if (Instance) Instance.MarkSelectedThis(); }
    private void MarkSelectedThis()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        SetVisibleThis(false);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        SetVisibleThis(false);
    }
}
