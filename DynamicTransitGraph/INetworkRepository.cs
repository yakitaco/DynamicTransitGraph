using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicTransitGraph
{
    public interface INetworkRepository
    {
        /// <summary>
        /// 指定されたファイルパスからネットワーク情報を読み込み、NetworkGraphを構築して返します。
        /// </summary>
        /// <param name="filePath">読み込むファイルのパス</param>
        /// <returns>構築されたNetworkGraph</returns>
        NetworkGraph LoadNetwork(string filePath);

        /// <summary>
        /// 指定されたNetworkGraphの情報を、ファイルに保存します。
        /// </summary>
        /// <param name="graph">保存するネットワークグラフ</param>
        /// <param name="filePath">保存先のファイルパス</param>
        void SaveNetwork(NetworkGraph graph, string filePath);
    }
}
