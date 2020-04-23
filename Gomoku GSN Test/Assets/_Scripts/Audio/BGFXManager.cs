/*
Author: Andres Mrad (Q-ro)
Date: Wednesday 08/April/2020 @ 17:00:47
Description:  A singleton that ensure the continuous playing of the background music across multiple scenes 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioSource))]
public class BGFXManager : Singleton<BGFXManager>
{

    #region Inspector Properties

    [SerializeField] private AudioClip bgSoundtrack;

    #endregion

    #region Private Properties

    private AudioSource _bgAudioSource;
    private AudioSource _sfxAudioSource;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        var temp = this.GetComponents<AudioSource>();
        this._bgAudioSource = temp[0];
        this._sfxAudioSource = temp[1];
        this._bgAudioSource.clip = bgSoundtrack;
        this._bgAudioSource.Play();

        this._bgAudioSource.loop = true;
    }

    public void PlaySFX(AudioClip sfx)
    {
        if (this._sfxAudioSource)
            this._sfxAudioSource.PlayOneShot(sfx);
    }

}