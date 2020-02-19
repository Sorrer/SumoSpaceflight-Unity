using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleLogger : MonoBehaviour
{


	public static void debug(string i) {
		Debug.Log(CreateTagLine("Null") + i);
	}

	public static void debug(string tag, string i) {
		Debug.Log(CreateTagLine(tag) + i);
	}

	public static void err(string i) {
		Debug.LogError(CreateTagLine("Error") + i);
	}

	public static string CreateTagLine(string tag) {
		return  "[" + tag + "][" + DateTime.Now.ToString("h:mm:ss.fff tt") + "]" + ": ";
	}
}
