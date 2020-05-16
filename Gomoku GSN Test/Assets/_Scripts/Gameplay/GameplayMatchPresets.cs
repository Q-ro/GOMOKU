/*
Author: Andres Mrad (Q-ro)
Creation Date : [ 2020/05 (May)/14 (Thu) ] @[ 10:55 ]
Description : Defines the current match presets
*/


using System;
using GameplayEnum;
using UnityEngine;

[Serializable]
public class GameplayMatchPresets
{
    #region Inspector Properties

    [SerializeField] GameplayEnum.GamePlayPieceTypes p1Pieces;
    [SerializeField] GameplayEnum.GamePlayPieceTypes p2Pieces;
    [SerializeField] GameplayEnum.GamePlayPlayerTypes p1Type;
    [SerializeField] GameplayEnum.GamePlayPlayerTypes p2Type;

    public GamePlayPieceTypes P1Pieces { get => p1Pieces; set => p1Pieces = value; }
    public GamePlayPieceTypes P2Pieces { get => p2Pieces; set => p2Pieces = value; }
    public GamePlayPlayerTypes P1Type { get => p1Type; set => p1Type = value; }
    public GamePlayPlayerTypes P2Type { get => p2Type; set => p2Type = value; }

    #endregion
}
