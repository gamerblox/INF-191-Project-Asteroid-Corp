using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipManager : MonoBehaviour {

    //string data
    private string _data;

    //reference to tooltip object
    public GameObject ToolTip;
    private GameObject _tooltip;

    void Start()
    {
        _tooltip = ToolTip;
        Deactivate();

    }

    void Update()
    {
        if (_tooltip.GetComponent<CanvasGroup>().alpha >= 1f)
        {
			UpdatePos();

        }

    }

    public void Activate(string str)
    {
        string tempStr = "black";
        _data = "<color=" + tempStr + "><b><size=20>" + str + "</size></b></color>";
        _tooltip.transform.GetChild(0).GetComponent<Text>().text = _data;
		UpdatePos();
		_tooltip.GetComponent<CanvasGroup>().alpha = 1f;

    }

    public void Deactivate()
    {
		_tooltip.GetComponent<CanvasGroup>().alpha = 0f;

    }
	
	void UpdatePos()
	{
		float _offset = 10f;
		Vector3 _pos = Input.mousePosition + new Vector3((_tooltip.GetComponent<RectTransform>().rect.width / 2f) + _offset, ((_tooltip.GetComponent<RectTransform>().rect.height / 2f) * -1f) + -_offset, 0f);
        _tooltip.transform.position = _pos;
		
	}
	
}
