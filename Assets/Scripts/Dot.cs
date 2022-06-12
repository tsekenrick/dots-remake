using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    private Board board = Board.instance;
    public SpriteRenderer sr;
    public SpriteRenderer selectSr;

    public static readonly Color[] Palette = {
      new Color(.55f, .75f, 1), new Color(.9f, .86f, .13f), new Color(.94f, .36f, .26f), new Color(.55f, .92f, .58f), new Color(.61f, .36f, .71f)
    };
    public Color dotColor;
    public int dotIdx;
    public int colIdx;

    void Start()
    {
        dotColor = Palette[Random.Range(0, Palette.Length)];
        sr = this.GetComponent<SpriteRenderer>();
        sr.color = dotColor;
        selectSr = transform.GetChild(0).GetComponent<SpriteRenderer>();
 
        // TODO - consider a coroutine that sets .simulated = false x seconds after instantiating a dot, then set
        // column of dots back to .simulated = true whenever FillColumn is called
    }

    void Update()
    {

    }

    void OnMouseDown() 
    {
      // if no current selections, add this dot to selection
      if(board.selected.Count == 0) {
        board.selected.Add(this);
        board.selectedColor = dotColor;

        // TODO - better selected state
        selectSr.enabled = true;
      }
      // check adjacency and colour match, add to selected if passing
      // TODO - deselection logic
      else if(!board.selected.Contains(this) && dotColor == board.selectedColor
        && CheckAdjacency(board.selected[board.selected.Count - 1]))
      {
        selectSr.enabled = true;
        board.selected.Add(this);
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

    void OnCollisionEnter2D(Collision2D coll)
    {
      
    }

    void OnCollisionExit2D()
    {
    }
}