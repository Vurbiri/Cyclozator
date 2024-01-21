using System;
using static GameStorage;

public partial class Level
{
    public void Modification(GameSave gameSave)
    {
        _types = gameSave.Types;
        _figureCount = gameSave.Figures;
        _sectorsOpen = ModifySectorsArray(gameSave.Turn);

        Bridges = new(gameSave.Bridges, gameSave.Turn);

        Boxes.Modification(gameSave.Boxes);
    }

    private bool[] ModifySectorsArray(Turn turnBridges)
    {
        int closeSubSectorsCount = 0;
        foreach (var sector in _sectorsOpen) 
            if(!sector) closeSubSectorsCount++;

        return CalkSectorsArray(closeSubSectorsCount / 4, turnBridges);
    }

    private bool[] CalkSectorsArray(int subSectorClose, Turn turnBridges)
    {
        int countSectors = 16;
        int countSubSectors = 3;
        bool[] sectors = new bool[countSectors];
        bool[] subSectors = new bool[countSubSectors];

        subSectorClose = subSectorClose > 3 ? 3 : subSectorClose;

        if (subSectorClose == 0)
        {
            Array.Fill(subSectors, true);
        }
        else if (subSectorClose == 1)
        {
            subSectors[0] = true; subSectors[2] = true;
        }
        else if (subSectorClose == 2)
        {
            subSectors[1] = true;
        }

        int index = turnBridges.ToInt();
        while (countSectors > 0)
        {
            sectors[index] = true;
            index = sectors.Right(index);
            countSectors--;
            for (int i = 0; i < countSubSectors; i++, countSectors--, index = sectors.Right(index))
                sectors[index] = subSectors[i];
        }

        return sectors;
    }


    public partial class LBridges
    {

        private BridgeType CalkBridgeOpen(int openCount)
        {
            BridgeType open = BridgeType.None;

            BridgeType[] types;

            if (openCount == 4)
            {
                types = Enum<BridgeType>.GetValues();
            }
            else
            {
                types = new BridgeType[openCount];
                types.RandomFillArray();
            }

            foreach (var t in types)
                open |= t;

            return open;
        }
    }

    public partial class LSpawn
    {
        private (float two, float three) CalkChanceMultispawn(Level parent, int openBridgesCount, float ratioMultiSpawn, float ratioThreeMultiSpawn)
        {
            float chanceTwo = Math.Clamp(openBridgesCount > 1 ? chanceMultiSpawnBase.two + parent.chanceMultiSpawnPerLevel.two : 0, 0f , chanceMultiSpawnMax.two) * ratioMultiSpawn;
            float chanceThree = Math.Clamp((openBridgesCount > 2 ? chanceMultiSpawnBase.three + parent.chanceMultiSpawnPerLevel.three : 0) * ratioThreeMultiSpawn, 0f, chanceMultiSpawnMax.three) * ratioMultiSpawn;

            return (chanceTwo, chanceThree);
        }

    }

    public partial class LBoxes
    {
        public void Modification(int[] boxes)
        {
            Boxes = boxes;

            int count = 0;

            foreach (var box in boxes) 
                if(box != 0)
                    count++;

            Count = count;
        }

    }

}
