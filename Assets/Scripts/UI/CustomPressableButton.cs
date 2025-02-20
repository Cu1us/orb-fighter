using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Overriding the button class seems to be the only way to get access to a button's SelectionState
// this script moves the button's children to follow the button sprite as it (visually, but not actually) gets pressed down on hover/click
[RequireComponent(typeof(RectTransform))]
public class CustomPressableButton : Button
{
    public Vector2 highlightedChildOffset;
    public Vector2 pressedChildOffset;
    RectTransform rectTransform;

    protected override void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // Hardcoded values to make it follow my button sprite
        // (These cannot be exposed in the inspector without overriding the button class's custom inspector)
        highlightedChildOffset = new Vector2(0.525f, 0.475f);
        pressedChildOffset = new Vector2(0.6f, 0.4f);
        base.Awake();
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        SetChildOffset(state switch
        {
            SelectionState.Highlighted => highlightedChildOffset,
            SelectionState.Selected => highlightedChildOffset,
            SelectionState.Pressed => pressedChildOffset,
            _ => new(0.5f, 0.5f)
        });
    }

    void SetChildOffset(Vector2 offset)
    {
        if (!Application.isPlaying) return;
        float aspect = rectTransform.rect.width / rectTransform.rect.height;
        offset.x = ((offset.x - 0.5f) / aspect) + 0.5f;
        foreach (RectTransform child in rectTransform)
        {
            child.anchorMax = child.anchorMin = offset;
        }
    }
}
