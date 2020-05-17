/*
Author: Andres Mrad (Q-ro)
Creation Date : [ 2020/05 (May)/16 (Sat) ] @[ 22:50 ]
Description : Handles the ingame menu for selecting the current game's preferences
*/


using UnityEngine;
using UnityEngine.UI;

public class GamePlayMatchPresetsSelectionMenu : MonoBehaviour
{
    #region Inspector Properties

    [SerializeField] Slider p1Pieces;
    [SerializeField] Slider p1Type;
    [SerializeField] Slider p2Type;

    #endregion

    #region Private Properties

    GameplayMatchPresets _currentMatchPresets = new GameplayMatchPresets(0, 0, 0, 0);

    #endregion

    public void SelectP1Pieces()
    {
        this._currentMatchPresets.P1Pieces = (GameplayEnum.GamePlayPieceTypes)this.p1Pieces.value;
        this.SelectP2Pieces();
    }

    private void SelectP2Pieces()
    {
        this._currentMatchPresets.P2Pieces = (this._currentMatchPresets.P1Pieces == GameplayEnum.GamePlayPieceTypes.O_Pieces) ? GameplayEnum.GamePlayPieceTypes.X_Pieces : GameplayEnum.GamePlayPieceTypes.O_Pieces;
    }

    public void SelectP1Type()
    {
        this._currentMatchPresets.P1Type = (GameplayEnum.GamePlayPlayerTypes)this.p1Type.value;

    }

    public void SelectP2Type()
    {
        this._currentMatchPresets.P2Type = (GameplayEnum.GamePlayPlayerTypes)this.p2Type.value;
    }


    public void PlayButtonSelectionCick()
    {
        GameManager.Instance.InitGamePlay(this._currentMatchPresets);
    }
}
