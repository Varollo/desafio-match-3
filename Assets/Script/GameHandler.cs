using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private GameController gameController;

    [SerializeField] public int boardWidth = 10;

    [SerializeField] public int boardHeight = 10;

    [SerializeField] public BoardView boardView;

    private GameController Controller => gameController ??= new GameController();

    private void Awake()
    {
        boardView.onTileClick += OnTileClick;
    }

    private void Start()
    {
        List<List<Tile>> board = Controller.StartGame(boardWidth, boardHeight);
        boardView.Setup(this);
        boardView.CreateBoard(board);
    }

    private int selectedX, selectedY = -1;

    private bool isAnimating;


    private void OnTileClick(int x, int y)
    {
        if (isAnimating) return;

        if (selectedX > -1 && selectedY > -1)
        {
            if (Mathf.Abs(selectedX - x) + Mathf.Abs(selectedY - y) > 1)
            {
                selectedX = -1;
                selectedY = -1;
            }
            else
            {
                isAnimating = true;
                boardView.SwapTiles(selectedX, selectedY, x, y).onComplete += () =>
                {
                    bool isValid = Controller.IsValidMovement(selectedX, selectedY, x, y);
                    if (!isValid)
                    {
                        boardView.SwapTiles(x, y, selectedX, selectedY)
                        .onComplete += () => isAnimating = false;
                    }
                    else
                    {
                        List<BoardSequence> swapResult = Controller.SwapTile(selectedX, selectedY, x, y);

                        AnimateBoard(swapResult, 0, () => isAnimating = false);
                    }

                    selectedX = -1;
                    selectedY = -1;
                };
            }
        }
        else
        {
            selectedX = x;
            selectedY = y;
        }
    }

    private void AnimateBoard(List<BoardSequence> boardSequences, int i, Action onComplete)
    {
        Sequence sequence = DOTween.Sequence();

        BoardSequence boardSequence = boardSequences[i];
        sequence.Append(boardView.DestroyTiles(boardSequence.matchedPosition));
        sequence.Append(boardView.MoveTiles(boardSequence.movedTiles));
        sequence.Append(boardView.CreateTile(boardSequence.addedTiles));

        i++;
        if (i < boardSequences.Count)
        {
            sequence.onComplete += () => AnimateBoard(boardSequences, i, onComplete);
        }
        else
        {
            sequence.onComplete += () => onComplete();
        }
    }

    public void AddScoreListener(ScoreManager.ScoreChangedDelegate listener) => Controller.ScoreManager.OnScoreChanged += listener;
    public void RemoveScoreListener(ScoreManager.ScoreChangedDelegate listener) => Controller.ScoreManager.OnScoreChanged -= listener;

    public void IncreaseScore(int amt = 1) => Controller.ScoreManager.Score += amt;
}
