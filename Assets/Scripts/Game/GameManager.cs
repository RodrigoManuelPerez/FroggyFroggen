using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Box { locked, empty, cursor, downRight, downLeft, upRight, upLeft, vertical, horizontal, coin, cross, piercer, piercerCursor}

public class Player : MonoBehaviour
{
    private Vector2Int originalDirection;  // Para tener una referencia para el reseteo de lenguas
    private Vector2Int direction;

    private bool piercer = false;

    private int Coins = 0;

    List<Vector2Int> path;
    Vector2Int lastCasilla = new Vector2Int();
    Vector2Int actualCasilla = new Vector2Int();

    public bool reseting { get; set; }

    public Player(int x, int y, Vector2Int dir)    // Posicion inicial y direccion de salida
    {
        lastCasilla = new Vector2Int(x, y) - dir;  // Calculamos de la que "venimos"
        actualCasilla = new Vector2Int(x, y);      // Guardamos la actual
        direction = dir;                        // Guardamos la direccion actual y la original
        originalDirection = dir;

        path = new List<Vector2Int>();             // Guardamos todas las casillas de donde venimos
        path.Add(actualCasilla);                  // Guardamos la primera para tomarla de referencia
    }

    public void SetDirection(Vector2Int d) 
    {
        // Estamos aqui
        Vector2Int nextCasilla = actualCasilla + d;

        // Calculamos la que seria la nueva casilla y vemos que no sea de la que venimos
        if (lastCasilla != nextCasilla)
            direction = d;
    }

    public Vector2Int GetDirection() { return direction; }
    public int GetDirectionValue() 
    {
        if (direction.x == 0)
        {
            if (direction.y == 1)
            {
                return 0;
            }
            else if (direction.y == -1)
            {
                return 2;
            }
        }
        else if (direction.x == 1)
        {
            return 1;
        }
        else if (direction.x == -1)
        {
            return 3;
        }

        return 0;
    }
    public Vector2Int GetActualCasilla() { return actualCasilla; }
    public Vector2Int GetLastCasilla() { return lastCasilla; }

    public Vector2Int NextMove()   // Calcula la nueva posicion y la devuelve para que la procese
    {
        return (actualCasilla + direction);
    }

    public void SetCasilla(Vector2Int p)   // Calcula la nueva posicion y la devuelve para que la procese
    {
        lastCasilla = actualCasilla;    // La ultima es en la que está ahora 
        actualCasilla = p;              // Ponemos la nueva actual
        path.Add(actualCasilla);
    }

    public List<Vector2Int> GetPath() { return path;}

    public void SetPiercer(bool v) { piercer = v;  }
    public bool GetPiercer() { return piercer; }  

    public void ResetTongue()
    {
        Vector2Int start = path[0];
        path = new List<Vector2Int>();
        path.Add(start);

        direction = originalDirection;

        lastCasilla = start - direction;  // Calculamos de la que "venimos"
        actualCasilla = start;

        reseting = false;
    }
}

public class GameManager : MonoBehaviour
{
    Player player1;
    Player player2;

    int player1Points = 0;
    int player2Points = 0;

    int maxPoints = 7;

    public Vector3 pivot;
    public float incr;

    float tick = 0.5f;
    public float easyTick = 0.375f;
    public float medTick = 0.25f;
    public float hardTick = 0.125f;

    public GameObject casilla;

    Casilla[,] board;

    float actualTime = 0;

    // Spawn de bufos
    float actualPiercerTime = 0;
    float actualPiercerTimeTick = 2.5f;
    int piercerBuffs = 0;
    [SerializeField]
    int maxPiercerBuffs = 1;

    float actualCrossTime = 0;
    float actualCrossTimeTick = 2f;
    int crossBuffs = 0;
    [SerializeField]
    int maxCrossBuffs = 2;

    float actualCoinTime = 0;
    float actualCoinTimeTick = 5f;
    int coins = 0;
    [SerializeField]
    int maxCoins = 1;

    public Animator[] p1PointsAnim;
    public Animator[] p2PointsAnim;

    public Animator smallFrog;
    public Animator bigFrog;

    public static GameManager instance;

    public Animator pauseAnimator;
    public Animator winAnimator;

    public GameObject pauseSelector;
    public Vector3[] pausePositions;
    public int pauseIndex = 0;

    public GameObject endSelector;
    public Vector3[] endPositions;
    public int endIndex = 0;

