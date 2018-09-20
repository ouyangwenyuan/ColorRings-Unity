using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRing : MonoBehaviour {
	public enum RingType {
		unknown = 0, small = 1, midle = 2, big = 4
	}
	public RingType type;

	public Color color { get; set; }

	public int sizeType; //unknown = 0, small = 1, midle = 2, big = 4
	public int colorType;
	public int x;
	public int y;

}