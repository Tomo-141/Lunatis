// MapZoomController.cs
// このスクリプトは、マップUIのマウスホイールによるズームイン・ズームアウトを管理します。
// ズーム対象のRectTransformのlocalScaleを操作し、ズーム範囲を制限します。
// プレイヤーアイコンが移動中の場合は、ズーム操作を一時的に無効化し、干渉を防ぎます。
// このスクリプトは主に「マップのズーム操作」という単一の機能に責任を持ちます。

using UnityEngine;
using UnityEngine.UI; // RectTransformを使用するために必要

public class MapZoomController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform worldMapRawImageRectTransform; // ズーム対象のワールドマップRawImageのRectTransform
    [SerializeField] private PlayerMapMovementController playerMapMovementController; // プレイヤーの移動状態を確認するためのPlayerMapMovementControllerへの参照
    [SerializeField] private MapBoundsClamper mapBoundsClamper; // マップのズーム後に境界にクランプするためのMapBoundsClamperへの参照

    [Header("Zoom Settings")]
    [SerializeField] private float initialZoomScale = 1.0f; // マップの初期ズームスケール (例: 1.0fは原寸大)
    [SerializeField] private float minZoomScale = 0.5f;     // マップの最小ズームスケール（これ以上縮小しない下限値）
    [SerializeField] private float maxZoomScale = 5.0f;     // マップの最大ズームスケール（これ以上拡大しない上限値）
    [SerializeField] private float zoomSpeed = 0.1f;        // マウスホイール1回あたりのズーム変化量

    private float currentZoomScale; // 現在適用されているマップのズームスケール

    /// <summary>
    /// Startは最初のフレーム更新の前に呼び出されます。
    /// 必要な参照の確認と、初期ズームスケールの適用を行います。
    /// </summary>
    void Start()
    {
        // ズーム対象のRectTransformがアサインされているか確認
        if (worldMapRawImageRectTransform == null)
        {
            Debug.LogError("[MapZoomController] World Map Raw Image Rect Transformがアサインされていません！Inspectorで設定してください。", this);
            enabled = false; // スクリプトを無効にする
            return;
        }
        // プレイヤー移動コントローラーの参照がアサインされているか確認
        if (playerMapMovementController == null)
        {
            Debug.LogError("[MapZoomController] Player Map Movement Controllerがアサインされていません！Inspectorで設定してください。", this);
            enabled = false;
            return;
        }
        // MapBoundsClamperの参照がアサインされているか確認
        if (mapBoundsClamper == null)
        {
            Debug.LogError("[MapZoomController] Map Bounds Clamperがアサインされていません！Inspectorで設定してください。", this);
            enabled = false;
            return;
        }

        // 初期ズームスケールを適用します。
        currentZoomScale = initialZoomScale;
        worldMapRawImageRectTransform.localScale = new Vector3(currentZoomScale, currentZoomScale, 1f);

        // 初期ロード時にマップ位置を境界内にクランプします。
        mapBoundsClamper.ClampMapPosition();

        Debug.Log("[MapZoomController] スクリプトが開始されました。初期ズームスケール: " + currentZoomScale);
    }

    /// <summary>
    /// 毎フレーム呼び出されるUpdateメソッドです。
    /// プレイヤーが移動中でない限り、マウスホイールによるズーム操作を検出し、処理します。
    /// </summary>
    void Update()
    {
        // プレイヤーアイコンが現在移動中である場合、ズーム操作を無効化します。
        // これにより、プレイヤー移動中とズーム操作の干渉を防ぎます。
        if (playerMapMovementController != null && playerMapMovementController.IsPlayerMoving())
        {
            // デバッグログは必要に応じてコメントアウトしてください。
            // Debug.Log("[MapZoomController] プレイヤー移動中のため、ズーム操作を無効化しています。");
            return; // プレイヤー移動中はこれ以降のズーム処理を実行しません。
        }

        // マウスホイールの入力を検出し、ズーム処理を実行します。
        HandleZoom();

        // ★ここから mapBoundsClamper.ClampMapPosition(); の呼び出しを削除しました。
        // ★ClampMapPosition() は HandleZoom() 内（スケール変更時）と、
        // ★MapInteractionHandler のドラッグ/センタリング時に呼ばれれば十分です。
    }

    /// <summary>
    /// マウスホイールの入力に基づいてマップのズームスケールを調整します。
    /// currentZoomScaleを更新し、worldMapRawImageRectTransformのlocalScaleに適用します。
    /// </summary>
    private void HandleZoom()
    {
        // マウスホイールのスクロール量を取得します。（上方向スクロールで正の値、下方向で負の値）
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // スクロール量がある場合（微小なノイズを除外するため絶対値でチェック）のみ処理を実行します。
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // 新しいズームスケールを計算します。
            // スクロール量にズーム速度を乗算し、現在のズームスケールに加算します。
            float newZoomScale = currentZoomScale + scroll * zoomSpeed;

            // 計算されたズームスケールを、事前に設定された最小値と最大値の範囲内にクランプ（制限）します。
            newZoomScale = Mathf.Clamp(newZoomScale, minZoomScale, maxZoomScale);

            // 計算された新しいズームスケールが現在のスケールと異なる場合のみ、更新を適用します。
            if (newZoomScale != currentZoomScale)
            {
                currentZoomScale = newZoomScale; // 現在のズームスケールを更新

                // WorldMapRawImageのlocalScaleを更新し、マップ全体をズームします。
                // z軸はUIでは通常1に設定します。
                worldMapRawImageRectTransform.localScale = new Vector3(currentZoomScale, currentZoomScale, 1f);

                Debug.Log($"[MapZoomController] ズームスケール変更: {currentZoomScale}");

                // ズームスケールが変更された後、マップが境界外に出ないようにクランプします。
                // ズームによってマップの表示範囲が変わるため、このタイミングでクランプが必要です。
                if (mapBoundsClamper != null)
                {
                    mapBoundsClamper.ClampMapPosition();
                }
            }
        }
    }
}