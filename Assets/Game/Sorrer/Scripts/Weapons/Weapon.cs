using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Client shoots, tells server that it shoots
//Server recieves ping timestamp and shoot command
//Server shoots on server side (maybe with timestep applyed backwards)

//Make sure weapon is derterministic as in you can rewind just by delta time and it will still produce the same results forward

public abstract class Weapon : MonoBehaviour
{

	public SpaceshipManager spaceship;

	public bool isPressed = false;

	public void Attack() {
		ConsoleLogger.debug("Recieved Input Attack");
		isPressed = true;
		OnAttack();

	}
	public void StopAttack() {
		ConsoleLogger.debug("Recieved Input Stop Attack");
		isPressed = false;
		OnStopAttack();
	}


	public void ServerAttack(ulong currentTime) {
		ConsoleLogger.debug("Recieved Input Server Attack - " + currentTime);
		isPressed = true;
		OnServerAttack(currentTime);
	}

	public void StopServerAttack(ulong currentTime) {
		ConsoleLogger.debug("Recieved Input Stop Server Attack - " + currentTime);
		isPressed = false;
		OnStopAttack();
	}

	private void HasEnergy() {

	}

	protected abstract void OnAttack();
	protected abstract void OnServerAttack(ulong currentTime); //Shoots with the deltatime backsteped traced forwards
	protected virtual void OnStopAttack() { } //Can be overriden for when call to stop attacking is executed (Laser shooting)


}
