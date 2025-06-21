// MapNode.cs 
// このスクリプトは、マップ上の各ノード（地点）の情報を保持します。
// ノードID、ノード名、そして接続されている他のノードのIDリストを含みます。
// また、UIクリックイベントを検出する機能も持ちます。

using UnityEngine;
using System.Collections.Generic; // List<T>を使用するために必要
using UnityEngine.EventSystems;   // IPointerClickHandler を使用するために必要
using System;                     // Action を使用するために必要
using System.Linq; // ConnectedNodesからNodeIdのリストを生成するために必要

public class MapNode : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("このノードの一意の識別子（例: TownA, Dungeon1など）。HierarchyのGameObject名と合わせることを推奨します。")]
    public string NodeId; // NodeIdは引き続き手動入力または自動生成（未実装）の文字列として保持

    [Tooltip("このノードの表示名（例: 「はじまりの街」など）")]
    public string NodeName;

    // ConnectedNodeIds を MapNode 型のリストに変更します
    [Tooltip("このノードから直接接続されている他のノードの参照リスト。HierarchyからMapNodeコンポーネントを持つGameObjectをドラッグ＆ドロップしてください。")]
    public List<MapNode> ConnectedNodes; // MapNodeの参照リストに変更

    // ノードがクリックされたときに呼び出される静的イベント
    // 引数にはクリックされたノードのIDが含まれます
    public static event Action<string> OnNodeClicked;

    /// <summary>
    /// Unityエディタでのみ呼び出され、コンポーネントがロードされたときに実行されます。
    /// デバッグ用に、ノードIDが未設定の場合に警告を出します。
    /// </summary>
    void OnValidate()
    {
        if (string.IsNullOrEmpty(NodeId))
        {
            Debug.LogWarning($"[MapNode] GameObject '{gameObject.name}' のNodeIdが設定されていません。一意のIDを設定してください。", this);
        }

        // ConnectedNodesリストがnullでないか確認し、要素がある場合は警告
        if (ConnectedNodes != null)
        {
            foreach (var node in ConnectedNodes)
            {
                if (node == null)
                {
                    Debug.LogWarning($"[MapNode] ノード '{NodeId}' のConnectedNodesリストにnullの要素があります。参照が切れているか、正しく設定されていません。", this);
                }
                else if (string.IsNullOrEmpty(node.NodeId))
                {
                    Debug.LogWarning($"[MapNode] ノード '{NodeId}' のConnectedNodesリストに含まれるノード '{node.name}' のNodeIdが設定されていません。", this);
                }
            }
        }
    }

    /// <summary>
    /// このノードがクリックされたときに呼び出されます。
    /// </summary>
    /// <param name="eventData">ポインターイベントデータ。</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 静的イベントを呼び出し、クリックされたノードのIDを渡します。
        OnNodeClicked?.Invoke(NodeId);
        Debug.Log($"[MapNode] ノード '{NodeName}' (ID: {NodeId}) がクリックされました。イベントを発火しました。", this);
    }

    /// <summary>
    /// ConnectedNodesリストから、接続されているノードのIDリストを取得します。
    /// このメソッドはMapInteractionHandlerなどが接続判定に使用します。
    /// </summary>
    /// <returns>接続されているノードのIDのリスト。</returns>
    public List<string> GetConnectedNodeIds()
    {
        // nullチェックを行い、null要素をフィルタリングしてNodeIdのリストを返します。
        if (ConnectedNodes == null)
        {
            return new List<string>();
        }
        return ConnectedNodes.Where(node => node != null && !string.IsNullOrEmpty(node.NodeId))
                             .Select(node => node.NodeId)
                             .ToList();
    }
}