using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;

public partial class NetworkingManager
{
    
	private void UpdateServer() {

	}

	

	public override void RequestClientData(RpcArgs args) {
		if (!networkObject.IsServer) {
			return;
		}


		MainThreadManager.Run(() => {

			NetworkingPlayer targetPlayer = args.Info.SendingPlayer;

			PlayerData data = GeneratePlayerServerData(targetPlayer.NetworkId);

			byte[] bytes = SerializeUtility.Serialize2Bytes(data);

			networkObject.SendRpc(targetPlayer, RPC_RECIEVE_CLIENT_DATA, bytes);


		});
	}

	private void OnPlayerConnected(NetworkingPlayer player, NetWorker sender) {
		ConsoleLogger.debug("Player has joined! NetworkID " + player.NetworkId);
	}
	private void OnPlayerDisconnected(NetworkingPlayer player, NetWorker sender) {
		ConsoleLogger.debug("Player has left ;_; NetworkID " + player.NetworkId);
	}

}
