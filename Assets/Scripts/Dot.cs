using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Dot : MonoBehaviour
{
    private Board board = Board.instance;
    private SpriteRenderer sr;
    private SpriteRenderer selectSr;
    private Vector3 mousePos;
    private bool isLatestSelection;

    public LineRenderer lr;
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

        selectSr = transform.Find("SelectedDot").GetComponent<SpriteRenderer>();
        selectSr.color = new Color(dotColor.r, dotColor.g, dotColor.b, 0.5f);
        
        lr = transform.Find("SelectPath").GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startColor = dotColor;
        lr.endColor = dotColor;
    }

    void Update()
    {
      if(board.isSelecting) 
      {
        mousePos = Camera.main.ScreenToWorldPoint(
          new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f)
        );
      }

      if(isLatestSelection) lr.SetPosition(1, mousePos);
    }

    void OnMouseDown() 
    {
      // if no current selections, add this dot to board.selected
      if(board.selected.Count == 0) {
        StartCoroutine(SelectEffect());
        board.selected.Add(this);
        lr.enabled = true;
        lr.SetPosition(0, this.transform.position);
        isLatestSelection = true; // sets flag to update line with mousePos per frame
        board.selectedColor = dotColor;
      }
      
      // check adjacency and colour match, add to board.selected if passing
      else if(!board.selected.Contains(this) && dotColor == board.selectedColor
        && CheckAdjacency(board.selected.Last()))
      {
        Dot prevDot = board.selected.Last();
        StartCoroutine(SelectEffect());
        board.selected.Add(this);
        prevDot.isLatestSelection = false;
        prevDot.lr.SetPosition(1, this.transform.position);

        lr.enabled = true;
        lr.SetPosition(0, this.transform.position);
        isLatestSelection = true;
      }

      // deselection - if you enter the collider of the penultimate dot
      // in selection, remove the most recently selected dot
      else if(board.selected.Count > 1 && board.selected.IndexOf(this) == board.selected.Count - 2) {
        Dot prevDot = board.selected[board.selected.Count - 2];
        Dot toRemove = board.selected.Last();

        toRemove.isLatestSelection = false;
        toRemove.lr.enabled = false;
        toRemove.transform.Find("SelectedDot").localScale = Vector3.one * 1.45f; // reset scale to allow selection coroutine to play
        board.selected.Remove(toRemove);

        lr.enabled = true;
        lr.SetPosition(0, this.transform.position);
        isLatestSelection = true;
      }

      // check if selection is eligible for square mechanic
      else if(board.selected.Count == 4 && DetectSquare(board.selected))
      {
        // player must complete the loop to confirm the mechanic
        if(board.selected.First() == this) 
        {
          board.squareSelection = true;
          board.selected.Last().isLatestSelection = false;
          board.selected.Last().lr.SetPosition(1, this.transform.position);

          foreach(Spawner s in board.spawners)
          {
            List<Dot> dotsToSelect = s.column.GetAllDotsOfColor(board.selectedColor);
            foreach(Dot d in dotsToSelect) StartCoroutine(d.SelectEffect());
            board.selected.AddRange(dotsToSelect);
          }
        }
      }
    }

    void OnMouseEnter() 
    {
      if(!board.isSelecting) return;
      OnMouseDown();
    }

    IEnumerator SelectEffect()
    {
      selectSr.enabled = true;
      Transform toScale = selectSr.transform;
      for(float scale = toScale.localScale.x; scale <= 2.25; scale += 0.025f)
      {
        toScale.localScale = Vector3.one * scale;
        yield return null;
      }
      selectSr.enabled = false;
      toScale.localScale = Vector3.one * 1.45f;
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
