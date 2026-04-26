using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynamicTransitGraph
{
    public class JsonNetworkRepository : INetworkRepository
    {
        // JSONのシリアライズ・デシリアライズ設定
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonNetworkRepository()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                // 保存時にインデント（改行と字下げ）を有効にして、人間が読みやすくする
                WriteIndented = true,
                // 日本語などの文字化けを防ぐ（Unicodeエスケープをしない）
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        public NetworkGraph LoadNetwork(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"指定されたファイルが見つかりません: {filePath}");
            }

            string jsonString = File.ReadAllText(filePath);

            // JSONを内部専用のDTOクラスにデシリアライズ
            var dto = JsonSerializer.Deserialize<NetworkDataDto>(jsonString, _jsonOptions);

            if (dto == null)
            {
                throw new InvalidOperationException("JSONファイルの読み込みに失敗しました。形式が正しくありません。");
            }

            var graph = new NetworkGraph();

            // 1. ノードの追加
            if (dto.Nodes != null)
            {
                foreach (var nodeDto in dto.Nodes)
                {
                    graph.AddNode(new Node(nodeDto.Id, nodeDto.Name));
                }
            }

            // 2. エッジの追加
            if (dto.Edges != null)
            {
                foreach (var edgeDto in dto.Edges)
                {
                    var edge = new Edge(edgeDto.Source, edgeDto.Target, edgeDto.Cost);
                    // JSONで定義された通りの片道として追加する（双方向にしたければ true にする）
                    graph.AddEdge(edge, isBidirectional: false);
                }
            }

            return graph;
        }

        public void SaveNetwork(NetworkGraph graph, string filePath)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));

            // NetworkGraph から保存用のDTOを構築
            var dto = new NetworkDataDto
            {
                Nodes = new List<NodeDto>(),
                Edges = new List<EdgeDto>()
            };

            // ノードの変換
            foreach (var node in graph.Nodes.Values)
            {
                dto.Nodes.Add(new NodeDto { Id = node.Id, Name = node.Name });
            }

            // エッジの変換（隣接リストからすべてのエッジを抽出）
            // ※双方向エッジとして登録されている場合、重複して保存されないような工夫が本来は必要ですが、
            // 今回はシンプルに保持しているエッジをそのまま出力します。
            foreach (var node in graph.Nodes.Values)
            {
                foreach (var edge in graph.GetAdjacentEdges(node.Id))
                {
                    dto.Edges.Add(new EdgeDto
                    {
                        Source = edge.SourceId,
                        Target = edge.TargetId,
                        Cost = edge.Cost
                    });
                }
            }

            // DTOをJSON文字列に変換してファイルへ書き込み
            string jsonString = JsonSerializer.Serialize(dto, _jsonOptions);
            File.WriteAllText(filePath, jsonString);
        }

        // =========================================================
        // 内部専用のDTO (Data Transfer Object) クラス群
        // JSONのキー名と、C#のプロパティ名のズレを吸収するためのクラス
        // =========================================================

        private class NetworkDataDto
        {
            [JsonPropertyName("nodes")]
            public List<NodeDto> Nodes { get; set; }

            [JsonPropertyName("edges")]
            public List<EdgeDto> Edges { get; set; }
        }

        private class NodeDto
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        private class EdgeDto
        {
            [JsonPropertyName("source")]
            public string Source { get; set; }

            [JsonPropertyName("target")]
            public string Target { get; set; }

            [JsonPropertyName("cost")]
            public double Cost { get; set; }
        }
    }
}
