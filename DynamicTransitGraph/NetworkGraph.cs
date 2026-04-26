namespace DynamicTransitGraph
{
    public class NetworkGraph
    {
        /// <summary>
        /// 全ノードを管理する辞書 (Key: NodeId, Value: Node)
        /// </summary>
        public Dictionary<string, Node> Nodes { get; private set; }

        /// <summary>
        /// 隣接リスト (Key: NodeId, Value: そのノードから出発するエッジのリスト)
        /// 探索を高速化するための内部データ構造
        /// </summary>
        private Dictionary<string, List<Edge>> _adjacencyList;

        public NetworkGraph()
        {
            Nodes = new Dictionary<string, Node>();
            _adjacencyList = new Dictionary<string, List<Edge>>();
        }

        /// <summary>
        /// グラフにノードを追加します。
        /// </summary>
        public void AddNode(Node node)
        {
            if (node == null || string.IsNullOrEmpty(node.Id))
                throw new ArgumentException("無効なノードです。");

            if (!Nodes.ContainsKey(node.Id))
            {
                Nodes[node.Id] = node;
                // ノード追加時に、そのノード用の空の隣接リストも準備する
                _adjacencyList[node.Id] = new List<Edge>();
            }
        }

        /// <summary>
        /// グラフにエッジを追加します。
        /// 電車の路線のように双方向の移動が可能でコストが同じ場合は、isBidirectionalをtrueにします。
        /// </summary>
        public void AddEdge(Edge edge, bool isBidirectional = false)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            // エッジを追加する前に、両端のノードが存在するかチェック（安全対策）
            if (!Nodes.ContainsKey(edge.SourceId) || !Nodes.ContainsKey(edge.TargetId))
            {
                throw new ArgumentException($"エッジの始点({edge.SourceId})または終点({edge.TargetId})のノードが存在しません。先にAddNodeしてください。");
            }

            // 順方向のエッジを追加
            _adjacencyList[edge.SourceId].Add(edge);

            // 双方向指定がある場合、逆向きのエッジを自動生成して追加
            if (isBidirectional)
            {
                var reverseEdge = new Edge(edge.TargetId, edge.SourceId, edge.Cost);
                _adjacencyList[edge.TargetId].Add(reverseEdge);
            }
        }

        /// <summary>
        /// 指定したノードから直接移動できるエッジ（経路）のリストを取得します。
        /// ダイクストラ法で最も頻繁に呼び出されるメソッドです。
        /// </summary>
        public IReadOnlyList<Edge> GetAdjacentEdges(string nodeId)
        {
            if (_adjacencyList.TryGetValue(nodeId, out var edges))
            {
                return edges;
            }

            // ノードが存在しない、またはどこにも繋がっていない場合は空のリストを返す
            return new List<Edge>();
        }
    }
}
