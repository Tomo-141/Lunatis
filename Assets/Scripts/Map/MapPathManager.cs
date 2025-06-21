// MapPathManager.cs
// このスクリプトは、マップ上の全てのノード（MapNodeコンポーネントを持つGameObject）を管理し、
// ノードIDをキーとして簡単にアクセスできるようにします。
// 主に経路探索やノード間の接続情報が必要な他のシステムから参照されます。
// Dijkstra法による最短経路探索機能も提供します。

using UnityEngine;
using System.Collections.Generic; // Dictionary<TKey, TValue>を使用するために必要
using System.Linq; // LINQを使用するために必要

public class MapPathManager : MonoBehaviour
{
    // 全てのノードをIDで検索できるように辞書で管理します。
    // Unityエディタで直接設定するのではなく、Awake時に自動で収集します。
    private Dictionary<string, MapNode> allNodes = new Dictionary<string, MapNode>();

    // シングルトンパターン（簡易版）：他のスクリプトから簡単にアクセスできるようにします。
    public static MapPathManager Instance { get; private set; }

    /// <summary>
    /// Awakeはスクリプトインスタンスがロードされたときに呼び出されます。
    /// シングルトンの設定と、シーン内の全てのMapNodeを収集します。
    /// </summary>
    void Awake()
    {
        // シングルトンインスタンスの初期化
        if (Instance == null)
        {
            Instance = this;
            // シーンをロードし直してもインスタンスが破棄されないようにする場合は、DontDestroyOnLoad(gameObject); を使用しますが、
            // 今回はマップ画面専用なので不要かもしれません。
        }
        else
        {
            Debug.LogWarning("[MapPathManager] 複数のMapPathManagerインスタンスが見つかりました。既存のものを保持し、このインスタンスは破棄します。", this);
            Destroy(gameObject);
            return;
        }

        // シーン内の全てのMapNodeコンポーネントを検索し、辞書に登録します。
        MapNode[] nodesInScene = FindObjectsByType<MapNode>(FindObjectsSortMode.None);
        foreach (MapNode node in nodesInScene)
        {
            if (allNodes.ContainsKey(node.NodeId))
            {
                // 同じIDのノードが複数存在する場合、エラーを出力します。
                Debug.LogError($"[MapPathManager] 重複するノードIDが見つかりました: '{node.NodeId}'。GameObject: '{node.name}' と '{allNodes[node.NodeId].name}'", node);
                continue;
            }
            allNodes.Add(node.NodeId, node);
            Debug.Log($"[MapPathManager] ノードを登録しました: ID='{node.NodeId}', 名前='{node.name}'");
        }

        // 各ノードの接続IDが実際に存在するノードIDであるか確認する（デバッグ用）
        CheckNodeConnections();
    }

    /// <summary>
    /// 指定されたノードIDに対応するMapNodeコンポーネントを取得します。
    /// </summary>
    /// <param name="nodeId">取得したいノードのID。</param>
    /// <returns>指定されたIDのMapNode、見つからない場合はnull。</returns>
    public MapNode GetNode(string nodeId)
    {
        MapNode node;
        if (allNodes.TryGetValue(nodeId, out node))
        {
            return node;
        }
        Debug.LogWarning($"[MapPathManager] ノードID '{nodeId}' が見つかりませんでした。", this);
        return null;
    }

    /// <summary>
    /// 全てのノードの接続設定が正しいか（接続先IDが存在するか）をチェックします。
    /// デバッグ目的で使用します。
    /// </summary>
    private void CheckNodeConnections()
    {
        foreach (var entry in allNodes)
        {
            MapNode currentNode = entry.Value;
            // MapNodeクラスの GetConnectedNodeIds() メソッドを使用
            foreach (string connectedId in currentNode.GetConnectedNodeIds()) // ここを修正
            {
                if (!allNodes.ContainsKey(connectedId))
                {
                    Debug.LogWarning($"[MapPathManager] ノード '{currentNode.NodeId}' (GameObject: {currentNode.gameObject.name}) の接続先ノードID '{connectedId}' が存在しません。接続を削除するか、ノードを作成してください。", currentNode);
                }
            }
        }
        Debug.Log("[MapPathManager] ノード接続の整合性チェックが完了しました。");
    }
}