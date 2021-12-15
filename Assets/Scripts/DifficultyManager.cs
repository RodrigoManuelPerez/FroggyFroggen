using UnityEngine;

public enum Difficulty { easy, hard }
public enum Sounds { resetB, resetS, menuOver, menuSelect, winB, winS, pierceS, pierceB, pointS, pointB, credits, logo, exit}

// Se encarga tanto de controlar la dificultad como 
public class DifficultyManager : MonoBehaviour
{
    [SerializeField]
    private Difficulty dificultad;

    public static DifficultyManager instance { private set; get; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    public Difficulty RotateDifficultty()
    {
        switch (dificultad)
        {
            case Difficulty.easy:
                dificultad = Difficulty.hard;
                break;
            case Difficulty.hard:
                dificultad = Difficulty.easy;
                break;
        }

        return dificultad;
    }

    public void SetDifficulty(Difficulty d)
    {
        dificultad = d;
    }
    public Difficulty GetDifficulty()
    {
        return dificultad;
    }
}
