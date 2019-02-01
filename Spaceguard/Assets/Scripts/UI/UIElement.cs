using UnityEngine;

namespace Game.UI
{
    [ExecuteInEditMode]
    public class UIElement : MonoBehaviour
    {
        public UISkin skin;

        protected virtual void OnSkinUI()
        {

        }

        public virtual void Awake()
        {
            OnSkinUI();
        }

        // To visualize changes without having to play the scene every time...
        // This should really be a custom editor script to call the Update()
        // method so that we're not running this check while the game is playing.
        public virtual void Update()
        {
            if (Application.isEditor)
            {
                OnSkinUI();
            }
        }
    }
}
