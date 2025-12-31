using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioManagement
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        
        [SerializeField] private AudioHolder audioHolder;
        [SerializeField] private bool logWarnings = true;
        
        private Dictionary<string, List<AudioData>> _soundGroups;
        private Dictionary<string, int> _lastPlayedIndices;
        private AudioSource _musicSource;
        private string _currentMusicTrack;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioSources();
        }

        private void InitializeAudioSources()
        {
            _soundGroups = new Dictionary<string, List<AudioData>>();
            _lastPlayedIndices = new Dictionary<string, int>();

            foreach (var audioData in audioHolder.AudioDatas)
            {
                if (!_soundGroups.ContainsKey(audioData.name.ToString()))
                {
                    _soundGroups[audioData.name.ToString()] = new List<AudioData>();
                }
                
                _soundGroups[audioData.name.ToString()].Add(audioData);
                audioData.source = gameObject.AddComponent<AudioSource>();
                audioData.source.clip = audioData.clip;
                audioData.source.volume = audioData.volume;
                audioData.source.pitch = audioData.pitch;
                audioData.source.loop = audioData.loop;
                audioData.source.mute = audioData.mute;
                audioData.source.playOnAwake = false;
            }
        }

        private void Start()
        {
            UnmuteAllSounds();
            UnmuteAllMusics();
            PlayStartSounds();
        }

        public void PlayStartSounds()
        {
            foreach (var soundGroup in _soundGroups.Values)
            {
                foreach (var sound in soundGroup.Where(s => s.playOnAwake))
                {
                    sound.source.Play();
                }
            }
        }

        #region Playback Controls
        
        public void Play(string name)
        {
            if (!_soundGroups.ContainsKey(name))
            {
                if (logWarnings) Debug.LogWarning($"Sound '{name}' not found!");
                return;
            }

            var sounds = _soundGroups[name];
            var sound = sounds[0];
            
            sound.source.Play();
        }
        
        public async Task AsyncAwaitPlay(string name, float time = 1f)
        {
            if (!_soundGroups.ContainsKey(name))
            {
                if (logWarnings) Debug.LogWarning($"Sound '{name}' not found!");
                await Task.CompletedTask;
            }

            var sounds = _soundGroups[name];
            var sound = sounds[0];
            
            sound.source.Play();
            await Task.Delay((int)(time * 1000));
        }

        public void PlayRandom(string name)
        {
            if (!_soundGroups.ContainsKey(name))
            {
                if (logWarnings) Debug.LogWarning($"Sound '{name}' not found!");
                return;
            }

            var sounds = _soundGroups[name];
            var randomIndex = UnityEngine.Random.Range(0, sounds.Count);
            var sound = sounds[randomIndex];
            
            sound.source.Play();
        }

        public void PlaySequential(string name)
        {
            if (!_soundGroups.ContainsKey(name))
            {
                if (logWarnings) Debug.LogWarning($"Sound '{name}' not found!");
                return;
            }

            var sounds = _soundGroups[name];
            
            if (!_lastPlayedIndices.ContainsKey(name))
            {
                _lastPlayedIndices[name] = -1;
            }

            _lastPlayedIndices[name] = (_lastPlayedIndices[name] + 1) % sounds.Count;
            var sound = sounds[_lastPlayedIndices[name]];
            
            sound.source.Play();
        }

        public void Stop(string name)
        {
            if (!_soundGroups.ContainsKey(name))
            {
                if (logWarnings) Debug.LogWarning($"Sound '{name}' not found!");
                return;
            }

            foreach (var sound in _soundGroups[name])
            {
                sound.source.Stop();
            }
        }

        public void Pause(string name)
        {
            if (!_soundGroups.ContainsKey(name))
            {
                if (logWarnings) Debug.LogWarning($"Sound '{name}' not found!");
                return;
            }

            foreach (var sound in _soundGroups[name])
            {
                sound.source.Pause();
            }
        }

        public void Unpause(string name)
        {
            if (!_soundGroups.ContainsKey(name))
            {
                if (logWarnings) Debug.LogWarning($"Sound '{name}' not found!");
                return;
            }

            foreach (var sound in _soundGroups[name])
            {
                sound.source.UnPause();
            }
        }

        public bool IsPlaying(string name)
        {
            if (!_soundGroups.ContainsKey(name))
            {
                if (logWarnings) Debug.LogWarning($"Sound '{name}' not found!");
                return false;
            }

            return _soundGroups[name].Any(sound => sound.source.isPlaying);
        }

        #endregion

        #region Volume Controls

        public void SetVolume(string name, float volume)
        {
            if (!_soundGroups.ContainsKey(name))
            {
                if (logWarnings) Debug.LogWarning($"Sound '{name}' not found!");
                return;
            }

            foreach (var sound in _soundGroups[name])
            {
                sound.source.volume = Mathf.Clamp01(volume);
            }
        }

        public void SetVolumeForType(AudioType audioType, float volume)
        {
            foreach (var soundGroup in _soundGroups.Values)
            {
                foreach (var sound in soundGroup.Where(s => s.audioType == audioType))
                {
                    sound.source.volume = Mathf.Clamp01(volume);
                }
            }
        }

        #endregion

        #region Mute Controls

        public void MuteAllSounds()
        {
            foreach (var soundGroup in _soundGroups.Values)
            {
                foreach (var sound in soundGroup.Where(s => s.audioType == AudioType.Sound))
                {
                    sound.source.mute = true;
                }
            }
        }

        public void UnmuteAllSounds()
        {
            foreach (var soundGroup in _soundGroups.Values)
            {
                foreach (var sound in soundGroup.Where(s => s.audioType == AudioType.Sound))
                {
                    sound.source.mute = false;
                }
            }
        }

        public void MuteAllMusics()
        {
            foreach (var soundGroup in _soundGroups.Values)
            {
                foreach (var sound in soundGroup.Where(s => s.audioType == AudioType.Music))
                {
                    sound.source.mute = true;
                }
            }
        }

        public void UnmuteAllMusics()
        {
            foreach (var soundGroup in _soundGroups.Values)
            {
                foreach (var sound in soundGroup.Where(s => s.audioType == AudioType.Music))
                {
                    sound.source.mute = false;
                }
            }
        }

        public void MuteSpecificTrack(string name)
        {
            if (!_soundGroups.ContainsKey(name))
            {
                if (logWarnings) Debug.LogWarning($"Sound '{name}' not found!");
                return;
            }

            foreach (var sound in _soundGroups[name])
            {
                sound.source.mute = true;
            }
        }

        public void UnmuteSpecificTrack(string name)
        {
            if (!_soundGroups.ContainsKey(name))
            {
                if (logWarnings) Debug.LogWarning($"Sound '{name}' not found!");
                return;
            }

            foreach (var sound in _soundGroups[name])
            {
                sound.source.mute = false;
            }
        }

        #endregion

        #region Music Playlist Functionality

        public void PlayMusic(string name, bool loop = true, float fadeDuration = 0f)
        {
            if (!_soundGroups.ContainsKey(name))
            {
                if (logWarnings) Debug.LogWarning($"Music track '{name}' not found!");
                return;
            }

            var music = _soundGroups[name][0];
            
            if (fadeDuration > 0)
            {
                StartCoroutine(FadeMusic(music, fadeDuration));
            }
            else
            {
                music.source.volume = music.volume;
                music.source.Play();
            }
            
            music.source.loop = loop;
            _currentMusicTrack = name;
        }

        public void StopMusic(float fadeDuration = 0f)
        {
            if (string.IsNullOrEmpty(_currentMusicTrack)) return;

            var music = _soundGroups[_currentMusicTrack][0];
            
            if (fadeDuration > 0)
            {
                StartCoroutine(FadeOutMusic(music, fadeDuration));
            }
            else
            {
                music.source.Stop();
            }
            
            _currentMusicTrack = null;
        }

        private IEnumerator FadeMusic(AudioData music, float duration)
        {
            float currentTime = 0;
            float startVolume = 0;
            music.source.volume = startVolume;
            music.source.Play();

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                music.source.volume = Mathf.Lerp(startVolume, music.volume, currentTime / duration);
                yield return null;
            }
        }

        private IEnumerator FadeOutMusic(AudioData music, float duration)
        {
            float currentTime = 0;
            float startVolume = music.source.volume;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                music.source.volume = Mathf.Lerp(startVolume, 0, currentTime / duration);
                yield return null;
            }

            music.source.Stop();
            music.source.volume = music.volume;
        }

        #endregion

        #region Utility Methods

        public float GetSoundLength(string name)
        {
            if (!_soundGroups.ContainsKey(name))
            {
                if (logWarnings) Debug.LogWarning($"Sound '{name}' not found!");
                return 0;
            }

            return _soundGroups[name][0].clip.length;
        }

        public List<string> GetAllSoundNames()
        {
            return _soundGroups.Keys.ToList();
        }

        public List<string> GetMusicTrackNames()
        {
            return _soundGroups
                .Where(group => group.Value.Any(s => s.audioType == AudioType.Music))
                .Select(group => group.Key)
                .ToList();
        }

        #endregion
    }
}