/*
Author: Andres Mrad (Q-ro)
Date: Wednesday 08/April/2020 @ 18:17:01
Description:  Keeps track of the gameboard internal structure 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using GameplayEnum;
using UnityEngine;

public class GameplayBoard : MonoBehaviour
{

    #region Inspector Properties

    [Range(5, 30)]
    [Tooltip("Width of the board (in number of lines)")]
    [SerializeField] private int boardWidth = 15;

    [Range(5, 30)]
    [Tooltip("Height of the board (in number of lines)")]
    [SerializeField] private int boardHeight = 15;
    [SerializeField] private float boardSquareSize = 0.5f;
    // [SerializeField] private float boardPaddingTop = 4.5f;
    // [SerializeField] private float boardPaddingLeft = 4.5f;

    [SerializeField] private float boardGridBorderLineWidth = 0.15f;
    [SerializeField] private Material boardGridBorderLineMaterial;
    [SerializeField] private Color boardGridBorderColor = Color.gray;

    [SerializeField] private float boardGridLineWidth = 0.05f;
    [SerializeField] private Material boardGridLineMaterial;
    [SerializeField] private Color boardGridColor = Color.gray;
    // the prefabs for the gameplay pieces
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;

    #endregion

    #region Private Properties

    private GameObject[,] _gameBoard; // The actual game board positions
    private int[,] _simplifiedGameBoard; // The actual game board positions, simplified for use by the A.I.
    private GameObject _verticalLinesContainer; // containers for the different game objects to be created, keeping things clean inside the scene hierarchy
    private GameObject _horizontalLinesContainer; // containers for the different game objects to be created, keeping things clean inside the scene hierarchy
    List<Vector2Int> _freeRealState = new List<Vector2Int>();

    // Deleting since apparently you can no longer set the parent of an instantiated prefab
    // private GameObject _p1GamePiecesContainer; // containers for the different game objects to be created, keeping things clean inside the scene hierarchy
    // private GameObject _p2GamePiecesContainer; // containers for the different game objects to be created, keeping things clean inside the scene hierarchy

    public int BoardWidth { get => boardWidth; }
    public int BoardHeight { get => boardHeight; }
    public int[,] SimplifiedGameBoard { get => _simplifiedGameBoard; }

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        this.InitGameObjectContainers();
        this.InitBoard();
        this.DrawGameplayGrid();
    }

    #endregion

    #region Class Methods

    // Initializes a clean game board
    private void InitBoard()
    {

        // generate the gamefield
        this._gameBoard = new GameObject[this.boardWidth, this.boardHeight];
        this._simplifiedGameBoard = new int[this.boardWidth, this.boardHeight];

        for (int i = 0; i < this.boardWidth; i++)
        {
            for (int j = 0; j < this.boardHeight; j++)
            {
                this._gameBoard[i, j] = null;
                this._simplifiedGameBoard[i, j] = (int)GamePlayPieceTypes.NONE;
                this._freeRealState.Add(new Vector2Int(i, j));
            }
        }

    }

    private void InitGameObjectContainers()
    {

        this._verticalLinesContainer = new GameObject();
        this._verticalLinesContainer.transform.parent = this.transform;
        this._verticalLinesContainer.gameObject.name = "VerticalLinesContainer";
        this._verticalLinesContainer.gameObject.transform.position = Vector3.zero;
        this._verticalLinesContainer.gameObject.transform.rotation = Quaternion.identity;
        //Make sure the containers belong to the same layer as this game object
        this._verticalLinesContainer.layer = this.gameObject.layer;

        this._horizontalLinesContainer = new GameObject();
        this._horizontalLinesContainer.transform.parent = this.transform;
        this._horizontalLinesContainer.gameObject.name = "HorizontalLinesContainer";
        this._horizontalLinesContainer.gameObject.transform.position = Vector3.zero;
        this._horizontalLinesContainer.gameObject.transform.rotation = Quaternion.identity;
        //Make sure the containers belong to the same layer as this game object
        this._horizontalLinesContainer.layer = this.gameObject.layer;

        // this._p1GamePiecesContainer = new GameObject ();
        // this._p1GamePiecesContainer.transform.parent = this.transform;
        // this._p1GamePiecesContainer.gameObject.name = "P1GamePiecesContainer";
        // this._p1GamePiecesContainer.gameObject.transform.position = Vector3.zero;
        // this._p1GamePiecesContainer.gameObject.transform.rotation = Quaternion.identity;

        // this._p2GamePiecesContainer = new GameObject ();
        // this._p2GamePiecesContainer.transform.parent = this.transform;
        // this._p2GamePiecesContainer.gameObject.name = "P2GamePiecesContainer";
        // this._p2GamePiecesContainer.gameObject.transform.position = Vector3.zero;
        // this._p2GamePiecesContainer.gameObject.transform.rotation = Quaternion.identity;

    }

    // Draws the gameplay field
    private void DrawGameplayGrid()
    {

        // Draw vertical lines
        for (int i = 0; i < this.boardWidth + 2; i++)
        {
            // Determine if we are drowing the edges of the board and swap colors accordingly
            var tempLineColor = (i == 0 || i == this.boardWidth + 1) ? this.boardGridBorderColor : this.boardGridColor;
            var tempLineWidth = (i == 0 || i == this.boardWidth + 1) ? this.boardGridBorderLineWidth : this.boardGridLineWidth;
            var tempLineMaterial = (i == 0 || i == this.boardWidth + 1) ? this.boardGridBorderLineMaterial : this.boardGridLineMaterial;
            var tempOrderInLayer = (i == 0 || i == this.boardHeight + 1) ? 1 : 0;

            this.DrawGridLine(new Vector2(i * this.boardSquareSize, 0),
                new Vector2(i * this.boardSquareSize, (this.boardWidth + 1) * this.boardSquareSize),
                "VerticalLine-" + i,
                tempLineColor,
                tempLineColor,
                tempLineMaterial,
                tempLineWidth,
                tempLineWidth,
                tempOrderInLayer,
                this._verticalLinesContainer
            );
        }

        // Draw horizontal lines
        for (int i = 0; i < this.boardHeight + 2; i++)
        {
            // Determine if we are drowing the edges of the board and swap colors accordingly
            var tempLineColor = (i == 0 || i == this.boardHeight + 1) ? this.boardGridBorderColor : this.boardGridColor;
            var tempLineWidth = (i == 0 || i == this.boardHeight + 1) ? this.boardGridBorderLineWidth : this.boardGridLineWidth;
            var tempLineMaterial = (i == 0 || i == this.boardHeight + 1) ? this.boardGridBorderLineMaterial : this.boardGridLineMaterial;
            var tempOrderInLayer = (i == 0 || i == this.boardHeight + 1) ? 1 : 0;

            this.DrawGridLine(new Vector2(0, i * this.boardSquareSize),
                new Vector2(this.boardSquareSize * (this.boardHeight + 1), i * this.boardSquareSize),
                "HorizontalLine-" + i,
                tempLineColor,
                tempLineColor,
                tempLineMaterial,
                tempLineWidth,
                tempLineWidth,
                tempOrderInLayer,
                this._horizontalLinesContainer
            );
        }

        // center the camera on the board 
        // TODO:(really hacky, should be improved)
        var center = this.GetBoardWorldPositionCenter();
        Vector3 tempBoardCenter = new Vector3(center.x, center.y, Camera.main.transform.position.z);
        Camera.main.transform.SetPositionAndRotation(tempBoardCenter, Quaternion.identity);

    }

    private void DrawGridLine(
        Vector2 startingPoint,
        Vector2 endPoint,
        string lineGameObjectName,
        Color startColor,
        Color endColor,
        Material material,
        float startWidth,
        float endWidth,
        int orderInLayer,
        GameObject parentContainer
    )
    {

        GameObject line = new GameObject("Line-" + lineGameObjectName);
        LineRenderer render = line.AddComponent<LineRenderer>();
        render.startColor = startColor;
        render.endColor = endColor;
        render.material = material;
        render.startWidth = startWidth;
        render.endWidth = endWidth;
        render.sortingOrder = orderInLayer;

        //Set the start position of the line
        render.SetPosition(0, new Vector3(startingPoint.x, startingPoint.y, 0));
        //Set the end position of the line
        render.SetPosition(1, new Vector3(endPoint.x, endPoint.y, 0));

        // make sure the lines do not cast or recieve shadows
        render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        render.receiveShadows = false;

        // Make the line game object a child of the board to keep the scene clean
        line.transform.parent = parentContainer.transform;

    }

    // Returns the center point of the game board
    public Vector2 GetBoardWorldPositionCenter()
    {
        return this.GetWorldPositionFromBoardXY((this.boardHeight) / 2, (this.boardWidth) / 2);
    }

    public Vector2 GetWorldPositionFromBoardXY(int x, int y)
    {
        // add 1 due to the board borders
        x += 1;
        y += 1;
        return new Vector2((((x) * this.boardSquareSize)), (((y) * this.boardSquareSize)));
    }

    public Vector2 GetBoardMatrixPosition(Vector2 position)
    {
        return new Vector2(
            ((position.x) / this.boardSquareSize),
            ((position.y) / this.boardSquareSize)
        );
    }

    public bool PlaceGamePiece(GamePlayPieceTypes pieceType, int x, int y)
    {
        // Remove 1 due to the board borders
        x -= 1;
        y -= 1;

        // Make sure we aren't trying to make an illegal move
        if (this._gameBoard[x, y] != null)
        {
            return false;
        }

        GameObject tempGamepieceToPlace = (pieceType == GamePlayPieceTypes.X_Pieces) ? (GameObject)Instantiate(this.player1Prefab) : (GameObject)Instantiate(this.player2Prefab);
        tempGamepieceToPlace.transform.position = this.GetWorldPositionFromBoardXY(x, y);
        tempGamepieceToPlace.GetComponent<GamePiece>().GamePieceType = pieceType;
        this._gameBoard[x, y] = tempGamepieceToPlace;
        this._freeRealState.Remove(new Vector2Int(x, y));
        this._simplifiedGameBoard[x, y] = (int)pieceType;

        return true;
    }

    public List<Vector2Int> GetAvailableLocations()
    {
        return this._freeRealState;
    }

    #endregion
}