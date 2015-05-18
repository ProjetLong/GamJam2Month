﻿using UnityEngine;
using System.Collections;

public class SoundManager : Singleton<SoundManager> {
    #region Properties
    [Header ("Sources")]
    [SerializeField]
    [Tooltip ("Audio source which will play the music")]
    AudioSource _musicSource;
    [SerializeField]
    [Tooltip ("Audio source which will play the SFX")]
    AudioSource _sfxSource;

    [Header ("Sound Variations")]
    [SerializeField]
    [Tooltip ("Variation of the pitch of a SFX that is randomly played")]
    float _pitchVariation = 0.1f;
    #endregion

    #region API
    public void PlayMusic () {
        _musicSource.Play ();
    }

    public void PlaySingle (AudioClip clip) {
        _sfxSource.clip = clip;
        _sfxSource.Play ();
    }

    public void RandomizeSfx (params AudioClip[] clips) {
        _sfxSource.clip = clips[Random.Range (0, clips.Length)];
        _sfxSource.pitch = Mathf.Clamp (Random.Range (
            1 - _pitchVariation,
            1 + _pitchVariation
        ), -3, 3);
        _sfxSource.Play ();
    }

    public void StopMusic () {
        _musicSource.Stop ();
    }
    #endregion

    #region Unity
    
    #endregion
}