/*
Author: Andres Mrad (Q-ro)
Date: Thursday 09/April/2020 @ 23:59:10
Description:  Keeps track of who's turn it is 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using GameplayEnum;
using UnityEngine;

public class GameplayTurnTracker
{

    #region Inspector Properties

    [SerializeField] bool isStartingPlayerRandom = true;
    [SerializeField] private GameObject currentTurnDisplay;
    [SerializeField] private GameObject currentTurnPlayerXDisplay;
    [SerializeField] private GameObject currentTurnPlayerODisplay;

    #endregion

    #region Private Properties

    private int _currentTurnNumber;
    private GamePlayPlayerTurnTypes _currentPlayerToMove;
    public GamePlayPlayerTurnTypes CurrentPlayerToMove { get => _currentPlayerToMove; }

    #endregion

    #region Class Methods

    /// <summary>
    /// Keeps track of who's turn it is and the current turn order/number
    /// </summary>
    /// <param name="isStartingPlayerRandom"></param>
    /// <param name="currentTurnDisplay"></param>
    /// <param name="currentTurnPlayerXDisplay"></param>
    /// <param name="currentTurnPlayerODisplay"></param>
    public GameplayTurnTracker(
        bool isStartingPlayerRandom,
        GameObject currentTurnDisplay,
        GameObject currentTurnPlayerXDisplay,
        GameObject currentTurnPlayerODisplay
        )
    {

        this.isStartingPlayerRandom = isStartingPlayerRandom;
        this.currentTurnDisplay = currentTurnDisplay;
        this.currentTurnPlayerXDisplay = currentTurnPlayerXDisplay;
        this.currentTurnPlayerODisplay = currentTurnPlayerODisplay;

    }

    internal void MoveToNextTurn()
    {
        this._currentPlayerToMove = (this._currentPlayerToMove == GamePlayPlayerTurnTypes.P1Turn) ? GamePlayPlayerTurnTypes.P2Turn : GamePlayPlayerTurnTypes.P1Turn;
        this.UpdateCurrentPlayerTurDisplaY();
        this._currentTurnNumber++;
    }

    internal void SelectStartingPlayer()
    {
        this._currentTurnNumber = 1;
        if (isStartingPlayerRandom)
            this._currentPlayerToMove = (GamePlayPlayerTurnTypes)UnityEngine.Random.Range(0, 2);
        else
            this._currentPlayerToMove = GamePlayPlayerTurnTypes.P1Turn;

        UpdateCurrentPlayerTurDisplaY();
    }

    private void UpdateCurrentPlayerTurDisplaY()
    {
        if (_currentPlayerToMove == GamePlayPlayerTurnTypes.P1Turn)
        {
            //Player X"
            this.currentTurnPlayerXDisplay.SetActive(true);
            this.currentTurnPlayerODisplay.SetActive(false);
        }
        else
        {
            //Player O
            this.currentTurnPlayerXDisplay.SetActive(false);
            this.currentTurnPlayerODisplay.SetActive(true);
        }
    }

    #endregion
}