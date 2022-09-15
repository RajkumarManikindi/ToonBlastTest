namespace ToonBlast.Model {

	public class PieceSpawner : IPieceSpawner
	{
		private const int MinPieceType = (int) PieceTypes.Blue;
		private const int MaxPieceType = (int) PieceTypes.Yellow + 1;
		private const int MinSpecialPieceType = (int) SpecialPieceTypes.HorizontalPower;
		private const int MaxSpecialPieceType = (int) SpecialPieceTypes.BombPower + 1;

		public IPiece CreateBasicPiece() {
			int pieceType = UnityEngine.Random.Range(MinPieceType,MaxPieceType);
			return CreateBasicPiece(pieceType);
		}

		public IPiece CreateBasicPiece(int pieceType)
		{
			IPiece piece = new NormalPiece();
			piece = new NormalPiece(pieceType);
	      
			return piece;
		}

		private int RandomSpecialPiece() {
			return UnityEngine.Random.Range(MinSpecialPieceType, MaxSpecialPieceType);
		}

		public IPiece CteatSpecialPiece( )
		{
			int pieceType = RandomSpecialPiece();
			IPiece piece = new NormalPiece();
	        
			switch (pieceType)
			{
				case 0:
					piece = new HorizontalPowerPiece(pieceType);
					break;
				case 1:
					piece = new VerticalPowerPiece(pieceType);
					break;
				case 2:
					piece = new BombPowerPiece(pieceType);
					break;
		        
			}
			
			return piece;
		}
	}

}