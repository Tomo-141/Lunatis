// PlayerLocationSetter.cs
// このスクリプトは、プレイヤーアイコンが初期位置として指定すべきMapNodeの情報を取得する機能を提供します。
// マップの初期化時や、ゲームイベントによってプレイヤーの位置を強制的に変更する際に、
// その移動先のノード情報を他のスクリプトに提供します。

using UnityEngine;
using UnityEngine.UI; // RectTransformを使用するために必要

public class PlayerLocationSetter : MonoBehaviour
{
    // このスクリプト自体はプレイヤーアイコンのRectTransformを直接操作しないため、参照は不要になります。
    // その代わり、必要なノードの情報を返す形にします。

    /// <summary>
    /// 指定されたMapNodeの情報を取得します。
    /// このメソッドは外部（例: MapInteractionHandlerやゲームイベントスクリプト）から呼び出され、
    /// そのノードの位置情報を使ってプレイヤーアイコンを配置したり、視点を調整したりするために使われます。
    /// </summary>
    /// <param name="targetNodeId">取得したいMapNodeのID。</param>
    /// <returns>指定されたIDのMapNodeのRectTransform。見つからない場合はnull。</returns>
    public RectTransform GetTargetNodeRectTransform(string targetNodeId)
    {
        if (MapPathManager.Instance == null)
        {
            Debug.LogError("[PlayerLocationSetter] MapPathManagerのインスタンスが見つかりません。シーンにMapPathManagerをアタッチしたGameObjectがあるか確認してください。", this);
            return null;
        }

        MapNode targetNode = MapPathManager.Instance.GetNode(targetNodeId);
        if (targetNode != null)
        {
            Debug.Log($"[PlayerLocationSetter] ノード '{targetNodeId}' の位置情報を取得しました。");
            return targetNode.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogWarning($"[PlayerLocationSetter] 指定されたノードID '{targetNodeId}' が見つかりませんでした。", this);
            return null;
        }
    }
}