using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutRingMgr : GameManager {

	// private GameObject RingList;
	private GameObject clickRing;

	void Start () {
		// RingList = this.gameObject;
		for (int i = 0; i < 27; i++) {
			generateRandomRing(i);
		}
	}
	private int[] ringType = { 1, 2, 4 };

	private void generateRandomRing (int i) {
		int seed = Random.Range (0, 3);
		GameObject ring = null;
		if (ringType[seed] == 1) {
			ring = Instantiate (UIManager.Instance.smallRing);
		} else if (ringType[seed] == 2) {
			ring = Instantiate (UIManager.Instance.normalRing);
		} else if (ringType[seed] == 4) {
			ring = Instantiate (UIManager.Instance.bigRing);
		}

		ring.transform.parent = transform;
		ring.transform.position = new Vector3 (transform.position.x + i * 1.5f, transform.position.y, transform.position.z);
		ring.transform.localScale = Vector3.one;
		ring.layer = 9;
		int colorIndex = Random.Range (0, initialColorNumber);
		ring.GetComponent<SpriteRenderer> ().color = UIManager.ringColors[colorIndex];
	}

	// Update is called once per frame
	void Update () {
		moveRing ();
	}

	private void FixedUpdate () {

	}

	private void moveRing () {
		if (Input.GetMouseButtonDown (0)) //First touch
		{
			Vector2 touchPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition); //Tranform mouse position to world position
			float distance = float.MaxValue;
			int nearestIndex = 0;
			for (int i = 0; i < transform.childCount; i++) {
				Vector2 p = transform.GetChild (i).position;
				if ((touchPosition - p).magnitude < distance) {
					distance = (touchPosition - p).magnitude;
					nearestIndex = i;
				}
			}
			Transform trans = transform.GetChild (nearestIndex);
			Vector2 pos = trans.position;
			if ((touchPosition - pos).magnitude < 1f) {
				allowDrag = true;
				firstRandomPointPosition = pos;
				clickRing = trans.gameObject;
			}
			//If the mouse position too far away the rearest random point -> not allow drag the rings
			// if ((touchPosition - firstRandomPointPosition).magnitude < 1f) {
			// 	allowDrag = true;
			// }
		} else if (Input.GetMouseButton (0)) //Touch stay
		{
			//If allow drag -> move all rings by move the random point (the rings is child of random point)
			if (allowDrag) {
				float x = Input.mousePosition.x;
				float y = Input.mousePosition.y;
				clickRing.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (x, y, 1f)) + new Vector3 (0, 0.3f, 0); // lift the ring a bit for not being covered by finger.
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			if (allowDrag) {
				allowDrag = false;
				Vector2 mouseUpPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition); //Get mouse position
				GameObject theNearestDot = TheNearestDot (mouseUpPosition); //Find the nearest dot (place where the ring stay)

				Vector2 theNearestDotPosition = theNearestDot.transform.position;

				if ((mouseUpPosition - theNearestDotPosition).magnitude < 1f) //The ring not far away
				{
					//Check if allow drop - > drop all ring to the dot
					if (AllowDrop (theNearestDot)) {
						// SoundManager.Instance.PlaySound (SoundManager.Instance.dropRing);
						dotManager.dotIndex = theNearestDot.GetComponent<DotController> ().dotIndex;
						// while (randomPoint.transform.childCount > 0) {
						StartCoroutine (MoveRingToTheDot (clickRing, theNearestDot.transform.position));
						clickRing.transform.parent = theNearestDot.transform;
						// }
					} else {
						StartCoroutine (MoveRandomPointBack (clickRing, firstRandomPointPosition));
					}
				} else {
					StartCoroutine (MoveRandomPointBack (clickRing, firstRandomPointPosition));
				}

				//Move the ranrom point back to the first position of it

			}
		}

		//After check -> generate rings
		// if (dotManager.finishCheckAfterDestroy && finishMoveRandomPointBack) {
		// 	finishMoveRandomPointBack = false;
		// 	dotManager.finishCheckAfterDestroy = false;
		// 	// UpdateColorNumber ();
		// 	GenerateRings ();
		// 	uIManager.CheckAndShowWatchAdOption ();
		// }
	}

}