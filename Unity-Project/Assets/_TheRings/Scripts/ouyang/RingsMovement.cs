using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingsMovement : MonoBehaviour {

	public RectTransform container;
	float cameraStarPos;
	const float distance = 0.01f;

	// Use this for initialization
	void Start () {
		// cameraStarPos = 3600;
		transform.position += new Vector3 (0, 0);
		container.anchoredPosition = new Vector2 (0, container.anchoredPosition.y);
	}

	public void CameraPosUpdate () {
		// if (container.anchoredPosition.x >= 0 && container.anchoredPosition.x <= 3600) {
			transform.position = new Vector3 ((container.anchoredPosition.x+320) * distance, transform.position.y, transform.position.z);
		// }
	}
	// Update is called once per frame
	void Update () {

	}
}