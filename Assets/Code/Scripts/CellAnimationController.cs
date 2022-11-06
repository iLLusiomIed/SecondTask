using UnityEngine;
using DG.Tweening;
using Zenject;

public class CellAnimationController : MonoBehaviour
{
    private CellAnimation.Factory _cellAnimFactory;
    [Inject]public void Construct(CellAnimation.Factory cellAnimFactory)
    {
        this._cellAnimFactory = cellAnimFactory;
    }
    public void Awake()
    {
        DOTween.Init();
    }

    public void SmoothTransition(Cell from, Cell to, bool isMerging)
    {
        var cellAnim = _cellAnimFactory.Create(this.transform);
        cellAnim.Move(from, to, isMerging);
    }

    public void SmothAppear(Cell cell)
    {
        var cellAnim = _cellAnimFactory.Create(this.transform);
        cellAnim.Appear(cell);
    }
}
