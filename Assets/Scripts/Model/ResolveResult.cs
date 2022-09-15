using System.Collections.Generic;

namespace ToonBlast.Model {

	public class ResolveResult {
		public readonly Dictionary<IPiece, ChangeInfo> changes = new Dictionary<IPiece, ChangeInfo>();
	}

}