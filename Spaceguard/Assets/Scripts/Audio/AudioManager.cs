using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

namespace Game.Audio
{
    [Serializable]
    public class Sound
    {
        public AudioMixerGroup audioMixerGroup;

        private AudioSource source;

        public string clipName;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(.1f,3f)]
        public float pitch = 1f;

        public bool loop = false;
        public bool playOnAwake = false;

        public void SetSource(AudioSource audioSource)
        {
            source = audioSource;
            source.clip = clip;
            source.pitch = pitch;
            source.volume = volume;
            source.playOnAwake = playOnAwake;
            source.loop = loop;
            source.outputAudioMixerGroup = audioMixerGroup;
        }

        /// <summary>
        /// Method to play the audio clip.
        /// </summary>
        public void Play()
        {
            source.Play();
        }

        /// <summary>
        /// Method to stop playing the audio clip.
        /// </summary>
        public void Stop()
        {
            source.Stop();
        }
    }

    [AddComponentMenu("Game Managers/Audio Manager")]
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// Static Instance of the Audio Manager that allows it to be access by other scripts
        /// </summary>
        public static AudioManager instance;

        /// <summary>
        /// Serialized Array of sound objects
        /// </summary>
        [SerializeField]
        Sound[] sound;

        /// <summary>
        /// Reference to Unity Audio Mixer component
        /// </summary>
        public AudioMixer audioMixer;

        /// <summary>
        /// A link/pointer to the Text component on the Settings screen.
        /// Used to show the user which theme music is currently being played.
        /// </summary>
        public Text themeLBLText;

        public Text muteLBLText;

        /// <summary>
        /// If mute is active it will be false, if not will be true.
        /// </summary>
        private bool muted = false;

        /// <summary>
        /// A string that holds the name of the current theme music being played.
        /// </summary>
        private string themeMusic = "";

        #region Unity Methods

        /// <summary>
        /// Called before all other methods in this script.
        /// Checks to see if the AudioManager instance already exists, and if so destroys the new AudioManager gameObject.
        /// Only one instance of AudioManager is allowed or needed.
        /// </summary>
        void Awake()
        {
            // Check if Instance exists
            if (instance == null)
                instance = this; // if not, set it to this

            else if (instance != this)
                Destroy(gameObject); // if it does, destroy it
        }

        /// <summary>
        /// Called after Awake() but before any other methods.
        /// Creates GameObjects for each sound in the sound array so they can be called by their name to be played on demand.
        /// These GameObjects are then placed as children to the AudioManager GameObject. 
        /// </summary>
        void Start()
        {
            for (int i = 0; i < sound.Length; i++)
            {
                print(sound[i]);
                GameObject _go = new GameObject("Sound_" + i + "_" + sound[i].clipName);
                _go.transform.SetParent(this.transform);
                sound[i].SetSource(_go.AddComponent<AudioSource>());
            }

            RandomThemeMusic();
        }

        #endregion

        /// <summary>
        /// Sets the master volume level from <see cref="muted"/>.
        /// </summary>
        public void SetMasterMute()
        {
            if (muted)
            {
                muted = false;
                muteLBLText.gameObject.SetActive(muted);
                audioMixer.SetFloat("MutedVolume", 0f);
            } else
            {
                muted = true;
                muteLBLText.gameObject.SetActive(muted);
                audioMixer.SetFloat("MutedVolume", -80f);
            }

        }

        /// <summary>
        /// Set the master volume.
        /// </summary>
        /// <param name="masterVolume"></param>
        public void SetMasterVolume(float masterVolume)
        {
            audioMixer.SetFloat("MasterVolume", masterVolume);
        }

        /// <summary>
        /// Sets music volume level. Music volume is child of master volume.
        /// </summary>
        /// <param name="musicVolume"></param>
        public void SetMusicVolume(float musicVolume)
        {
            audioMixer.SetFloat("MusicVolume", musicVolume);
        }

        /// <summary>
        /// Set sound effects volume level. Sound effects volume is child of 
        /// master volume.
        /// </summary>
        /// <param name="effectsVolume"></param>
        public void SetEffectsVolume(float effectsVolume)
        {
            audioMixer.SetFloat("EffectsVolume", effectsVolume);
        }

        /// <summary>
        /// Plays theme music by name and stores name of the theme music track 
        /// in <see cref="themeMusic"/> field (string value is used by 
        /// <see cref="StopSound(string)"/> to mute theme music.
        /// </summary>
        public void PlayThemeMusic(string name)
        {
            if (themeMusic != "")
            {
                StopSound(themeMusic);
            }
            themeMusic = name;
            ThemeChangedLBL(themeMusic);
            PlaySound(themeMusic);
        }

        /// <summary>
        /// Play game theme 1 music, "Ambient Karma".
        /// </summary>
        public void PlayTheme1()
        { PlayThemeMusic("Ambient Karma"); }

        /// <summary>
        /// Play game theme 2 music, "Electric Dreams".
        /// </summary>
        public void PlayTheme2()
        { PlayThemeMusic("Electric Dreams"); }

        /// <summary>
        /// Play game theme 3 music, "Extract of Sweetness".
        /// </summary>
        public void PlayTheme3()
        { PlayThemeMusic("Extract of Sweetness"); }

        public void PlayTheme4()
        { PlayThemeMusic("Ether Oar"); }


        /// <summary>
        /// Randomly selects one of the three game themes and plays it.
        /// </summary>
        public void RandomThemeMusic()
        {

            PlayTheme4();
            //Themes downloaded from freesound.org
            //switch (UnityEngine.Random.Range(0, 2))
            //{
            //    case 0:
            //        PlayTheme1();
            //        break;
            
            //    case 1:
            //        PlayTheme2();
            //        break;

            //    case 2:
            //        PlayTheme3();
            //        break;
            //}
        }

        //https://freesound.org/people/Splatez07/sounds/413690/
        public void ButtonClick()
        {
            PlaySound("Button Click");
        }

        /// <summary>
        /// Search for name in sound array, then Play() it.
        /// </summary>
        public void PlaySound(string name)
        {
            Sound s = Array.Find(sound, sound => sound.clipName == name);
            if (s == null)
            {
                Debug.LogError("PlaySound() name not found (" + name + ")");
                return;
            }
            s.Play();
        }

        /// <summary>
        /// Search for name in sound array, then Stop() it.
        /// </summary>
        public void StopSound(string name)
        {
            Sound s = Array.Find(sound, sound => sound.clipName == name);
            if (s == null)
            {
                Debug.LogError("StopSound() _name not found (" + name + ")");
                return;
            }
            s.Stop();
        }

        /// <summary>
        /// Changes the component pointed to by themeLNLText on screen to the theme (name of theme) string.
        /// </summary>
        public void ThemeChangedLBL(string theme)
        {
            themeLBLText.text = theme;
        }
    }

}