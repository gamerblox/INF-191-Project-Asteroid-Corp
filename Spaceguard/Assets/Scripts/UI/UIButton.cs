#define SET_ONCLICK_INSPECTOR_ONLY // Don't set onClick routines via script
//using Game.Bodies;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    [AddComponentMenu("Custom UI/Button")]
    public class UIButton : UIElement
    {
        protected Button button;
        protected Image image;
        protected Image icon;

        public ButtonType buttonType;

        //BodyManager manager;

        public enum ButtonType
        {
            // Multimedia Buttons
            Play,
            PlayBackward,
            Pause,
            Stop,
            FastForward,
            Rewind,
            Skip,
            SkipBack,
            // Menu Buttons
            Menu,
            ToggleLeft,
            ToggleRight,
            Plus,
            Minus,
            Replay,
            Settings,
            // Dialog Buttons
            Close,
            Confirm,
            // Special Buttons
            FullscreenEnter,
            FullscreenExit,
            Launch
        }

        public override void Awake()
        {
            //manager = FindObjectOfType<BodyManager>();
            image = GetComponent<Image>();
            button = GetComponent<Button>();

            // TODO: Get button's child image another way; Find() is error-prone.
            icon = transform.Find("Icon").GetComponent<Image>();
            base.Awake();
        }

        protected override void OnSkinUI()
        {
            button.transition = Selectable.Transition.SpriteSwap;
            button.targetGraphic = image;
            image.sprite = skin.buttonSprite;
            image.type = Image.Type.Sliced;
            button.spriteState = skin.buttonSpriteState;

            switch (buttonType)
            {
                case ButtonType.Play:
                    image.color = skin.multimediaButtonColor;
                    icon.sprite = skin.playIcon;
                    //button.onClick.AddListener(manager.PlayForward);
                    break;
                case ButtonType.PlayBackward:
                    image.color = skin.multimediaButtonColor;
                    icon.sprite = skin.playBackwardIcon;
                    //button.onClick.AddListener(manager.PlayBackward);
                    break;
                case ButtonType.Pause:
                    image.color = skin.multimediaButtonColor;
                    icon.sprite = skin.pauseIcon;
                    //button.onClick.AddListener(manager.Pause);
                    break;
                case ButtonType.Stop:
                    image.color = skin.multimediaButtonColor;
                    icon.sprite = skin.stopIcon;
                    //button.onClick.AddListener(manager.Pause);
                    break;
                case ButtonType.FastForward:
                    image.color = skin.multimediaButtonColor;
                    icon.sprite = skin.fastForwardIcon;
                    break;
                case ButtonType.Rewind:
                    image.color = skin.multimediaButtonColor;
                    icon.sprite = skin.rewindIcon;
                    //button.onClick.AddListener(manager.Rewind);
                    break;
                case ButtonType.Skip:
                    image.color = skin.menuButtonColor;
                    icon.sprite = skin.skipIcon;
                    //button.onClick.AddListener(manager.StepForward);
                    break;
                case ButtonType.SkipBack:
                    image.color = skin.menuButtonColor;
                    icon.sprite = skin.skipBackIcon;
                    //button.onClick.AddListener(manager.StepBackward);
                    break;
                case ButtonType.Menu:
                    image.color = skin.iconBackgroundColor;
                    icon.sprite = skin.menuIcon;
                    break;
                case ButtonType.ToggleLeft:
                    image.color = skin.menuButtonColor;
                    icon.sprite = skin.toggleLeftIcon;
                    break;
                case ButtonType.ToggleRight:
                    image.color = skin.menuButtonColor;
                    icon.sprite = skin.toggleRightIcon;
                    break;
                case ButtonType.Plus:
                    image.color = skin.menuButtonColor;
                    icon.sprite = skin.plusIcon;
                    break;
                case ButtonType.Minus:
                    image.color = skin.menuButtonColor;
                    icon.sprite = skin.minusIcon;
                    break;
                case ButtonType.Replay:
                    image.color = skin.menuButtonColor;
                    icon.sprite = skin.replayIcon;
                    break;
                case ButtonType.Settings:
                    image.color = skin.menuButtonColor;
                    icon.sprite = skin.settingsIcon;
                    break;
                case ButtonType.Close:
                    image.color = skin.closeButtonColor;
                    icon.sprite = skin.closeIcon;
                    break;
                case ButtonType.Confirm:
                    image.color = skin.confirmButtonColor;
                    icon.sprite = skin.confirmIcon;
                    break;
                // Special buttons
                case ButtonType.FullscreenEnter:
                    image.color = skin.iconBackgroundColor;
                    icon.sprite = skin.fullscreenEnterIcon;
                    break;
                case ButtonType.FullscreenExit:
                    image.color = skin.iconBackgroundColor;
                    icon.sprite = skin.fullscreenExitIcon;
                    break;
                case ButtonType.Launch:
                    image.color = skin.launchButtonColor;
                    icon.sprite = skin.launchIcon;
                    //button.onClick.AddListener(manager.LaunchSpacecraft);
                    break;
            }

            icon.color = skin.defaultIconColor;
            base.OnSkinUI();
        }
    }
}