    // Variables de control del estado de juego
    private bool gameStarted = false;
    private bool paused = false;
    private bool canPause = false;
    private bool gameFinished = false;
    private bool canSelectEndGame = false;

    [SerializeField]
    private GameObject crownFroggy;
    [SerializeField]
    private GameObject crownFroggen;

    private void Awake()
    {
        if(instance == null)        
            instance = this;
        else 
            Destroy(this.gameObject);

        maxPoints = p1PointsAnim.Length;
    }

    // Start is called before the first frame update
    void Start()
    {
        switch (DifficultyManager.instance.GetDifficulty())
        {
            case Difficulty.easy:
                tick = easyTick;                
                break;
            case Difficulty.hard:
                tick = hardTick;
                break;
        }

        actualCoinTimeTick = tick * Random.Range(4, 9);
        actualPiercerTimeTick = tick * Random.Range(4, 9);
        actualCrossTimeTick = tick * Random.Range(4, 9);

        board = new Casilla[14,26];

        Vector3 auxPivot = pivot;

        for (int i = 0; i < 14; i++)
        {
            for (int j = 0; j < 26; j++)
            {
                GameObject go = Instantiate(casilla ,auxPivot, transform.rotation, this.transform);
                Casilla c = go.GetComponent<Casilla>();

                board[i, j] = c;

                if (i <= 8 || (i > 8 && (j >= 8 && j <= 17)))
                    c.SetBox(Box.empty);
                else
                    c.SetBox(Box.locked); 

                auxPivot += new Vector3(incr, 0, 0);
            }
            
            auxPivot = new Vector3(pivot.x, auxPivot.y, 0);
            auxPivot -= new Vector3(0, incr, 0);
        }
        
        player1 = new Player(12, 8, new Vector2Int(0,1));
        player2 = new Player(12, 17, new Vector2Int(0, -1));

        Invoke("StartGame", 1.0f);

        for (int i = 0; i < maxCoins; i++)
        {
            coins++;
            SpawnBuff(Box.coin);
        }        

        for (int i = 0; i < maxPiercerBuffs; i++) 
        {
            piercerBuffs++;
            SpawnBuff(Box.piercer);
        }

        for (int i = 0; i < maxCrossBuffs; i++)
        {
            crossBuffs++;
            SpawnBuff(Box.cross);
        }

        MusicManager.instance.PlayMusic();
    }

    private void StartGame()
    {
        smallFrog.SetTrigger("Start");
        bigFrog.SetTrigger("Start");
        Invoke("SetCursors", 0.67f);
    }

    private void SetCursors()
    {
        board[12, 8].SetBox(Box.cursor);
        board[12, 17].SetBox(Box.cursor);

        gameStarted = true;
        canPause = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted && !paused)
        {
            if (canPause && Input.GetKeyDown(KeyCode.Escape))
            {                
                StartCoroutine(TogglePause(true));
                MusicManager.instance.Pause();
            }
         
            // Input
            #region INPUT_PLAYER1

            if (Input.GetKeyDown(KeyCode.A))
            {
                player1.SetDirection(new Vector2Int(0,-1));
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                player1.SetDirection(new Vector2Int(1, 0));
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                player1.SetDirection(new Vector2Int(0, 1));
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                player1.SetDirection(new Vector2Int(-1, 0));
            }

            #endregion
            #region INPUT_PLAYER2

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                player2.SetDirection(new Vector2Int(0, -1));
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                player2.SetDirection(new Vector2Int(1, 0));
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                player2.SetDirection(new Vector2Int(0, 1));
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                player2.SetDirection(new Vector2Int(-1, 0));
            }

            #endregion

            // Aumento de tiempo
            actualTime += Time.deltaTime;
            
            actualPiercerTime += Time.deltaTime;
            actualCrossTime += Time.deltaTime;
            actualCoinTime += Time.deltaTime;

            #region BUFFOS

            if (actualCoinTime >= actualCoinTimeTick)
            {
                if (coins < maxCoins) {
                    coins++;
                    SpawnBuff(Box.coin);

                    // Si esto lo ponemos fuera hacemos que si se coge una moneda, pueda tardar
                    // entre 0 y actualCoinTimeTick en aparecer otra

                    // Al ponerlo dentro nos aseguramos que casi siempre haya alguna si hay un hueco
                    // Mas monedas en pantalla mas velocidad de juego
                    // actualCoinTime = 0;
                }
                actualCoinTime = 0;
            }

