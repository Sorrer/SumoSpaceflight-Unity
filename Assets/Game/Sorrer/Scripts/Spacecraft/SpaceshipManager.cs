using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipManager : MonoBehaviour
{
	public SpaceshipController controller;
	public SpaceshipNetworkController networkController;
	public Weapon Primary;
	public Weapon Secondary;

	public double Energy;



	private void Update() {

		if(Primary != null && Primary.spaceship != this) {
			Primary.spaceship = this;
		}

		if(Secondary != null && Secondary.spaceship != this) {
			Secondary.spaceship = this;
		}

		if(networkController.networkObject != null) {
			this.Energy = networkController.networkObject.energy;

			if (networkController.networkObject.IsServer) {

				SeverCommandUpdate();


			} else {
				if(networkController.OwnerID != NetworkingManager.ClientData.ID) {
					SeverCommandUpdate();
				} else {
					if (Input.GetMouseButtonDown(0) && Primary != null) {
						SendServerInput(0, true);
						Primary.Attack();
						ConsoleLogger.debug("Shooting primary");
					}
					if (Input.GetMouseButtonDown(1) && Secondary != null) {
						SendServerInput(1, true);
						Secondary.Attack();
						ConsoleLogger.debug("Shooting secondary");

					}

					if (Input.GetMouseButtonUp(0) && Primary != null) {
						SendServerInput(0, false);
						Primary.StopAttack();
						ConsoleLogger.debug("Releasing primary");
					}
					if (Input.GetMouseButtonUp(1) && Secondary != null) {
						SendServerInput(1, false);
						Secondary.StopAttack();
						ConsoleLogger.debug("Releasing secondary");

					}
				}
			}
		} else {
			if (Input.GetMouseButtonDown(0) && Primary != null) {
				Primary.Attack();
			}
			if (Input.GetMouseButtonDown(1) && Secondary != null) {
				Secondary.Attack();
			}

			if (Input.GetMouseButtonUp(0) && Primary != null) {
				Primary.StopAttack();
			}
			if (Input.GetMouseButtonUp(1) && Secondary != null) {
				Secondary.StopAttack();
			}
		}

		
	}

	public void SeverCommandUpdate() {
		bool isPrimaryPressed = false;
		CommandData primaryData = null;
		bool isSecondaryPressed = false;
		CommandData secondaryData = null;

		for (int i = 0; i < networkController.HeldCommands.Count; i++) {
			if (networkController.HeldCommands[i].CommandID == 0) {
				isPrimaryPressed = networkController.HeldCommands[i].IsPressed;
				primaryData = networkController.HeldCommands[i];
			} else if (networkController.HeldCommands[i].CommandID == 1) {
				isSecondaryPressed = networkController.HeldCommands[i].IsPressed;
				secondaryData = networkController.HeldCommands[i];
			}
		}


		if (Primary != null && isPrimaryPressed != Primary.isPressed) {
			if (isPrimaryPressed) {
				Primary.ServerAttack(primaryData.timeStep);
			} else {
				Primary.StopServerAttack(primaryData.timeStep);
			}
		}

		if (Secondary != null && isSecondaryPressed != Secondary.isPressed) {
			if (isSecondaryPressed) {
				Secondary.ServerAttack(secondaryData.timeStep);
			} else {
				Secondary.StopServerAttack(secondaryData.timeStep);
			}
		}
	}


	public void SendServerInput(int id, bool pressed) {
		CommandData data = new CommandData();
		data.CommandID = id;
		data.IsPressed = pressed;

		networkController.SendCommand(data);
	}


	public void RewindForAttack() {

	}
}

[System.Serializable]
public class CommandData {
	public int CommandID;
	public bool IsPressed;
	public ulong timeStep;
}