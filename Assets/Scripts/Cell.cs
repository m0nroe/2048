using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public int Value { get; private set; }
    public int Points => Value == 0 ? 0 : (int)Mathf.Pow(2, Value);

    public bool isEmpty => Value == 0;
    public bool HasMerged { get; private set; }

    public const int MaxValue = 11; // 2 ^ 11 = 2048


    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI points;

    public void SetValue(int x, int y, int value)
    {
        X = x;
        Y = y;
        Value = value;
        UpdateCell();
    }

    // Update cell's info
    public void UpdateCell() 
    {
        points.text = isEmpty ? string.Empty : Points.ToString(); // set value of the cell
        points.color = Value <= 2 ? ColorManager.Instance.pointsDark : 
            ColorManager.Instance.pointsLight; // set text color
        image.color = ColorManager.Instance.CellColors[Value]; // set cell color
    }

    // Calls when two cells merged
    private void IncreaseValue() 
    {
        Value++;
        HasMerged = true;
        GameController.Instance.AddScorePoints(Points);
        UpdateCell();
    }

    public void ResetFlags()
    {
        HasMerged = false;
    }

    // Merge with other cell with same Value
    public void MergeWithCell(Cell othercell)
    {
        othercell.IncreaseValue();
        SetValue(X, Y, 0);
        UpdateCell();
    }

    // Move to empty cell
    public void MoveToCell(Cell target) 
    {
        target.SetValue(target.X, target.Y, Value);
        SetValue(X, Y, 0);
        UpdateCell();
    }
}
