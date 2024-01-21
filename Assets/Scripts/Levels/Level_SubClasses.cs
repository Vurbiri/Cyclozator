using UnityEngine;

public partial class Level
{
    public partial class LBridges
    {
        public BridgeType Open { get; }
        public Turn Turn { get; }

        public LBridges(BridgeType open, Turn turn)
        {
            Open = open;
            Turn = turn;
        }

        public LBridges(int openBridgesCount, Turn turn)
        {
            
            Open = CalkBridgeOpen(openBridgesCount);
            Turn = turn;
        }
    }

    public partial class LSpawn
    {
        public FigureType[] Types => _parent._types;
        public int Count => _parent._figureCount;
        public float Time { get; }
        public float MultiTime => spawnMultiTime;
        public float Speed { get; }
        public bool IsCoin { get; }
        public float ChanceFromOne { get; }
        public float ChanceJumpSpawn { get; }
        public float ChanceSameFigure => chanceSameFigure;
        public (float two, float three) ChanceMultiSpawn { get; }
        public (float min, float max) UncertaintySpawn => uncertaintySpawn;

        private readonly Level _parent;


        public LSpawn(Level parent, float time, float speed, int openBridgesCount, float fromOneSpawn, float multiSpawn, float threeMultiSpawn)
        {
            _parent = parent;

            Time = time;
            Speed = speed;

            IsCoin = chanceCoin.NextIncrement(_parent.chanceCoinPerLevel);
            ChanceFromOne = openBridgesCount > 1 ? Mathf.Clamp(chanceFromOneBase + _parent.chanceFromOnePerLevel, 0f, chanceFromOneMax) * fromOneSpawn : 146f;
            ChanceJumpSpawn = Mathf.Clamp(openBridgesCount > 2 ? chanceJumpSpawnBase + _parent.chanceJumpSpawnPerLevel : 0f, 0f, chanceJumpSpawnMax);
            ChanceMultiSpawn = CalkChanceMultispawn(_parent, openBridgesCount, multiSpawn, threeMultiSpawn);
        }
    }

    public partial class LBoxes
    {
        public FigureType[] Types => _parent._types;
        public int FigureCount => _parent._figureCount;
        public int[] Boxes { get; private set; }
        public int Count { get; private set; } = 16;

        private readonly Level _parent;

        public LBoxes(Level parent)
        {
            _parent = parent;
            Boxes = new int[Count];
        }
    }
}
