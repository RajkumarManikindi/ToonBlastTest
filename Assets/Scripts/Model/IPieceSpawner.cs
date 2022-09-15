namespace ToonBlast.Model {

	public interface IPieceSpawner {
		
		IPiece CreateBasicPiece();
		
		IPiece CreateBasicPiece(int pieceType);

		IPiece CteatSpecialPiece();
	}

}