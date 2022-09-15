using System.Collections.Generic;
using ToonBlast.Model;


namespace ToonBlast
{
    public class VerticalPowerPiece : IPiece
    {
        public VerticalPowerPiece(int pieceNumber)
        {
            pieceTypeNumber = pieceNumber;
            this.powerPiece = true;
        }

        public int pieceTypeNumber { get; set; }

        public bool powerPiece { get; set; }


        public List<IPiece> GetNeighbors(int x, int y,IPiece[,] boardState) {
            List<IPiece> neighbors = new List<IPiece>();
                for (var i = 0; i < boardState.GetLength(1); i++) {
                    var piece = boardState[x, i];
                    if (!neighbors.Contains(piece)) {
                        neighbors.Add(piece);
                    }
                }

                return neighbors;
        }
        
    }
}