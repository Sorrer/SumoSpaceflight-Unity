using System.Collections;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Security;
using System.Runtime.Serialization;

public class SerializeUtility : MonoBehaviour
{


	public static SurrogateSelector selector;

	public static void LoadSurrogateSelector() {

		SurrogateSelector surrogateSelector = new SurrogateSelector();
		Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
		surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);

		selector = surrogateSelector;
	}

	public static byte[] Serialize2Bytes(object data) {
		if (data == null) {
			return new byte[0];
		} else {
			MemoryStream streamMemory = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();

			formatter.SurrogateSelector = selector;

			formatter.Serialize(streamMemory, data);
			return streamMemory.GetBuffer();
		}
	}
	
	public static object DeserializeFromBytes(byte[] binData) {
		if (binData == null) {
			return null;
		} else {
			if (binData.Length == 0) {
				return null;
			} else {
				BinaryFormatter formatter = new BinaryFormatter();
				MemoryStream ms = new MemoryStream(binData);
				formatter.SurrogateSelector = selector;
				return formatter.Deserialize(ms);
			}
		}
	}
	
	public static string Serialize2String(object data) {
		if (data == null) {
			return string.Empty;
		} else {
			MemoryStream streamMemory = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.SurrogateSelector = selector;
			formatter.Serialize(streamMemory, data);
			return Convert.ToBase64String(streamMemory.GetBuffer());
		}
	}
	
	public static object DeserializeFromString(string binString) {
		if (binString == null) {
			return null;
		} else {
			if (binString.Length == 0) {
				return null;
			} else {
				byte[] binData = Convert.FromBase64String(binString);
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.SurrogateSelector = selector;
				MemoryStream ms = new MemoryStream(binData);
				return formatter.Deserialize(ms);
			}
	}
	
	
	}
}
