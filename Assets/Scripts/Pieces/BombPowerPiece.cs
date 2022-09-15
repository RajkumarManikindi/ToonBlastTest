using System.Collections.Generic;
using ToonBlast.Model;

namespace ToonBlast
{
    public class BombPowerPiece : IPiece
    {
        public BombPowerPiece(int pieceNumber)
        {
            pieceTypeNumber = pieceNumber;
            this.powerPiece = true;
        }

        public int pieceTypeNumber { get; set; }

        public bool powerPiece { get; set; }
      

        public List<IPiece> GetNeighbors(int x, int y,IPiece[,] boardState)
        {
            List<IPiece> neighbors = new List<IPiece>();
            for (var xIndex = -1; xIndex <= 1; ++xIndex) {
                for (var yIndex = -1; yIndex <= 1; ++yIndex) {
                    if (x + xIndex < boardState.GetLength(0) && y + yIndex < boardState.GetLength(1) && x + xIndex >= 0 && y + yIndex >= 0)
                    {
                        var piece = boardState[x+xIndex, y+yIndex];
                        if (!neighbors.Contains(piece)) {
                            neighbors.Add(piece);
                        }
                    }
                    
                }
            }

            return neighbors;

        }
    }
}