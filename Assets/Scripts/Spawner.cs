using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private Board board = Board.instance;
    public Column<Dot> column;
    public int columnIdx;
    public GameObject dotPrefab;
    
    // extension of List that functions as a model for a column of dots.
    // add/remove functions are overriden to destroy/instantiate new dots.
    // this[0] = dot at bottom of board
    public class Column<T> : List<T> where T : Dot
    {
      private GameObject toSpawn; 
      private Vector3 spawnPos;
      public int columnIdx;

      public Column(GameObject toSpawn, Vector3 spawnPos, int columnIdx) : base()
      {
        this.toSpawn = toSpawn;
        this.spawnPos = spawnPos;
        this.columnIdx = columnIdx;
      }

      public new void RemoveAt(int index)
      {
        GameObject.Destroy(this[index].gameObject);
        base.RemoveAt(index);
        this.FillColumn();
      }

      public void FillColumn() 
      {
        int dotsToSpawn = Board.ColumnCount - this.Count;
        for(int i = dotsToSpawn; i > 0; i--)
        {
          // spawns dots at a fixed height apart
          GameObject go = Instantiate(toSpawn, new Vector3(spawnPos.x, spawnPos.y - (2.5f*i), spawnPos.z), Quaternion.identity);
          Dot curDot = go.GetComponent<Dot>();
          curDot.dotIdx = this.Count;
          curDot.colIdx = this.columnIdx;
          base.Add(curDot as T);
        }
      }
    }

    void Start()
    {
      column = new Column<Dot>(dotPrefab, this.transform.position, columnIdx);
      column.FillColumn();
    }

    void Update()
    {

    }
}
