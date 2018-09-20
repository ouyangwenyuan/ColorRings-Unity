using UnityEngine;
public class DotController : MonoBehaviour {
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
    // Update is called once per frame
    public int ringTotal;
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
                hasBigRing = true;
                ringTotal = 4;
            } else if (ringChildController.ringType == RingType.NORMAL_RING) //Is normal ring
            {
                hasNormalRing = true;
                ringTotal = 2;
            } else //Is small ring
            {
                hasSmallRing = true;
                ringTotal = 1;
            }
        } else if (transform.childCount == 2) //Has two rings
        {
            for (int i = 0; i < transform.childCount; i++) {
                if (transform.GetChild (i).GetComponent<RingController> ().ringType == RingType.BIG_RING) {
                    ringTotal += 4;
                    hasBigRing = true;
                } else if (transform.GetChild (i).GetComponent<RingController> ().ringType == RingType.NORMAL_RING) {
                    ringTotal += 2;
                    hasNormalRing = true;
                } else {
                    ringTotal += 1;
                    hasSmallRing = true;
                }
            }
        } else if (transform.childCount == 3) //Has three rings
        {
            ringTotal = 7;
            RingController ring_0Controller = transform.GetChild (0).GetComponent<RingController> ();
            RingController ring_1Controller = transform.GetChild (1).GetComponent<RingController> ();
            RingController ring_2Controller = transform.GetChild (2).GetComponent<RingController> ();

            if (ring_0Controller.colorIndex == ring_1Controller.colorIndex && ring_1Controller.colorIndex == ring_2Controller.colorIndex) {
                isSameColor = true;
            }

            hasBigRing = true;
            hasNormalRing = true;
            hasSmallRing = true;
            isFullRing = true;
        } else {
            ringTotal = 0;
            isEmptyRing = true;
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
}