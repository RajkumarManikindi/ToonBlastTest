using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ToonBlast.Model;

namespace ToonBlast
{
    public class LevelSelectionManager : MonoBehaviour
    {
        [Header("Level Editor for fetching Levels from Scriptable object")]
        public LevelEditor levelEditor;
        
        [Header("Requirement prefab for showing requirements in Game Page")]
        [SerializeField] private GameObject requirement;
        [Header("Parent for Requirement prefab After Initializing")]
        [SerializeField] private GameObject levelRequirementLocation;
        [Header("Board to activate once level is selected")]
        [SerializeField] private Boot board;
        [Header("PieceTypeDatabase prefab for fetching sprite")]
        [SerializeField] public PieceTypeDatabase pieceTypeDatabase;

        public static int CurrentLevel;
        

        [Tooltip("We are using these for populating levels icons dynamically")]
        [Header("Level Selection Page Elements")]
        [SerializeField] private GameObject levelsPageParent;

        /// <summary>
        /// Requirements are used for the level
        /// </summary>
        public readonly List<Requirement> Requirements = new List<Requirement>();
        
        public static LevelSelectionManager Instance { get; private set; }

        private void Awake() 
        { 
            if (Instance != null && Instance != this) { 
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
        }
        private void Start() {
            EventTriggers.SetupLevelPage();
            EventTriggers.onNextLevel += NextLevel;
            EventTriggers.LevelRestart += ResetLevel;
        }
        
        private void OnDisable() {
            Instance = null;
            EventTriggers.onNextLevel -= NextLevel;
            EventTriggers.LevelRestart -= ResetLevel;
        }

        
        /// <summary>
        /// Here we are resetting all requirements 
        /// </summary>
        public void ClearRequirements()
        {
            if (Requirements.Count < 1) {
                return;
            }
            foreach (var t in Requirements) {
                t.transform.gameObject.SetActive(false);
            }
            ResetRequirements();
        }

        private void ResetRequirements()
        {
            if (Requirements.Count < 1) {
                return;
            }
            foreach (var t in Requirements) {
                t.ResetCurrentProgress();
            }

        }

        /// <summary>
        /// Fetching Position of the Requirement to animate particle element 
        /// </summary>
        public Transform GetPositionForRequirement(int type)
        {
            var selectedRequirement =  Requirements.Where(i => i.pieceColorNumber == type);
            return selectedRequirement.Select(iRequirement => iRequirement.transform).FirstOrDefault();
        }
        
        private void SetUpLevelRequirements()
        {
            var colorTarget = levelEditor.levels[CurrentLevel].colorTargetCount;
            ClearRequirements();
            for (var i = 0; i < colorTarget.Count; i++)
            {
                if (colorTarget[i] <= 0) continue;
                Requirement piece = null;
                if (Requirements.Count > i) {
                    piece = Requirements[i];
                }else{
                    piece = Instantiate(requirement, levelRequirementLocation.transform).GetComponent<Requirement>();
                    Requirements.Add(piece);
                }
                piece.gameObject.SetActive(true);
                piece.SetUpValues(pieceTypeDatabase.GetSpriteForPieceType(i), i, colorTarget[i]);
            }
            levelRequirementLocation.gameObject.SetActive(true);
        }
        
        
        /// <summary>
        /// When we select level from LevelSelection page
        /// Setting up level requirement
        /// Setting board
        /// Activating board
        /// Setting player score object
        /// </summary>
        /// <param name="levelNumber"></param>
        public void OnLevelSelect(int levelNumber)
        {
            GameManager.gameState = GameState.Started;
            
            CurrentLevel = levelNumber - 1;
             
            EventTriggers.OnUpdateMoveValue(); 
            EventTriggers.SetupLevelInfo();
            SetUpLevelRequirements();
            levelsPageParent.gameObject.SetActive(false);
            board.gameObject.SetActive(true);
            board.SetUp();

            if (!GameManager.playerScore.ContainsKey(CurrentLevel)) {
                GameManager.playerScore.Add(CurrentLevel, new LevelScore(CurrentLevel,0,new List<int>(),0));
            } 
            GameManager.playerScore[CurrentLevel].Reset();
        }

        public void ResetLevel()
        {
            ResetRequirements();
            board.SetUp();
        }
        public void NextLevel()
        {
            ClearRequirements();
            CurrentLevel = ++CurrentLevel <levelEditor.levels.Count? CurrentLevel : 0;
            OnLevelSelect(CurrentLevel+1);
        }

        public int GetTotalMovesCount()
        {
            return levelEditor.levels[CurrentLevel].targetMoves;
        }
    }
}
