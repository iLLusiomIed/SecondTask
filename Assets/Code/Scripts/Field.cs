using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class Field : MonoBehaviour
{

    [Header("Field Properties")]
    public float CellSize;
    public float Spacing;
    public int FieldSize;
    public int InitMaxCellCount = 3;

    [Space(10)]
    [SerializeField]
    private RectTransform rt;

    private Cell[,] field;

    private bool anyCellMoved;

    private GameController _gameControl;

    private Cell.Factory _cellFactory;
    private CellAnimationController _cellAnimControl;
    [Inject]public void Constructor(Cell.Factory cellFactory, CellAnimationController cellAnimControl, GameController gameControl)
    {
        this._cellFactory = cellFactory;
        this._cellAnimControl = cellAnimControl;
        this._gameControl = gameControl;
    }
    private void Start()
    {
        SwipeDetection.SwipeEvent += OnInput;
    }
    private void OnInput(Vector2 direction)
    {
        if (!GameController.GameStarted)
            return;

        anyCellMoved = false;
        ResetCellsFlags();

        Move(direction);

        if (anyCellMoved)
        {
            int randomCellCount = Random.Range(2, InitMaxCellCount + 1);
            Debug.Log(randomCellCount);
            for (int i = 0; i < randomCellCount; i++)
            {
                SpawnRandomCells();
            }
            CheckGameResult();
        }
    }
    private void Move(Vector2 direction)
    {
        int startXY = direction.x > 0 || direction.y < 0 ? FieldSize - 1 : 0;
        int dir = direction.x != 0 ? (int)direction.x : -(int)direction.y;

        for (int i = 0; i < FieldSize; i++)
        {
            for (int k = startXY; k >= 0 && k < FieldSize; k -= dir)
            {
                var cell = direction.x != 0 ? field[k, i] : field[i, k];

                if (cell.IsEmpty)
                    continue;

                var cellToMarge = FindCellToMarge(cell, direction);
                if (cellToMarge != null)
                {
                    cell.MergeWithCell(cellToMarge);
                    anyCellMoved = true;
                    continue;
                }

                var emptyCell = FindEmptyCell(cell, direction);
                if (emptyCell != null)
                {
                    cell.MoveToCell(emptyCell);
                    anyCellMoved = true;
                }
            }
        }

    }

    private Cell FindCellToMarge(Cell cell, Vector2 dir)
    {
        int startX = cell.PosX + (int)dir.x;
        int startY = cell.PosY - (int)dir.y;


        for (int x = startX, y = startY;
            x >= 0 && x < FieldSize &&
            y >= 0 && y < FieldSize;
            x += (int)dir.x, y -=(int)dir.y)
        {
            if (field[x, y].IsEmpty)
            {
                continue;
            }
            if (field[x, y].Value == cell.Value && !field[x, y].HasMerged)
            {
                return field[x, y];
            }

            break;
        }
        return null;
    }
    private Cell FindEmptyCell(Cell cell, Vector2 dir)
    {
        Cell emptyCell = null;

        int startX = cell.PosX + (int)dir.x;
        int startY = cell.PosY - (int)dir.y;


        for (int x = startX, y = startY;
            x >= 0 && x < FieldSize &&
            y >= 0 && y < FieldSize;
            x += (int)dir.x, y -= (int)dir.y)
        {
            if (field[x, y].IsEmpty)
            {
                emptyCell = field[x, y];
            }
            else
                break;
        }
        return emptyCell;
    }
    private void CheckGameResult()
    {
        bool lose = true;

        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                if (field[x, y].Value == Cell.MaxValue)
                {
                    _gameControl.Win();
                    return;
                }
                if (lose && field[x, y].IsEmpty ||
                    FindCellToMarge(field[x, y], Vector2.left) ||
                    FindCellToMarge(field[x, y], Vector2.right) ||
                    FindCellToMarge(field[x, y], Vector2.up) ||
                    FindCellToMarge(field[x, y], Vector2.down))
                {
                    lose = false;
                }
            }
        }
        if (lose)
        {
            _gameControl.Lose();
        }
    }
    private void CreateField()
    {
        field = new Cell[FieldSize, FieldSize];

        float fieldWidth = FieldSize * (CellSize + Spacing) + Spacing;
        rt.sizeDelta = new Vector2(fieldWidth, fieldWidth);
        float startX = -(fieldWidth / 2) + (CellSize / 2) + Spacing;
        float startY = (fieldWidth / 2) - (CellSize / 2) - Spacing;

        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                Cell cell = _cellFactory.Create(this.transform);
                Vector2 position = new Vector2(startX + (x * (CellSize + Spacing)), startY - (y * (CellSize + Spacing)));
                cell.transform.localPosition = position;
                cell.SetValue(x, y, 0);
                field[x, y] = cell;
            }
        }
    }
    public void GenerateField()
    {
        if (field == null)
            CreateField();
        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                field[x,y].SetValue(x, y, 0);
            }
        }

        int randomCellCount = Random.Range(2, InitMaxCellCount+1);
        for (int i = 0; i < randomCellCount; i++)
        {
            SpawnRandomCells();
        }
    }

    public void SpawnRandomCells()
    {
        var emptyCells = new List<Cell>();

        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                if (field[x,y].IsEmpty)
                {
                    emptyCells.Add(field[x,y]);
                }
            }
        }
        if (emptyCells.Count > 0)
        {
            Cell cell = emptyCells[Random.Range(0, emptyCells.Count)];
            cell.SetValue(cell.PosX, cell.PosY, 1, false);

            _cellAnimControl.SmothAppear(cell);
        }
    }

    private void ResetCellsFlags()
    {
        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                field[x, y].ResetFlag();
            }
        }
    }
}