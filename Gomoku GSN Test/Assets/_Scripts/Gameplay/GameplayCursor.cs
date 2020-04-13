/*
Author: Andres Mrad (Q-ro)
Date: Thursday 09/April/2020 @ 16:20:41
Description:  Used to visually inicate where the current player wants to place a game piece 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayCursor : MonoBehaviour {

    #region Inspector Properties

    [SerializeField] private GameplayBoard gamePlayBoard; // Stores a reference to the gameplayboard
    [SerializeField] private GameplayTurnTracker gamePlayTurnTracker; // Stores a reference to the gameplay turn tracker

    #endregion

    #region Private Properties

    int _currentXPosition;
    int _currentYPosition;

    public GameplayBoard GamePlayBoard { get => gamePlayBoard; set => gamePlayBoard = value; }
    public GameplayTurnTracker GamePlayTurnTracker { get => gamePlayTurnTracker; set => gamePlayTurnTracker = value; }

    public int CurrentXPosition { get => _currentXPosition;  }
    public int CurrentYPosition { get => _currentYPosition;  }

    #endregion

    #region Unity Methods


    #endregion

    #region Class Methods

    public void InitCursor()
    {
        var tempWorldPosition = this.gamePlayBoard.GetBoardWorldPositionCenter();
        var tempBoardGridXYPosition = this.gamePlayBoard.GetBoardMatrixPosition(tempWorldPosition);
        this.MoveCursorToBoardPosition(tempBoardGridXYPosition);
        this.MoveCursorToWorldPosition(this._currentXPosition, this._currentYPosition);
    }

    public Vector2Int GetCursorPositionVector()
    {
        return new Vector2Int(this._currentXPosition, this._currentYPosition);
    }

    public void MoveCursorToGivenXYPoint(Vector2Int positionXY) {

        // Remove 1 from x and y to account for the borders
        this.MoveCursorToBoardPosition(positionXY- new Vector2Int(-1,-1));
        this.MoveCursorToWorldPosition(this._currentXPosition, this._currentYPosition);
    }

    public void MoveCursorXYAmount(int x, int y)
    {
        MoveCursor(this._currentXPosition + x, this._currentYPosition+y);
    }

        private void MoveCursor(int x, int y)
    {
        this.MoveCursorToBoardPosition(new Vector2(x, y));
        this.MoveCursorToWorldPosition(this._currentXPosition, this._currentYPosition);
    }

    private void MoveCursorToBoardPosition (Vector2 boardGridPositionXY) {
        this._currentXPosition = Mathf.Clamp ((int) boardGridPositionXY.x, 1, this.gamePlayBoard.BoardWidth);
        this._currentYPosition = Mathf.Clamp ((int) boardGridPositionXY.y, 1, this.gamePlayBoard.BoardHeight);
    }

    private void MoveCursorToWorldPosition (int x, int y) {
        // Remove 1 due to the board borders
        x -= 1;
        y -= 1;
        this.transform.position = this.gamePlayBoard.GetWorldPositionFromBoardXY (x, y);
    }

    #endregion
}