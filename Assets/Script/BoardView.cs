﻿using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BoardView : MonoBehaviour
{
    [SerializeField] private TileSpotView tileSpotPrefab;

    [SerializeField] private TilePrefabRepository tilePrefabRepo;

    [SerializeField] private GridLayoutGroup boardContainer;

    private GameHandler gameHandler;

    private TileSpotView[][] _tileSpots;

    private TileView[][] _tiles;

    public event Action<int, int> onTileClick;

    public void Setup(GameHandler gameHandler) => this.gameHandler = gameHandler;

    public void CreateBoard(List<List<Tile>> board)
    {
        boardContainer.constraintCount = board[0].Count;

        _tileSpots = new TileSpotView[board.Count][];
        _tiles = new TileView[board.Count][];

        for (int y = 0; y < board.Count; y++)
        {
            _tileSpots[y] = new TileSpotView[board[0].Count];
            _tiles[y] = new TileView[board[0].Count];

            for (int x = 0; x < board[0].Count; x++)
            {
                Tile tile = board[y][x];
                TileSpotView tileSpot = Instantiate(tileSpotPrefab);
                tileSpot.transform.SetParent(boardContainer.transform, false);
                tileSpot.SetPosition(x, y);
                tileSpot.onClick += OnTileSpotClick;

                _tileSpots[y][x] = tileSpot;
                tileSpot.name = $"tile ({x}, {y})";
                
                int tileTypeIndex = tile.group;
                if (tileTypeIndex > -1)
                {
                    TileView tileView = tile.GetTileViewInstance(tilePrefabRepo);
                    tileView.OnTileCreated(this, tile);
                    
                    tileSpot.SetTile(tileView);
                    _tiles[y][x] = tileView;
                }
            }
        }
    }

    public void DestroyBoard()
    {
        for (int y = 0; y < _tiles.Length; y++)
        {
            for (int x = 0; x < _tiles[y].Length; x++)
            {
                _tiles[y][x].OnTileDestroyed(this);
                Destroy(_tiles[y][x].gameObject);
                Destroy(_tileSpots[y][x].gameObject);
            }
        }

        _tileSpots = null;
        _tiles = null;
    }

    public Tween SwapTiles(int fromX, int fromY, int toX, int toY)
    {
        Sequence swapSequence = DOTween.Sequence();
        swapSequence.Append(_tileSpots[fromY][fromX].AnimatedSetTile(_tiles[toY][toX]));
        swapSequence.Join(_tileSpots[toY][toX].AnimatedSetTile(_tiles[fromY][fromX]));

        TileView SwapedTile = _tiles[fromY][fromX];
        _tiles[fromY][fromX] = _tiles[toY][toX];
        _tiles[toY][toX] = SwapedTile;

        return swapSequence;
    }

    public Tween DestroyTiles(List<Vector2Int> matchedPosition)
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < matchedPosition.Count; i++)
        {
            Vector2Int position = matchedPosition[i];
            gameHandler.IncreaseScore();
            
            TileView tileView = _tiles[position.y][position.x];
            Tween onDestroy = tileView.OnTileDestroyed(this);
            
            if (onDestroy != null)
            {
                onDestroy.onComplete += () => Destroy(tileView.gameObject);
                sequence.Join(onDestroy);
            }
            else
                Destroy(tileView.gameObject);

            _tiles[position.y][position.x] = null;
        }
        return sequence;
    }

    public Tween MoveTiles(List<MovedTileInfo> movedTiles)
    {
        TileView[][] tiles = new TileView[_tiles.Length][];
        for (int y = 0; y < _tiles.Length; y++)
        {
            tiles[y] = new TileView[_tiles[y].Length];
            for (int x = 0; x < _tiles[y].Length; x++)
            {
                tiles[y][x] = _tiles[y][x];
            }
        }

        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < movedTiles.Count; i++)
        {
            MovedTileInfo movedTileInfo = movedTiles[i];

            Vector2Int from = movedTileInfo.from;
            Vector2Int to = movedTileInfo.to;

            sequence.Join(_tileSpots[to.y][to.x].AnimatedSetTile(_tiles[from.y][from.x]));
            tiles[to.y][to.x] = _tiles[from.y][from.x];
        }

        _tiles = tiles;
        return sequence;
    }

    public Tween CreateTile(List<AddedTileInfo> addedTiles)
    {
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < addedTiles.Count; i++)
        {
            AddedTileInfo addedTileInfo = addedTiles[i];
            Vector2Int position = addedTileInfo.position;

            TileSpotView tileSpot = _tileSpots[position.y][position.x];
            TileView tileView = addedTileInfo.Tile.GetTileViewInstance(tilePrefabRepo);
            
            seq.Join(tileView.OnTileCreated(this, addedTileInfo.Tile));
            tileSpot.SetTile(tileView);
            _tiles[position.y][position.x] = tileView;

            tileView.transform.localScale = Vector2.zero;
            seq.Join(tileView.transform.DOScale(1.0f, 0.2f));
        }

        return seq;
    }

    private void OnTileSpotClick(int x, int y)
    {
        onTileClick(x, y);
    }
}