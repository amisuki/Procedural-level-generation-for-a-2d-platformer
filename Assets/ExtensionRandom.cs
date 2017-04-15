using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionRandom
{

	public static int RoomType()
	{
		return UnityEngine.Random.Range (0, 4);
	}
}
