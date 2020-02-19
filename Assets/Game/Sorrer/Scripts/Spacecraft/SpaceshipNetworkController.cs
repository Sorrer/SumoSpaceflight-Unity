using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;

public class SpaceshipNetworkController : SpacecraftBehavior
{


	public int OwnerID = 0;
	public bool Initialized = false;

	public Rigidbody rigidBody;
	public GameObject controllerBody;

	public GameObject ControllerPrefab;

	// Start is called before the first frame update
	void Start()
    {
		networkObject.UpdateInterval = 1 / 60;
    }

	protected override void NetworkStart() {
		base.NetworkStart();


		//If client
		//Initialize spaceship data (id owner, spaceship stuff, etc)
		//If client matches the owner of spaceship (ID) then allow control with update
		
		if (networkObject.IsServer) {
			MainThreadManager.Run(() => {
				GameObject controllerObj = Instantiate(ControllerPrefab);

				var controller = controllerObj.GetComponent<SpaceshipController>();
				controller.body = this.transform;
				controller.rigidBody = this.rigidBody;
				controller.CanControl = false;
				controllerBody = controller.gameObject;

				var manager = controllerObj.GetComponent<SpaceshipManager>();
				manager.networkController = this;
			});
		} else {
			networkObject.SendRpc(RPC_REQUEST_INIT, Receivers.Server);
		}
	}

	// Update is called once per frame
	void Update()
    {

		if (!Initialized) {
			return;
		}

		if (networkObject.IsServer) {
			networkObject.position = this.transform.position;
			networkObject.rotation = this.transform.rotation;
			networkObject.velocity = rigidBody.velocity;
		} else {
			if(this.OwnerID == NetworkingManager.ClientData.ID) {
				networkObject.SendRpcUnreliable(RPC_UPDATE_TRANSFORM_R_P_C, Receivers.Server, 
					SerializeUtility.Serialize2Bytes(NetworkingManager.ClientData), this.rigidBody.velocity, this.transform.position, this.transform.rotation.eulerAngles);
			} else {
				this.transform.position = networkObject.position;
				this.transform.rotation = networkObject.rotation;
				this.rigidBody.velocity = networkObject.velocity;
			}
		}

    }

	public void ServerInitialize() {
		byte[] data = SerializeUtility.Serialize2Bytes(GenerateInitData());
		for (int i = 0; i < requiresInit.Count; i++) {
			networkObject.SendRpc(requiresInit[i].Info.SendingPlayer, RPC_RECIEVE_INIT, data);
		}
		requiresInit.Clear();
		Initialized = true;
	}

	


	private List<RpcArgs> requiresInit = new List<RpcArgs>();


	public override void UpdateTransformRPC(RpcArgs args) {

		if (networkObject.IsServer) {

			PlayerData data = (PlayerData) SerializeUtility.DeserializeFromBytes(args.GetNext<byte[]>());
			Vector3 velocity = args.GetNext<Vector3>();
			Vector3 position = args.GetNext<Vector3>();
			Vector3 rotation = args.GetNext<Vector3>();

			
			MainThreadManager.Run(() => {

				if (NetworkingManager.Instance.ValidatePlayer(args.Info.SendingPlayer.NetworkId ,data) && data.ID == this.OwnerID) {//Validate

					//TODO - Validate if position makes sense


					//Update
					this.transform.position = position;
					this.transform.rotation = Quaternion.Euler(rotation);
					this.rigidBody.velocity = velocity;
				}

			});


		}


	}

	public override void RequestInit(RpcArgs args) {

		if (!networkObject.IsServer) return;

		MainThreadManager.Run(() => {

			if (!Initialized) {
				requiresInit.Add(args);
			} else {
				networkObject.SendRpc(args.Info.SendingPlayer, RPC_RECIEVE_INIT, SerializeUtility.Serialize2Bytes(GenerateInitData()));
			}

  
		});

	}

	public List<CommandData> HeldCommands = new List<CommandData>();

	public void SendCommand(CommandData data) {

		networkObject.SendRpc(RPC_COMMAND, Receivers.Server, NetworkingManager.ClientDataSerialized ,SerializeUtility.Serialize2Bytes(data));

	}

	private void SendCommandToClients(CommandData data) {

		networkObject.SendRpc(RPC_COMMAND, Receivers.All, NetworkingManager.ClientDataSerialized, SerializeUtility.Serialize2Bytes(data));
	}

	public override void Command(RpcArgs args) {

		if (networkObject.IsServer) {

			PlayerData playerData = (PlayerData) SerializeUtility.DeserializeFromBytes(args.GetNext<byte[]>());
			if (!NetworkingManager.Instance.ValidatePlayer(args.Info.SendingPlayer.NetworkId, playerData)) {
				return;
			}

			if(playerData.ID != OwnerID) {
				return;
			}

		}
		
		CommandData data = (CommandData)SerializeUtility.DeserializeFromBytes(args.GetNext<byte[]>());

		if (networkObject.IsServer) data.timeStep = args.Info.TimeStep;


		MainThreadManager.Run(() => {

			if (data.IsPressed) {
				bool found = false;
				for (int i = 0; i < HeldCommands.Count; i++) {
					if (HeldCommands[i].CommandID == data.CommandID) {
						found = true;
						HeldCommands[i] = data;
						break;
					}
				}

				if (!found) {
					HeldCommands.Add(data);
				}

			} else {

				for (int i = 0; i < HeldCommands.Count; i++) {
					if (HeldCommands[i].CommandID == data.CommandID) {
						HeldCommands[i] = data;
						break;
					}
				}

			}

		});



	}

	public override void RecieveInit(RpcArgs args) {
		if (networkObject.IsServer || !args.Info.SendingPlayer.IsHost) {

			return;
		}

		MainThreadManager.Run(() => {
			SyncInitData((InitData)SerializeUtility.DeserializeFromBytes(args.GetNext<byte[]>()));

			GameObject controllerObj = Instantiate(ControllerPrefab);

			var controller = controllerObj.GetComponent<SpaceshipController>();
			controller.body = this.transform;
			controller.rigidBody = this.rigidBody;
			controllerBody = controller.gameObject;

			var manager = controllerObj.GetComponent <SpaceshipManager>();
			manager.networkController = this;

			if (NetworkingManager.ClientData.ID != this.OwnerID) {
				controller.CanControl = false;
			}

		});


		Initialized = true;
	}

	private void SyncInitData(InitData data) {
		this.OwnerID = data.Owner;
	}

	private InitData GenerateInitData() {
		InitData data = new InitData();
		data.Owner = this.OwnerID;
		return data;
	}

	[System.Serializable]
	private class InitData {
		public int Owner;
	}
}

