/*
Author: Andres Mrad (Q-ro)
Date: Friday 10/April/2020 @ 15:42:16
Description:  Defines the AI behaviour using a sort of simplified min max strategy to find the best move in the current board context
*/

using GameplayEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


public class AIPlayerController : MonoBehaviour
{


    #region Inspector Properties

    #endregion

    #region Private Properties

    //[SerializeField] GameplayBoard gameplayBoard;
    //[SerializeField] GameplayCursor gameplayCursor;
    //[SerializeField] GameplayTurnTracker gameplayTurnTracker;

    //public GameplayBoard GameplayBoard { get => gameplayBoard; set => gameplayBoard = value; }
    //public GameplayCursor GameplayCursor { get => gameplayCursor; set => gameplayCursor = value; }
    //public GameplayTurnTracker GameplayTurnTracker { get => gameplayTurnTracker; set => gameplayTurnTracker = value; }

    public static ArrayList blackNo = new ArrayList();
    static System.Random random = new System.Random();

    #endregion

    // Somewhat naive approach to implemente a Gomoku AI
    // I honestly don't know much about the strategy involved in the game, but i'm assuming
    // the steps described below make sense and work as a base for a somewhat decent Gomoku AI
    public Vector2Int MakeMove(GameplayBoard gameplayBoard, GameplayEnum.GamePlayPieceTypes playingPieces = GameplayEnum.GamePlayPieceTypes.P2Pieces)
    {
        // Get available spaces
        var boardFreePositions = gameplayBoard.GetAvailableLocations();

        //if first move, or the center has not been taken, take center (assuming this is the best possible position to take, as this is the case in tic-tac-toe)
        var bestMove = new Vector2Int((gameplayBoard.BoardWidth - 1) / 2, (gameplayBoard.BoardHeight - 1) / 2);
        if (boardFreePositions.Contains((bestMove)))
        {
            return bestMove;
        }



        // Get current owned pieces
        // Get current oponent pieces

        // if opponent has 2 or more in a row, and the aret blocked, attempt to block

        // if we we own 2 or more in a row and they aren't blocked, feed the chain

        // NVM, falling back to this A.I. I found where the A.I. uses the min max strategy with alpha beta pruning
        var scores = this.AlphaBetaMax(gameplayBoard, 0, Mathf.Infinity, -Mathf.Infinity, playingPieces);
        return new Vector2Int(scores[0], scores[1]);
    }

    //private KeyValuePair<Vector2Int, int> MinMax(GameplayBoard board, int depth, float alpha, float beta, GameplayEnum.GamePlayPieceTypes playingPieces)
    // AlphaBetaMax
    // NVM, falling back to this A.I. I found where the A.I. uses the min max strategy with alpha beta pruning
    // https://github.com/W-KE/Unity-Gomoku-with-AI/blob/master/Chess/Assets/ChessAI.cs
    private int[] AlphaBetaMax(GameplayBoard board, int depth, float alpha, float beta, GameplayEnum.GamePlayPieceTypes playingPieces)
    {

        int[] v = MaxModelPoints((int)playingPieces, board);

        if (depth <= 0)
        {
            return v;
        }
        int[] best = new int[] { -1, -1, -10000 };
        ArrayList points = Gen(board);

        for (int i = 0; i < points.Count; i++)
        {
            int[] p = (int[])points[i];
            board.SimplifiedGameBoard[p[0], p[1]] = (int)playingPieces == 1 ? 2 : 1;
            v = AlphaBetaMax(board, depth - 1, best[2] > alpha ? best[2] : alpha, beta, playingPieces);
            board.SimplifiedGameBoard[p[0], p[1]] = 0;
            if (v[2] > best[2])
            {
                best = v;
            }
            if (v[2] > beta)
            {  //AB剪枝
                break;
            }
        }
        return best;
    }

