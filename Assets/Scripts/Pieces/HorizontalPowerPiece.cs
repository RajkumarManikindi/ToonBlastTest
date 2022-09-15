using System.Collections.Generic;
using ToonBlast.Model;

namespace ToonBlast
{
    public class HorizontalPowerPiece : IPiece
    {
        public HorizontalPowerPiece (int pieceNumber)
        {
            pieceTypeNumber = pieceNumber;
            this.powerPiece = true;
        }

        public int pieceTypeNumber { get; set; }

        public bool powerPiece { get; set; }

        public List<IPiece> GetNeighbors(int x, int y, IPiece[,] boardState) {
            List<IPiece> neighbors = new List<IPiece>();
            for (int i = 0; i < boardState.GetLength(0); i++) {
                var piece = boardState[i, y];
                if (!neighbors.Contains(piece)) {
                    neighbors.Add(piece);
                }
            }
            return neighbors;
           
        }
    }
    
    
}