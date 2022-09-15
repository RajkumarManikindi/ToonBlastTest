using System.Collections.Generic;
using System.Linq;
using ToonBlast.Model;

namespace ToonBlast {
    
    public class Board : IBoard {
        
        private IPiece[,] boardState;
        private readonly IPieceSpawner pieceSpawner;
        public static Board Create(int[,] definition, IPieceSpawner pieceSpawner) {
            return new Board(definition, pieceSpawner);
        }
        
        public int Width {
            get { return boardState.GetLength(0); }
        }
        
        public int Height {
            get { return boardState.GetLength(1); }
        }
        
        public Board(int[,] definition, IPieceSpawner pieceSpawner) {
            
            this.pieceSpawner = pieceSpawner;
            var transposed = ArrayUtility.TransposeArray(definition);
            CreatePieces(transposed);
            
        }

        private void CreatePieces(int[,] array) {
            
            var defWidth = array.GetLength(0);
            var defHeight = array.GetLength(1);
            
            boardState = new IPiece[defWidth,defHeight];
            
            for (int y = 0; y < defHeight; y++) {
                for (int x = 0; x < defWidth; x++)
                {
	                IPiece piece = pieceSpawner.CreateBasicPiece(array[x,y]);
                   boardState[x, y] = piece;
                }
            }
        }
        
       
       
        public ResolveResult Resolve() {
	        return MoveAndCreatePiecesUntilFull();
        }

        public IPiece CreatePiece(int pieceType, int x, int y)
        {
	        throw new System.NotImplementedException();
        }

        public IPiece GetAt(int x, int y) {
            return boardState[x, y];
        }
        
        public IEnumerable<PiecePosition> IteratePieces() {
            for (var y = 0; y < Height; y++) {
                for (var x = 0; x < Width; x++) {
                    yield return new PiecePosition() {
                        piece = boardState[x, y],
                        pos = new BoardPos(x, y)
                    };
                }
            }
        }

        public void MovePiece(int fromX, int fromY, int toX, int toY) {
            boardState[toX, toY] = boardState[fromX, fromY];
            boardState[fromX, fromY] = null;
        }
        
        public bool IsWithinBounds(int x, int y) {
            
            if (x < Width && y < Height && x >= 0 && y >= 0) {
                return true;
            }
            return false;
        } 
        
        public void RemovePieceAt(int x, int y) {
            boardState[x, y] = null;
        }
        
        public bool TryGetPiecePos(IPiece piece, out int px, out int py) {
               for (int y = 0; y < Height; y++) {
                   for (int x = 0; x < Width; x++) {
                       if (boardState[x, y] == piece) {
                           px = x;
                           py = y;
                           return true;
                       }
                   }
               }

               px = -1;
               py = -1;
               return false;
        }
        
        public List<IPiece> GetConnected(int x, int y) {
            var start = GetAt(x, y);
            return SearchForConnected(start, new List<IPiece>());
        }

        private List<IPiece> SearchForConnected(IPiece piece, List<IPiece> searched) {
            int x, y;
            if (!TryGetPiecePos(piece, out x, out y)) {
                return searched;
            }

            searched.Add(piece);
            var neighbors = (piece)?.GetNeighbors(x, y, boardState);
            if (neighbors.Count == 0) {
                return searched;
            }
            
            foreach (var t in neighbors.Where(t => !searched.Contains(t)))
            {
	            if(piece.powerPiece)
	            {
		            if (t.powerPiece) 
		            {
			            SearchForConnected(t, searched);      
		            } else {
			            searched.Add(t); 
		            }
	            }
	            else if(t.GetType() == piece.GetType() && t.pieceTypeNumber == piece.pieceTypeNumber)
	            {
		            SearchForConnected(t, searched);		            
	            }
            }
            return searched;
        }
        
         
        
        

		public ResolveResult MoveAndCreatePiecesUntilFull() {
			
			var result = new ResolveResult();
			
			int resolveStep = 0;
			bool moreToResolve = true;
			
			while (moreToResolve) {
				moreToResolve = MovePiecesOneDownIfAble(result);
				moreToResolve |= CreatePiecesAtTop(result, resolveStep);
				resolveStep++;
			}

			return result;
		}

		private void RemovePieces(List<IPiece> connections, ResolveResult resolveResult = null) {
			foreach (var piece in connections) {
				int x,y;
				if(TryGetPiecePos(piece, out x, out y)){ 
					AddToResolveList(piece, resolveResult, x, y);
					RemovePieceAt(x,y);
				}
			}
		}
		
		private bool CreatePiecesAtTop(ResolveResult resolveResult, int resolveStep) {
			var createdAnyPieces = false;
			var y = 0;
			for (int x = 0; x < Width; x++) {
				if (GetAt(x, y) == null)
				{
					var piece = pieceSpawner.CreateBasicPiece();
					boardState[x, y] = piece;
					createdAnyPieces = true;
                    
					resolveResult.changes[piece] = new ChangeInfo(){
						CreationTime = resolveStep,
						WasCreated = true,
						ToPos = new BoardPos(x,y),
						FromPos = new BoardPos(x,y-1)
					};
				}
			}

			return createdAnyPieces;
		}

		private bool MovePiecesOneDownIfAble(ResolveResult resolveResult) {
			
			bool movedAny = false;
			
			for (int y = Height - 1; y >= 1; y--) {
				for (int x = 0; x < Width; x++) {
					
					var dest = GetAt(x, y);
					if (dest != null) {
						continue;
					}
					
					var pieceToMove = GetAt(x, y - 1);
					if (pieceToMove == null) {
						continue;
					}

					var fromX = x;
					var fromY = y - 1;
					MovePiece(fromX,fromY, x, y);
					movedAny = true;
					
					if(!resolveResult.changes.ContainsKey(pieceToMove)) {
						resolveResult.changes[pieceToMove] = new ChangeInfo();
						resolveResult.changes[pieceToMove].FromPos = new BoardPos(fromX,fromY);
					};
					resolveResult.changes[pieceToMove].ToPos = new BoardPos(x,y);
					
				}
			}

			return movedAny;
		}
		public void FindAndRemoveConnectedAt(int x, int y, ResolveResult resolveResult) {
			var connections = GetConnected(x, y); 
			// Checking are we using any power piece to get morethan MinimumPieceCount
			if (connections.Count >= LevelSelectionManager.Instance.levelEditor.minimumPieceCount && !boardState[x, y].powerPiece)
			{
				var piece = connections[0];
				AddToResolveList(piece, resolveResult, x, y);
				connections.Remove(piece);

				boardState[x, y] = pieceSpawner.CteatSpecialPiece(); 
			}
			
			if (connections.Count > 1) {
				RemovePieces(connections, resolveResult);
			}
		}

		private static void AddToResolveList(IPiece piece,ResolveResult resolveResult, int x, int y)
		{
			if(!resolveResult.changes.ContainsKey(piece)) {
				resolveResult.changes[piece] = new ChangeInfo();
				resolveResult.changes[piece].FromPos = new BoardPos(x, y);
			};
		}
    }
}