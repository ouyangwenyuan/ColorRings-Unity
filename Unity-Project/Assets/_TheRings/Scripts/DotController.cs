using UnityEngine;
using UnityEngine.EventSystems;
public class DotController : NailPoint, IPointerClickHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler {
    [HideInInspector]
    public bool hasBigRing;
    [HideInInspector]
    public bool hasNormalRing;
    [HideInInspector]
    public bool hasSmallRing;
    [HideInInspector]
    public bool isSameColor;
    [HideInInspector]
    public bool isFullRing;
    [HideInInspector]
    public bool isEmptyRing;

    public int dotIndex;

    public int bigRingColor = -1;
    public int midRingColor = -1;
    public int smallRingColor = -1;

    // Update is called once per frame
    public void CheckRing () {
        hasBigRing = false;
        hasNormalRing = false;
        hasSmallRing = false;
        isSameColor = false;
        isFullRing = false;
        isEmptyRing = false;

        if (transform.childCount == 1) //Has one ring
        {
            RingController ringChildController = transform.GetChild (0).GetComponent<RingController> ();
            if (ringChildController.ringType == RingType.BIG_RING) //Is big ring
            {
                ringTotal = 4;
                bigRingColor = ringChildController.colorIndex;
                hasBigRing = true;
            } else if (ringChildController.ringType == RingType.NORMAL_RING) //Is normal ring
            {
                ringTotal = 2;
                midRingColor = ringChildController.colorIndex;
                hasNormalRing = true;
            } else //Is small ring
            {
                ringTotal = 1;
                smallRingColor = ringChildController.colorIndex;
                hasSmallRing = true;
            }
        } else if (transform.childCount == 2) //Has two rings
        {
            for (int i = 0; i < transform.childCount; i++) {
                RingController ring_0Controller = transform.GetChild (i).GetComponent<RingController> ();
                if (ring_0Controller.ringType == RingType.BIG_RING) {
                    ringTotal += 4;
                    bigRingColor = ring_0Controller.colorIndex;
                    hasBigRing = true;
                } else if (ring_0Controller.ringType == RingType.NORMAL_RING) {
                    ringTotal += 2;
                    midRingColor = ring_0Controller.colorIndex;
                    hasNormalRing = true;
                } else {
                    ringTotal += 1;
                    smallRingColor = ring_0Controller.colorIndex;
                    hasSmallRing = true;
                }
            }
        } else if (transform.childCount == 3) //Has three rings
        {
            ringTotal = 7;

            for (int i = 0; i < transform.childCount; i++) {
                RingController ringController = transform.GetChild (i).GetComponent<RingController> ();
                if (ringController.ringType == RingType.BIG_RING) {
                    ringTotal += 4;
                    bigRingColor = ringController.colorIndex;
                    hasBigRing = true;
                } else if (ringController.ringType == RingType.NORMAL_RING) {
                    ringTotal += 2;
                    midRingColor = ringController.colorIndex;
                    hasNormalRing = true;
                } else {
                    ringTotal += 1;
                    smallRingColor = ringController.colorIndex;
                    hasSmallRing = true;
                }
            }

            if (bigRingColor != -1 && bigRingColor == midRingColor && bigRingColor == smallRingColor) {
                isSameColor = true;
            }

            isFullRing = true;
        } else if (transform.childCount == 0) {
            ringTotal = 0;
            bigRingColor = -1;
            midRingColor = -1;
            smallRingColor = -1;
            isEmptyRing = true;
        } else {
            Debug.LogError ("一个点超过3个环，逻辑有问题");
        }
    }

    public string saveString () {
        string str = "";
        if (transform.childCount == 0) return str;
        for (int i = 0; i < transform.childCount; i++) {
            RingController ringController = transform.GetChild (i).GetComponent<RingController> ();
            str += ringController.ringType.GetHashCode () + "," + ringController.colorIndex + ",";
        }
        return str;
    }

    public void OnPointerClick (PointerEventData eventData) {

    }

    public void OnDrag (PointerEventData eventData) {

    }

    public void OnPointerDown (PointerEventData eventData) {

    }

    public void OnPointerUp (PointerEventData eventData) {

    }
}