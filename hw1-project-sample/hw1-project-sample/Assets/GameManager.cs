using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _boardRoot;

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private Text _elapsedTimeFromStartText;

    private const int Width = 6;
    private const int Height = 10;

    private const int MiddleX = Width / 2;
    private const int Top = Height - 1;

    private const float MaxElapsedTimePerStep = 0.3f;
    private const float MaxElapsedTimeFromStart = 60f;

    private float _elapsedTimeFromStart;

    private float ElapsedTimeFromStart
    {
        get => _elapsedTimeFromStart;

        set
        {
            _elapsedTimeFromStart = value;
            _elapsedTimeFromStartText.text = _elapsedTimeFromStart.ToString("F2");
        }
    }

    private IEnumerator Start()
    {
        while (true)
        {
            yield return PlayGame();
        }
    }

    private IEnumerator PlayGame()
    {
        var tiles = new List<Tile>();

        var tile = CreateNewTile(tiles);

        ElapsedTimeFromStart = 0f;
        var elapsedTime = 0f;

        while (ElapsedTimeFromStart <= MaxElapsedTimeFromStart)
        {
            ElapsedTimeFromStart += Time.deltaTime;
            elapsedTime += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveTileLeft(tiles, tile);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveTileRight(tiles, tile);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (!MoveTileDown(tiles, tile))
                {
                    tile = CreateNewTile(tiles);

                    if (tile == null) break;
                }
            }
            
            if (elapsedTime >= MaxElapsedTimePerStep)
            {
                if (!MoveTileDown(tiles, tile))
                {
                    tile = CreateNewTile(tiles);

                    if (tile == null) break;
                }

                elapsedTime = 0;
            }

            yield return null;
        }
    
        // GameOver
        ElapsedTimeFromStart = Mathf.Min(ElapsedTimeFromStart, MaxElapsedTimeFromStart);

        yield return new WaitForSeconds(1f);

        Destroy(tile?.gameObject);

        foreach (var t in tiles)
        {
            Destroy(t.gameObject);
        }
    }
    private Tile CreateNewTile(IEnumerable<Tile> tiles)
    {
        if (tiles.Any(other => other.X == MiddleX && other.Y == Top))
            return null;

        var tile = Instantiate(_tilePrefab, _boardRoot);
        tile.X = MiddleX;
        tile.Y = Top;
        return tile;
    }
    private static bool CanTileMoveTo(IEnumerable<Tile> tiles, int x, int y, int width)
    {
        if (x < 0 || x >= width || y < 0)
            return false;

        return !tiles.Any(other => other.X == x && other.Y == y);
    }

    private static void MoveTileLeft(IEnumerable<Tile> tiles, Tile tile)
    {
        if (!CanTileMoveTo(tiles, tile.X - 1, tile.Y, Width)) return;
        tile.X--;
    }
    private static void MoveTileRight(IEnumerable<Tile> tiles, Tile tile)
    {
        if (!CanTileMoveTo(tiles, tile.X + 1, tile.Y, Width)) return;
        tile.X++;
    }

    private static bool MoveTileDown(ICollection<Tile> tiles, Tile tile)
    {
        if (CanTileMoveTo(tiles, tile.X, tile.Y - 1, Width))
        {
            tile.Y--;
            return true;
        }

        tile.Hit();
        tiles.Add(tile);
        return false;
    }
    public void Fill()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                var tile = Instantiate(_tilePrefab, _boardRoot);
                tile.X = x;
                tile.Y = y;
            }
        }
    }
}
