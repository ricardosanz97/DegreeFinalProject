using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadConfiguration : MonoBehaviour
{
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

    public enum SQUAD_LEVEL
    {
        First,
        Second,
        Third,
        Fourth
    }

    public enum SQUAD_FORMATION
    {
        Contention,
        Penetration,
        AroundPlayer,
        CoverBody
    }

    [System.Serializable]
    public class Squad
    {
        public SQUAD_LEVEL squadLevel;
        public SQUAD_FORMATION squadFormation;
        public Index leaderPosition;
        public SquadSlot[,] squad;
        public int squadCols;
        public int squadRows;
        public bool hasPlayer;
        public bool hasBody;

        public Squad(SQUAD_LEVEL level, SQUAD_FORMATION formation)
        {
            this.squadLevel = level;
            this.squadFormation = formation;
            switch (this.squadFormation)
            {
                case SQUAD_FORMATION.Contention:
                    BuildFirstContentionSquad();
                    break;
                case SQUAD_FORMATION.Penetration:
                    BuildFirstPenetrationSquad();
                    break;
                case SQUAD_FORMATION.AroundPlayer:
                    BuildFirstAroundPlayerSquad();
                    break;
                case SQUAD_FORMATION.CoverBody:
                    BuildFirstCoverBodySquad();
                    break;
            }
        }

        private void BuildFirstContentionSquad()
        {
            squadCols = 3;
            squadRows = 3;
            squad = new SquadSlot[squadRows, squadCols];
            squad[0, 0] = new SquadSlot(SQUAD_ROL.Distance);
            squad[0, 1] = new SquadSlot(SQUAD_ROL.Distance);
            squad[0, 2] = new SquadSlot(SQUAD_ROL.Distance);
            squad[1, 0] = new SquadSlot(SQUAD_ROL.Close);
            squad[1, 1] = new SquadSlot(SQUAD_ROL.Commander);
            leaderPosition = new Index(1, 1);
            squad[1, 2] = new SquadSlot(SQUAD_ROL.Close);
            squad[2, 0] = new SquadSlot(SQUAD_ROL.Giant);
            squad[2, 1] = new SquadSlot(SQUAD_ROL.Close);
            squad[2, 2] = new SquadSlot(SQUAD_ROL.Giant);
            for (int i = 0; i<squadRows; i++)
            {
                for (int j = 0; j<squadCols; j++)
                {
                    squad[i, j].position = new Index(i, j);
                }
            }
        }

        private void BuildFirstPenetrationSquad()
        {
            squadCols = 3;
            squadRows = 3;
            squad = new SquadSlot[squadRows, squadCols];
            squad[0, 0] = new SquadSlot(SQUAD_ROL.Giant);
            squad[0, 1] = new SquadSlot(SQUAD_ROL.Distance);
            squad[0, 2] = new SquadSlot(SQUAD_ROL.Giant);
            squad[1, 0] = new SquadSlot(SQUAD_ROL.Distance);
            squad[1, 1] = new SquadSlot(SQUAD_ROL.Commander);
            leaderPosition = new Index(1, 1);
            squad[1, 2] = new SquadSlot(SQUAD_ROL.Distance);
            squad[2, 0] = new SquadSlot(SQUAD_ROL.Close);
            squad[2, 1] = new SquadSlot(SQUAD_ROL.Close);
            squad[2, 2] = new SquadSlot(SQUAD_ROL.Close);
            for (int i = 0; i < squadRows; i++)
            {
                for (int j = 0; j < squadCols; j++)
                {
                    squad[i, j].position = new Index(i, j);
                }
            }
        }

        private void BuildFirstAroundPlayerSquad()
        {
            squadCols = 3;
            squadRows = 4;
            squad = new SquadSlot[squadRows, squadCols];
            squad[0, 0] = new SquadSlot(SQUAD_ROL.None);
            squad[0, 1] = new SquadSlot(SQUAD_ROL.Close);
            squad[0, 2] = new SquadSlot(SQUAD_ROL.None);
            squad[1, 0] = new SquadSlot(SQUAD_ROL.Distance);
            squad[1, 1] = new SquadSlot(SQUAD_ROL.Distance);
            squad[1, 2] = new SquadSlot(SQUAD_ROL.Distance);
            squad[2, 0] = new SquadSlot(SQUAD_ROL.Close);
            squad[2, 1] = new SquadSlot(SQUAD_ROL.Player);
            hasPlayer = true;
            leaderPosition = new Index(2, 1);
            squad[2, 2] = new SquadSlot(SQUAD_ROL.Close);
            squad[3, 0] = new SquadSlot(SQUAD_ROL.Giant);
            squad[3, 1] = new SquadSlot(SQUAD_ROL.Commander);
            squad[3, 2] = new SquadSlot(SQUAD_ROL.Giant);
            
            
            for (int i = 0; i < squadRows; i++)
            {
                for (int j = 0; j < squadCols; j++)
                {
                    squad[i, j].position = new Index(i, j);
                }
            }
        }

        private void BuildFirstCoverBodySquad()
        {
            squadCols = 3;
            squadRows = 4;
            squad = new SquadSlot[squadRows, squadCols];
            squad[0, 0] = new SquadSlot(SQUAD_ROL.None);
            squad[0, 1] = new SquadSlot(SQUAD_ROL.Close);
            squad[0, 2] = new SquadSlot(SQUAD_ROL.None);
            squad[1, 0] = new SquadSlot(SQUAD_ROL.Distance);
            squad[1, 1] = new SquadSlot(SQUAD_ROL.Distance);
            squad[1, 2] = new SquadSlot(SQUAD_ROL.Distance);
            squad[2, 0] = new SquadSlot(SQUAD_ROL.Close);
            squad[2, 1] = new SquadSlot(SQUAD_ROL.Body);
            hasBody = true;
            leaderPosition = new Index(2, 1);
            squad[2, 2] = new SquadSlot(SQUAD_ROL.Close);
            squad[3, 0] = new SquadSlot(SQUAD_ROL.Giant);
            squad[3, 1] = new SquadSlot(SQUAD_ROL.Commander);
            squad[3, 2] = new SquadSlot(SQUAD_ROL.Giant);


            for (int i = 0; i < squadRows; i++)
            {
                for (int j = 0; j < squadCols; j++)
                {
                    squad[i, j].position = new Index(i, j);
                }
            }
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
