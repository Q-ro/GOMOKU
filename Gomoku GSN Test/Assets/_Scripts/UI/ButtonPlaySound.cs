/*
Author: Andres Mrad (Q-ro)
Date: Wednesday 08/April/2020 @ 16:17:11
Description: Add sound effect to the mouse over event 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent (typeof (AudioSource))] // Ensure we have an audiosource on the game object
public class ButtonPlaySound : MonoBehaviour, IPointerEnterHandler {

    #region Inspector properties

    [SerializeField] private AudioClip mouseOverSound;

    #endregion

    #region Private Properties

    private AudioSource _mouseOverAudioSource;

    #endregion

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start () {
        // Store a reference to the object{s } audiosource
        this._mouseOverAudioSource = this.GetComponent<AudioSource> ();
    }

    void IPointerEnterHandler.OnPointerEnter (PointerEventData eventData) {
        // Attempt to play the sound
        // Else Fall back to whatever sound the audiosource has been asigned
        if (this.mouseOverSound != null)
            this._mouseOverAudioSource.PlayOneShot (this.mouseOverSound);
        else
            this._mouseOverAudioSource.PlayOneShot (this._mouseOverAudioSource.clip);
    }

}