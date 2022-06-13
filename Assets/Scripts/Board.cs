using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Board : MonoBehaviour
{

    public static Board instance;
    public const int ColumnCount = 6;
    public static readonly Color[] Palette = {
      new Color(.55f, .75f, 1), new Color(.9f, .86f, .13f), new Color(.94f, .36f, .26f), new Color(.55f, .92f, .58f), new Color(.61f, .36f, .71f)
    };
    public GameObject spawner;
    public Spawner[] spawners;

    public bool isSelecting;
    public List<Dot> selected = new List<Dot>();
    public Color selectedColor;
    public bool squareSelection = false;
    
    void Awake()
    {
      instance = this; // allow other classes to easily reference singleton Board instance
      
      // instantiate Spawner for each column of the board
      spawners = new Spawner[ColumnCount];
      for(int i = 0; i < ColumnCount; i++) 
      {
        GameObject go = Instantiate(spawner, new Vector3(1 + (i*2f), 28f, 0), Quaternion.identity);
        Spawner s = go.GetComponent<Spawner>();
        s.columnIdx = i;
        spawners[i] = s;
      }
    }

    void Update()
    {
      if(Input.GetKeyDown(KeyCode.R)) 
      {
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

      // user finished selection
      if(Input.GetMouseButtonUp(0) && selected.Count > 0) 
      {
        ExecuteSelection(squareSelection);
      }
    }

    private void ExecuteSelection(bool isSquare)
    {
      // for square selections, spawned dots should 
      // not be of the previous selection's color
      if(isSquare) Dot.Palette.Remove(selectedColor);

      if(selected.Count > 1)
      {
        List<Spawner> colsToFill = new List<Spawner>();
        foreach(Dot d in selected)
        {
          spawners[d.colIdx].column.Remove(d);
          if(!colsToFill.Contains(spawners[d.colIdx])) colsToFill.Add(spawners[d.colIdx]);
        }

        foreach(Spawner s in colsToFill) s.column.FillColumn();
      }
      selected.Last().lr.enabled = false;
      selected.Clear();

      if(isSquare) {
        Dot.Palette.Add(selectedColor);
        squareSelection = false;
      }
    }
}
