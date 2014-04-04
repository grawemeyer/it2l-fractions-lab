using System;
using System.Collections.Generic;
using UnityEngine;
using pumpkin.display;
using pumpkin.events;
using pumpkin.text;
using pumpkin.tweener;


public class LocalizedButton
{
    #region Protected members
    public MovieClip mc;
    protected TextField textfield;
    protected string text;
    protected MovieClip centerMc;
    protected MovieClip leftMc;
    protected MovieClip rightMc;
    protected MovieClip arrowMc;
    protected Alignment alignment;
    protected Color textColor;
    //protected ButtonType buttonType;
    protected ButtonState buttonState;
    protected bool arrow;

    protected float centerX;
    protected float leftX;
    protected float rightX;

    protected InterfaceBehaviour interfaces;


    public enum Alignment
    {
        center = 0,
        left,
        right
    }

    public enum ButtonType
    {
        small = 0,
        big
    }

    public enum ButtonState
    {
        up = 0,
        down,
        over,
        ghost
    }
    #endregion

    #region Public properties
    
    #endregion

    #region Ctor
    public LocalizedButton(MovieClip _mc, string _text, Alignment _alignment, Color _textColor, bool _arrow)
    {
        mc = _mc;
        text = _text;
        //buttonType = _buttonType;
        alignment = _alignment;
        arrow = _arrow;
        textColor = _textColor;

        rightMc = mc.getChildByName<MovieClip>("mcButtonRight");
        leftMc = mc.getChildByName<MovieClip>("mcButtonLeft");
        centerMc = mc.getChildByName<MovieClip>("mcButtonFill");

        centerX = centerMc.x;
        leftX = leftMc.x;
        rightX = rightMc.x;

        arrowMc = mc.getChildByName<MovieClip>("mcPlayArrow");
        if (!arrow)
        {
            if(arrowMc!=null)
                arrowMc.visible = false;
            //arrowMc = null;
        }

        interfaces = GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>();

        SetButtonState(ButtonState.up);

        if (!mc.hasEventListener(MouseEvent.MOUSE_UP))
            mc.addEventListener(MouseEvent.MOUSE_UP, OnBtnsClick);
        if (!mc.hasEventListener(MouseEvent.MOUSE_ENTER))
            mc.addEventListener(MouseEvent.MOUSE_ENTER, OnBtnsEnter);
        if (!mc.hasEventListener(MouseEvent.MOUSE_LEAVE))
            mc.addEventListener(MouseEvent.MOUSE_LEAVE, OnBtnsLeave);
    }

    public void OnBtnsClick(CEvent evt)
    {
        if (!interfaces.InputEnabled)
            return;

        SetButtonState(ButtonState.down);
        interfaces.OnTranslationBtnsClick(this);
    }
    public void OnBtnsEnter(CEvent evt)
    {
        if (!interfaces.InputEnabled)
            return;

        SetButtonState(ButtonState.over);
        interfaces.OnTranslationBtnsEnter(this);
    }
    public void OnBtnsLeave(CEvent evt)
    {
        if (!interfaces.InputEnabled)
            return;

        SetButtonState(ButtonState.up);
        interfaces.OnTranslationBtnsLeave(this);
    }

    /*
    public void AddEventListener(string _event, EventDispatcher.EventCallback _callback )
    {
        if (mc.hasEventListener(_event))
            mc.removeAllEventListeners(_event);

        mc.addEventListener(_event, _callback);
    }
    */
    int btPlayWidthAdded = 40;

