namespace DynamicTransitGraph
{
    public class Node
    {
        /// <summary>
        /// ノードの一意な識別子 (例: "N1", "Tokyo")
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// ノードの表示名 (例: "東京駅")
        /// </summary>
        public string Name { get; set; }

        // JSONデシリアライズ用の空コンストラクタ
        public Node() { }

        public Node(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
