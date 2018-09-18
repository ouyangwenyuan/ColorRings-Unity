using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRunState {

	// public static int currentLevel = 1;
	public const string CONSTANT_NAME = "value";
	public static StoredValue<int> currentLevel = new StoredValue<int>("currentLevel",1);
}