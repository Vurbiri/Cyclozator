using System;

public partial class Level
{
    public int NumberLevel { get; }

    public LBridges Bridges { get; private set; }
    public LSpawn Spawn { get; }
    public LBoxes Boxes { get; }

    public bool[] Sectors => _sectorsOpen;
    private bool[] _sectorsOpen;

    private FigureType[] _types;
    private int _figureCount;

    private static float spawnMultiTime;
    private static readonly float chanceSameFigure;
    private static readonly (float min, float max) uncertaintySpawn;

    private static readonly Chance chanceCoin;

    private static readonly float spawnMultiTimeBase;
    private static readonly float chanceFromOneBase;
    private static readonly float chanceJumpSpawnBase;
    private static readonly (float two, float three) chanceMultiSpawnBase;
    private static readonly int figureCountBase;

    private float ChanceCoinPerFigure => (float)figureCountBase / (float)_figureCount;
    private readonly float chanceCoinPerLevel;
    private static readonly float chanceCoinPerLevelMax;
    private readonly float chanceFromOnePerLevel;
    private readonly float chanceJumpSpawnPerLevel;
    private readonly (float two, float three) chanceMultiSpawnPerLevel;
    private readonly int figureCountPerLevel;
   
    private static readonly float chanceFromOneMax;
    private static readonly float chanceJumpSpawnMax;
    private static readonly (float two, float three) chanceMultiSpawnMax;
    private static readonly int figureCountMax;

    static Level()
    {
        chanceCoin = new(17.5f);

        chanceFromOneBase = 9.9f;
        chanceJumpSpawnBase = 11.11f;
        chanceMultiSpawnBase = (12.21f, 5.5f);
        figureCountBase = 10;

        spawnMultiTimeBase = 0.7f;

        chanceCoinPerLevelMax = 15f;
        chanceFromOneMax = 21.12f;
        chanceJumpSpawnMax = 25.52f;
        chanceMultiSpawnMax = (70.07f, 34.43f);
        figureCountMax = 75;

        chanceSameFigure = 8.8f;
        uncertaintySpawn = (0.975f, 1.025f);

    }

    private Level(int numberLevel, int openBridgesCount, BridgeType openBridges, int closeSubSectorsCount, Turn turnBridges, int figureTypeCount, float figureSpeed, float spawnTime, Buff[] buffs)
    {
        NumberLevel = numberLevel;

        chanceFromOnePerLevel = NumberLevel / 8f;
        chanceJumpSpawnPerLevel = NumberLevel / 7f;
        chanceMultiSpawnPerLevel = (NumberLevel / 4f, NumberLevel / 8f);
        chanceCoinPerLevel = NumberLevel / 20f;
        chanceCoinPerLevel = chanceCoinPerLevel > chanceCoinPerLevelMax ? chanceCoinPerLevelMax : chanceCoinPerLevel;

        float level = NumberLevel;
        float limit = 45;
        figureCountPerLevel = 0;
        if (level > limit)
        {
            figureCountPerLevel += (int)((level - limit) / 3f);
            level = limit;
        }
        figureCountPerLevel += (int)(level / 1.5f);

        float ratioFromOneSpawn = 1f;
        float ratioMultiSpawn = 1f;
        float ratioThreeMultiSpawn = 1f;

        if (openBridgesCount == 0) openBridgesCount = openBridges.Count();

        figureTypeCount = buffs.Buffing(ItemType.Count, figureTypeCount);
        closeSubSectorsCount = buffs.Buffing(ItemType.Sector, closeSubSectorsCount);
        openBridgesCount = buffs.Buffing(ItemType.Bridge, openBridgesCount);

        figureSpeed = buffs.Buffing(ItemType.Speed, figureSpeed);
        spawnTime = buffs.Buffing(ItemType.Spawn, spawnTime);
        spawnMultiTime = buffs.Buffing(ItemType.Spawn, spawnMultiTimeBase);
        ratioFromOneSpawn = buffs.Buffing(ItemType.SpawnOne, ratioFromOneSpawn);
        ratioMultiSpawn = buffs.Buffing(ItemType.MultiSpawn, ratioMultiSpawn);
        ratioThreeMultiSpawn = buffs.Buffing(ItemType.MultiSpawn, ratioThreeMultiSpawn);


        _types = new FigureType[figureTypeCount];
        _types.RandomFillArray();

        _figureCount = Math.Clamp(figureCountBase + figureCountPerLevel, 0, figureCountMax);
        _sectorsOpen = CalkSectorsArray(closeSubSectorsCount, turnBridges);

        Boxes = new LBoxes(this);
        Spawn = new(this, spawnTime, figureSpeed, openBridgesCount, ratioFromOneSpawn, ratioMultiSpawn, ratioThreeMultiSpawn);

        if (openBridges == BridgeType.None || openBridgesCount < openBridges.Count())
            Bridges = new(openBridgesCount, turnBridges);
        else
            Bridges = new(openBridges, turnBridges);
    }

    public Level(int numberLevel, BridgeType openBridges, int closeSubSectorsCount, Turn turnBridges, int figureTypeCount, float figureSpeed, float spawnTime, Buff[] buffs) :
        this(numberLevel, 0, openBridges, closeSubSectorsCount, turnBridges, figureTypeCount, figureSpeed, spawnTime, buffs) { }

    public Level(int numberLevel, int openBridgesCount, int closeSubSectorsCount, Turn turnBridges, int figureTypeCount, float figureSpeed, float spawnTime, Buff[] buffs) :
        this(numberLevel, openBridgesCount, BridgeType.None, closeSubSectorsCount, turnBridges, figureTypeCount, figureSpeed, spawnTime, buffs) { }

}



