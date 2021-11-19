using System.Collections.Generic;
using UnityEngine;

public class Field : Singleton<Field>
{
    [Header("Field Properties")]
    public float CellSize; 
    public float Spacing; 
    public int FieldSize; 
    public int InitialCellcounter = 2;

    [Space(10)]
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private RectTransform rectTransform;

    private Cell[,] field;

    private bool anyCellMoved; // was there any moves

    private void Start()
    {
        SwipeDetection.SwipeEvent += OnInput;
    }

    private void OnDisable()
    {
        SwipeDetection.SwipeEvent -= OnInput;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.W))
            OnInput(Vector2.up);        
        if (Input.GetKeyDown(KeyCode.A))
            OnInput(Vector2.left);        
        if (Input.GetKeyDown(KeyCode.S))
            OnInput(Vector2.down);        
        if (Input.GetKeyDown(KeyCode.D))
            OnInput(Vector2.right);
#endif
    }

    private void OnInput(Vector2 direction) 
    {
        if (!GameController.isGameStarted)
            return;

        anyCellMoved = false;
        ResetCellsflags();

        Move(direction);

        if (anyCellMoved) 
        {
            GenerateRandomCells();
            CheckGameResult();
        }
    }

    private void Move(Vector2 direction)
    {
        int startIndex = direction.x > 0 || direction.y < 0 ? FieldSize - 1 : 0;        // if move direction is RIGHT or DOWN start from last index
        int directionSign = direction.x != 0 ? (int)direction.x : -(int)direction.y;    // move to positive or negative direction on coordinate plane 

        for (int i = 0; i < FieldSize; i++)
        {
            for (int j = startIndex; j >= 0 && j < FieldSize; j -= directionSign)
            {
                var currentCell = direction.x != 0 ? field[j, i] : field[i, j];

                if (currentCell.isEmpty)
                    continue;

                // is there cell to merge with
                var cellToMerge = FindCellToMerge(currentCell, direction);
                if(cellToMerge != null) 
                {
                    currentCell.MergeWithCell(cellToMerge);
                    anyCellMoved = true;
                    continue;
                }

                // is there empty cell to move to
                var emptyCell = FindEmptyCell(currentCell, direction);
                if(emptyCell != null)
                {
                    currentCell.MoveToCell(emptyCell);
                    anyCellMoved = true;
                    continue;
                }
            }
        }
    }

    private Cell FindEmptyCell(Cell cell, Vector2 direction) 
    {
        Cell emptyCell = null;
        int startX = cell.X + (int)direction.x;
        int startY = cell.Y - (int)direction.y;
        for (int x = startX, y = startY;
            x >= 0 && x < FieldSize && y >= 0 && y < FieldSize;
            x += (int)direction.x, y -= (int)direction.y)
        {
            if (field[x, y].isEmpty)
                emptyCell = field[x, y];
            else
                break;
        }
            return emptyCell; 
    }

    private Cell FindCellToMerge(Cell cell, Vector2 direction) 
    {
        int startX = cell.X + (int)direction.x;
        int startY = cell.Y - (int)direction.y;
        for (int x = startX, y = startY;
            x >= 0 && x < FieldSize && y >= 0 && y < FieldSize;
            x += (int)direction.x, y -= (int)direction.y) 
        {
            if (field[x, y].isEmpty)
                continue;
            if (field[x, y].Value == cell.Value && !field[x, y].HasMerged)
                return field[x, y];
            break;
        }
        return null; 
    }

    // Check if there any moves
    private void CheckGameResult() 
    {
        bool lose = true;

        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                if (field[x, y].Value == Cell.MaxValue)
                {
                    GameController.Instance.Win();
                    return;
                }
                if (lose &&
                    field[x, y].isEmpty || 
                    FindCellToMerge(field[x, y], Vector2.up) ||
                    FindCellToMerge(field[x, y], Vector2.down) ||
                    FindCellToMerge(field[x, y], Vector2.left) ||
                    FindCellToMerge(field[x, y], Vector2.right)) 
                {
                    lose = false;
                }
            }
        }
        if (lose)
            GameController.Instance.Lose();
    }

    // Creates empty field
    private void CreateField() 
    {
        field = new Cell[FieldSize, FieldSize];

        float fieldWidth = FieldSize * (CellSize + Spacing) + Spacing;
        rectTransform.sizeDelta = new Vector2(fieldWidth, fieldWidth);

        float startPosX = -(fieldWidth / 2) + (CellSize / 2) + Spacing;
        float startPosY = (fieldWidth / 2) - (CellSize / 2) - Spacing;

        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                var cell = Instantiate(cellPrefab, transform, false);
                var position = new Vector2(startPosX + (x * (CellSize + Spacing)), startPosY - (y * (CellSize + Spacing)));
                cell.transform.localPosition = position;

                field[x, y] = cell;
                cell.SetValue(x, y, 0);
            }
        }

    }

    public void GenerateField()
    {
        if (field == null)
            CreateField();
        

        for (int x = 0; x < FieldSize; x++)
            for (int y = 0; y < FieldSize; y++)
                field[x, y].SetValue(x, y, 0);

        for (int i = 0; i < InitialCellcounter; i++)
        {
            GenerateRandomCells();
        }
    }

    private void GenerateRandomCells() 
    {
        var emptyCells = new List<Cell>();
        for (int x = 0; x < FieldSize; x++)
            for (int y = 0; y < FieldSize; y++)
                if(field[x, y].isEmpty) 
                    emptyCells.Add(field[x, y]);

        if (emptyCells.Count == 0)
            throw new System.Exception("There is no empty cells.");

        int value = Random.Range(0, 10) == 0 ? 2 : 1; // 90% - generate 1, 10% - generate 2
        var cell = emptyCells[Random.Range(0, emptyCells.Count)];
        cell.SetValue(cell.X, cell.Y, value, false);

        CellAnimationController.Instance.SmoothAppear(cell);
    }

    private void ResetCellsflags() 
    {
        for (int x = 0; x < FieldSize; x++)
            for (int y = 0; y < FieldSize; y++)
                field[x, y].ResetFlags();
    }
}
