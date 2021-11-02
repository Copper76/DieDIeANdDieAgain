using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public GridLayout gridLayout;

    public Vector3Int playerGridPosition;
    public Vector3 playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        GridLayout gridLayout = transform.parent.GetComponentInParent<GridLayout>();
        Vector3Int cellPosition = gridLayout.WorldToCell(transform.position);
        transform.position = gridLayout.CellToWorld(cellPosition);
    }

    // Update is called once per frame
    void Update()
    {
        playerGridPosition = gridLayout.WorldToCell(transform.position);
        
        Debug.Log(string.Format("Tick {0}: player at grid pos {1} and real world pos {2}.", Time.frameCount, playerGridPosition.ToString(), transform.position.ToString()));
    }
}
