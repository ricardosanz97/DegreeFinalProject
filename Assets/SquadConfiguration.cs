using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadConfiguration : MonoBehaviour
{
    public enum SQUAD_ROL
    {
        Commander,
        Behind,
        Front,
        Lateral,
        CornerFront,
        CornerBehind
    }

    public enum SQUAD_LEVEL
    {
        First,
        Second,
        Third,
        Fourth
    }

    [System.Serializable]
    public class Squad
    {
        public SQUAD_LEVEL squadLevel;
        public SquadSlot[,] squad;

        public Squad(SQUAD_LEVEL level)
        {
            this.squadLevel = level;
            switch (this.squadLevel)
            {
                case SQUAD_LEVEL.First:
                    BuildFirstSquad();
                    break;
                case SQUAD_LEVEL.Second:
                    BuildSecondSquad();
                    break;
                case SQUAD_LEVEL.Third:
                    BuildThirdSquad();
                    break;
                case SQUAD_LEVEL.Fourth:
                    break;
            }
        }

        private void BuildFirstSquad()
        {
            squad = new SquadSlot[3, 3];
            squad[0, 0] = new SquadSlot(SQUAD_ROL.CornerBehind);
            squad[0, 1] = new SquadSlot(SQUAD_ROL.Behind);
            squad[0, 2] = new SquadSlot(SQUAD_ROL.CornerBehind);
            squad[1, 0] = new SquadSlot(SQUAD_ROL.Lateral);
            squad[1, 1] = new SquadSlot(SQUAD_ROL.Commander);
            squad[1, 2] = new SquadSlot(SQUAD_ROL.Lateral);
            squad[2, 0] = new SquadSlot(SQUAD_ROL.CornerFront);
            squad[2, 1] = new SquadSlot(SQUAD_ROL.Front);
            squad[2, 2] = new SquadSlot(SQUAD_ROL.CornerFront);
            for (int i = 0; i<3; i++)
            {
                for (int j = 0; j<3; j++)
                {
                    squad[i, j].position = new Index(i, j);
                }
            }
        }

        private void BuildSecondSquad()
        {
            squad = new SquadSlot[5, 5];
            squad[0, 0].rol = SQUAD_ROL.CornerBehind;
            squad[0, 1].rol = SQUAD_ROL.Behind;
            squad[0, 2].rol = SQUAD_ROL.Behind;
            squad[0, 3].rol = SQUAD_ROL.Behind;
            squad[0, 4].rol = SQUAD_ROL.CornerBehind;
            squad[1, 0].rol = SQUAD_ROL.Lateral;
            squad[1, 1].rol = SQUAD_ROL.Behind;
            squad[1, 2].rol = SQUAD_ROL.Behind;
            squad[1, 3].rol = SQUAD_ROL.Behind;
            squad[1, 4].rol = SQUAD_ROL.Lateral;
            squad[2, 0].rol = SQUAD_ROL.Lateral;
            squad[2, 1].rol = SQUAD_ROL.Lateral;
            squad[2, 2].rol = SQUAD_ROL.Commander;
            squad[2, 3].rol = SQUAD_ROL.Lateral;
            squad[2, 4].rol = SQUAD_ROL.Lateral;
            squad[3, 0].rol = SQUAD_ROL.Lateral;
            squad[3, 1].rol = SQUAD_ROL.Front;
            squad[3, 2].rol = SQUAD_ROL.Front;
            squad[3, 3].rol = SQUAD_ROL.Front;
            squad[3, 4].rol = SQUAD_ROL.Lateral;
            squad[4, 0].rol = SQUAD_ROL.CornerFront;
            squad[4, 1].rol = SQUAD_ROL.Front;
            squad[4, 2].rol = SQUAD_ROL.Front;
            squad[4, 3].rol = SQUAD_ROL.Front;
            squad[4, 4].rol = SQUAD_ROL.CornerFront;
        }

        private void BuildThirdSquad()
        {
            squad = new SquadSlot[7, 7];
            for (int i = 0; i<7; i++)
            {
                for (int j = 0; j<7; j++)
                {
                    squad[i, j].position = new Index(i, j);
                    squad[i, j].wrestlerIn = null;
                }
            }
            squad[0, 0].rol = SQUAD_ROL.CornerBehind;
            squad[0, 1].rol = SQUAD_ROL.Behind;
            squad[0, 2].rol = SQUAD_ROL.Behind;
            squad[0, 3].rol = SQUAD_ROL.Behind;
            squad[0, 4].rol = SQUAD_ROL.Behind;
            squad[0, 5].rol = SQUAD_ROL.Behind;
            squad[0, 6].rol = SQUAD_ROL.CornerBehind;
            squad[1, 0].rol = SQUAD_ROL.Lateral;
            squad[1, 1].rol = SQUAD_ROL.Behind;
            squad[1, 2].rol = SQUAD_ROL.Behind;
            squad[1, 3].rol = SQUAD_ROL.Behind;
            squad[1, 4].rol = SQUAD_ROL.Behind;
            squad[1, 5].rol = SQUAD_ROL.Behind;
            squad[1, 6].rol = SQUAD_ROL.Lateral;
            squad[2, 0].rol = SQUAD_ROL.Lateral;
            squad[2, 1].rol = SQUAD_ROL.Behind;
            squad[2, 2].rol = SQUAD_ROL.Behind;
            squad[2, 3].rol = SQUAD_ROL.Behind;
            squad[2, 4].rol = SQUAD_ROL.Behind;
            squad[2, 5].rol = SQUAD_ROL.Behind;
            squad[2, 6].rol = SQUAD_ROL.Lateral;
            squad[3, 0].rol = SQUAD_ROL.Lateral;
            squad[3, 1].rol = SQUAD_ROL.Lateral;
            squad[3, 2].rol = SQUAD_ROL.Lateral;
            squad[3, 3].rol = SQUAD_ROL.Commander;
            squad[3, 4].rol = SQUAD_ROL.Lateral;
            squad[3, 5].rol = SQUAD_ROL.Lateral;
            squad[3, 6].rol = SQUAD_ROL.Lateral;
            squad[4, 0].rol = SQUAD_ROL.Lateral;
            squad[4, 1].rol = SQUAD_ROL.Front;
            squad[4, 2].rol = SQUAD_ROL.Front;
            squad[4, 3].rol = SQUAD_ROL.Front;
            squad[4, 4].rol = SQUAD_ROL.Front;
            squad[4, 5].rol = SQUAD_ROL.Front;
            squad[4, 6].rol = SQUAD_ROL.Lateral;
            squad[5, 0].rol = SQUAD_ROL.Lateral;
            squad[5, 1].rol = SQUAD_ROL.Front;
            squad[5, 2].rol = SQUAD_ROL.Front;
            squad[5, 3].rol = SQUAD_ROL.Front;
            squad[5, 4].rol = SQUAD_ROL.Front;
            squad[5, 5].rol = SQUAD_ROL.Front;
            squad[5, 6].rol = SQUAD_ROL.Lateral;
            squad[6, 0].rol = SQUAD_ROL.CornerFront;
            squad[6, 1].rol = SQUAD_ROL.Front;
            squad[6, 2].rol = SQUAD_ROL.Front;
            squad[6, 3].rol = SQUAD_ROL.Front;
            squad[6, 4].rol = SQUAD_ROL.Front;
            squad[6, 5].rol = SQUAD_ROL.Front;
            squad[6, 6].rol = SQUAD_ROL.CornerFront;
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