    public void SetButtonState(ButtonState _buttonState)
    {
        buttonState = _buttonState;
        string frameLabel = "up";
        switch (buttonState)
        {
            case ButtonState.up:
                frameLabel = "up";
                break;
            case ButtonState.down:
                frameLabel = "dn";
                break;
            case ButtonState.over:
                frameLabel = "ov";
                break;
            case ButtonState.ghost:
                frameLabel = "gh";
                break;
        }

        mc.gotoAndStop(frameLabel);
        centerMc.gotoAndStop(frameLabel);
        leftMc.gotoAndStop(frameLabel);
        rightMc.gotoAndStop(frameLabel);

        TextField txtField = mc.getChildByName<TextField>("tfLabel");
        txtField.textFormat.color = textColor;
        txtField.multiline = false;

        LocalizedText localizedText = new LocalizedText(txtField, text);
        string translatedStr = localizedText.BaseText;
        float edgePercentageDistance = 0.5f;
        int textfieldPadding = 6;
        float letterSpacing = txtField.textFormat.letterSpacing;
        float strLength = 0.0f;
        for (int k = 0; k < translatedStr.Length; k++)
        {
            strLength = strLength + txtField.getGlyph(translatedStr[k]).charWidth + letterSpacing;
        }
        int textFieldLength = (int)(strLength + textfieldPadding); // 16 * translationText.BaseText.Length;

        float textLength;
        if (arrow)
            textLength = textFieldLength + arrowMc.width + 5.0f;
        else
            textLength = textFieldLength;

        if (mc.name.Equals("btPlaySP"))
            textLength += btPlayWidthAdded;

        centerMc.scaleX = 1.0f;
        centerMc.scaleX = (textLength - leftMc.width * edgePercentageDistance * 2) / 2.0f;// +arrowMc.width / 8.0f + 1.0f; // larghezza base di centerMc

        if(alignment == Alignment.center)
        {
            centerMc.x = centerX;
            rightMc.x = centerMc.x + centerMc.width * 0.5f - 1;
            leftMc.x = centerMc.x - (centerMc.width * 0.5f + leftMc.width);
        }
        else if (alignment == Alignment.right)
        {
            leftMc.x = leftX;
            centerMc.x = leftMc.x + leftMc.width + centerMc.width * 0.5f;
            rightMc.x = centerMc.x + centerMc.width * 0.5f;
        }
        else if (alignment == Alignment.left)
        {
            rightMc.x = rightX;
            centerMc.x = rightMc.x - centerMc.width * 0.5f;
            leftMc.x = centerMc.x - (centerMc.width * 0.5f + leftMc.width);
        }

        txtField.x = leftMc.x + leftMc.width * edgePercentageDistance;
        if (mc.name.Equals("btPlaySP"))
            txtField.x += btPlayWidthAdded * 0.5f - arrowMc.width * 0.5f;

        txtField.width = textFieldLength;
        
        if(textColor != null)
            txtField.textFormat.color = textColor;
        
        if (arrowMc != null )
        {
            arrowMc.x = txtField.x + txtField.width + 10.0f;
        }
        if (arrowMc != null)
        {
            arrowMc.visible = arrow;
        }
        FixUVs(centerMc);
    }
    #endregion

    #region Private Functions

    protected List<GraphicsDrawOP> drawOps = new List<GraphicsDrawOP>();
    void FixUVs(pumpkin.display.Sprite mc)
    {
        if (null == mc)
            return;

        foreach (GraphicsDrawOP drawOp in mc.graphics.drawOPs)
        {
            if (drawOps.IndexOf(drawOp) < 0)
            {
                Vector2 halfTexel = drawOp.material.mainTexture.texelSize * 0.5f;
                Rect r = drawOp.drawSrcRect;
                r.xMin += halfTexel.x;
                r.xMax -= halfTexel.x;
                r.yMin += halfTexel.y;
                r.yMax -= halfTexel.y;
                drawOp.drawSrcRect = r;
                drawOps.Add(drawOp);
            }
        }

        for (int i = 0; i < mc.numChildren; ++i)
            FixUVs(mc.getChildAt<pumpkin.display.Sprite>(i));
    }
    #endregion

    #region Public methods
    public void Destroy()
    {
        //Translations.Instance.RemoveElement(this);
        //base.Destroy();
    }

    public void OnLanguageChanged(string lang)
    {
        SetButtonState(buttonState);
    }
    #endregion
}