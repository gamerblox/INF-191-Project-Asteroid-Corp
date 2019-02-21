using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

    public GameObject ToolTipMgr;
    private ToolTipManager tooltip;
	
	[TextArea]
	public string HighlightText = "Enter text here.";

    void Start ()
    {
        tooltip = ToolTipMgr.GetComponent<ToolTipManager>();

    }

    //enables tooltip info for item
    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Activate(HighlightText);

    }

    //disables tooltip info for item
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Deactivate();

    }
	
	//enables tooltip info for item
    public void OnMouseOver()
    {
        tooltip.Activate(HighlightText);

    }

    //disables tooltip info for item
    public void OnMouseExit()
    {
        tooltip.Deactivate();

    }

}
