using UnityEngine;
using DG.Tweening;

public class CellAnimationController : Singleton<CellAnimationController>
{
    [SerializeField] AnimationCell animationCellPref;
    private void Awake()
    {
        DOTween.Init();
    }

    public void SmoothTransition(Cell from, Cell to, bool isMerging) 
    {
        Instantiate(animationCellPref, transform, false).Move(from, to, isMerging);
    }

    public void SmoothAppear(Cell cell) 
    {
        Instantiate(animationCellPref, transform, false).Appear(cell);
    }
}
