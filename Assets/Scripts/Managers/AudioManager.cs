using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private int _audioSourcePoolSize;

    [Header("Chomper audios")]
    [SerializeField] private AudioClip _chomperAttack;
    [SerializeField] private AudioClip _chomperIdle;
    [SerializeField] private AudioClip _chomperStep;
    [SerializeField] private AudioClip _chomperDeath;

    [Header("Spitter audios")]
    [SerializeField] private AudioClip _spitterAttack;
    [SerializeField] private AudioClip _spitterIdle;
    [SerializeField] private AudioClip _spitterStep;
    [SerializeField] private AudioClip _spitterDeath;

    [Header("Grenadier audios")]
    [SerializeField] private AudioClip _grenadierAttack;
    [SerializeField] private AudioClip _grenadierIdle;
    [SerializeField] private AudioClip _grenadierStep;
    [SerializeField] private AudioClip _grenadierDeath;

    [Header("Buildings audios")]
    [SerializeField] private AudioClip _buildingDismantle;
    [SerializeField] private AudioClip _placingBuilding;

    [Header("UI audios")]
    [SerializeField] private AudioClip _buttonClicked;
    [SerializeField] private AudioClip _confirmation;
    [SerializeField] private AudioClip _acceptBtn;
    [SerializeField] private AudioClip _rejectBtn;
    [SerializeField] private AudioClip _popUp;

    [Header("Gameloop")]
    [SerializeField] private AudioClip _defeatSound;

    public enum UIType
    {
        None = 0,
        ButtonClicked = 1,
        PopUp = 2,
        Confirmation = 3,
        Accept = 4,
        Reject = 5,
    }

    public enum BuildingAction
    {
        None = 0,
        Placing = 1,
        Dismantle = 2,
    }

    private Queue<AudioSource> _audioSourcePool;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _audioSourcePool = new Queue<AudioSource>();
        for (int i = 0; i < _audioSourcePoolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            _audioSourcePool.Enqueue(source);
        }
    }

    public void AttackSound(UnitType wantedUnit)
    {
        AudioClip requestedClip = null;
        switch (wantedUnit)
        {
            case UnitType.None:
                requestedClip = null;
                Debug.LogWarning($"UnitType enum was not specify when requesting play attack sound");
                break;

            case UnitType.BasicSoldier:
                requestedClip = _chomperAttack;
                break;

            case UnitType.RangeAttack:
                requestedClip = _spitterAttack;
                break;

            case UnitType.Tank:
                requestedClip = _grenadierAttack;
                break;
        }

        if (requestedClip != null)
        {
            PlayClip(requestedClip);
        }
    }
    public void DeathSound(UnitType wantedUnit)
    {
        AudioClip requestedClip = null;
        switch (wantedUnit)
        {
            case UnitType.None:
                requestedClip = null;
                Debug.LogWarning($"UnitType enum was not specify when requesting play death sound");
                break;

            case UnitType.BasicSoldier:
                requestedClip = _chomperDeath;
                break;

            case UnitType.RangeAttack:
                requestedClip = _spitterDeath;
                break;

            case UnitType.Tank:
                requestedClip = _grenadierDeath;
                break;
        }

        if (requestedClip != null)
        {
            PlayClip(requestedClip);
        }
    }
    public void IdleSound(UnitType wantedUnit)
    {
        AudioClip requestedClip = null;
        switch (wantedUnit)
        {
            case UnitType.None:
                requestedClip = null;
                Debug.LogWarning($"UnitType enum was not specify when requesting play death sound");
                break;

            case UnitType.BasicSoldier:
                requestedClip = _chomperIdle;
                break;

            case UnitType.RangeAttack:
                requestedClip = _spitterIdle;
                break;

            case UnitType.Tank:
                requestedClip = _grenadierIdle;
                break;
        }

        if (requestedClip != null)
        {
            PlayClip(requestedClip);
        }
    }
    public void MovementSound(UnitType wantedUnit)
    {
        AudioClip requestedClip = null;
        switch (wantedUnit)
        {
            case UnitType.None:
                requestedClip = null;
                Debug.LogWarning($"UnitType enum was not specify when requesting play death sound");
                break;

            case UnitType.BasicSoldier:
                requestedClip = _chomperStep;
                break;

            case UnitType.RangeAttack:
                requestedClip = _spitterStep;
                break;

            case UnitType.Tank:
                requestedClip = _grenadierStep;
                break;
        }

        if (requestedClip != null)
        {
            PlayClip(requestedClip);
        }
    }

    public void UISound(UIType requestedSound)
    {
        AudioClip clip = null;
        switch (requestedSound)
        {
            case UIType.None:
                clip = null;
                Debug.LogWarning($"UISound not specify when requesting play UI sound");
                break;

            case UIType.ButtonClicked:
                clip = _buttonClicked;
                break;
            case UIType.PopUp:
                clip = _popUp;
                break;
            case UIType.Confirmation:
                clip = _confirmation;
                break;
            case UIType.Reject:
                clip = _rejectBtn;
                break;
            case UIType.Accept:
                clip = _acceptBtn;
                break;
        }
        if (clip != null)
        {
            PlayClip(clip);
        }
    }
    public void BuildingSound(BuildingAction sound)
    {
        AudioClip clip = null;
        switch (sound)
        {
            case BuildingAction.None:
                clip = null;
                Debug.LogWarning($"BuildingAction not specify when requesting play Building sound");
                break;

            case BuildingAction.Placing:
                clip = _placingBuilding;
                break;
            case BuildingAction.Dismantle:
                clip = _buildingDismantle;
                break;
        }
        if (clip != null)
        {
            PlayClip(clip);
        }
    }

    public void EndingSound()
    {
        PlayClip(_defeatSound);
    }
    void PlayClip(AudioClip source)
    {
        AudioSource availableSource = GetAudioSource();
        if (availableSource != null)
        {
            availableSource.clip = source;
            availableSource.Play();
            StartCoroutine(ReturnSourceAfterPlayback(availableSource, source.length));
        }
    }

    private IEnumerator ReturnSourceAfterPlayback(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnAudioSource(source);
    }

    private AudioSource GetAudioSource()
    {
        if (_audioSourcePool.Count > 0)
        {
            return _audioSourcePool.Dequeue();
        }
        else
        {
            return null;
        }
    }

    private void ReturnAudioSource(AudioSource source)
    {
        source.Stop();
        _audioSourcePool.Enqueue(source);
    }
}
