/*
Author: Andres Mrad (Q-ro)
Date: Thursday 09/April/2020 @ 23:52:00
Description:  Cheap and easy way to determine and keep track of the piece type for a given piece on the board 
*/

using System.Collections;
using System.Collections.Generic;
using GameplayEnum;
using UnityEngine;

public class GamePiece : MonoBehaviour {
    private GameplayEnum.GamePlayPieceTypes _gamePieceType = GamePlayPieceTypes.NONE;
    public GamePlayPieceTypes GamePieceType { get => _gamePieceType; set => _gamePieceType = value; }
}