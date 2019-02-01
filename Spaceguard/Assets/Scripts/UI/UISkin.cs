using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [CreateAssetMenu(menuName = "Custom UI/Skin")]
    public class UISkin : ScriptableObject
    {
        public Sprite buttonSprite;
        public SpriteState buttonSpriteState;
        public Color defaultIconColor;
        public Color iconBackgroundColor;

        [Header("Multimedia Buttons")]
        public Color multimediaButtonColor;

        public Sprite playIcon;
        public Sprite playBackwardIcon;
        public Sprite pauseIcon;
        public Sprite stopIcon;
        public Sprite fastForwardIcon;
        public Sprite rewindIcon;
        public Sprite skipIcon;
        public Sprite skipBackIcon;

        [Header("Menu/Navigation Buttons")]
        public Color menuButtonColor;

        public Sprite menuIcon;

        public Sprite toggleLeftIcon;
        public Sprite toggleRightIcon;

        public Sprite plusIcon;
        public Sprite minusIcon;

        public Sprite replayIcon;

        public Sprite settingsIcon;

        [Header("Dialog Buttons")]
        public Color closeButtonColor;
        public Sprite closeIcon;

        public Color confirmButtonColor;
        public Sprite confirmIcon;

        [Header("Special Buttons")]
        public Color fullscreenButtonColor;
        public Sprite fullscreenEnterIcon;
        public Sprite fullscreenExitIcon;

        public Color launchButtonColor;
        public Sprite launchIcon;
    }
}
