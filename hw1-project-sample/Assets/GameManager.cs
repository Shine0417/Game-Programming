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
    
    [SerializeField] private Text _accScoreFromStartText;

    private const int Width = 6;
    private const int Height = 10;
    private const float TimeBeforeDestroy = 0.2f;

    private const int MiddleX = Width / 2;
    private const int Top = Height - 1;

    private const float MaxElapsedTimePerStep = 0.3f;
    private const float MaxElapsedTimeFromStart = 60f;

    private float _elapsedTimeFromStart;
    private int _accScoreFromStart;

    private float ElapsedTimeFromStart
    {
        get => _elapsedTimeFromStart;

        set
        {
            _elapsedTimeFromStart = value;
            _elapsedTimeFromStartText.text = _elapsedTimeFromStart.ToString("F2");
        }
    }

    private int AccScoreFromStart
    {
        get => _accScoreFromStart;

        set
        {
            _accScoreFromStart = value;
            _accScoreFromStartText.text = _accScoreFromStart.ToString();
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

        AccScoreFromStart = 0;

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
                    tiles = DestroyIfRowFull(tiles, tile.Y);
                    tile = CreateNewTile(tiles);

                    if (tile == null) break;
                }
            }
            
            if (elapsedTime >= MaxElapsedTimePerStep)
            {
                if (!MoveTileDown(tiles, tile))
                {
                    tiles = DestroyIfRowFull(tiles, tile.Y);
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

    private List<Tile> DestroyIfRowFull(List<Tile> tiles, int y) 
    {
        List<Tile> newTiles = tiles.Where(t => t.Y != y).ToList();
        if (tiles.Count - newTiles.Count == Width) {
            StartCoroutine(DestroyRowCoroutine(tiles, y));
            StartCoroutine(DecendHigherCoroutine(tiles, y));

            tiles = newTiles;
            AccScoreFromStart += 10;
        }
        return tiles;
    }

    private IEnumerator DestroyRowCoroutine(List<Tile> tiles, int y)
    {
        List<Tile> sameRowTiles = tiles.Where(t => t.Y == y).ToList();

        foreach(Tile t in sameRowTiles)
        {
            t.SetColor(Color.grey);
            t.Hit();
        }

        yield return new WaitForSeconds(TimeBeforeDestroy);

        sameRowTiles.ForEach(t => Destroy(t.gameObject));
    }

    private IEnumerator DecendHigherCoroutine(List<Tile> tiles, int y)
    {
        yield return new WaitForSeconds(TimeBeforeDestroy);

        List<Tile> higherRowTiles = tiles.Where(t => t.Y > y && t.Y != y).ToList();
        higherRowTiles.ForEach(t => t.Y--);
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
