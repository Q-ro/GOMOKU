using GameplayEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Inspector Properties


    [SerializeField] private AudioClip gameEndedSFX;
    [SerializeField] private AudioClip gameStartedSFX;
    [SerializeField] private AudioClip playerXMoveSFX;
    [SerializeField] private AudioClip playerOMoveSFX;

    [SerializeField] private GameObject currentTurnDisplay;
    [SerializeField] private GameObject currentTurnPlayerXDisplay;
    [SerializeField] private GameObject currentTurnPlayerODisplay;
    [SerializeField] private GameObject endGameMenuContainer;
    [SerializeField] private Text winnerPlaverDisplay;
    [SerializeField] private GameplayBoard gameplayBoardPrefab;
    [SerializeField] private GameplayCursor gameplayCursorPrefab;
    //[SerializeField] private GameplayTurnTracker gameplayTurnTrackerPrefab;
    [SerializeField] private AIPlayerController gameplayAIControllerPrefab;
    // [SerializeField] private bool vsAIGame = true;

    [SerializeField] private GameplayMatchPresets matchPresets;
    // [SerializeField] private float aiDlay = 2.0f;

    [SerializeField] private FloatRange delayRange;

    #endregion

    #region Private Properties

    private GameplayBoard gamePlayBoard;
    private GameplayCursor gameplayCursor;
    private GameplayTurnTracker gamePlayTurnTracker;
    private AIPlayerController gameplayAIController;

    private GamePlayPlayerTypes _player1Type = GamePlayPlayerTypes.Human;
    private GamePlayPlayerTypes _player2Type = GamePlayPlayerTypes.Bot;

    private bool _isVerticalAxisInUse = false;
    private bool _isHorizontalAxisInUse = false;
    private bool _isGameOver = false;
    private bool _isGameIntroOver = false;
    private bool _isWaitForAIMoveOver = false;
    private bool _canAIMakeAMove = true;
    // private Coroutine waitForAIMove;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        this.InitGamePlay();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Do Nothing if the game is already over
        if (!this._isGameIntroOver)
            return;

        // Do Nothing if the game is already over
        if (this._isGameOver)
            return;

        //A.I.'s turn
        // if (this.matchPresets.P2Type == GameplayEnum.GamePlayPlayerTypes.Bot)
        if (this.GetGivenPlayerType(this.gamePlayTurnTracker.CurrentPlayerToMove) == GamePlayPlayerTypes.Bot)
        {
            //if (this._isWaitForAIMoveOver)
            //    return;
            //else
            if (this._canAIMakeAMove)
                StartCoroutine(DelayedAIMove());
            // this.waitForAIMove = StartCoroutine(DelayedAIMove());

            // // // Added this last minute as a way to start work on V2.0 and forgot to take it out before making the build
            // // // which made the game unplayable
            // // if ((this.GetGivenPlayerType(this.gamePlayTurnTracker.CurrentPlayerToMove) == GamePlayPlayerTypes.Bot) && this._isWaitForAIMoveOver)
            // // // if ((this.GetGivenPlayerType(this.gamePlayTurnTracker.CurrentPlayerToMove) == GamePlayPlayerTypes.Bot))
            // // {
            // //     Vector2Int tempAIMove = this.gameplayAIController.MakeMove(this.gamePlayBoard);
            // //     this.gameplayCursor.MoveCursorToGivenXYPoint(tempAIMove);
            // //     //this.gamePlayBoard.PlaceGamePiece(tempAIMove);
            // //     this.PlaceGamePiece();
            // // }
        }

        // Take no inputs if the current player is not a human
        if (this.GetGivenPlayerType(this.gamePlayTurnTracker.CurrentPlayerToMove) == GamePlayPlayerTypes.Bot)
            return;

        this.HandlePlayerInput();

    }

    private void HandlePlayerInput()
    {
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            if (!_isVerticalAxisInUse)
            {
                this._isVerticalAxisInUse = true;
                this.gameplayCursor.MoveCursorXYAmount(0, 1);

                //this.MoveCursor(this._currentXPosition, this._currentYPosition + 1);
                //this.MoveCursorToBoardPosition (new Vector2 (this._currentXPosition, this._currentYPosition + 1));
                //this.MoveCursorToWorldPosition (this._currentXPosition, this._currentYPosition);
            }
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            if (!_isVerticalAxisInUse)
            {
                this._isVerticalAxisInUse = true;

                this.gameplayCursor.MoveCursorXYAmount(0, -1);
                //this.MoveCursor(this._currentXPosition, this._currentYPosition - 1);

                //this.MoveCursorToBoardPosition (new Vector2 (this._currentXPosition, this._currentYPosition - 1));
                //this.MoveCursorToWorldPosition (this._currentXPosition, this._currentYPosition);
            }
        }
        else if (Input.GetAxisRaw("Vertical") == 0)
        {
            this._isVerticalAxisInUse = false;
        }

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            if (!_isHorizontalAxisInUse)
            {
                this._isHorizontalAxisInUse = true;

                this.gameplayCursor.MoveCursorXYAmount(1, 0);

                //this.MoveCursor(this._currentXPosition + 1, this._currentYPosition);
                //this.MoveCursorToBoardPosition (new Vector2 (this._currentXPosition + 1, this._currentYPosition));
                //this.MoveCursorToWorldPosition (this._currentXPosition, this._currentYPosition);
            }
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            if (!_isHorizontalAxisInUse)
            {
                this._isHorizontalAxisInUse = true;

                this.gameplayCursor.MoveCursorXYAmount(-1, 0);

                //this._gameplayCursorReference.MoveCursor(this._currentXPosition - 1, this._currentYPosition);
                //this.MoveCursorToBoardPosition (new Vector2(this._currentXPosition - 1, this._currentYPosition));
                //this.MoveCursorToWorldPosition (this._currentXPosition, this._currentYPosition);
            }
        }
        else if (Input.GetAxisRaw("Horizontal") == 0)
        {
            this._isHorizontalAxisInUse = false;
        }

        if (Input.GetAxisRaw("Jump") > 0)
        {
            this.PlaceGamePiece();
        }
    }

    private GamePlayPlayerTypes GetGivenPlayerType(GamePlayPlayerTurnTypes player)
    {
        if (player == GamePlayPlayerTurnTypes.P1Turn)
            return this._player1Type;
        if (player == GamePlayPlayerTurnTypes.P2Turn)
            return this._player2Type;

        return this._player1Type;
    }

    public void PlaceGamePiece()
    {
        var pieceType = (this.gamePlayTurnTracker.CurrentPlayerToMove == GamePlayPlayerTurnTypes.P1Turn) ? GamePlayPieceTypes.X_Pieces : GamePlayPieceTypes.O_Pieces;


        if (this.gamePlayBoard.PlaceGamePiece(pieceType, this.gameplayCursor.CurrentXPosition, this.gameplayCursor.CurrentYPosition))
        {
            this.PlaySFXForMove(pieceType);
            // If the piece that was place didn't result in a win
            if (!CheckGameOver(this.gameplayCursor.CurrentXPosition - 1, this.gameplayCursor.CurrentYPosition - 1, (int)pieceType))
                this.gamePlayTurnTracker.MoveToNextTurn();// Go to the next turn
            else
                this.EndGame();
        }
    }

    private void PlaySFXForMove(GamePlayPieceTypes pieceType)
    {
        if (GamePlayPieceTypes.X_Pieces == pieceType)
            BGFXManager.Instance.PlaySFX(this.playerXMoveSFX);
        else
            BGFXManager.Instance.PlaySFX(this.playerOMoveSFX);
    }

    private void InitGamePlay()
    {
        this.endGameMenuContainer.SetActive(false);
        this._isGameIntroOver = false;

        this.gamePlayBoard = Instantiate(this.gameplayBoardPrefab);
        this.gamePlayBoard.gameObject.layer = this.gameObject.layer;
        this.gameplayCursor = Instantiate(this.gameplayCursorPrefab);


        //this.gamePlayTurnTracker = Instantiate(this.gameplayTurnTrackerPrefab);
        this.gamePlayTurnTracker = new GameplayTurnTracker(false, this.currentTurnDisplay, this.currentTurnPlayerXDisplay, this.currentTurnPlayerODisplay);

        this.gamePlayTurnTracker.SelectStartingPlayer();
        this.gameplayCursor.GamePlayTurnTracker = this.gamePlayTurnTracker;

        this.gameplayCursor.GamePlayBoard = this.gamePlayBoard;
        this.gameplayCursor.InitCursor();

        // // if (vsAIGame)
        // // {
        // //     this._player1Type = GamePlayPlayerTypes.Human;
        // //     this._player2Type = GamePlayPlayerTypes.Bot;
        // //     this.gameplayAIController = Instantiate(this.gameplayAIControllerPrefab);
        // // }
        // // else
        // // {
        // //     this._player2Type = GamePlayPlayerTypes.Human;
        // //     this._player1Type = GamePlayPlayerTypes.Human;
        // // }

        this._player1Type = this.matchPresets.P1Type;
        this._player2Type = this.matchPresets.P2Type;
        this.gameplayAIController = Instantiate(this.gameplayAIControllerPrefab);

        StartCoroutine(WaitForGameIntro());

    }

    private bool CheckGameOver(int x, int y, int role)
    {
        //水平分数
        int score = 1;
        for (int i = 1; i < 5; i++)
        {
            if (x + i < this.gamePlayBoard.BoardWidth)
            {
                if (this.gamePlayBoard.SimplifiedGameBoard[x + i, y] == role)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= 5)
        {
            return true;
        }
        for (int i = 1; i < 5; i++)
        {
            if (x - i >= 0)
            {
                if (this.gamePlayBoard.SimplifiedGameBoard[x - i, y] == role)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= 5)
        {
            return true;
        }

        //垂直分数
        score = 1;
        for (int i = 1; i < 5; i++)
        {
            if (y + i < this.gamePlayBoard.BoardHeight)
            {
                if (this.gamePlayBoard.SimplifiedGameBoard[x, y + i] == role)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= 5)
        {
            return true;
        }
        for (int i = 1; i < 5; i++)
        {
            if (y - i >= 0)
            {
                if (this.gamePlayBoard.SimplifiedGameBoard[x, y - i] == role)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= 5)
        {
            return true;
        }

        //斜线分数
        score = 1;
        for (int i = 1; i < 5; i++)
        {
            if (x + 1 < this.gamePlayBoard.BoardWidth && y + i < this.gamePlayBoard.BoardHeight)
            {
                if (this.gamePlayBoard.SimplifiedGameBoard[x + i, y + i] == role)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= 5)
        {
            return true;
        }
        for (int i = 1; i < 5; i++)
        {
            if (x - i >= 0 && y - i >= 0)
            {
                if (this.gamePlayBoard.SimplifiedGameBoard[x - i, y - i] == role)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= 5)
        {
            return true;
        }

        //斜线分数
        score = 1;
        for (int i = 1; i < 5; i++)
        {
            if (x + i < this.gamePlayBoard.BoardWidth && y - i >= 0)
            {
                if (this.gamePlayBoard.SimplifiedGameBoard[x + i, y - i] == role)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= 5)
        {
            return true;
        }
        for (int i = 1; i < 5; i++)
        {
            if (x - i >= 0 && y + i < this.gamePlayBoard.BoardHeight)
            {
                if (this.gamePlayBoard.SimplifiedGameBoard[x - i, y + i] == role)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= 5)
        {
            return true;
        }
        return false;

    }

    private void EndGame()
    {
        this._isGameOver = true;
        BGFXManager.Instance.PlaySFX(this.gameEndedSFX);
        StartCoroutine(WaitForGameOver());

    }

    private IEnumerator WaitForGameOver()
    {
        yield return new WaitForSecondsRealtime(2f);
        this.endGameMenuContainer.SetActive(true);
        this.winnerPlaverDisplay.text = (this.gamePlayTurnTracker.CurrentPlayerToMove == GamePlayPlayerTurnTypes.P1Turn) ? "Player X" : "Player O";
    }

    private IEnumerator WaitForGameIntro()
    {
        BGFXManager.Instance.PlaySFX(this.gameStartedSFX);
        yield return new WaitForSecondsRealtime(1.5f);
        this._isGameIntroOver = true;

    }

    // Add a small delay between A.I.'s turns
    private IEnumerator DelayedAIMove()
    {
        this._canAIMakeAMove = false;

        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(this.delayRange.MinRange, this.delayRange.MaxRange));

        Vector2Int tempAIMove = this.gameplayAIController.MakeMove(this.gamePlayBoard);
        this.gameplayCursor.MoveCursorToGivenXYPoint(tempAIMove);
        //this.gamePlayBoard.PlaceGamePiece(tempAIMove);
        this.PlaceGamePiece();

        this._canAIMakeAMove = true;
    }
}