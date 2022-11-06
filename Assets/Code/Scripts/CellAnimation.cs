using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

public class CellAnimation : MonoBehaviour
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private Text textValue;

    private float moveTime = 0.1f;
    private float appearTime = 0.1f;

    private Sequence sequence;
    private ColorList _colorList;
    [Inject]public void Constructor(Transform parent, ColorList colorList)
    {
        transform.SetParent(parent, false);
        this._colorList = colorList;
    }
    public void Move(Cell from, Cell to, bool IsMerging)
    {
        from.CancelAnimation();
        to.SetAnimation(this);

        image.color = _colorList.CellColors[from.Value];
        textValue.text = Mathf.Pow(2, from.Value).ToString();

        transform.position = from.transform.position;

        sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(to.transform.position, moveTime).SetEase(Ease.InOutQuad));

        if (IsMerging)
        {
            sequence.AppendCallback(() =>
            { 
                image.color = _colorList.CellColors[to.Value];
                textValue.text = Mathf.Pow(2, to.Value).ToString();
            });

            sequence.Append(transform.DOScale(1.2f, appearTime));
            sequence.Append(transform.DOScale(1, appearTime));
        }
        sequence.AppendCallback(()=>
        {
            to.UpdateCell();
            Destroy();
        });
    }

    public void Appear(Cell cell)
    {
        cell.CancelAnimation();
        cell.SetAnimation(this);

        image.color = _colorList.CellColors[cell.Value];
        textValue.text = Mathf.Pow(2, cell.Value).ToString();

        transform.position = cell.transform.position;
        transform.localScale = Vector2.zero;

        sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(1.2f, appearTime * 2));
        sequence.Append(transform.DOScale(1f, appearTime * 2));

        sequence.AppendCallback(()=>
        {
            cell.UpdateCell();
            Destroy();
        });
    }

    public void Destroy()
    {
        sequence.Kill();
        Destroy(this.gameObject);
    }


    public class Factory : PlaceholderFactory<Transform, CellAnimation> { }
}