            if (actualPiercerTime >= actualPiercerTimeTick)
            {
                if (piercerBuffs < maxPiercerBuffs)
                {
                    piercerBuffs++;
                    SpawnBuff(Box.piercer);                    
                }
                actualPiercerTimeTick = 0;
            }

            if (actualCrossTime >= actualCrossTimeTick)
            {
                if (crossBuffs < maxCrossBuffs)
                {
                    crossBuffs++;
                    SpawnBuff(Box.cross);                    
                }

                actualCrossTimeTick = 0;
            }

            #endregion

            #region TICK

            if (actualTime >= tick)
            {
                ClearCursors();

                // Siempre va a ser una direccion correcta
                Vector2Int pos1 = player1.NextMove();
                Vector2Int pos2 = player2.NextMove();

                // Procesar las nuevas posiciones
                bool resetTongue1 = false;
                bool resetTongue2 = false;
                bool point1 = false;
                bool point2 = false;
                bool pierced1 = false;
                bool pierced2 = false;

                // CASO DEL BESO
                if (pos1 == pos2)
                {
                    resetTongue1 = true;
                    resetTongue2 = true;                 
                }
                else 
                {                    
                    // Procesando player 1
                    if (!player1.reseting) // No ha habido colision directa
                    {                     
                        if (!CheckPosIn(pos1))  // Si nos salimos del tablero
                        {
                            resetTongue1 = true; // Reseteamos la lengua
                        }
                        else                    // Si estamos dentro del tablero vemos que casilla vamos a ocupar
                        {
                            switch (board[pos1.x, pos1.y].box)
                            {
                                case Box.coin:
                                    point1 = true;
                                    coins--;
                                    board[pos1.x, pos1.y].SetBox(Box.empty);
                                    actualCoinTimeTick = 0;

                                    StartCoroutine(MusicManager.instance.PlaySound(Sounds.pointB));

                                    break;
                                case Box.piercer:
                                    player1.SetPiercer(true);
                                    piercerBuffs--;
                                    board[pos1.x, pos1.y].SetBox(Box.empty);
                                    actualPiercerTimeTick = 0;

                                    StartCoroutine(MusicManager.instance.PlaySound(Sounds.pierceB));

                                    break;
                                case Box.cross:
                                    if (!player2.reseting)
                                        resetTongue2 = true;
                                    crossBuffs--;
                                    board[pos1.x, pos1.y].SetBox(Box.empty);
                                    actualCrossTimeTick = 0;

                                    

                                    break;
                                case Box.horizontal:
                                case Box.vertical:
                                case Box.upLeft:
                                case Box.upRight:
                                case Box.downLeft:
                                case Box.downRight:
                                    if (!player1.GetPiercer())
                                        resetTongue1 = true;
                                    else                                    
                                        pierced1 = true;                                     
                                    break;
                                case Box.cursor:
                                case Box.locked:
                                    resetTongue1 = true;
                                    break;
                            }
                        }
                    }

                    // Procesando player2
                    if (!player2.reseting) 
                    {
                        if (!CheckPosIn(pos2))  // Si nos salimos del tablero
                        {
                            resetTongue2 = true; // Reseteamos la lengua
                        }
                        else                    // Si estamos dentro del tablero vemos que casilla vamos a ocupar
                        {
                            switch (board[pos2.x, pos2.y].box)
                            {
                                case Box.coin:
                                    point2 = true;
                                    coins--;
                                    board[pos2.x, pos2.y].SetBox(Box.empty);
                                    actualCoinTimeTick = 0;

                                    StartCoroutine(MusicManager.instance.PlaySound(Sounds.pointS));

                                    break;
                                case Box.piercer:
                                    player2.SetPiercer(true);
                                    piercerBuffs--;
                                    board[pos2.x, pos2.y].SetBox(Box.empty);
                                    actualPiercerTimeTick = 0;

                                    StartCoroutine(MusicManager.instance.PlaySound(Sounds.pierceS));

                                    break;
                                case Box.cross:
                                    if (!player1.reseting)
                                        resetTongue1 = true;
                                    crossBuffs--;
                                    board[pos2.x, pos2.y].SetBox(Box.empty);
                                    actualCrossTimeTick = 0;

                                    

                                    break;
                                case Box.horizontal:
                                case Box.vertical:
                                case Box.upLeft:
                                case Box.upRight:
                                case Box.downLeft:
                                case Box.downRight:
                                    if (!player2.GetPiercer())
                                        resetTongue2 = true;
                                    else
                                        pierced2 = true;
                                    break;
                                case Box.cursor:
                                case Box.locked:
                                    resetTongue2 = true;
                                    break;
                            }
                        }
                    }
                }

                // Ejecutamos
                if (resetTongue1)
                {
                    StartCoroutine(ResetTongue(player1, 1));
                }
                else
                {
                    Vector2Int auxActual = player1.GetActualCasilla();
                    Vector2Int auxLast = player1.GetLastCasilla();

                    // Comparamos la casilla anterior y a la que nos vamos a mover
                    // para determina que tipo de casilla representar
                    board[auxActual.x, auxActual.y].SetBox(Compare3Boxes(auxLast, auxActual, pos1));

                    // Actualizamos la casilla del player1
                    player1.SetCasilla(pos1);

                    // Asignamos el cursor
                    if(player1.GetPiercer())
                        board[pos1.x, pos1.y].SetBox(Box.piercerCursor, player1.GetDirectionValue());
                    else
                        board[pos1.x, pos1.y].SetBox(Box.cursor);

                    if(pierced1)
                        player1.SetPiercer(false);
                }

                if (resetTongue2)
                {
                    StartCoroutine(ResetTongue(player2, 2));
                }
                else
                {
                    Vector2Int auxActual = player2.GetActualCasilla();
                    Vector2Int auxLast = player2.GetLastCasilla();

                    // Comparamos la casilla anterior y a la que nos vamos a mover
                    // para determina que tipo de casilla representar
                    board[auxActual.x, auxActual.y].SetBox(Compare3Boxes(auxLast, auxActual, pos2));

                    // Actualizamos la casilla del player2
                    player2.SetCasilla(pos2);

                    // Asignamos el cursor

                    if (player2.GetPiercer())
                        board[pos2.x, pos2.y].SetBox(Box.piercerCursor, player2.GetDirectionValue());
                    else
                        board[pos2.x, pos2.y].SetBox(Box.cursor);

                    if (pierced2)
                        player2.SetPiercer(false);
                }
                #region POINTS

                if (point1)
                {
                    player1Points++;
                    p1PointsAnim[player1Points-1].SetTrigger("in");
                }

                if (point2)
                {
                    player2Points++;
                    p2PointsAnim[player2Points-1].SetTrigger("in");
                }

                if(player1Points == maxPoints && player2Points == maxPoints)
                {
                    EndGame(0);                    
                }
                else if (player1Points == maxPoints)
                {                                        
                    EndGame(1);
                }
                else if (player2Points == maxPoints)
                {                    
                    EndGame(2);
                }

                actualTime = 0.0f;

                #endregion
            }

