using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;

public partial class NetworkingManager : GameManagerBehavior
{

	public static NetworkingManager Instance;

	Dictionary<uint, PlayerData> PlayerDataDict = new Dictionary<uint, PlayerData>();
	int IDCounter;


	private void Start() {
		if(Instance == null) {
			Instance = this;
		} else {
			Destroy(this);
		}
	}

	protected override void NetworkStart() {
		base.NetworkStart();
		if (networkObject.IsServer) {
			NetworkManager.Instance.Networker.playerAuthenticated += OnPlayerConnected;
			NetworkManager.Instance.Networker.playerDisconnected += OnPlayerDisconnected;
		} else {
			networkObject.Networker.onPingPong += OnThePing;
			networkObject.SendRpc(RPC_REQUEST_CLIENT_DATA, Receivers.Server);
		}

	}

	private void Update() {
		if (networkObject == null) return;

		if (networkObject.IsServer) {
			UpdateServer();
		} else {
			UpdateClient();
		}
	}

	public bool ValidatePlayer(uint NetworkID, byte[] byteData) {
		PlayerData data = (PlayerData)SerializeUtility.DeserializeFromBytes(byteData);

		if(data == null) {
			return false;
		}

		PlayerData data2;

		if (PlayerDataDict.TryGetValue(NetworkID, out data2)) {
			if (data2.ID == data.ID && data2.Secret == data.Secret) {
				return true;
			}
		}

		return false;
	}

	public bool ValidatePlayer(uint NetworkID, PlayerData data) {

		if (data == null) {
			return false;
		}

		PlayerData data2;

		if(PlayerDataDict.TryGetValue(NetworkID, out data2)) {
			if(data2.ID == data.ID && data2.Secret == data.Secret) {
				return true;
			}
		}

		return false;
	}

	public PlayerData GeneratePlayerServerData(uint NetworkID) {
		PlayerData data = new PlayerData();

		data.ID = IDCounter++;
		data.Secret = GenerateClientSecret();

		//Temp generate their ship
		var spaceCraft = (SpaceshipNetworkController)NetworkManager.Instance.InstantiateSpacecraft();
		spaceCraft.OwnerID = data.ID;
		spaceCraft.ServerInitialize();
		

		PlayerDataDict.Add(NetworkID, data);

		return data;
	}

	public int GenerateClientSecret() {
		int secret = 0;

		bool finding = true;
		while (finding) {
			secret = Random.Range(int.MinValue, int.MaxValue);
			foreach (PlayerData data in PlayerDataDict.Values) {
				if (data.Secret == secret) {
					continue;
				}
			}

			finding = false;
		}

		return secret;
	}


}

[System.Serializable]
public class PlayerData {
	public int ID;
	public int Secret;

}