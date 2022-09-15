using System.Collections.Generic;

namespace ToonBlast.Model {
    
    public interface IBoard {
        
        int Width { get; }
        int Height { get; }
        
        IPiece CreatePiece(int pieceType, int x, int y);
        
        IPiece GetAt(int x, int y);
        
        void MovePiece(int fromX, int fromY, int toX, int toY);
        void RemovePieceAt(int x, int y);
        
        List<IPiece> GetConnected(int x, int y);
        bool TryGetPiecePos(IPiece piece, out int px, out int py);

    }
    
}