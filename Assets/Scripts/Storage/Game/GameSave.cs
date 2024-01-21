using Newtonsoft.Json;

public partial class GameStorage
{
    [System.Serializable]
    public class GameSimpleSave
    {
        [JsonIgnore]
        public const string Key = "gms";

        public long A { get; }
        public long B { get; }

        [JsonProperty("L")]
        public int Level { get; }

        [JsonConstructor]
        public GameSimpleSave(long a, long b, int level)
        {
            A = a;
            B = b;
            Level = level;
        }

        public GameSimpleSave(Score score, int level) : this(score.A, score.B, level) { }

        public GameSimpleSave() { }

    }

    [System.Serializable]
    public class GameSave
    {
        [JsonIgnore]
        public const string Key = "gmf";

        [JsonProperty("tps")]
        public FigureType[] Types { get; }
        [JsonProperty("brs")]
        public BridgeType Bridges { get; }
        [JsonProperty("trn")]
        public Turn Turn { get; }
        [JsonProperty("fgs")]
        public int Figures { get; }
        [JsonProperty("bxs")]
        public int[] Boxes { get; }

        [JsonConstructor]
        public GameSave(FigureType[] types, BridgeType bridges, Turn turn, int figures, int[] boxes)
        {
            Types = types;
            Bridges = bridges;
            Turn = turn;
            Figures = figures;
            Boxes = boxes;
        }

        public GameSave(FigureType[] types, Level.LBridges bridges, int figures, int[] boxes) : this(types, bridges.Open, bridges.Turn, figures, boxes) { }

        public GameSave() { }
    }
}
