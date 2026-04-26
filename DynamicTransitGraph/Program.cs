using DynamicTransitGraph;

class Program
{
    static void Main(string[] args)
    {
        // 保存・読み込みを行うJSONファイルのパス
        string filePath = "network.json";

        // インフラ層のリポジトリをインスタンス化
        INetworkRepository repository = new JsonNetworkRepository();

        // 1. テスト用のサンプルデータを作成し、JSONファイルとして保存する
        // （※すでにファイルがある場合は上書きされます。手動でJSONを作った場合はこの行をコメントアウトしてください）
        CreateSampleNetworkFile(repository, filePath);

        // 2. 外部ファイル(JSON)からネットワーク情報を読み込む
        Console.WriteLine($"[{filePath}] からネットワーク情報を読み込んでいます...");
        NetworkGraph graph = repository.LoadNetwork(filePath);
        Console.WriteLine($"読み込み完了: 登録ノード数 = {graph.Nodes.Count} 駅\n");

        // 3. 最短経路の計算を準備
        var calculator = new ShortestPathCalculator();

        // 探索するスタート地点とゴール地点のIDを指定
        string startNodeId = "N1"; // 東京
        string endNodeId = "N4";   // 渋谷

        Console.WriteLine($"経路探索を開始します: {graph.Nodes[startNodeId].Name} ⇒ {graph.Nodes[endNodeId].Name}");

        // 4. 計算の実行
        PathResult result = calculator.Calculate(graph, startNodeId, endNodeId);

        // 5. 結果の表示
        PrintResult(result);

        Console.WriteLine("\nEnterキーを押して終了します...");
        Console.ReadLine();
    }

    /// <summary>
    /// 動作確認用のサンプルネットワークを生成し、ファイルに保存します。
    /// </summary>
    static void CreateSampleNetworkFile(INetworkRepository repository, string filePath)
    {
        var graph = new NetworkGraph();

        // ノード（駅）の追加
        graph.AddNode(new Node("N1", "東京"));
        graph.AddNode(new Node("N2", "品川"));
        graph.AddNode(new Node("N3", "新宿"));
        graph.AddNode(new Node("N4", "渋谷"));
        graph.AddNode(new Node("N5", "池袋"));

        // エッジ（路線とコスト）の追加
        // ※isBidirectional = true にして、往復どちらも同じコストで移動できるようにします
        graph.AddEdge(new Edge("N1", "N2", 170), isBidirectional: true); // 東京 - 品川
        graph.AddEdge(new Edge("N1", "N3", 200), isBidirectional: true); // 東京 - 新宿 (中央線経由など)
        graph.AddEdge(new Edge("N2", "N4", 170), isBidirectional: true); // 品川 - 渋谷
        graph.AddEdge(new Edge("N3", "N4", 160), isBidirectional: true); // 新宿 - 渋谷
        graph.AddEdge(new Edge("N3", "N5", 160), isBidirectional: true); // 新宿 - 池袋
        graph.AddEdge(new Edge("N4", "N5", 170), isBidirectional: true); // 渋谷 - 池袋

        // JSONファイルとして保存
        repository.SaveNetwork(graph, filePath);
    }

    /// <summary>
    /// 計算結果をコンソールに分かりやすく出力します。
    /// </summary>
    static void PrintResult(PathResult result)
    {
        if (result == null)
        {
            Console.WriteLine("エラー: 指定された条件で到達可能な経路が見つかりませんでした。");
            return;
        }

        Console.WriteLine("\n==============================");
        Console.WriteLine(" 最短経路が見つかりました！");
        Console.WriteLine("==============================");
        Console.WriteLine($"総コスト (運賃等): {result.TotalCost}");
        Console.WriteLine("------------------------------");
        Console.WriteLine("【 ルート案内 】");

        // ノードとエッジを順番に表示していく
        for (int i = 0; i < result.RouteNodes.Count; i++)
        {
            // 駅名を表示
            Console.Write($"[{result.RouteNodes[i].Name}]");

            // 次の駅がある場合は、その間に通るエッジのコストを表示
            if (i < result.RouteEdges.Count)
            {
                Console.Write($" ==( {result.RouteEdges[i].Cost} )==> ");
            }
        }
        Console.WriteLine("\n==============================");
    }
}