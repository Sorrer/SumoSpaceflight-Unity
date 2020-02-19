using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseShotty : Weapon {

	protected override void OnAttack() {
		ConsoleLogger.debug("Pew");
	}

	protected override void OnServerAttack(ulong currentTime) {
		ConsoleLogger.debug("Pew");
	}
}