    public static ArrayList Gen(GameplayBoard chessBoard)
    {
        ArrayList points = new ArrayList();
        var width = chessBoard.BoardWidth;
        var height = chessBoard.BoardHeight;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (HasNeighbour(x, y, chessBoard) && chessBoard.SimplifiedGameBoard[x, y] == 0)
                {
                    points.Add(new int[] { x, y });
                }
            }
        }
        return points;
    }

    public int[] MaxModelPoints(int playingPieces, GameplayBoard board)
    {
        ArrayList localBestPoints = FindMaxModel(playingPieces, board);
        ArrayList counterBestPoints = FindMaxModel(playingPieces == 1 ? 2 : 1, board);
        if (Math.Abs(((int[])localBestPoints[0])[2]) > Math.Abs(((int[])counterBestPoints[0])[2] / 5 * 4))
        {
            return (int[])localBestPoints[random.Next(localBestPoints.Count)];
        }
        return (int[])counterBestPoints[random.Next(counterBestPoints.Count)];
    }

    public ArrayList FindMaxModel(int playingPieces, GameplayBoard board)
    {
        ArrayList bestPoints = new ArrayList();
        int max_score = -1;
        int max_x = -1;
        int max_y = -1;

        for (int x = 0; x < board.BoardWidth; x++)
        {
            for (int y = 0; y < board.BoardHeight; y++)
            {
                if (HasNeighbour(x, y, board) && board.SimplifiedGameBoard[x, y] == (int)GameplayEnum.GamePlayPieceTypes.NONE)
                {
                    int score = GetTotalScoreModel(x, y, playingPieces, board);
                    if (max_x < 3 || max_y < 3 || max_x > 11 || max_y > 11)
                    {
                        score = score / 5 * 4;
                    }
                    if (score > max_score)
                    {
                        bestPoints.Clear();
                        max_score = score;
                        max_x = x;
                        max_y = y;
                    }
                    if (score == max_score)
                    {
                        bestPoints.Add(new int[] { max_x, max_y, playingPieces == (int)GamePlayPieceTypes.P2Pieces ? max_score : -max_score });
                    }
                }
            }
        }
        return bestPoints;
    }

    public static bool HasNeighbour(int x, int y, GameplayBoard chessBoard, int distance = 2)
    {
        for (int i = x - distance; i < x + distance; i++)
        {
            if (i < 0 || i >= chessBoard.BoardWidth)
            {
                continue;
            }
            for (int j = y - distance; j < y + distance; j++)
            {
                if (j < 0 || j >= chessBoard.BoardHeight)
                {
                    continue;
                }
                if (i == x && j == y)
                {
                    continue;
                }
                if (chessBoard.SimplifiedGameBoard[i, j] != 0)
                {
                    return true;
                }
            }
        }
        return false;
    }


    public static int GetTotalScoreModel(int x, int y, int role, GameplayBoard chessBoard)
    {
        int totalScore = 0;
        string model1 = "3";
        string model2 = "3";
        string model3 = "3";
        string model4 = "3";
        blackNo.Clear();

        //水平分数
        for (int i = 1; i < 5; i++)
        {
            if (x + i < chessBoard.BoardWidth)
            {
                if (chessBoard.SimplifiedGameBoard[x + i, y] == role && model1.IndexOf("00") == -1)
                {
                    model1 += '1';
                }
                else if (chessBoard.SimplifiedGameBoard[x + i, y] == 0 && model1.IndexOf("00") == -1)
                {
                    model1 += '0';
                }
                else
                {
                    model1 += '2';
                    break;
                }
                if (model1.IndexOf("00") != -1)
                {
                    model1 = model1.Substring(0, model1.Length - 1);
                    break;
                }
            }
            else
            {
                model1 += '2';
                break;
            }
        }
        for (int i = 1; i < 5; i++)
        {
            if (x - i >= 0)
            {
                if (chessBoard.SimplifiedGameBoard[x - i, y] == role && model1.IndexOf("00") == -1)
                {
                    model1 = '1' + model1;
                }
                else if (chessBoard.SimplifiedGameBoard[x - i, y] == 0 && model1.IndexOf("00") == -1)
                {
                    model1 = '0' + model1;
                }
                else
                {
                    model1 = '2' + model1;
                    break;
                }
                if (model1.IndexOf("00") != -1)
                {
                    model1 = model1.Substring(1, model1.Length - 1);
                    break;
                }
            }
            else
            {
                model1 = '2' + model1;
                break;
            }
        }

        //垂直分数
        for (int i = 1; i < 5; i++)
        {
            if (y + i < chessBoard.BoardWidth)
            {
                if (chessBoard.SimplifiedGameBoard[x, y + i] == role && model2.IndexOf("00") == -1)
                {
                    model2 += '1';
                }
                else if (chessBoard.SimplifiedGameBoard[x, y + i] == 0 && model2.IndexOf("00") == -1)
                {
                    model2 += '0';
                }
                else
                {
                    model2 += '2';
                    break;
                }
                if (model2.IndexOf("00") != -1)
                {
                    model2 = model2.Substring(0, model2.Length - 1);
                    break;
                }
            }
            else
            {
                model2 += '2';
                break;
            }
        }
        for (int i = 1; i < 5; i++)
        {
            if (y - i >= 0)
            {
                if (chessBoard.SimplifiedGameBoard[x, y - i] == role && model2.IndexOf("00") == -1)
                {
                    model2 = '1' + model2;
                }
                else if (chessBoard.SimplifiedGameBoard[x, y - i] == 0 && model2.IndexOf("00") == -1)
                {
                    model2 = '0' + model2;
                }
                else
                {
                    model2 = '2' + model2;
                    break;
                }
                if (model2.IndexOf("00") != -1)
                {
                    model2 = model2.Substring(1, model2.Length - 1);
                    break;
                }
            }
            else
            {
                model2 = '2' + model2;
                break;
            }
        }

        //斜线分数
        for (int i = 1; i < 5; i++)
        {
            if (x + i < chessBoard.BoardWidth && y + i < chessBoard.BoardWidth)
            {
                if (chessBoard.SimplifiedGameBoard[x + i, y + i] == role && model3.IndexOf("00") == -1)
                {
                    model3 += '1';
                }
                else if (chessBoard.SimplifiedGameBoard[x + i, y + i] == 0 && model3.IndexOf("00") == -1)
                {
                    model3 += '0';
                }
                else
                {
                    model3 += '2';
                    break;
                }
                if (model3.IndexOf("00") != -1)
                {
                    model3 = model3.Substring(0, model3.Length - 1);
                    break;
                }
            }
            else
            {
                model3 += '2';
                break;
            }
        }
        for (int i = 1; i < 5; i++)
        {
            if (x - i >= 0 && y - i >= 0)
            {
                if (chessBoard.SimplifiedGameBoard[x - i, y - i] == role && model3.IndexOf("00") == -1)
                {
                    model3 = '1' + model3;
                }
                else if (chessBoard.SimplifiedGameBoard[x - i, y - i] == 0 && model3.IndexOf("00") == -1)
                {
                    model3 = '0' + model3;
                }
                else
                {
                    model3 = '2' + model3;
                    break;
                }
                if (model3.IndexOf("00") != -1)
                {
                    model3 = model3.Substring(1, model3.Length - 1);
                    break;
                }
            }
            else
            {
                model3 = '2' + model3;
                break;
            }
        }

        //斜线分数
        for (int i = 1; i < 5; i++)
        {
            if (x + i < chessBoard.BoardWidth && y - i >= 0)
            {
                if (chessBoard.SimplifiedGameBoard[x + i, y - i] == role && model4.IndexOf("00") == -1)
                {
                    model4 += '1';
                }
                else if (chessBoard.SimplifiedGameBoard[x + i, y - i] == 0 && model4.IndexOf("00") == -1)
                {
                    model4 += '0';
                }
                else
                {
                    model4 += '2';
                    break;
                }
                if (model4.IndexOf("00") != -1)
                {
                    model4 = model4.Substring(0, model4.Length - 1);
                    break;
                }
            }
            else
            {
                model4 += '2';
                break;
            }
        }
        for (int i = 1; i < 5; i++)
        {
            if (x - i >= 0 && y + i < chessBoard.BoardWidth)
            {
                if (chessBoard.SimplifiedGameBoard[x - i, y + i] == role && model4.IndexOf("00") == -1)
                {
                    model4 = '1' + model4;
                }
                else if (chessBoard.SimplifiedGameBoard[x - i, y + i] == 0 && model4.IndexOf("00") == -1)
                {
                    model4 = '0' + model4;
                }
                else
                {
                    model4 = '2' + model4;
                    break;
                }
                if (model4.IndexOf("00") != -1)
                {
                    model4 = model4.Substring(1, model4.Length - 1);
                    break;
                }
            }
            else
            {
                model4 = '2' + model4;
                break;
            }
        }

        string[] models = new string[] { model1, model2, model3, model4 };
        string cPattern = "(^0*(1*31+|1+31*)0*2*$)|(^2*0*(1*31+|1+31*)0*$)";
        string bPattern = "(^2+0*(1*31+|1+31*)0*2*$)|(^2*0*(1*31+|1+31*)0*2+$)";
        string ncPattern = "[2]*[0]*(((1110|1101|1011|110|101)?3(0111|1011|1101|011|101))|((1110|1101|1011|110|101)3(0111|1011|1101|011|101)?)|((1110|1101|1011|110|101)3(0111|1011|1101|011|101)))[0]*[2]*";
        string n2Pattern = "[2]*[0]*10301[0]*[2]*";
        string n3Pattern = "[2]*[0]*(1301|1031)[0]*[2]*";
        string n4Pattern = "[2]*[0]*(11301|10311|10131|13101)[0]*[2]*";
        string nd4Pattern = "[2]*[0]*(1013101|111030111|11013011|11031011)[0]*[2]*";

        foreach (string model in models)
        {
            int twoFour = 0;
            int twoThree = 0;
            int blockThree = 0;
            if (Regex.IsMatch(model, nd4Pattern))
            {
                //Debug.Log(String.Format("ND4:{0},{1}", model, totalScore));
                if (role == 1)
                {
                    blackNo.Add(new int[x, y]);
                    totalScore = 0;
                    break;
                }
                totalScore += (int)GamePlayMovementScore.FOUR;
            }
            else if (Regex.IsMatch(model, n4Pattern))
            {
                //Debug.Log(String.Format("N4:{0},{1}", model, totalScore));
                twoFour++;
                if (twoFour >= 2 && role == 1)
                {
                    blackNo.Add(new int[x, y]);
                    totalScore = 0;
                    break;
                }
                totalScore += (int)GamePlayMovementScore.FOUR;
            }
            else if (Regex.IsMatch(model, n3Pattern))
            {
                //Debug.Log(String.Format("N3:{0},{1}", model, totalScore));
                if (model.Trim('2').Length >= 5)
                {
                    twoThree++;
                    if (twoThree >= 2 && role == 1)
                    {
                        blackNo.Add(new int[x, y]);
                        totalScore = 0;
                        break;
                    }
                    else if (twoThree >= 2 && role == 2)
                    {
                        totalScore += (int)GamePlayMovementScore.TWO_THREE;
                        twoThree--;
                    }
                    else if (twoThree == 1 && blockThree >= 1)
                    {
                        totalScore += (int)GamePlayMovementScore.THREE_THREE;
                        twoThree--;
                        blockThree--;
                    }
                    else
                    {
                        totalScore += (int)GamePlayMovementScore.THREE;
                    }
                }
                else
                {
                    blockThree++;
                    if (twoThree == 1 && blockThree >= 1 && role == 1)
                    {
                        totalScore += (int)GamePlayMovementScore.THREE_THREE;
                        blockThree--;
                    }
                    else if (twoThree == 1 && blockThree >= 1 && role == 2)
                    {
                        totalScore += (int)GamePlayMovementScore.THREE_THREE;
                        twoThree--;
                        blockThree--;
                    }
                    else
                    {
                        totalScore += (int)GamePlayMovementScore.BLOCKED_THREE;
                    }
                }
            }
            else if (Regex.IsMatch(model, ncPattern))
            {
                //Debug.Log(String.Format("NC:{0},{1}", model, totalScore));
                if (model.Trim('2').Trim('0').Length == 9)
                {
                    twoFour++;
                    if (twoFour >= 2 && role == 1)
                    {
                        blackNo.Add(new int[x, y]);
                        totalScore = 0;
                        break;
                    }
                    totalScore += (int)GamePlayMovementScore.FOUR;
                }
                else if (model.Trim('2').Trim('0').Length == 8)
                {
                    twoFour++;
                    twoThree++;
                    if ((twoThree >= 2 || twoFour >= 2) && role == 1)
                    {
                        blackNo.Add(new int[x, y]);
                        totalScore = 0;
                        break;
                    }
                    totalScore += (int)GamePlayMovementScore.FOUR;
                }
                else if (model.Trim('2').Trim('0').Length == 7)
                {
                    if (model.Trim('2').Trim('0').Length == model.Trim('0').Length)
                    {
                        totalScore += (int)GamePlayMovementScore.TWO_THREE;
                    }
                    else
                    {
                        totalScore += (int)GamePlayMovementScore.THREE_THREE;
                    }
                }
            }
            else if (Regex.IsMatch(model, n2Pattern))
            {
                //Debug.Log(String.Format("N2:{0},{1}", model, totalScore));
                if (model.Trim('2').Trim('0').Length == 5)
                {
                    if (model.Trim('2').Trim('0').Length == model.Trim('0').Length)
                    {
                        totalScore += (int)GamePlayMovementScore.TWO_TWO;
                    }
                    else
                    {
                        totalScore += (int)GamePlayMovementScore.BLOCKED_THREE;
                    }
                }
            }
            else if (Regex.IsMatch(model, bPattern))
            {
                if (model.Trim('2').Trim('0').Length > 5 && role == 1)
                {
                    blackNo.Add(new int[x, y]);
                    totalScore = 0;
                    break;
                }
                if (model.Trim('2').Trim('0').Length >= 5)
                {
                    totalScore += (int)GamePlayMovementScore.FIVE;
                    //Debug.Log(String.Format("B:{0},{1}", model, totalScore));
                }
                else if (model.Trim('2').Trim('0').Length == 4)
                {
                    twoFour++;
                    if (twoFour >= 2 && role == 1)
                    {
                        blackNo.Add(new int[x, y]);
                        totalScore = 0;
                        break;
                    }
                    totalScore += (int)GamePlayMovementScore.BLOCKED_FOUR;
                }
                else if (model.Trim('2').Trim('0').Length == 3)
                {
                    totalScore += (int)GamePlayMovementScore.BLOCKED_THREE;
                }
                else if (model.Trim('2').Trim('0').Length == 2)
                {
                    totalScore += (int)GamePlayMovementScore.BLOCKED_TWO;
                }
                else
                {
                    totalScore += (int)GamePlayMovementScore.BLOCKED_ONE;
                    //Debug.Log(String.Format("B:{0},{1}", model, totalScore));
                }
            }
            else if (Regex.IsMatch(model, cPattern))
            {
                if (model.Trim('0').Length > 5 && role == 1)
                {
                    blackNo.Add(new int[x, y]);
                    totalScore = 0;
                    break;
                }
                if (model.Trim('0').Length >= 5)
                {
                    totalScore += (int)GamePlayMovementScore.FIVE;
                    //Debug.Log(String.Format("C:{0},{1}", model, totalScore));
                }
                else if (model.Trim('0').Length == 4)
                {
                    twoFour++;
                    if (twoFour >= 2 && role == 1)
                    {
                        blackNo.Add(new int[x, y]);
                        totalScore = 0;
                        break;
                    }
                    totalScore += (int)GamePlayMovementScore.FOUR;
                }
                else if (model.Trim('0').Length == 3)
                {
                    twoThree++;
                    if (twoThree >= 2 && role == 1)
                    {
                        blackNo.Add(new int[x, y]);
                        totalScore = 0;
                        break;
                    }
                    totalScore += (int)GamePlayMovementScore.THREE;
                }
                else if (model.Trim('0').Length == 2)
                {
                    totalScore += (int)GamePlayMovementScore.TWO;
                }
                else
                {
                    totalScore += (int)GamePlayMovementScore.ONE;
                }
            }
        }
        return totalScore;
    }
}