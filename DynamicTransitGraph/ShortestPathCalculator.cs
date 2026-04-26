namespace DynamicTransitGraph
{
    public class ShortestPathCalculator
    {
        /// <summary>
        /// ダイクストラ法を用いて最短経路を計算
        /// </summary>
        /// <param name="graph">ネットワークグラフ</param>
        /// <param name="startNodeId">出発ノードのID</param>
        /// <param name="endNodeId">目的ノードのID</param>
        /// <returns>最短経路の計算結果。経路が存在しない場合は null を返却</returns>
        public PathResult Calculate(NetworkGraph graph, string startNodeId, string endNodeId)
        {
            // 入力チェック
            if (!graph.Nodes.ContainsKey(startNodeId) || !graph.Nodes.ContainsKey(endNodeId))
            {
                throw new ArgumentException("スタートまたはゴールのノードがグラフに存在しません。");
            }

            // 1. 各ノードへの最小コストを記録する辞書（初期値は無限大）
            var minCosts = new Dictionary<string, double>();

            // 2. どのエッジを通ってそのノードに辿り着いたかを記録する辞書（ルート復元用）
            var previousEdges = new Dictionary<string, Edge>();

            // 3. 探索候補を管理する優先度付きキュー（Key: ノードID, Priority: そこまでの累積コスト）
            // ※ 累積コストが低いものから順番に取り出されます
            var priorityQueue = new PriorityQueue<string, double>();

            // 初期化: スタート地点のコストを0にしてキューに入れる
            minCosts[startNodeId] = 0;
            priorityQueue.Enqueue(startNodeId, 0);

            // キューが空になるまで探索を続ける
            while (priorityQueue.Count > 0)
            {
                // 現在の探索候補の中で、最も累積コストが低いノードを取り出す
                // （C# の PriorityQueue の Dequeue は、値と優先度を同時に取り出せます）
                priorityQueue.TryDequeue(out string currentNodeId, out double currentCost);

                // ゴールに到達した時点で、それが最短経路であることが保証されるため探索終了
                if (currentNodeId == endNodeId)
                {
                    break;
                }

                // 【最適化】キューに積まれていた古い（より高い）コストの情報は無視する
                if (currentCost > minCosts.GetValueOrDefault(currentNodeId, double.PositiveInfinity))
                {
                    continue;
                }

                // 隣接するノードを調べる
                foreach (var edge in graph.GetAdjacentEdges(currentNodeId))
                {
                    string neighborId = edge.TargetId;
                    double newCost = currentCost + edge.Cost;

                    // 記録されている最小コストよりも安く到達できるルートを見つけた場合
                    if (newCost < minCosts.GetValueOrDefault(neighborId, double.PositiveInfinity))
                    {
                        minCosts[neighborId] = newCost;       // 最小コストを更新
                        previousEdges[neighborId] = edge;     // どこから来たかを記録
                        priorityQueue.Enqueue(neighborId, newCost); // 新たな探索候補としてキューに追加
                    }
                }
            }

            // ゴールに到達できなかった場合（経路が存在しない場合）
            if (!minCosts.ContainsKey(endNodeId))
            {
                return null;
            }

            // 4. 結果の復元（ゴールからスタートへ逆順に辿る）
            return BuildPathResult(graph, startNodeId, endNodeId, minCosts[endNodeId], previousEdges);
        }

        /// <summary>
        /// previousEdges の情報をもとに、スタートからゴールまでのルートを構築します。
        /// </summary>
        private PathResult BuildPathResult(
            NetworkGraph graph,
            string startNodeId,
            string endNodeId,
            double totalCost,
            Dictionary<string, Edge> previousEdges)
        {
            var routeNodes = new List<Node>();
            var routeEdges = new List<Edge>();

            string currentNodeId = endNodeId;

            // ゴールからスタートに向かって逆戻りする
            while (currentNodeId != startNodeId)
            {
                routeNodes.Add(graph.Nodes[currentNodeId]);

                var edge = previousEdges[currentNodeId];
                routeEdges.Add(edge);

                currentNodeId = edge.SourceId;
            }

            // 最後にスタート地点のノード追加
            routeNodes.Add(graph.Nodes[startNodeId]);

            // リスト反転
            routeNodes.Reverse();
            routeEdges.Reverse();

            return new PathResult(totalCost, routeNodes, routeEdges);
        }
    }
}
