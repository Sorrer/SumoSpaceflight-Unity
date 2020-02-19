using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Frame;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated {

	public partial class NetworkObjectFactory {
		protected override bool ValidateCreateRequest(NetWorker networker, int identity, uint id, FrameStream frame) {

			if (networker.IsServer) {
				return true;
			}
			return false;
		}
	}
}
