using ToonBlast.Model;
using UnityEngine;

namespace ToonBlast.ViewComponents {

	public class BoardRenderer : MonoBehaviour {
		
		[SerializeField] private PieceTypeDatabase pieceTypeDatabase;
		[SerializeField] private VisualPiece visualPiecePrefab;
		[SerializeField] private GameManager gameManager;
		private Board board;
		private float lastClick;
		private const float PieceFallSpeed = 0.6f;
		public void Initialize(Board board) {
			this.board = board;

			CenterCamera();
			CreateVisualPiecesFromBoardState();
		}

		private void CenterCamera() {
			Camera.main.transform.position = new Vector3((board.Width-1)*0.5f,-(board.Height-1)*0.5f);
		}

		private void CreateVisualPiecesFromBoardState() {
			DestroyVisualPieces();

			foreach (var pieceInfo in board.IteratePieces()) {
				
				var visualPiece = CreateVisualPiece(pieceInfo.piece);
				visualPiece.transform.localPosition = LogicPosToVisualPos(pieceInfo.pos.x, pieceInfo.pos.y);

			}
		}
		
		public Vector3 LogicPosToVisualPos(float x,float y) { 
			return new Vector3(x, -y, -y);
		}

		private BoardPos ScreenPosToLogicPos(float x, float y) { 
			
			var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(x,y,-Camera.main.transform.position.z));
			var boardSpace = transform.InverseTransformPoint(worldPos);

			return new BoardPos() {
				x = Mathf.RoundToInt(boardSpace.x),
				y = -Mathf.RoundToInt(boardSpace.y)
			};

		}

		private VisualPiece CreateVisualPiece(IPiece piece) {
			
			var pieceObject = Instantiate(visualPiecePrefab, transform, true);
			var sprite = pieceTypeDatabase.GetSpecialSpriteForPieceType(0);
			if (piece.powerPiece) 
			{
				sprite = pieceTypeDatabase.GetSpecialSpriteForPieceType(piece.pieceTypeNumber);
			}
			else
			{
				sprite = pieceTypeDatabase.GetSpriteForPieceType(piece.pieceTypeNumber);
			}

			pieceObject.SetSprite(sprite);
			return pieceObject;
			
		}

		private void DestroyVisualPieces() {
			foreach (var visualPiece in GetComponentsInChildren<VisualPiece>()) {
				Object.Destroy(visualPiece.gameObject);
			}
		}

		private void Update() {
			if (GameManager.gameState != GameState.Started || board == null) {
				return;
			}

			if (lastClick + PieceFallSpeed > Time.time)
			{
				return;
			}

			if (Input.GetMouseButtonDown(0))
			{
				lastClick = Time.time;
				var pos = ScreenPosToLogicPos(Input.mousePosition.x, Input.mousePosition.y);

				if (!board.IsWithinBounds(pos.x, pos.y)) return;
				//Adding the connected piece to GameManager to find all the pieces count
				var connections = new ResolveResult();
				board.FindAndRemoveConnectedAt(pos.x, pos.y, connections);
				gameManager.AddToCollectedPiece(connections, ref pieceTypeDatabase);
				var result = board.Resolve();
					
				DestroyVisualPieces();
				UpdateBoardVisuals(result);

			}
		}
		/// <summary>
		/// taking list of pieces 
		/// And Applying Tween animation
		/// </summary>
		/// <param name="result"></param>
		private void UpdateBoardVisuals(ResolveResult result)
		{
			foreach (var pieceInfo in board.IteratePieces()) {
				var visualPiece = CreateVisualPiece(pieceInfo.piece).transform;
				if (result.changes.ContainsKey(pieceInfo.piece)) {
					var selectedPiece = result.changes[pieceInfo.piece];
					var fromVal = LogicPosToVisualPos(selectedPiece.FromPos.x,selectedPiece.FromPos.y);
					fromVal.y += selectedPiece.WasCreated ?  selectedPiece.CreationTime : 0;
					visualPiece.localPosition = fromVal;
					var toVal = LogicPosToVisualPos(selectedPiece.ToPos.x,selectedPiece.ToPos.y);
					Tween.Instance.Move(visualPiece, toVal, PieceFallSpeed);
				}else {
					visualPiece.localPosition = LogicPosToVisualPos(pieceInfo.pos.x, pieceInfo.pos.y);
				}
			}
		}
	}

}
