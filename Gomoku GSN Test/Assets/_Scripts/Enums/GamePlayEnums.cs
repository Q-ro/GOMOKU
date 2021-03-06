/*
Author: Andres Mrad (Q-ro)
Date: Thursday 09/April/2020 @ 17:59:54
Description:  Game pieces enums 
*/

namespace GameplayEnum
{
    public enum GamePlayPieceTypes
    {
        NONE,
        X_Pieces,
        O_Pieces
    }

    public enum GamePlayPlayerTypes
    {
        Human,
        Bot,
    }

    public enum GamePlayPlayerTurnTypes
    {
        P1Turn,
        P2Turn
    }

    public enum GamePlayMovementScore
    {
        ONE = 0,
        TWO = 20,
        THREE = 80,
        FOUR = 320,
        FIVE = 1280,
        BLOCKED_ONE = 0,
        BLOCKED_TWO = 5,
        BLOCKED_THREE = 20,
        BLOCKED_FOUR = 80,
        TWO_THREE = 310,
        THREE_THREE = 270,
        TWO_TWO = 40
    }
}