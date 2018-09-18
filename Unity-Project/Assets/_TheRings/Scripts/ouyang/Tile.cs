using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Tile : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler{
    public const float EDGE_SIZE = 42; //50
    public const float WIDTH = 2f * EDGE_SIZE;
    public const float HEIGHT = WIDTH * 0.866f;

    public enum Type { Background, Normal, Stone, RedBtn, GreenBtn, RedBlock, GreenBlock};
    public Type type = Type.Normal;

    public bool isActive = true;
    public bool hasCover = false;
    // position 是在地图板块的相对坐标，boardPosition 是拼接后在板块中的相对坐标
    public Vector2 position, boardPosition;


    private void Start()
    {
            CanvasGroup cg = GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
  
    }

    public void SetActive(bool isActive)
    {
        this.isActive = isActive;

    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
