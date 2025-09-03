using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
    {
        public static MusicController Instance;
    
        [SerializeField] private Image _musicButtonIcon;
        [SerializeField] private Image _soundButtonIcon;

        [SerializeField] private Sprite _soundButtonIconOn;
        [SerializeField] private Sprite _soundButtonIconOff;

        [SerializeField] public AudioSource _audioSourceMusic;
        [SerializeField] public AudioSource _audioSourceSound;

        [SerializeField] private AudioClip _uiClickSound;

        private bool _isMusicOn = true;
        private bool _isSoundOn = true;

        private void Awake()
        {
            Instance = this;
        }

        public void PlayClickSound()
        {
            _audioSourceSound.PlayOneShot(_uiClickSound);
        }

        public void PlaySpecificSound(AudioClip audioClip, float volume = 0.7f)
        {
            _audioSourceSound.PlayOneShot(audioClip, volume);
        }

        public void HandleMusicOn()
        {
            if (_isMusicOn)
            {
                _isMusicOn = false;
                _audioSourceMusic.mute = true;
                _musicButtonIcon.sprite = _soundButtonIconOff;
            }
            else
            {
                _isMusicOn = true;
                _audioSourceMusic.mute = false;
                _musicButtonIcon.sprite = _soundButtonIconOn;
            }
        }
        public void HandleSoundOn()
        {
            if (_isSoundOn)
            {
                _isSoundOn = false;
                _audioSourceSound.mute = true;
                _soundButtonIcon.sprite = _soundButtonIconOff;
            }
            else
            {
                _isSoundOn = true;
                _audioSourceSound.mute = false;
                _soundButtonIcon.sprite = _soundButtonIconOn;
            }
        }
    }