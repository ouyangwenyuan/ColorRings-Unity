using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : ScriptableObject {
	[Header ("Width")]
	public int mapW;
	[Header ("Height")]
	public int mapH;
	[Header ("Positions")]
	public string positions; //0,0|0,1|0,2|1,0|1,1|1,2|2,0|2,1|2,2|
	[Header ("Rings")]
	public string posRings; //1,x,2,y,4,z-0,0|1,x,2,y,4,z-2,2
	[Header ("Blocks")]
	public string posBlocks; //0,0,x|1,0,y 
}