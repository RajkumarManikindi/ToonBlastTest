using System.Collections.Generic;
using ToonBlast.Model;

namespace ToonBlast
{
    public class NormalPiece : IPiece
    {
        public NormalPiece(int pieceNumber = 0)
        {
            pieceTypeNumber = pieceNumber ;
        }

        public int pieceTypeNumber { get; set; }

        public bool powerPiece { get; set; }

        public List<IPiece> GetNeighbors(int x, int y, IPiece[,] boardState)
        {
            List<IPiece> neighbors = new List<IPiece>();
            GetNeighbor(x - 1, y,ref neighbors, boardState); // Left
            GetNeighbor(x, y - 1,ref neighbors, boardState); // Top
            GetNeighbor(x + 1, y,ref neighbors, boardState); // Right
            GetNeighbor(x, y + 1,ref neighbors, boardState); // Bottom

            return neighbors;
        }

        void GetNeighbor(int x, int y, ref List<IPiece> neighbors, IPiece[,] boardState)
        {
            if (x < boardState.GetLength(0) && y < boardState.GetLength(1) && x >= 0 && y >= 0)
            {
                var piece = boardState[x, y];
                if (!neighbors.Contains(piece)) {
                    neighbors.Add(piece);
                }
            }
        }
    }
    
    
}