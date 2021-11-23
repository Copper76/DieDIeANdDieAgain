using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class DisappearingBlockBehaviour : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject player;
    public Vector3Int playerGridPosition;
    public Vector3 playerTransform;
    public TileBase[] allTiles;
    public GridLayout gridLayout;
    public List<Vector3Int> trackedCells = new List<Vector3Int>();

    // Start is called before the first frame update
    void Start()
    {
        gridLayout = GetComponentInParent<GridLayout>();
        tilemap = GetComponent<Tilemap>();
        playerTransform = player.transform.position;
        playerGridPosition = tilemap.LocalToCell(playerTransform);
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        allTiles = tilemap.GetTilesBlock(bounds);
        

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                tile.name = "DisappearingTile3";
                if (tile != null)
                {
                    Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
                }
                else
                {
                    Debug.Log("x:" + x + " y:" + y + " tile: (null)");
                }
            }
        }

        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // NB: Bounds cannot have zero width in any dimension, including z
        var cellBounds = new BoundsInt(
            gridLayout.WorldToCell(other.bounds.min),
            gridLayout.WorldToCell(other.bounds.size) + new Vector3Int(0, 0, 1));

        IdentifyIntersections(other, cellBounds);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Same as OnTriggerEnter2D()
        var cellBounds = new BoundsInt(
            gridLayout.WorldToCell(other.bounds.min),
            gridLayout.WorldToCell(other.bounds.size) + new Vector3Int(0, 0, 1));

        IdentifyIntersections(other, cellBounds);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Intentionally pass zero size bounds
        IdentifyIntersections(other, new BoundsInt(Vector3Int.zero, Vector3Int.zero));
    }

    void IdentifyIntersections(Collider2D other, BoundsInt cellBounds)
    {
        var exitedCells = trackedCells.ToList();

        foreach (var cell in cellBounds.allPositionsWithin)
        {
            // check not null in cellPos
            if (tilemap.HasTile(cell))
            {
                // Find closest world point to this cell's center within other collider
                var cellWorldCenter = gridLayout.CellToWorld(cell);
                var otherClosestPoint = other.ClosestPoint(cellWorldCenter);
                var otherClosestCell = gridLayout.WorldToCell(otherClosestPoint);

                // Check if intersection point is within this cell
                if (otherClosestCell == cell)
                {
                    if (!trackedCells.Contains(cell))
                    {
                        // other collider just entered this cell
                        trackedCells.Add(cell);

                        // Do actions based on other collider entered this cell
                        
                    }
                    else
                    {
                        // other collider remains in this cell, so remove it from the list of exited cells
                        exitedCells.Remove(cell);
                    }
                }
            }
        }

        // Remove cells that are no longer intersected with
        foreach (var cell in exitedCells)
        {
            trackedCells.Remove(cell);

            // Do actions based on other collider exited this cell
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerTransform = player.transform.position;
        playerGridPosition = tilemap.LocalToCell(playerTransform);
    }
}
/*
public class DisappearingTile : TileBase
{
    public int LivesLeft = 3;
    public Sprite sprite1 = Resources.Load<Sprite>("Sprites/Anan's masterpiece/green");
    public Sprite sprite2 = Resources.Load<Sprite>("Sprites/Anan's masterpiece/pink");
    public Sprite sprite3 = Resources.Load<Sprite>("Sprites/Anan's masterpiece/bright_teal");

    public DisappearingTile()
    {
        
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite
        base.GetTileData(position, tilemap, ref tileData);
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        
        base.RefreshTile(position, tilemap);
    }
}
*/