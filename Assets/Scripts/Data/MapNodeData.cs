// MapNodeData.cs
// 個々のマップノードのデータ（ID、位置、接続ノード）を保持するScriptableObject

using UnityEngine;
using System.Collections.Generic;

// Unityエディタでこのアセットを作成するためのメニュー項目を追加します。
[CreateAssetMenu(fileName = "NewMapNodeData", menuName = "Map System/Map Node Data", order = 1)]
public class MapNodeData : ScriptableObject
{
    public string nodeId;             // ノードのユニークなID
    public Vector2 position;          // ノードのマップ上でのUI位置（anchoredPosition）
    public List<string> connectedNodeIds = new List<string>(); // 接続されているノードのIDリスト
}

// 複数のMapNodeDataアセットをまとめて管理するためのScriptableObject
// 全てのノードデータをこの一つのアセットで参照できるようにします。
[CreateAssetMenu(fileName = "MapDataCollection", menuName = "Map System/Map Data Collection", order = 2)]
public class MapDataCollection : ScriptableObject
{
    public List<MapNodeData> nodes = new List<MapNodeData>(); // 全てのノードデータのリスト
}