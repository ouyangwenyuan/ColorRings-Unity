﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorLayoutRingMgr : GameManager {
	public class RingData {
		public int colorType;
		public int ringType;
		public int count;
		public string toSaveString () {
			return ringType + "," + colorType + ",";
		}
	}
	// private GameObject RingList;
	private GameObject clickRing;
	public InputField ringNumber;
	private List<GameObject> dots = new List<GameObject> ();
	private int ringIndex;
	private List<RingData> rings = new List<RingData> ();
	void Start () {
		// RingList = this.gameObject;
		GameLevelData levelData = CSVReader.gameMessLevelDatas[0];
		string[] datas = levelData.RingColorCount;
		List<RingData> ringstemp = new List<RingData> ();
		foreach (string item in datas) {
			print ("item=" + item);
			if (!string.IsNullOrEmpty (item)) {
				string[] item1 = item.Split ('-');
				RingData ringData = new RingData ();
				ringData.ringType = int.Parse (item1[0].Trim ());
				ringData.colorType = int.Parse (item1[1].Trim ());
				ringData.count = int.Parse (item1[2].Trim ());
				ringstemp.Add (ringData);
			}
		}
		for (int i = 0; i < ringstemp.Count; i++) {
			for (int j = 0; j < ringstemp[i].count; j++) {
				rings.Add (ringstemp[i]);
			}
		}
		print ("rings count =" + rings.Count);

		Vector2 theBottomPosition = Camera.main.ViewportToWorldPoint (new Vector2 (0.5f, 0));
		// theLeftPosition = Camera.main.ViewportToWorldPoint (new Vector2 (1f, 0));

		// originalRingPostion = theBottomPosition + new Vector2 (0, -1f);

		//Save the first position of random point
		firstRandomPointPosition = randomPoint.transform.position;

		finishMoveRandomPointBack = true;
		// showNextRings ();

		for (int i = 0; i < 27; i++) {
			generateRandomRing (i);
		}
	}
	private int[] ringType = { 1, 2, 4 };

	private void generateRandomRing (int i) {
		// int seed = Random.Range (0, 3);
		GameObject ring = null;
		// if (ringType[seed] == 1) {
		// 	ring = Instantiate (UIManager.Instance.smallRing);
		// } else if (ringType[seed] == 2) {
		// 	ring = Instantiate (UIManager.Instance.normalRing);
		// } else if (ringType[seed] == 4) {
		// 	ring = Instantiate (UIManager.Instance.bigRing);
		// }
		ring = Instantiate (UIManager.Instance.dotPoint);
		ring.transform.parent = transform;
		ring.transform.position = new Vector3 (transform.position.x + i * 1.5f, transform.position.y, transform.position.z);
		ring.transform.localScale = Vector3.one;
		// ring.layer = 9;
		dots.Add (ring);
		// int colorIndex = Random.Range (0, initialColorNumber);
		// ring.GetComponent<SpriteRenderer> ().color = UIManager.ringColors[colorIndex];
		// ring.GetComponent<RingController> ().colorIndex = colorIndex;
	}

	// Update is called once per frame
	void Update () {
		// moveRing ();
		if (Input.GetMouseButtonDown (0)) //First touch
		{
			Vector2 touchPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition); //Tranform mouse position to world position

			//If the mouse position too far away the rearest random point -> not allow drag the rings
			if ((touchPosition - firstRandomPointPosition).magnitude < 1f) {
				allowDrag = true;
			}
		} else if (Input.GetMouseButton (0)) //Touch stay
		{
			//If allow drag -> move all rings by move the random point (the rings is child of random point)
			if (allowDrag) {
				float x = Input.mousePosition.x;
				float y = Input.mousePosition.y;
				randomPoint.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (x, y, 1f)) + new Vector3 (0, 0.3f, 0); // lift the ring a bit for not being covered by finger.
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			if (allowDrag) {
				allowDrag = false;
				Vector2 mouseUpPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition); //Get mouse position
				GameObject theNearestDot = TheNearestDot1 (mouseUpPosition); //Find the nearest dot (place where the ring stay)

				Vector2 theNearestDotPosition = theNearestDot.transform.position;

				if ((mouseUpPosition - theNearestDotPosition).magnitude < 1f) //The ring not far away
				{
					//Check if allow drop - > drop all ring to the dot
					// if (AllowDrop (theNearestDot)) {
					// SoundManager.Instance.PlaySound (SoundManager.Instance.dropRing);
					// dotManager.dotIndex = theNearestDot.GetComponent<DotController> ().dotIndex;
					// while (randomPoint.transform.childCount > 0) {
					StartCoroutine (MoveRingToTheDot (randomPoint.transform.GetChild (0).gameObject, theNearestDot.transform.position));
					randomPoint.transform.GetChild (0).transform.parent = theNearestDot.transform;
					// }
					// }
				}
				theNearestDot = TheNearestDot (mouseUpPosition); //Find the nearest dot (place where the ring stay)

				theNearestDotPosition = theNearestDot.transform.position;
				if ((mouseUpPosition - theNearestDotPosition).magnitude < 1f) //The ring not far away
				{
					//Check if allow drop - > drop all ring to the dot
					// if (AllowDrop (theNearestDot)) {
					// SoundManager.Instance.PlaySound (SoundManager.Instance.dropRing);
					// dotManager.dotIndex = theNearestDot.GetComponent<DotController> ().dotIndex;
					// while (randomPoint.transform.childCount > 0) {
					StartCoroutine (MoveRingToTheDot (randomPoint.transform.GetChild (0).gameObject, theNearestDot.transform.position));
					randomPoint.transform.GetChild (0).transform.parent = theNearestDot.transform;
					// }
					// }
				}

				//Move the ranrom point back to the first position of it
				StartCoroutine (MoveRandomPointBack (randomPoint, firstRandomPointPosition));
			}
		}
		if (finishMoveRandomPointBack) {
			finishMoveRandomPointBack = false;
			// dotManager.finishCheckAfterDestroy = false;
			// UpdateColorNumber ();
			// GenerateRings ();
			showNextRings ();
			uIManager.CheckAndShowWatchAdOption ();
		}
	}

	public GameObject TheNearestDot1 (Vector2 mouseUpPosition) {
		GameObject theNearestDot = dots[0];
		Vector2 dotPosition = dots[0].transform.position;
		float minDistance = (mouseUpPosition - dotPosition).magnitude;

		for (int i = 0; i < dots.Count; i++) {
			dotPosition = dots[i].transform.position;
			if ((mouseUpPosition - dotPosition).magnitude < minDistance) {
				minDistance = (mouseUpPosition - dotPosition).magnitude;
				theNearestDot = dots[i];
			}
		}
		return theNearestDot;
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
						// dotManager.dotIndex = theNearestDot.GetComponent<DotController> ().dotIndex;
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
		// if (finishMoveRandomPointBack) {
		// 	finishMoveRandomPointBack = false;
		// 	dotManager.finishCheckAfterDestroy = false;
		// 	// UpdateColorNumber ();
		// 	// showNextRings ();
		// 	uIManager.CheckAndShowWatchAdOption ();
		// }
	}

	private void showNextRings () {
		if (ringIndex >= rings.Count - 1) { return; }
		RingData ringData = rings[ringIndex];
		int seed = ringData.ringType; //Random.Range (0, 3);
		GameObject ring = null;
		if (seed == 1) {
			ring = Instantiate (UIManager.Instance.smallRing);
		} else if (seed == 2) {
			ring = Instantiate (UIManager.Instance.normalRing);
		} else if (seed == 4) {
			ring = Instantiate (UIManager.Instance.bigRing);
		}
		int colorIndex = ringData.colorType; //Random.Range (0, initialColorNumber);
		ring.GetComponent<SpriteRenderer> ().color = UIManager.ringColors[colorIndex];
		ring.GetComponent<RingController> ().colorIndex = colorIndex;
		ringIndex++;
		ring.transform.position = randomPoint.transform.position;
		// ring.transform.position = new Vector3 (transform.position.x + ringIndex * 1.5f, transform.position.y, transform.position.z);
		// ring.transform.localScale = Vector3.one;
		ring.transform.parent = randomPoint.transform;
		// ring.layer = 9;

		// print ("ring =" + ring);
	}

}