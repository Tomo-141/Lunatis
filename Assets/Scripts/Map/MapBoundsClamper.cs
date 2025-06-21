// MapBoundsClamper.cs 
// このスクリプトは、ワールドマップのRectTransformのanchoredPositionを制限し、
// マップが画面の表示範囲を超えて移動しないようにクランプする機能を提供します。
// ズームスケールと画面解像度を考慮して、動的に範囲を計算します。

using UnityEngine;
using UnityEngine.UI; // RectTransformを使用するために必要

public class MapBoundsClamper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform worldMapRawImageRectTransform; // クランプ対象のワールドマップRawImageのRectTransform

    // マップのオリジナルサイズ (要件に基づき2048x2048と仮定)
    private const float MAP_ORIGINAL_WIDTH = 2048f;
    private const float MAP_ORIGINAL_HEIGHT = 2048f;

    /// <summary>
    /// Startは最初のフレーム更新の前に呼び出されます。
    /// 必要な参照の確認と、初期位置のクランプを行います。
    /// </summary>
    void Start()
    {
        // クランプ対象のRectTransformがアサインされているか確認
        if (worldMapRawImageRectTransform == null)
        {
            Debug.LogError("[MapBoundsClamper] World Map Raw Image Rect Transformがアサインされていません！Inspectorで設定してください。", this);
            enabled = false; // スクリプトを無効にする
            return;
        }

        // 初期ロード時にマップ位置をクランプし、マップが範囲内に収まるようにします。
        ClampMapPosition();

        Debug.Log("[MapBoundsClamper] スクリプトが開始されました。");
    }

    /// <summary>
    /// Updateは毎フレーム呼び出されます。
    /// ドラッグなどでマップの位置が変更される可能性があるので、常に位置をクランプします。
    /// </summary>
    void Update()
    {
        // 毎フレーム、マップの位置が範囲内に収まっているか確認し、必要であれば修正します。
        // これにより、ドラッグ後だけでなく、ズームなどで位置が範囲外に出た場合も自動的に修正されます。
        ClampMapPosition();
    }

    /// <summary>
    /// マップのRectTransformのanchoredPositionを、マップの表示範囲内に制限します。
    /// 現在のズームスケールと画面サイズを考慮します。
    /// </summary>
    public void ClampMapPosition()
    {
        // 現在のマップのスケールを取得します。
        // MapZoomControllerによってlocalScaleが変更されていることを想定します。
        float currentScale = worldMapRawImageRectTransform.localScale.x;

        // 親のRectTransform（WorldMapPanel）のサイズを取得します。これが画面の表示範囲に相当します。
        // MapBoundsClamperはWorldMapPanelにアタッチされるため、自身のRectTransformの親を考慮します。
        RectTransform parentRect = worldMapRawImageRectTransform.parent.GetComponent<RectTransform>();
        if (parentRect == null)
        {
            Debug.LogError("[MapBoundsClamper] 親のRectTransformが見つかりません。マップの位置制限ができません。", this);
            return;
        }

        float screenWidth = parentRect.rect.width;
        float screenHeight = parentRect.rect.height;

        // スケール後のマップの実際のピクセルサイズを計算します。
        float scaledMapWidth = MAP_ORIGINAL_WIDTH * currentScale;
        float scaledMapHeight = MAP_ORIGINAL_HEIGHT * currentScale;

        // マップのanchoredPositionが取り得る最小値と最大値を計算します。
        // これらの値は、マップの端が画面の端にぴったり合うときのanchoredPositionです。
        // Unity UIのanchoredPositionは親の中心を(0,0)とするため、その点を考慮します。

        // X軸のクランプ範囲
        // 例: マップ幅2048、画面幅1920、スケール1.0の場合
        // scaledMapWidth = 2048, screenWidth = 1920
        // maxPossibleX = (2048 - 1920) / 2 = 64 (マップ左端が画面左端に来る最大のX値)
        // minPossibleX = -(2048 - 1920) / 2 = -64 (マップ右端が画面右端に来る最小のX値)
        float maxPossibleX = (scaledMapWidth - screenWidth) / 2f;
        float minPossibleX = -maxPossibleX;

        // Y軸のクランプ範囲
        float maxPossibleY = (scaledMapHeight - screenHeight) / 2f;
        float minPossibleY = -maxPossibleY;

        // マップが画面サイズよりも小さい場合の調整
        // マップが画面に収まる場合は、中央に固定します。
        if (scaledMapWidth < screenWidth)
        {
            minPossibleX = 0; // 中央固定
            maxPossibleX = 0; // 中央固定
        }
        if (scaledMapHeight < screenHeight)
        {
            minPossibleY = 0; // 中央固定
            maxPossibleY = 0; // 中央固定
        }

        // 現在のマップの位置を取得します。
        Vector2 currentAnchoredPos = worldMapRawImageRectTransform.anchoredPosition;

        // XとYのそれぞれを計算された範囲にクランプします。
        float clampedX = Mathf.Clamp(currentAnchoredPos.x, minPossibleX, maxPossibleX);
        float clampedY = Mathf.Clamp(currentAnchoredPos.y, minPossibleY, maxPossibleY);

        // クランプされた位置をマップのanchoredPositionに設定します。
        worldMapRawImageRectTransform.anchoredPosition = new Vector2(clampedX, clampedY);

        //Debug.Log($"[MapBoundsClamper] マップ位置をクランプしました。現在の位置: {currentAnchoredPos}, クランプ後の位置: {worldMapRawImageRectTransform.anchoredPosition}");
    }
}