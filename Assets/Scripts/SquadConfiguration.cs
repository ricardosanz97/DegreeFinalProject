using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadConfiguration : MonoBehaviour
{
    public Squad currentSquadSelected;

    public enum SQUAD_ROL
    {
        None,
        Commander,
        Giant,
        Close,
        Distance,
        Player,
        Body
    }

    public static bool ListsAreEquals(List<SQUAD_ROL> list1, List<SQUAD_ROL> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }
        else
        {
            for (int i = 0; i<list1.Count; i++)
            {
                if (list1[i] != list2[i])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static bool ListsAreNumberByWrestlersEqual(Squad a, Squad b)
    {
        bool areEqual = a.numClose == b.numClose && a.numDistance == b.numDistance && a.numGiant == b.numGiant && a != b;
        if (areEqual)
        {
            Debug.Log(a.name + " is equal to " + b.name);
            return true;
        }
        else
        {
            Debug.Log(a.name + " is not equal to " + b.name);
            return false;
        }
    }

    [System.Serializable]
    public class Squad
    {
        public List<SQUAD_ROL> listFormation;
        public string name;
        public Index leaderPosition; //when spawn the squad, the leader is always the commander.
        public Index bodyPosition;
        public SquadSlot[,] squad;
        public int squadCols;
        public int squadRows;
        public bool hasBody;
        public int numClose;
        public int numDistance;
        public int numGiant;

        public Squad(string name, List<SQUAD_ROL> listFormation, int numRows, int numCols)
        {
            this.name = name;
            this.listFormation = listFormation;
            this.squadRows = numRows;
            this.squadCols = numCols;
            squad = CreateCustomSquad(listFormation, numRows, numCols);
        }

        public SquadSlot[,] CreateCustomSquad(List<SQUAD_ROL> squadRols, int numRows, int numCols)
        {
            SquadSlot[,] newSquad = new SquadSlot[numRows, numCols];

            for (int i = 0; i<numRows; i++)
            {
                for (int j = 0; j<numCols; j++)
                {
                    newSquad[i, j] = new SquadSlot(squadRols[i * numCols + j]);
                    if (newSquad[i,j].rol == SQUAD_ROL.Body)
                    {
                        bodyPosition = new Index(i, j);
                        hasBody = true;
                    }
                    if (newSquad[i,j].rol == SQUAD_ROL.Commander)
                    {
                        leaderPosition = new Index(i, j);
                    }
                    newSquad[i, j].position = new Index(i, j);
                    switch (newSquad[i, j].rol)
                    {
                        case SQUAD_ROL.Close:
                            numClose++;
                            break;
                        case SQUAD_ROL.Distance:
                            numDistance++;
                            break;
                        case SQUAD_ROL.Giant:
                            numGiant++;
                            break;
                    }
                }
            }

            return newSquad;
        }
    }

    [System.Serializable]
    public class SquadSlot
    {
        public SQUAD_ROL rol;
        public Index position;
        public BoogieWrestler wrestlerIn;
        public SquadSlot(SQUAD_ROL rol)
        {
            this.rol = rol;
        }
    }

    [System.Serializable]
    public class Index
    {
        public int i;
        public int j;
        public Index(int i, int j)
        {
            this.i = i;
            this.j = j;
        }
    }
    
}
