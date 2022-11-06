using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Cell : MonoBehaviour
{
    public int PosX  { get; private set; }
    public int PosY  { get; private set; }
    public int Value { get; private set; }

    public bool IsEmpty => Value == 0;
    public bool HasMerged { get; private set; }
    public static int MaxValue = 11;

    [SerializeField]
    private Image image;
    [SerializeField]
    private Text text;

    private ColorList _colorList;
    private GameController _gameControl;

    private CellAnimation currentAnimation;
    private CellAnimationController _cellAnimControl;

    [Inject] public void Constructor(Transform transform, ColorList colorList, CellAnimationController cellAnimControl, GameController gameControl)
    {
        _colorList = colorList;
        _gameControl = gameControl;
        this.transform.SetParent(transform, false);
        this._cellAnimControl = cellAnimControl;
    }
    

    public void SetValue(int x, int y, int value, bool updateUI = true)
    {
        PosX = x;
        PosY = y;
        Value = value;
        if(updateUI)
            UpdateCell();
    }
    public void IncreaseValue()
    {
        Value++;
        HasMerged = true;
        _gameControl.AddScore(1);
    }
    public void ResetFlag()
    {
        HasMerged = false;
    }

    public void MergeWithCell(Cell otherCell)
    {
        _cellAnimControl.SmoothTransition(this, otherCell, true);
        otherCell.IncreaseValue();
        SetValue(PosX,PosY, 0);
    }
    public void MoveToCell(Cell target)
    {
        _cellAnimControl.SmoothTransition(this, target, false);
        target.SetValue(target.PosX,target.PosY, Value, false);
        SetValue(PosX, PosY, 0);
    }
    public void UpdateCell()
    {
        text.text = Value == 0 ? "" : ((int)Mathf.Pow(2, Value)).ToString();
        image.color = _colorList.CellColors[Value];
    }

    public void SetAnimation(CellAnimation anim)
    {
        currentAnimation = anim;
    }
    public void CancelAnimation()
    {
        if (currentAnimation != null)
        {
            currentAnimation.Destroy();
        }
    }

    public class Factory : PlaceholderFactory<Transform, Cell> { }
}
