using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Board : MonoBehaviour
{

    public static Board instance;
    public GameObject spawner;
    public const int ColumnCount = 6;
    public Spawner[] spawners;
    public bool isSelecting;

    public List<Dot> selected = new List<Dot>();
    public Color selectedColor;
    
    void Awake()
    {
      instance = this; // allow other classes to easily reference singleton Board instance
    
      // instantiate Spawner for each column of the board
      for(int i = 0; i < ColumnCount; i++) 
      {
        GameObject go = Instantiate(spawner, new Vector3(1 + (i*2f), 28f, 0), Quaternion.identity);
        go.GetComponent<Spawner>().columnIdx = i;
      }
      spawners = GameObject.FindObjectsOfType<Spawner>();
    }

    void Start()
    {
      
    }

    void Update()
    {
      if(Input.GetKeyDown(KeyCode.R)) {
        SceneManager.LoadScene(0);
      }

      if(Input.GetMouseButton(0))
      {
        isSelecting = true;
      } 
      else 
      {
        isSelecting = false;
      }

      // selection complete
      if(Input.GetMouseButtonUp(0) && selected.Count > 0) 
      {
        // square case
        if(selected.Count == 4 && DetectSquare(selected))
        {
          foreach(Spawner s in spawners)
          {
            s.column.RemoveList(s.column.GetAllDotsOfColor(selectedColor));
          }

        }

        // base case
        else if(selected.Count > 1)
        {

          foreach(Dot d in selected)
          {
            spawners[d.colIdx].column.Remove(d);
          }
        }
        selected.Clear();
      }
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
