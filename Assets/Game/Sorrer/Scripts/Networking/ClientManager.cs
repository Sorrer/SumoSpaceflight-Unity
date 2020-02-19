using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using System.Diagnostics;

public partial class NetworkingManager
{

	public static double Ping;
	public static PlayerData ClientData;
	public static byte[] ClientDataSerialized;


	//Stopwatch PingTimeoutTimer = new Stopwatch();
	Stopwatch PingTimer = new Stopwatch();

    private void UpdateClient() {
		if (!PingTimer.IsRunning) PingTimer.Start();

		if (PingTimer.ElapsedMilliseconds / 1000 > 2.5f) {
			networkObject.Networker.Ping();
			PingTimer.Restart();
		}
	}

	private void OnThePing(double ping, NetWorker sender) {
		MainThreadManager.Run(() => {
			Ping = ping;
		});
	}

	public override void RecieveClientData(RpcArgs args) {

		MainThreadManager.Run(() => {
			ConsoleLogger.debug("Client", "Recieved Game Data. Updating Data.");

			PlayerData data = (PlayerData)SerializeUtility.DeserializeFromBytes(args.GetNext<byte[]>());

			ConsoleLogger.debug("Client", "Client data - ID " + data.ID + " Secret " + data.Secret);

			SyncClientData(data);
		});
	}

	public void SyncClientData(PlayerData data) {
		if (ClientData == null) ClientData = new PlayerData();

		ClientData.ID = data.ID;
		ClientData.Secret = data.Secret;

		ClientDataSerialized = SerializeUtility.Serialize2Bytes(ClientData);
	}

}
