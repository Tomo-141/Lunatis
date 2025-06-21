// MapInteractionHandler.cs
// このスクリプトは、マップUIに対するユーザーの入力（ドラッグ、ノードクリック）を検出し、
// それに応じた処理（マップの視点移動、プレイヤー移動要求）を関連コンポーネントに発行します。
// 主にマップのドラッグ操作とノードのクリック検出を担当します。
// プレイヤーアイコンが移動中の間は、マップのドラッグ操作を一時的に無効化し、誤動作を防ぎます。

using UnityEngine;
using UnityEngine.EventSystems; // IDragHandler, IBeginDragHandler, IEndDragHandler を使用するために必要
using UnityEngine.UI; // RectTransformを使用するために必要
using System.Collections; // コルーチンを使用するために必要

// MapInteractionHandlerは、マップの視点操作（センタリング、ドラッグ）と、
// ノードクリックの検出を管理します。
// IBeginDragHandler, IDragHandler, IEndDragHandlerを実装することで、UIイベントシステムを通じてドラッグ操作を検出します。
// スクリプトの読み込み順序としては、MapPathManagerやPlayerMapMovementControllerより後に実行されることを想定しています。
public class MapInteractionHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private RectTransform worldMapRawImageRectTransform; // ワールドマップのRawImage（地図画像）のRectTransform
    [SerializeField] private MapBoundsClamper mapBoundsClamper;         // マップが画面外に出ないように境界にクランプするためのスクリプトへの参照
    [SerializeField] private PlayerMapMovementController playerMovementController; // プレイヤーアイコンの移動状態や位置情報を取得するためのスクリプトへの参照

    [Header("Drag Settings")]
    [SerializeField] private float dragSpeed = 1.0f; // マップをドラッグする際の速度係数

    [Header("Centering Settings")]
    [Tooltip("プレイヤーアイコンに視点をセンタリングする際の速度。")]
    [SerializeField] private float centerSpeed = 5.0f; // センタリングアニメーションの速度
    [Tooltip("センタリングアニメーションの最大継続時間（秒）。これを過ぎると強制終了。")]
    [SerializeField] private float maxCenteringDuration = 1.0f; // センタリングの最大継続時間 (1秒)
    [Tooltip("センタリング終了と判定する距離の閾値。この値以下になればセンタリングが終了します。")]
    [SerializeField] private float centeringThreshold = 0.5f; // センタリングの誤差許容範囲

    private Vector2 lastMousePosition; // ドラッグ開始時のマウス位置、または前フレームのマウス位置を保持
    private bool isDragging = false;   // 現在ドラッグ操作中かどうかを示すフラグ
    private Coroutine centeringCoroutine; // センタリングアニメーションを実行中のコルーチンへの参照を保持（重複実行防止のため）

    /// <summary>
    /// スクリプトが有効になったときに一度だけ呼び出されます。
    /// ここでMapNodeがクリックされた際に呼び出されるイベント（OnNodeClicked）の購読を開始します。
    /// これにより、マップ上のノードがクリックされた際にHandleNodeClickメソッドが実行されます。
    /// </summary>
    void OnEnable()
    {
        // MapNode.OnNodeClickedイベントにHandleNodeClickメソッドを登録します。
        MapNode.OnNodeClicked += HandleNodeClick;
        Debug.Log("[MapInteractionHandler] MapNode.OnNodeClicked イベントの購読を開始しました。");
    }

    /// <summary>
    /// スクリプトが無効になったときに一度だけ呼び出されます。
    /// これはメモリリークを防ぐために非常に重要です。
    /// ここでノードクリックイベントの購読を解除します。
    /// </summary>
    void OnDisable()
    {
        // MapNode.OnNodeClickedイベントからHandleNodeClickメソッドの登録を解除します。
        MapNode.OnNodeClicked -= HandleNodeClick;
        Debug.Log("[MapInteractionHandler] MapNode.OnNodeClicked イベントの購読を解除しました。");

        // スクリプトが無効になる際に、もしセンタリングコルーチンが実行中であれば停止します。
        if (centeringCoroutine != null)
        {
            StopCoroutine(centeringCoroutine);
            centeringCoroutine = null;
            Debug.Log("[MapInteractionHandler] OnDisable時、実行中のセンタリングコルーチンを停止しました。");
        }
    }

    /// <summary>
    /// Startはスクリプトがロードされ、最初のフレーム更新の前に一度だけ呼び出されます。
    /// 必要なコンポーネントの参照がInspectorで設定されているかを確認し、
    /// プロジェクト開始時にプレイヤーアイコンへ視点を即時センタリングします。
    /// </summary>
    void Start()
    {
        // ズーム対象のワールドマップRectTransformがInspectorでアサインされているかを確認します。
        if (worldMapRawImageRectTransform == null)
        {
            Debug.LogError("[MapInteractionHandler] World Map Raw Image Rect Transformがアサインされていません！Inspectorで設定してください。", this);
            enabled = false; // アサインされていない場合はスクリプトを無効化し、これ以上処理が進まないようにします。
            return;
        }
        // マップ境界クランプ用スクリプトへの参照がInspectorでアサインされているかを確認します。
        if (mapBoundsClamper == null)
        {
            Debug.LogError("[MapInteractionHandler] Map Bounds Clamperがアサインされていません！Inspectorで設定してください。", this);
            enabled = false; // アサインされていない場合はスクリプトを無効化し、これ以上処理が進まないようにします。
            return;
        }
        // プレイヤーアイコン移動制御スクリプトへの参照がInspectorでアサインされているかを確認します。
        // これがないと、プレイヤー移動中のドラッグ無効化やセンタリング機能が動作しません。
        if (playerMovementController == null)
        {
            Debug.LogError("[MapInteractionHandler] Player Map Movement Controllerがアサインされていません！Inspectorで設定してください。", this);
            enabled = false; // アサインされていない場合はスクリプトを無効化し、これ以上処理が進まないようにします。
            return;
        }

        // 初期表示はプレイヤーアイコンに即時（アニメーションなしで）視点をセンタリングします。
        CenterViewOnPlayerIcon(instant: true);

        Debug.Log("[MapInteractionHandler] スクリプトが開始されました。");
    }

    /// <summary>
    /// 毎フレーム呼び出されるUpdateメソッドです。
    /// このメソッドでは、主にプレイヤーアイコンが移動中の場合にマップ操作を無効化する処理を行います。
    /// </summary>
    void Update()
    {
        // プレイヤーアイコンが現在移動中であるかどうかをPlayerMapMovementControllerから取得します。
        // もしplayerMovementControllerがnullでなく、かつ移動中であれば、
        // マップのドラッグやズームなどのユーザー操作を一時的に無効化します。
        // これにより、移動アニメーション中のマップ操作による予期せぬ挙動を防ぎます。
        if (playerMovementController != null && playerMovementController.IsPlayerMoving())
        {
            // デバッグログは必要に応じてコメントアウトしてください。
            // Debug.Log("[MapInteractionHandler] プレイヤー移動中のため、マップ操作を無効化しています。");

            // もし移動中に誤ってドラッグ状態になってしまっていたら、その状態を解除します。
            if (isDragging)
            {
                isDragging = false;
                Debug.Log("[MapInteractionHandler] プレイヤー移動中にドラッグ状態が検出されたため、解除しました。");
            }
            return; // プレイヤー移動中はこれ以降のマップ操作処理（ドラッグ、ズームなど）を実行しません。
        }
    }

    /// <summary>
    /// MapNodeがクリックされたときにMapNodeスクリプトから呼び出されるイベントハンドラです。
    /// クリックされたノードへプレイヤーアイコンを移動させる処理をPlayerMapMovementControllerに要求します。
    /// </summary>
    /// <param name="clickedNodeId">クリックされたノードのID。</param>
    private void HandleNodeClick(string clickedNodeId)
    {
        Debug.Log($"[MapInteractionHandler] ノードクリックイベントを受信しました: {clickedNodeId}");

        // プレイヤーアイコンが移動中の場合、または現在ドラッグ操作中の場合は、ノードクリックを無視します。
        // これにより、移動中やマップ操作中の意図しないプレイヤー移動を防ぎます。
        // playerMovementControllerがnullでないことを確認してからIsPlayerMoving()を呼び出します。
        if (playerMovementController != null && playerMovementController.IsPlayerMoving() || isDragging)
        {
            Debug.Log($"[MapInteractionHandler] プレイヤーアイコンが移動中、またはドラッグ中のため、クリックを無視しました.", this);
            return;
        }

        // PlayerMapMovementControllerに、クリックされたノードへのプレイヤー移動を試みるよう要求します。
        playerMovementController.TryMovePlayerToNode(clickedNodeId);
    }

    /// <summary>
    /// マップの視点をプレイヤーアイコンの位置にセンタリングします。
    /// マップのRectTransformのanchoredPositionを調整することで、プレイヤーアイコンが画面の中央に来るようにします。
    /// このメソッドはPlayerMapMovementControllerからも呼び出されます。
    /// </summary>
    /// <param name="instant">trueの場合、アニメーションなしで即座にセンタリングします。falseの場合、滑らかなアニメーションでセンタリングします。</param>
    public Coroutine CenterViewOnPlayerIcon(bool instant = false)
    {
        // 既にセンタリングコルーチンが実行中であれば、そのコルーチンを停止します。
        // これにより、新しいセンタリング要求が来たときに、古いコルーチンが残り続けることを防ぎます。
        if (centeringCoroutine != null)
        {
            StopCoroutine(centeringCoroutine);
            Debug.Log("[MapInteractionHandler] 既存のセンタリングコルーチンを停止しました。");
            centeringCoroutine = null; // 停止したら参照を必ずクリアします。
        }

        // プレイヤーアイコンのRectTransformをPlayerMapMovementControllerから取得します。
        // センタリングの目標位置を計算するために必要です。
        // playerMovementControllerがnullでないことを確認します。
        if (playerMovementController == null)
        {
            Debug.LogError("[MapInteractionHandler] Player Map Movement Controllerがアサインされていないため、センタリングできません。", this);
            return null;
        }

        RectTransform playerIconRectTransform = playerMovementController.playerIconRectTransform;

        // プレイヤーアイコンのRectTransformが取得できなかった場合はエラーログを出力し、処理を中断します。
        if (playerIconRectTransform == null)
        {
            Debug.LogError("[MapInteractionHandler] プレイヤーアイコンのRect TransformがPlayerMapMovementControllerから取得できませんでした。センタリングを中断します。", this);
            return null;
        }

        // プレイヤーアイコンの現在の位置（ローカル座標）を取得します。
        Vector2 playerPosInMap = playerIconRectTransform.anchoredPosition;
        // 現在のマップのスケール（ズーム倍率）を取得します。
        float currentScale = worldMapRawImageRectTransform.localScale.x;
        // マップ画像をセンタリングするための目標anchoredPositionを計算します。
        // プレイヤーアイコンの位置を反転させ、現在のスケールを適用することで、マップが移動した際にアイコンが中央に来るようにします。
        Vector2 targetMapPosition = -playerPosInMap * currentScale;

        // 即時センタリングモードの場合
        if (instant)
        {
            // マップのanchoredPositionを計算された目標位置に直接設定します。
            worldMapRawImageRectTransform.anchoredPosition = targetMapPosition;
            Debug.Log($"[MapInteractionHandler] プレイヤーアイコンに視点を即時センタリングしました。プレイヤーアイコンの位置: {playerPosInMap}, マップ画像のanchoredPosition: {worldMapRawImageRectTransform.anchoredPosition}");
            // 位置を設定した後、マップが境界外に出ないようにクランプします。
            mapBoundsClamper.ClampMapPosition();
            return null; // コルーチンではないためnullを返します。
        }
        // アニメーションセンタリングモードの場合
        else
        {
            // CenterViewAnimatedCoroutineコルーチンを開始し、その参照を保持します。
            // これにより、コルーチンが実行中であることを管理できます。
            centeringCoroutine = StartCoroutine(CenterViewAnimatedCoroutine(targetMapPosition));
            Debug.Log("[MapInteractionHandler] 新しいセンタリングコルーチンを開始しました。");
            return centeringCoroutine; // 開始したコルーチンの参照を返します。
        }
    }

    /// <summary>
    /// マップの視点を目標位置へ滑らかなアニメーションで移動させるコルーチンです。
    /// Lerp関数を使用して、現在の位置から目標位置へ徐々に近づけます。
    /// </summary>
    /// <param name="targetPosition">マップの最終的なanchoredPosition（目標位置）。</param>
    private IEnumerator CenterViewAnimatedCoroutine(Vector2 targetPosition)
    {
        Debug.Log($"[MapInteractionHandler] CenterViewAnimatedCoroutine コルーチン開始。目標位置: {targetPosition}");
        float elapsedTime = 0f;

        // 目標位置に非常に近く（誤差許容範囲以内）なるか、または最大継続時間を超えるまでループを続けます。
        while (Vector2.Distance(worldMapRawImageRectTransform.anchoredPosition, targetPosition) > centeringThreshold &&
               elapsedTime < maxCenteringDuration)
        {
            // Vector2.LerpからVector2.MoveTowardsに変更
            worldMapRawImageRectTransform.anchoredPosition = Vector2.MoveTowards(
                worldMapRawImageRectTransform.anchoredPosition,
                targetPosition,
                Time.deltaTime * centerSpeed * 20f // Lerpに比べて速度調整が必要になる場合があるため、初期値を20倍に調整
            );

            // クランプ処理を有効化
            mapBoundsClamper.ClampMapPosition();

            elapsedTime += Time.deltaTime;

            // センタリング中の詳細ログ (動作確認後にコメントアウトしてください)
            Debug.Log($"[MapInteractionHandler DEBUG] 現在位置: {worldMapRawImageRectTransform.anchoredPosition}, 目標: {targetPosition}, 距離: {Vector2.Distance(worldMapRawImageRectTransform.anchoredPosition, targetPosition):F4}, 経過: {elapsedTime:F2}s, MaxDuration: {maxCenteringDuration:F2}s");

            yield return null;
        }

        // ループ終了後、マップの最終位置を目標位置に正確に設定します。
        worldMapRawImageRectTransform.anchoredPosition = targetPosition;
        // 最終位置設定後も、マップが境界外に出ないように再度クランプします。
        mapBoundsClamper.ClampMapPosition();

        // コルーチンがどのように終了したかをログで確認
        if (Vector2.Distance(worldMapRawImageRectTransform.anchoredPosition, targetPosition) <= centeringThreshold)
        {
            Debug.Log("[MapInteractionHandler] CenterViewAnimatedCoroutine コルーチンが目標距離に到達して完了しました。");
        }
        else
        {
            Debug.LogWarning($"[MapInteractionHandler] CenterViewAnimatedCoroutine コルーチンが最大継続時間({maxCenteringDuration}秒)を超えて強制終了しました。目標距離に未到達。最終距離: {Vector2.Distance(worldMapRawImageRectTransform.anchoredPosition, targetPosition):F4}");
        }

        centeringCoroutine = null; // コルーチンが終了したので、参照をnullにクリアします。
    }

    /// <summary>
    /// マップのドラッグ操作が開始されたときに呼び出されます。
    /// UIイベントシステム（IBeginDragHandler）によって検出されます。
    /// </summary>
    /// <param name="eventData">ポインターイベントデータ。マウスのボタンや位置情報を含みます。</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // プレイヤーアイコンが現在移動中である場合、ドラッグ操作を開始させないようにします。
        // これにより、プレイヤー移動中とマップドラッグ操作の干渉を防ぎます。
        // playerMovementControllerがnullでないことを確認してからIsPlayerMoving()を呼び出します。
        if (playerMovementController != null && playerMovementController.IsPlayerMoving())
        {
            Debug.Log("[MapInteractionHandler] プレイヤー移動中のため、ドラッグ開始を無効化しました。", this);
            isDragging = false; // 念のため、ドラッグ中フラグを確実にfalseにします。
            return;
        }

        // 左クリック（Primary Button）でのドラッグのみを検出します。
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = true; // ドラッグ中フラグをtrueに設定します。
            lastMousePosition = eventData.position; // 現在のマウス位置を記録します。
            Debug.Log("[MapInteractionHandler] ドラッグ開始。");
        }
    }

    /// <summary>
    /// ドラッグ中に毎フレーム呼び出されます。
    /// UIイベントシステム（IDragHandler）によって検出されます。
    /// マウスの移動量に応じてマップのanchoredPositionを調整し、マップを移動させます。
    /// </summary>
    /// <param name="eventData">ポインターイベントデータ。マウスの現在位置を含みます。</param>
    public void OnDrag(PointerEventData eventData)
    {
        // isDraggingフラグがtrueの場合（つまり、ドラッグ操作が開始されている場合）のみ処理を実行します。
        if (isDragging)
        {
            // 現在のマウス位置を取得します。
            Vector2 currentMousePosition = eventData.position;
            // 前のフレームからのマウスの移動量（デルタ）を計算します。
            Vector2 delta = currentMousePosition - lastMousePosition;

            // マップのanchoredPositionにマウスの移動量を適用し、ドラッグ速度を乗算します。
            worldMapRawImageRectTransform.anchoredPosition += delta * dragSpeed;

            // 現在のマウス位置を次フレームの計算のために保存します。
            lastMousePosition = currentMousePosition;

            // マップが境界外に出ないように、毎フレーム位置をクランプするようMapBoundsClamperに要求します。
            mapBoundsClamper.ClampMapPosition();
        }
    }

    /// <summary>
    /// ドラッグ操作が終了したときに呼び出されます。
    /// UIイベントシステム（IEndDragHandler）によって検出されます。
    /// </summary>
    /// <param name="eventData">ポインターイベントデータ。どのボタンが離されたかを含みます。</param>
    public void OnEndDrag(PointerEventData eventData)
    {
        // 左クリック（Primary Button）でのドラッグが終了した場合のみ処理を実行します。
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = false; // ドラッグ中フラグをfalseに設定します。
            Debug.Log("[MapInteractionHandler] ドラッグ終了。");
        }
    }
}