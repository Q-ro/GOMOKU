/*
Author: Andres Mrad (Q-ro)
Date: Wednesday 08/April/2020 @ 15:38:04
Description:  Chage the button's text color to provide better contrar and a better effect
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Make sure this script is used on a button
[RequireComponent (typeof (Button))]
public class ButtonContrastEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    #region Inspector Properties

    [SerializeField] private Color textContrastColor;

    #endregion

    #region Private Properties

    private Color _startingTextColor;
    private Text _buttonText;

    #endregion

    void Start () {
        // Assuming that every button must have a text child component
        this._buttonText = this.gameObject.GetComponentInChildren<Text> ();

        if (this._buttonText != null)
            this._startingTextColor = _buttonText.color;
    }

    void IPointerEnterHandler.OnPointerEnter (PointerEventData eventData) {
        if (this._buttonText == null)
            return;

        this._buttonText.color = this.textContrastColor;
    }

    void IPointerExitHandler.OnPointerExit (PointerEventData eventData) {
        if (this._buttonText == null)
            return;

        this._buttonText.color = this._startingTextColor;
    }
}