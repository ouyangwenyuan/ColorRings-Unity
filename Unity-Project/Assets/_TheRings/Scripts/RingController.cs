using UnityEngine;
using UnityEngine.EventSystems;
public class RingController : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler {
    public enum Status { Dragging, OnBoard, OnBottom, OnTween };
    public Color color { get; set; }
    public int ringType;
    public int colorIndex;
    public bool destroyed;

    public int x;
    public int y;

    //切片的状态
    public Status status = Status.OnBottom;
    //开始的坐标和触摸开始的坐标
    protected Vector3 beginPosition, beginTouchPosition;
    public GameObject nearestDot;
    public void OnPointerClick (PointerEventData eventData) {

    }

    public void OnDrag (PointerEventData eventData) {
        if (status != Status.Dragging) return;

        Vector3 moveDelta = Camera.main.ScreenToWorldPoint (Input.mousePosition) - beginTouchPosition;
        transform.position = beginPosition + moveDelta;

        // TileRegion2.instance.CheckMatch (this);

        // find the nearest dot
    }

    public void OnPointerDown (PointerEventData eventData) {
        beginPosition = transform.position;
        beginTouchPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

        // transform.SetParent (MonoUtils.instance.dragRegion);

        status = Status.Dragging;

    }

    public void OnPointerUp (PointerEventData eventData) {
        // if ((mouseUpPosition - theNearestDotPosition).magnitude < 1f) {
        //     iTween.MoveTo (gameObject, iTween.Hash ("position", matches[0].transform.position, "speed", 10f, "oncomplete", "CompleteMoveToBoard"));
        // } else {
        //     iTween.ScaleTo (gameObject, iTween.Hash ("scale", Vector3.one * Const.SCALED_TILE, "speed", 4f));
        //     iTween.MoveTo (gameObject, iTween.Hash ("position", beginPosition, "speed", 15f, "oncomplete", "CompleteMoveToBottom"));
        // }

        status = Status.OnTween;
    }

    private void CompleteMoveToBoard () {
        status = Status.OnBoard;
        // transform.SetParent (MonoUtils.instance.pieceRegion);

    }

    private void CompleteMoveToBottom () {
        status = Status.OnBottom;
        // transform.SetParent (MonoUtils.instance.pieceRegion);
    }
}