namespace DynamicTransitGraph
{
    public class PathResult
    {
        /// <summary>
        /// スタートからゴールまでの合計コスト
        /// </summary>
        public double TotalCost { get; set; }

        /// <summary>
        /// 経由したノードのリスト（スタート地点からゴール地点の順）
        /// </summary>
        public IReadOnlyList<Node> RouteNodes { get; set; }

        /// <summary>
        /// 経由したエッジのリスト（通った路線のリスト）
        /// </summary>
        public IReadOnlyList<Edge> RouteEdges { get; set; }

        public PathResult(double totalCost, List<Node> routeNodes, List<Edge> routeEdges)
        {
            TotalCost = totalCost;
            RouteNodes = routeNodes;
            RouteEdges = routeEdges;
        }
    }
}
