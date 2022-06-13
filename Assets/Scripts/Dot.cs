using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dot : MonoBehaviour
{
    private Board board = Board.instance;
    private SpriteRenderer sr;
    private SpriteRenderer selectSr;

    public static readonly List<Color> Palette = new List<Color> {
      new Color(.55f, .75f, 1), new Color(.9f, .86f, .13f), new Color(.94f, .36f, .26f), new Color(.55f, .92f, .58f), new Color(.61f, .36f, .71f)
    };
    public Color dotColor;
    public int dotIdx;
    public int colIdx;

    void Start()
    {
        dotColor = Palette[Random.Range(0, Palette.Count)];
        sr = this.GetComponent<SpriteRenderer>();
        sr.color = dotColor;
        selectSr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        selectSr.color = new Color(dotColor.r, dotColor.g, dotColor.b, 0.5f);
    }

    void Update()
    {
      if(board.selected.Contains(this)) 
      {
        selectSr.enabled = true;
      }
      else 
      {
        selectSr.enabled = false;
      }
    }

    void OnMouseDown() 
    {
      // if no current selections, add this dot to selection
      if(board.selected.Count == 0) {
        board.selected.Add(this);
        board.selectedColor = dotColor;
      }
      
      // check adjacency and colour match, add to selected if passing
      else if(!board.selected.Contains(this) && dotColor == board.selectedColor
        && CheckAdjacency(board.selected[board.selected.Count - 1]))
      {
        board.selected.Add(this);
      }

      // deselection - if you enter the collider of the penultimate dot
      // in selection, remove the most recently selected dot
      else if(board.selected.Count > 1 && board.selected.IndexOf(this) == board.selected.Count - 2) {
        board.selected.Remove(board.selected[board.selected.Count - 1]);
      }

      // after adding/removing, check if square condition 
      // is fulfilled, and add all relevant dots if so
      if(board.selected.Count == 4 && DetectSquare(board.selected))
      {
        board.squareSelection = true;
        board.selected.Clear();
        foreach(Spawner s in board.spawners)
        {
          board.selected.AddRange(s.column.GetAllDotsOfColor(board.selectedColor));
        }
      }
    }

    void OnMouseEnter() 
    {
      if(!board.isSelecting) return;
      OnMouseDown();
    }

    // checks if `this` dot is adjacent to `refDot`
    private bool CheckAdjacency(Dot refDot)
    {
      bool columnAdj = refDot.colIdx == this.colIdx && 
        (this.dotIdx == refDot.dotIdx + 1 || this.dotIdx == refDot.dotIdx - 1);

      bool rowAdj = this.dotIdx == refDot.dotIdx &&
        (this.colIdx == refDot.colIdx + 1 || this.colIdx == refDot.colIdx - 1);
      
      return (columnAdj || rowAdj);
    }

    // if diff in colIdx and dotIdx is no more than 1 for
    // all dots in selection, selection is a square
    private bool DetectSquare(List<Dot> selection)
    {
      if(selection.Count != 4) return false;
      
      int minRow = selection.Min(dot => dot.dotIdx);
      int maxRow = selection.Max(dot => dot.dotIdx);

      int minCol = selection.Min(dot => dot.colIdx);
      int maxCol = selection.Max(dot => dot.colIdx);
      
      return (maxRow - minRow == 1 && maxCol - minCol == 1);
    }
}
