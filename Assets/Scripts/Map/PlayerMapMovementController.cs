// PlayerMapMovementController.cs 
// このスクリプトは、マップ上のプレイヤーアイコンの位置管理と、
// ノード間のアニメーション移動（速度ベース）を制御します。
// MapInteractionHandlerからの移動要求を受け取り、MapPathManagerとPlayerLocationSetterと連携します。

using UnityEngine;
using UnityEngine.UI; // RectTransformを使用するために必要
using System.Collections; // コルーチンを使用するために必要
using System.Linq; // Contains() を使用するために必要

public class PlayerMapMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public RectTransform playerIconRectTransform;     // プレイヤーアイコンのRectTransform (publicにしてMapInteractionHandlerから参照できるように)
    [SerializeField] private PlayerLocationSetter playerLocationSetter; // PlayerLocationSetterへの参照
    [SerializeField] private MapInteractionHandler mapInteractionHandler; // MapInteractionHandlerへの参照 (センタリングを呼び出すため)

    [Header("Player Initial Position")]
    [Tooltip("プレイヤーアイコンの初期位置として設定するノードのID。")]
    [SerializeField] private string initialPlayerNodeId = "TownA"; // 例: 初期位置をTownAに設定

    [Header("Current Player Node")]
    [Tooltip("プレイヤーアイコンが現在いるノードのID。")]
    public string currentPlayerNodeId; // 現在プレイヤーアイコンがいるノードのID

    [Header("Player Movement Settings")]
    [Tooltip("プレイヤーアイコンがノード間を移動する速度（単位: ピクセル/秒）。")]
    [SerializeField] private float moveSpeed = 300f; // 移動アニメーションの速度（ピクセル/秒）
    [Tooltip("プレイヤーアイコンの移動完了後、視点がセンタリングされるまでの遅延時間（秒）。")]
    [SerializeField] private float centerViewDelay = 0.5f; // センタリングまでの遅延時間
    [Tooltip("プレイヤーアイコンの移動完了後、自動的に画面をセンタリングするかどうか。")]
    [SerializeField] private bool shouldCenterViewAfterMove = true; // センタリング機能のオンオフ

    private bool isMoving = false; // プレイヤーアイコンが移動中かどうかを示すフラグ

    /// <summary>
    /// Startは最初のフレーム更新の前に呼び出されます。
    /// 必要な参照の確認と、初期位置の設定を行います。
    /// </summary>
    void Start()
    {
        // 必要な参照が設定されているか確認
        if (playerIconRectTransform == null)
        {
            Debug.LogError("[PlayerMapMovementController] プレイヤーアイコンのRect Transformがアサインされていません！Inspectorで設定してください。", this);
            enabled = false;
            return;
        }
        if (playerLocationSetter == null)
        {
            Debug.LogError("[PlayerMapMovementController] Player Location Setterがアサインされていません！Inspectorで設定してください。", this);
            enabled = false;
            return;
        }
        if (MapPathManager.Instance == null)
        {
            Debug.LogError("[PlayerMapMovementController] MapPathManagerのインスタンスが見つかりません。シーンにMapPathManagerをアタッチしたGameObjectがあるか確認してください。", this);
            enabled = false;
            return;
        }
        if (mapInteractionHandler == null)
        {
            Debug.LogError("[PlayerMapMovementController] Map Interaction Handlerがアサインされていません！Inspectorで設定してください。", this);
            enabled = false;
            return;
        }

        // プレイヤーアイコンの初期位置を設定（アニメーションなしで即時設定）
        SetPlayerPositionInstant(initialPlayerNodeId);

        // 初期表示は即時センタリング（アニメーションなし）
        mapInteractionHandler.CenterViewOnPlayerIcon(instant: true);

        Debug.Log("[PlayerMapMovementController] スクリプトが開始されました。");
    }

    /// <summary>
    /// プレイヤーアイコンを、指定されたノードの位置にアニメーションなしで即時設定します。
    /// 初期設定用。
    /// </summary>
    /// <param name="nodeId">設定先として設定するノードのID。</param>
    private void SetPlayerPositionInstant(string nodeId)
    {
        RectTransform targetNodeRectTransform = playerLocationSetter.GetTargetNodeRectTransform(nodeId);

        if (targetNodeRectTransform != null)
        {
            playerIconRectTransform.anchoredPosition = targetNodeRectTransform.anchoredPosition;
            currentPlayerNodeId = nodeId;
            Debug.Log($"[PlayerMapMovementController] プレイヤーアイコンをノード '{nodeId}' の位置に即時設定しました。現在のノード: {currentPlayerNodeId}");
        }
        else
        {
            Debug.LogWarning($"[PlayerMapMovementController] 指定されたノードID '{nodeId}' が見つからなかったため、プレイヤーアイコンの位置は変更されませんでした.", this);
        }
    }

    /// <summary>
    /// 指定されたノードIDへプレイヤーアイコンを移動させる公開メソッド。
    /// MapInteractionHandlerから呼び出されます。
    /// </summary>
    /// <param name="targetNodeId">移動先のノードID。</param>
    /// <returns>移動を開始できた場合はtrue、移動中のため開始できなかった場合はfalse。</returns>
    public bool TryMovePlayerToNode(string targetNodeId)
    {
        // プレイヤーが移動中の場合は、新しい移動リクエストを無視する
        if (isMoving)
        {
            Debug.Log($"[PlayerMapMovementController] プレイヤーアイコンが移動中のため、移動リクエスト '{targetNodeId}' を無視しました。", this);
            return false;
        }

        // 現在のプレイヤーノード情報を取得
        MapNode currentPlayerNode = MapPathManager.Instance.GetNode(currentPlayerNodeId);

        if (currentPlayerNode == null)
        {
            Debug.LogError($"[PlayerMapMovementController] 現在のプレイヤーノード '{currentPlayerNodeId}' が見つかりません。移動できません。", this);
            return false;
        }

        // クリックされたノードが現在のノードの隣接ノードであるかを確認
        if (currentPlayerNode.GetConnectedNodeIds().Contains(targetNodeId))
        {
            // 隣接ノードであれば移動アニメーションを開始
            StartCoroutine(MovePlayerIconAnimated(targetNodeId));
            return true;
        }
        else
        {
            Debug.LogWarning($"[PlayerMapMovementController] ノード '{targetNodeId}' は現在のノード '{currentPlayerNodeId}' に直接接続されていません。移動できません。", this);
            return false;
        }
    }

    /// <summary>
    /// プレイヤーアイコンを指定されたノードへアニメーションで移動させます。
    /// </summary>
    /// <param name="targetNodeId">移動先のノードID。</param>
    private IEnumerator MovePlayerIconAnimated(string targetNodeId)
    {
        isMoving = true; // 移動中フラグを立てる

        RectTransform targetNodeRectTransform = playerLocationSetter.GetTargetNodeRectTransform(targetNodeId);

        if (targetNodeRectTransform == null)
        {
            Debug.LogWarning($"[PlayerMapMovementController] 移動先のノードID '{targetNodeId}' が見つかりませんでした。移動を中断します。", this);
            isMoving = false;
            yield break; // コルーチンを終了
        }

        Vector2 startPosition = playerIconRectTransform.anchoredPosition;
        Vector2 endPosition = targetNodeRectTransform.anchoredPosition;

        // 目標位置に到達するまでループ
        while (Vector2.Distance(playerIconRectTransform.anchoredPosition, endPosition) > 0.1f) // ある程度の誤差を許容
        {
            // 現在位置から目標位置へ、指定された速度で移動
            playerIconRectTransform.anchoredPosition = Vector2.MoveTowards(
                playerIconRectTransform.anchoredPosition,
                endPosition,
                moveSpeed * Time.deltaTime // 速度 * デルタタイム で1フレームの移動距離を計算
            );
            yield return null; // 1フレーム待機
        }

        // 移動完了後、正確な最終位置に設定
        playerIconRectTransform.anchoredPosition = endPosition;
        currentPlayerNodeId = targetNodeId; // 現在のノードIDを更新
        isMoving = false; // 移動中フラグを下ろす

        Debug.Log($"[PlayerMapMovementController] プレイヤーアイコンを '{targetNodeId}' に移動が完了しました。現在のノード: {currentPlayerNodeId}");

        // 移動完了後、自動センタリング設定がONであれば時間差で画面をセンタリング
        if (shouldCenterViewAfterMove)
        {
            StartCoroutine(CenterViewAfterDelay());
        }
    }

    /// <summary>
    /// 遅延後に画面をプレイヤーアイコンにセンタリングします。
    /// </summary>
    private IEnumerator CenterViewAfterDelay()
    {
        Debug.Log("[PlayerMapMovementController] CenterViewAfterDelay コルーチンが開始されました。"); // ★追加
        yield return new WaitForSeconds(centerViewDelay); // 指定された秒数待機

        if (mapInteractionHandler != null)
        {
            // MapInteractionHandlerのセンタリング機能をアニメーションで呼び出す
            mapInteractionHandler.CenterViewOnPlayerIcon(instant: false);
            Debug.Log("[PlayerMapMovementController] MapInteractionHandler.CenterViewOnPlayerIcon を呼び出しました。"); // ★追加
        }
        else
        {
            Debug.LogWarning("[PlayerMapMovementController] MapInteractionHandlerがアサインされていないため、遅延センタリングを実行できませんでした。", this);
        }
        Debug.Log("[PlayerMapMovementController] CenterViewAfterDelay コルーチンが終了しました。"); // ★追加
    }

    /// <summary>
    /// プレイヤーアイコンが現在移動中であるかを取得します。
    /// </summary>
    public bool IsPlayerMoving()
    {
        return isMoving;
    }
}