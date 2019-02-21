using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuToggle : MonoBehaviour{

    public GameObject HUDMenu;
    public KeyCode ToggleKey;
    public bool OffClickDisable;

    public Sprite ActiveSprite;
    public Sprite InactiveSprite;

    public bool IsActive = false;
    private bool _isToggled = false;

    public float shiftAmount;
    private Vector3 startLoc;

	void Start ()
    {
        startLoc = HUDMenu.transform.localPosition;

        HUDMenu.transform.localPosition += new Vector3(shiftAmount, 0, 0);
        this.GetComponent<Image>().sprite = InactiveSprite;

    }

    void Update()
    {
        //If player left clicks non-UI element when the menu is active, it disables menu, if OffClickDisable is true
        if (_isToggled && OffClickDisable && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Toggle();

        }
        //If togglekey is pressed and the menu is not opened, open it
        if (ToggleKey != KeyCode.None && Input.GetKeyDown(ToggleKey))
        {
            Toggle();

        }

        //update isactive
        IsActive = _isToggled;

    }
	
	public void Toggle()
    {
        //toggles menu active when called
        if (!_isToggled)
        {
            HUDMenu.transform.localPosition = startLoc;
            _isToggled = true;
            this.GetComponent<Image>().sprite = ActiveSprite;

        }
        else
        {
            HUDMenu.transform.localPosition += new Vector3(shiftAmount, 0, 0);
            _isToggled = false;
            this.GetComponent<Image>().sprite = InactiveSprite;

        }

    }

}
