using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController
{
    private readonly Type[] _allowedTypes = new Type[] { };

    private readonly ScoreManager _scoreManager = new ScoreManager();
    private List<List<Tile>> _boardTiles;
    private List<int> _tilesGroups;
    private int _tileCount;

    public ScoreManager ScoreManager => _scoreManager;

    public List<List<Tile>> StartGame(int boardWidth, int boardHeight)
    {
        _scoreManager.Reset();
        _tilesGroups = new List<int> { 0, 1, 2, 3 };
        _boardTiles = CreateBoard(boardWidth, boardHeight, _tilesGroups);
        
        return _boardTiles;
    }

    public bool IsValidMovement(int fromX, int fromY, int toX, int toY)
    {
        List<List<Tile>> newBoard = CopyBoard(_boardTiles);

        Tile switchedTile = newBoard[fromY][fromX];
        newBoard[fromY][fromX] = newBoard[toY][toX];
        newBoard[toY][toX] = switchedTile;

        for (int y = 0; y < newBoard.Count; y++)
        {
            for (int x = 0; x < newBoard[y].Count; x++)
            {
                if (x > 1
                    && newBoard[y][x].group == newBoard[y][x - 1].group
                    && newBoard[y][x - 1].group == newBoard[y][x - 2].group)
                {
                    return true;
                }
                if (y > 1
                    && newBoard[y][x].group == newBoard[y - 1][x].group
                    && newBoard[y - 1][x].group == newBoard[y - 2][x].group)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <param name="tileCallbackLimit">How many sequences to callback tile</param>
    public List<BoardSequence> SwapTile(int fromX, int fromY, int toX, int toY, Transform boardTransform, int tileCallbackLimit = -1)
    {
        List<BoardSequence> boardSequences = new List<BoardSequence>();
        List<List<Tile>> newBoard = CopyBoard(_boardTiles);
        List<List<bool>> matchedTiles;

        Tile switchedTile = newBoard[fromY][fromX];
        newBoard[fromY][fromX] = newBoard[toY][toX];
        newBoard[toY][toX] = switchedTile;

        BoardData boardData = new BoardData()
        {
            width = newBoard[0].Count,
            height = newBoard.Count,
            transform = boardTransform
        };

        TileCallbackData tileData = newBoard[fromY][fromX].
            OnTileSwap(new Vector2Int(fromX, fromY), boardData) +
            newBoard[toY][toX].OnTileSwap(new Vector2Int(toX, toY), boardData);

        while (HasMatch(matchedTiles = FindMatches(newBoard)))
        {
            List<AddedTileInfo> addedTiles = new List<AddedTileInfo>();
            List<MovedTileInfo> movedTilesList = new List<MovedTileInfo>();
            List<Vector2Int> matchedPosition = new List<Vector2Int>();

            #region Clearing matched tiles
            for (int y = 0; y < newBoard.Count; y++)
            {
                for (int x = 0; x < newBoard[y].Count; x++)
                {
                    if (matchedTiles[y][x])
                    {
                        if (tileCallbackLimit != 0)
                            tileData += newBoard[y][x].OnTileDestroyed(new Vector2Int(x, y), boardData);

                        matchedPosition.Add(new Vector2Int(x, y));
                        newBoard[y][x] = TileFactory.EmptyTile();
                    }
                }
            }

            // Clearing tiles requested by [OnTileDestroy] callback
            if (tileData.toDestroy != null)
            {
                foreach (Vector2Int tilePos in tileData.toDestroy)
                {
                    if (newBoard[tilePos.y][tilePos.x].group == -1)
                        continue;

                    matchedPosition.Add(tilePos);
                    newBoard[tilePos.y][tilePos.x] = TileFactory.EmptyTile();
                }
            }
            #endregion

            #region Dropping down tiles
            Dictionary<Vector2Int, MovedTileInfo> tilesToMove = new Dictionary<Vector2Int, MovedTileInfo>();
            foreach (Vector2Int matchedSpot in matchedPosition)
            {
                Vector2Int emptySpot = matchedSpot;

                for (int y = matchedSpot.y - 1; y >= 0; y--)
                {
                    Vector2Int spot = new Vector2Int(matchedSpot.x, y);
                    Tile tile = newBoard[spot.y][spot.x];

                    if (tile.group >= 0)
                    {
                        MovedTileInfo moveInfo = new MovedTileInfo()
                        {
                            from = spot,
                            to = emptySpot
                        };

                        if (tilesToMove.ContainsKey(spot))
                        {
                            moveInfo.from = tilesToMove[spot].from;
                            tilesToMove.Remove(spot);
                        }

                        tilesToMove.Add(emptySpot, moveInfo);

                        var empty = newBoard[emptySpot.y][emptySpot.x];
                        newBoard[emptySpot.y][emptySpot.x] = tile;
                        newBoard[spot.y][spot.x] = empty;

                        emptySpot = spot;
                    }
                }
            }

            foreach (var move in tilesToMove)
            {
                if (tileCallbackLimit != 0)
                    tileData += newBoard[move.Value.to.y][move.Value.to.x].OnTileMove(move.Value, boardData);
                movedTilesList.Add(move.Value);
            } 
            #endregion

            #region Filling the board
            for (int y = newBoard.Count - 1; y > -1; y--)
            {
                for (int x = newBoard[y].Count - 1; x > -1; x--)
                {
                    if (newBoard[y][x].group == -1)
                    {
                        int group = RNGManager.From(_tilesGroups);
                        Tile tile = TileFactory.RandomTile(_tileCount++, group);
                        
                        newBoard[y][x] = tile;
                        AddedTileInfo addInfo = new AddedTileInfo
                        {
                            position = new Vector2Int(x, y),
                            Tile = tile
                        };

                        if (!addedTiles.Contains(addInfo))
                        {
                            addedTiles.Add(addInfo);

                            if (tileCallbackLimit != 0)
                                tileData += tile.OnTileCreated(new Vector2Int(x, y), boardData);
                        }
                    }
                }
            }
            #endregion

            BoardSequence sequence = new BoardSequence
            {
                matchedPosition = matchedPosition,
                movedTiles = movedTilesList,
                addedTiles = addedTiles,
                beforeSequence = tileData.beforeSequence
            };

            boardSequences.Add(sequence);
            tileData = TileCallbackData.Empty;
            
            if(tileCallbackLimit > 0)
                tileCallbackLimit--;
        }

        _boardTiles = newBoard;
        return boardSequences;
    }

    private static bool HasMatch(List<List<bool>> list)
    {
        for (int y = 0; y < list.Count; y++)
            for (int x = 0; x < list[y].Count; x++)
                if (list[y][x])
                    return true;
        return false;
    }

    private static List<List<bool>> FindMatches(List<List<Tile>> newBoard)
    {
        List<List<bool>> matchedTiles = new List<List<bool>>();
        for (int y = 0; y < newBoard.Count; y++)
        {
            matchedTiles.Add(new List<bool>(newBoard[y].Count));
            for (int x = 0; x < newBoard.Count; x++)
            {
                matchedTiles[y].Add(false);
            }
        }

        for (int y = 0; y < newBoard.Count; y++)
        {
            for (int x = 0; x < newBoard[y].Count; x++)
            {
                if (x > 1
                    && newBoard[y][x].group == newBoard[y][x - 1].group
                    && newBoard[y][x - 1].group == newBoard[y][x - 2].group)
                {
                    matchedTiles[y][x] = true;
                    matchedTiles[y][x - 1] = true;
                    matchedTiles[y][x - 2] = true;
                }
                if (y > 1
                    && newBoard[y][x].group == newBoard[y - 1][x].group
                    && newBoard[y - 1][x].group == newBoard[y - 2][x].group)
                {
                    matchedTiles[y][x] = true;
                    matchedTiles[y - 1][x] = true;
                    matchedTiles[y - 2][x] = true;
                }
            }
        }

        return matchedTiles;
    }

    private static List<List<Tile>> CopyBoard(List<List<Tile>> boardToCopy)
    {
        List<List<Tile>> newBoard = new List<List<Tile>>(boardToCopy.Count);
        for (int y = 0; y < boardToCopy.Count; y++)
        {
            newBoard.Add(new List<Tile>(boardToCopy[y].Count));
            for (int x = 0; x < boardToCopy[y].Count; x++)
            {
                Tile tile = boardToCopy[y][x];
                newBoard[y].Add(TileFactory.CopyTile(tile));
            }
        }
        return newBoard;
    }

    private List<List<Tile>> CreateBoard(int width, int height, List<int> tileGroups)
    {
        List<List<Tile>> board = new List<List<Tile>>(height);
        _tileCount = 0;
        for (int y = 0; y < height; y++)
        {
            board.Add(new List<Tile>(width));
            for (int x = 0; x < width; x++)
            {
                board[y].Add(TileFactory.RandomTile(-1, tileGroups, _allowedTypes));
                board[y][x].OnTileCreated(new Vector2Int(x, y), new BoardData() 
                { 
                    width = width, 
                    height = height
                });
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                List<int> noMatchGroups = new List<int>(tileGroups.Count);
                for (int i = 0; i < tileGroups.Count; i++)
                {
                    noMatchGroups.Add(_tilesGroups[i]);
                }

                if (x > 1
                    && board[y][x - 1].group == board[y][x - 2].group)
                {
                    noMatchGroups.Remove(board[y][x - 1].group);
                }
                if (y > 1
                    && board[y - 1][x].group == board[y - 2][x].group)
                {
                    noMatchGroups.Remove(board[y - 1][x].group);
                }

                board[y][x] = TileFactory.RandomTile(board[y][x].id, noMatchGroups, _allowedTypes);
                board[y][x].OnTileCreated(new Vector2Int(x, y), new BoardData()
                {
                    width = width,
                    height = height
                });
            }
        }

        return board;
    }
}