            #endregion
        }
        else if (paused && canPause)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(TogglePause(false));
                MusicManager.instance.Pause(true);
            }
            else if (pauseIndex < 2 && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)))
            {
                StartCoroutine(MusicManager.instance.PlaySound(Sounds.menuOver));
                pauseIndex++;
                pauseSelector.transform.localPosition = pausePositions[pauseIndex];
            }
            else if (pauseIndex > 0 && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)))
            {
                StartCoroutine(MusicManager.instance.PlaySound(Sounds.menuOver));
                pauseIndex--;
                pauseSelector.transform.localPosition = pausePositions[pauseIndex];
            }
            else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                switch (pauseIndex)
                {
                    case 0:
                        StartCoroutine(TogglePause(false));
                        MusicManager.instance.Pause(true);
                        break;
                    case 1:
                        gameFinished = false;
                        gameStarted = false;
                        canPause = false;
                        StartCoroutine(MusicManager.instance.PlaySound(Sounds.menuSelect));
                        StartCoroutine(SceneLoader.instance.LoadScene(3));
                        break;
                    case 2:
                        gameFinished = false;
                        gameStarted = false;
                        canPause = false;
                        StartCoroutine(MusicManager.instance.PlaySound(Sounds.menuSelect));
                        StartCoroutine(SceneLoader.instance.LoadScene(2));
                        break;
                }
            }

        }
        else if (gameFinished && canSelectEndGame)
        {
            if (endIndex == 0 && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)))
            {
                endIndex = 1;
                StartCoroutine(MusicManager.instance.PlaySound(Sounds.menuOver));
                endSelector.transform.localPosition = endPositions[endIndex];
            }
            else if (endIndex == 1 && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)))
            {
                endIndex = 0;
                StartCoroutine(MusicManager.instance.PlaySound(Sounds.menuOver));
                endSelector.transform.localPosition = endPositions[endIndex];
            }
            else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                switch (endIndex)
                {
                    case 0:
                        gameFinished = false;
                        gameStarted = false;
                        canPause = false;
                        StartCoroutine(MusicManager.instance.PlaySound(Sounds.menuSelect));
                        StartCoroutine(SceneLoader.instance.LoadScene(3));                        
                        break;
                    case 1:
                        gameFinished = false;
                        gameStarted = false;
                        canPause = false;
                        StartCoroutine(MusicManager.instance.PlaySound(Sounds.menuSelect));
                        StartCoroutine(SceneLoader.instance.LoadScene(2));
                        break;
                }
            }
        }
    }

    private IEnumerator TogglePause(bool t)
    {
        canPause = false;

        if (t)
        {
            paused = t;

            TogglePauseBoard();

            pauseAnimator.SetTrigger("in");

            yield return new WaitForSeconds(0.5f);

            pauseSelector.SetActive(true);
            pauseSelector.transform.localPosition = pausePositions[pauseIndex];
        }
        else
        {
            pauseSelector.SetActive(false);
            pauseAnimator.SetTrigger("out");

            yield return new WaitForSeconds(0.5f);

            TogglePauseBoard();
            paused = t;
        }

        yield return new WaitForSeconds(0.25f);

        canPause = true;
    }

    private void TogglePauseBoard()
    {
        List<Casilla> activeCas = new List<Casilla>();

        for (int i = 0; i < board.GetLength(0) - 1; i++)
        {
            for (int j = 0; j < board.GetLength(1) - 1; j++)
            {
                if (board[i, j].box == Box.piercer || board[i, j].box == Box.cross || board[i, j].box == Box.coin)
                {
                    activeCas.Add(board[i, j]);
                }
            }
        }

        for (int i = 0; i < activeCas.Count; i++)
        {
            activeCas[i].paused = !activeCas[i].paused;
        }
        
    }

    private void EndGame(int winner)    // 0 empate // 1 winner P1 // 2 winner P2
    {
        gameStarted = false;

        if (winner == 1)
        {
            StartCoroutine(ResetTongueEnd(1));
        }
        else if (winner == 2)
        {
            StartCoroutine(ResetTongueEnd(2));
        }

        StartCoroutine(ShowEndMatch(winner));

        // Tiempo para la animacion final y que aparezcan opciones para volver a jugar o salir
        Invoke("EnableNextMatch", 2.0f);
    }

    private void EnableNextMatch()
    {
        TogglePauseBoard();
        gameFinished = true;        
    }

    private IEnumerator ShowEndMatch(int winner)
    {
        if (winner == 1)        
            winAnimator.SetTrigger("win1");
        else
            winAnimator.SetTrigger("win2");


        yield return new WaitForSeconds(0.5f);

        if (winner == 1)
            crownFroggen.SetActive(true);
        else
            crownFroggy.SetActive(true);

        endSelector.SetActive(true);
        endSelector.transform.localPosition = endPositions[endIndex];

        yield return new WaitForSeconds(2.5f);

        canSelectEndGame = true;
    }

    public bool CheckPosIn(Vector2Int pos)
    {
        bool inside = false;

        if ((pos.y >= 0 && pos.y < board.GetLength(1)) && pos.x >= 0 && pos.x <=8)
        {
            inside = true;
        }
        else if (pos.y >= 8 && pos.y <= 17 && pos.x < board.GetLength(0) && pos.x >= 0)
        {
            inside = true;
        }

        return inside;
    }

    public void SpawnBuff(Box b)
    {
        List<Casilla> validBoxes = new List<Casilla>();

        for (int i = 0; i < board.GetLength(0) - 1; i++)
        {
            for (int j = 0; j < board.GetLength(1) - 1; j++)
            {
                if (board[i, j].box == Box.empty)
                {
                    validBoxes.Add(board[i, j]);
                }
            }
        }

        validBoxes[Random.Range(0, validBoxes.Count)].SetBox(b);
    }

    private Box Compare3Boxes(Vector2Int last, Vector2Int mid, Vector2Int next)
    {
        Box box = Box.empty;

        if (last.x == next.x)
            box = Box.horizontal;
        else if (last.y == next.y)
            box = Box.vertical;

        
        else if ((last.x > next.x && last.y < next.y))
        {
            if(mid.x == last.x)            
                box = Box.upLeft;
            else            
                box = Box.downRight;                        
        }
        else if (last.x < next.x && last.y > next.y)
        {
            if (mid.x == last.x)            
                box = Box.downRight;            
            else
                box = Box.upLeft;
        }
        
        else if ((last.x < next.x && last.y < next.y))
        {
            if (mid.x == last.x)            
                box = Box.downLeft;            
            else            
                box = Box.upRight;            
        }
        else if (last.x > next.x && last.y > next.y)
        {
            if (mid.x == last.x)            
                box = Box.upRight;            
            else            
                box = Box.downLeft;            
        }

        return box;
    }

    private IEnumerator ResetTongue(Player p, int pIndex)
    {
        p.reseting = true;

        List<Vector2Int> path = new List<Vector2Int>();

        path = p.GetPath();

        p.SetPiercer(false);

        int cont = path.Count - 1;

        float timeStep = tick / path.Count;

        if(pIndex == 1)        
            StartCoroutine(MusicManager.instance.PlaySound(Sounds.resetB));        
        else        
            StartCoroutine(MusicManager.instance.PlaySound(Sounds.resetS));        

        while (cont > 0)
        {
            Vector2Int aux = path[cont];
            Vector2Int auxActu = path[cont - 1];

            board[aux.x, aux.y].SetBox(Box.empty);
            board[auxActu.x, auxActu.y].SetBox(Box.cursor);
            p.SetCasilla(auxActu);

            cont--;
            yield return new WaitForSeconds(timeStep);
        }

        p.ResetTongue();

        ClearCursors();

        yield return 0;
    }

    private IEnumerator ResetTongueEnd(int winner)
    {
        // Time.timeScale = 0.1f;

        player1.reseting = true;
        player2.reseting = true;

        List<Vector2Int> path1 = new List<Vector2Int>();
        List<Vector2Int> path2 = new List<Vector2Int>();

        path1 = player1.GetPath();
        path2 = player2.GetPath();

        player1.SetPiercer(false);
        player2.SetPiercer(false);

        int cont1 = path1.Count - 1;
        int cont2 = path2.Count - 1;

        float timeStep1 = (tick / path1.Count) * 2;
        float timeStep2 = (tick / path2.Count) * 2;

        if(timeStep2 < timeStep1)        
            timeStep1 = timeStep2;

        MusicManager.instance.StopMusic();

        while (cont1 >= 0 || cont2 >= 0)
        {
            if (cont1 >= 0)
            {
                Vector2Int aux = path1[cont1];
                board[aux.x, aux.y].SetBox(Box.empty);

                if (cont1 > 0)
                {
                    Vector2Int auxActu = path1[cont1 - 1];
                    board[auxActu.x, auxActu.y].SetBox(Box.cursor);
                    player1.SetCasilla(auxActu);
                }

                cont1--;                
            }

            if (cont2 >= 0)
            {
                Vector2Int aux = path2[cont2];
                board[aux.x, aux.y].SetBox(Box.empty);

                if (cont2 > 0) 
                {
                    Vector2Int auxActu = path2[cont2 - 1];
                    board[auxActu.x, auxActu.y].SetBox(Box.cursor);
                    player2.SetCasilla(auxActu);
                }

                cont2--;
            }

            yield return new WaitForSeconds(timeStep1);
        }

        if (winner == 1)
        {
            bigFrog.SetTrigger("Win");
            smallFrog.SetTrigger("Lose");            
            StartCoroutine(MusicManager.instance.PlaySound(Sounds.winB));
        }
        else
        {
            bigFrog.SetTrigger("Lose");
            smallFrog.SetTrigger("Win");
            StartCoroutine(MusicManager.instance.PlaySound(Sounds.winS));
        }

        player1.ResetTongue();
        player2.ResetTongue();

        ClearCursors(true);

        yield return 0;
    }

    private void ClearCursors(bool remove = false)
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j].box == Box.cursor)
                {
                    board[i, j].SetBox(Box.empty);
                }
            }
        }

        if (!remove)
        {
            Vector2Int pos1 = player1.GetActualCasilla();
            Vector2Int pos2 = player2.GetActualCasilla();

            board[pos1.x, pos1.y].SetBox(Box.cursor);
            board[pos2.x, pos2.y].SetBox(Box.cursor);
        }
    }
}
