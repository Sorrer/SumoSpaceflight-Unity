using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated {
	public partial class SpacecraftNetworkObject {
		protected override bool AllowOwnershipChange(NetworkingPlayer newOwner) {
			return false;
		}
	}
}
