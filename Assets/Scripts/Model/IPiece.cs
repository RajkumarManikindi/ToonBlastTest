using System.Collections.Generic;

namespace ToonBlast.Model
{
    public interface IPiece
    {
        int pieceTypeNumber { get; set; }
        string ToString();
        bool powerPiece { get; set; }
        List<IPiece> GetNeighbors(int x, int y, IPiece[,] boardState);
    }
    
    
}