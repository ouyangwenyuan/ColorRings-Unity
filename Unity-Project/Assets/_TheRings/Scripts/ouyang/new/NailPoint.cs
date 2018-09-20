using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailPoint : MonoBehaviour {
	public Vector2 Position { get; set; }
	public int x;
	public int y;
	public int ringTotal;
	public ColorRing bigRing;
	public ColorRing mediumRing;
	public ColorRing smallRing;

	public NailPoint (int x, int y) {
		this.Position = new Vector2 (x, y);
		this.x = x;
		this.y = y;
	}
}