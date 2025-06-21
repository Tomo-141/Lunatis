// MapBoundsClamper.cs
// ���̃X�N���v�g�́A���[���h�}�b�v��RectTransform��anchoredPosition�𐧌����A
// �}�b�v����ʂ̕\���͈͂𒴂��Ĉړ����Ȃ��悤�ɃN�����v����@�\��񋟂��܂��B
// �Y�[���X�P�[���Ɖ�ʉ𑜓x���l�����āA���I�ɔ͈͂��v�Z���܂��B

using UnityEngine;
using UnityEngine.UI; // RectTransform���g�p���邽�߂ɕK�v

public class MapBoundsClamper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform worldMapRawImageRectTransform; // �N�����v�Ώۂ̃��[���h�}�b�vRawImage��RectTransform

    // �}�b�v�̃I���W�i���T�C�Y (�v���Ɋ�Â�2048x2048�Ɖ���)
    private const float MAP_ORIGINAL_WIDTH = 2048f;
    private const float MAP_ORIGINAL_HEIGHT = 2048f;

    /// <summary>
    /// Start�͍ŏ��̃t���[���X�V�̑O�ɌĂяo����܂��B
    /// �K�v�ȎQ�Ƃ̊m�F�ƁA�����ʒu�̃N�����v���s���܂��B
    /// </summary>
    void Start()
    {
        // �N�����v�Ώۂ�RectTransform���A�T�C������Ă��邩�m�F
        if (worldMapRawImageRectTransform == null)
        {
            Debug.LogError("[MapBoundsClamper] World Map Raw Image Rect Transform���A�T�C������Ă��܂���IInspector�Őݒ肵�Ă��������B", this);
            enabled = false; // �X�N���v�g�𖳌��ɂ���
            return;
        }

        // �������[�h���Ƀ}�b�v�ʒu���N�����v���A�}�b�v���͈͓��Ɏ��܂�悤�ɂ��܂��B
        ClampMapPosition();

        Debug.Log("[MapBoundsClamper] �X�N���v�g���J�n����܂����B");
    }

    /// <summary>
    /// Update�͖��t���[���Ăяo����܂��B
    /// �h���b�O�ȂǂŃ}�b�v�̈ʒu���ύX�����\��������̂ŁA��Ɉʒu���N�����v���܂��B
    /// </summary>
    void Update()
    {
        // ���t���[���A�}�b�v�̈ʒu���͈͓��Ɏ��܂��Ă��邩�m�F���A�K�v�ł���ΏC�����܂��B
        // ����ɂ��A�h���b�O�ゾ���łȂ��A�Y�[���Ȃǂňʒu���͈͊O�ɏo���ꍇ�������I�ɏC������܂��B
        ClampMapPosition();
    }

    /// <summary>
    /// �}�b�v��RectTransform��anchoredPosition���A�}�b�v�̕\���͈͓��ɐ������܂��B
    /// ���݂̃Y�[���X�P�[���Ɖ�ʃT�C�Y���l�����܂��B
    /// </summary>
    public void ClampMapPosition()
    {
        // ���݂̃}�b�v�̃X�P�[�����擾���܂��B
        // MapZoomController�ɂ����localScale���ύX����Ă��邱�Ƃ�z�肵�܂��B
        float currentScale = worldMapRawImageRectTransform.localScale.x;

        // �e��RectTransform�iWorldMapPanel�j�̃T�C�Y���擾���܂��B���ꂪ��ʂ̕\���͈͂ɑ������܂��B
        // MapBoundsClamper��WorldMapPanel�ɃA�^�b�`����邽�߁A���g��RectTransform�̐e���l�����܂��B
        RectTransform parentRect = worldMapRawImageRectTransform.parent.GetComponent<RectTransform>();
        if (parentRect == null)
        {
            Debug.LogError("[MapBoundsClamper] �e��RectTransform��������܂���B�}�b�v�̈ʒu�������ł��܂���B", this);
            return;
        }

        float screenWidth = parentRect.rect.width;
        float screenHeight = parentRect.rect.height;

        // �X�P�[����̃}�b�v�̎��ۂ̃s�N�Z���T�C�Y���v�Z���܂��B
        float scaledMapWidth = MAP_ORIGINAL_WIDTH * currentScale;
        float scaledMapHeight = MAP_ORIGINAL_HEIGHT * currentScale;

        // �}�b�v��anchoredPosition����蓾��ŏ��l�ƍő�l���v�Z���܂��B
        // �����̒l�́A�}�b�v�̒[����ʂ̒[�ɂ҂����荇���Ƃ���anchoredPosition�ł��B
        // Unity UI��anchoredPosition�͐e�̒��S��(0,0)�Ƃ��邽�߁A���̓_���l�����܂��B

        // X���̃N�����v�͈�
        // ��: �}�b�v��2048�A��ʕ�1920�A�X�P�[��1.0�̏ꍇ
        // scaledMapWidth = 2048, screenWidth = 1920
        // maxPossibleX = (2048 - 1920) / 2 = 64 (�}�b�v���[����ʍ��[�ɗ���ő��X�l)
        // minPossibleX = -(2048 - 1920) / 2 = -64 (�}�b�v�E�[����ʉE�[�ɗ���ŏ���X�l)
        float maxPossibleX = (scaledMapWidth - screenWidth) / 2f;
        float minPossibleX = -maxPossibleX;

        // Y���̃N�����v�͈�
        float maxPossibleY = (scaledMapHeight - screenHeight) / 2f;
        float minPossibleY = -maxPossibleY;

        // �}�b�v����ʃT�C�Y�����������ꍇ�̒���
        // �}�b�v����ʂɎ��܂�ꍇ�́A�����ɌŒ肵�܂��B
        if (scaledMapWidth < screenWidth)
        {
            minPossibleX = 0; // �����Œ�
            maxPossibleX = 0; // �����Œ�
        }
        if (scaledMapHeight < screenHeight)
        {
            minPossibleY = 0; // �����Œ�
            maxPossibleY = 0; // �����Œ�
        }

        // ���݂̃}�b�v�̈ʒu���擾���܂��B
        Vector2 currentAnchoredPos = worldMapRawImageRectTransform.anchoredPosition;

        // X��Y�̂��ꂼ����v�Z���ꂽ�͈͂ɃN�����v���܂��B
        float clampedX = Mathf.Clamp(currentAnchoredPos.x, minPossibleX, maxPossibleX);
        float clampedY = Mathf.Clamp(currentAnchoredPos.y, minPossibleY, maxPossibleY);

        // �N�����v���ꂽ�ʒu���}�b�v��anchoredPosition�ɐݒ肵�܂��B
        worldMapRawImageRectTransform.anchoredPosition = new Vector2(clampedX, clampedY);

        //Debug.Log($"[MapBoundsClamper] �}�b�v�ʒu���N�����v���܂����B���݂̈ʒu: {currentAnchoredPos}, �N�����v��̈ʒu: {worldMapRawImageRectTransform.anchoredPosition}");
    }
}