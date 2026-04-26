namespace DynamicTransitGraph
{
    public class Edge
    {
        /// <summary>
        /// 出発元ノードのID
        /// </summary>
        public string SourceId { get; set; }

        /// <summary>
        /// 到着先ノードのID
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// 移動にかかるコスト
        /// </summary>
        public double Cost { get; set; }

        // JSONデシリアライズ用の空コンストラクタ
        public Edge() { }

        public Edge(string sourceId, string targetId, double cost)
        {
            SourceId = sourceId;
            TargetId = targetId;
            Cost = cost;
        }
    }
}
