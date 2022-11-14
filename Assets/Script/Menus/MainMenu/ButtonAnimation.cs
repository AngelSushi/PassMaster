using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimation : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {

    private RectTransform rectTransform;
    public Vector3 hoverSize;
    private Vector3 normalSize;

    private void Awake() => rectTransform = GetComponent<RectTransform>();

    private void Start() => normalSize = rectTransform.sizeDelta;

    public void OnPointerEnter(PointerEventData eventData) => rectTransform.sizeDelta = hoverSize;
    public void OnPointerExit(PointerEventData eventData) => rectTransform.sizeDelta = normalSize;
}   
