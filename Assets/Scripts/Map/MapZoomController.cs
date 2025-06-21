// MapZoomController.cs
// ���̃X�N���v�g�́A�}�b�vUI�̃}�E�X�z�C�[���ɂ��Y�[���C���E�Y�[���A�E�g���Ǘ����܂��B
// �Y�[���Ώۂ�RectTransform��localScale�𑀍삵�A�Y�[���͈͂𐧌����܂��B
// �v���C���[�A�C�R�����ړ����̏ꍇ�́A�Y�[��������ꎞ�I�ɖ��������A����h���܂��B
// ���̃X�N���v�g�͎�Ɂu�}�b�v�̃Y�[������v�Ƃ����P��̋@�\�ɐӔC�������܂��B

using UnityEngine;
using UnityEngine.UI; // RectTransform���g�p���邽�߂ɕK�v

public class MapZoomController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform worldMapRawImageRectTransform; // �Y�[���Ώۂ̃��[���h�}�b�vRawImage��RectTransform
    [SerializeField] private PlayerMapMovementController playerMapMovementController; // �v���C���[�̈ړ���Ԃ��m�F���邽�߂�PlayerMapMovementController�ւ̎Q��
    [SerializeField] private MapBoundsClamper mapBoundsClamper; // �}�b�v�̃Y�[����ɋ��E�ɃN�����v���邽�߂�MapBoundsClamper�ւ̎Q��

    [Header("Zoom Settings")]
    [SerializeField] private float initialZoomScale = 1.0f; // �}�b�v�̏����Y�[���X�P�[�� (��: 1.0f�͌�����)
    [SerializeField] private float minZoomScale = 0.5f;     // �}�b�v�̍ŏ��Y�[���X�P�[���i����ȏ�k�����Ȃ������l�j
    [SerializeField] private float maxZoomScale = 5.0f;     // �}�b�v�̍ő�Y�[���X�P�[���i����ȏ�g�債�Ȃ�����l�j
    [SerializeField] private float zoomSpeed = 0.1f;        // �}�E�X�z�C�[��1�񂠂���̃Y�[���ω���

    private float currentZoomScale; // ���ݓK�p����Ă���}�b�v�̃Y�[���X�P�[��

    /// <summary>
    /// Start�͍ŏ��̃t���[���X�V�̑O�ɌĂяo����܂��B
    /// �K�v�ȎQ�Ƃ̊m�F�ƁA�����Y�[���X�P�[���̓K�p���s���܂��B
    /// </summary>
    void Start()
    {
        // �Y�[���Ώۂ�RectTransform���A�T�C������Ă��邩�m�F
        if (worldMapRawImageRectTransform == null)
        {
            Debug.LogError("[MapZoomController] World Map Raw Image Rect Transform���A�T�C������Ă��܂���IInspector�Őݒ肵�Ă��������B", this);
            enabled = false; // �X�N���v�g�𖳌��ɂ���
            return;
        }
        // �v���C���[�ړ��R���g���[���[�̎Q�Ƃ��A�T�C������Ă��邩�m�F
        if (playerMapMovementController == null)
        {
            Debug.LogError("[MapZoomController] Player Map Movement Controller���A�T�C������Ă��܂���IInspector�Őݒ肵�Ă��������B", this);
            enabled = false;
            return;
        }
        // MapBoundsClamper�̎Q�Ƃ��A�T�C������Ă��邩�m�F
        if (mapBoundsClamper == null)
        {
            Debug.LogError("[MapZoomController] Map Bounds Clamper���A�T�C������Ă��܂���IInspector�Őݒ肵�Ă��������B", this);
            enabled = false;
            return;
        }

        // �����Y�[���X�P�[����K�p���܂��B
        currentZoomScale = initialZoomScale;
        worldMapRawImageRectTransform.localScale = new Vector3(currentZoomScale, currentZoomScale, 1f);

        // �������[�h���Ƀ}�b�v�ʒu�����E���ɃN�����v���܂��B
        mapBoundsClamper.ClampMapPosition();

        Debug.Log("[MapZoomController] �X�N���v�g���J�n����܂����B�����Y�[���X�P�[��: " + currentZoomScale);
    }

    /// <summary>
    /// ���t���[���Ăяo�����Update���\�b�h�ł��B
    /// �v���C���[���ړ����łȂ�����A�}�E�X�z�C�[���ɂ��Y�[����������o���A�������܂��B
    /// </summary>
    void Update()
    {
        // �v���C���[�A�C�R�������݈ړ����ł���ꍇ�A�Y�[������𖳌������܂��B
        // ����ɂ��A�v���C���[�ړ����ƃY�[������̊���h���܂��B
        if (playerMapMovementController != null && playerMapMovementController.IsPlayerMoving())
        {
            // �f�o�b�O���O�͕K�v�ɉ����ăR�����g�A�E�g���Ă��������B
            // Debug.Log("[MapZoomController] �v���C���[�ړ����̂��߁A�Y�[������𖳌������Ă��܂��B");
            return; // �v���C���[�ړ����͂���ȍ~�̃Y�[�����������s���܂���B
        }

        // �}�E�X�z�C�[���̓��͂����o���A�Y�[�����������s���܂��B
        HandleZoom();

        // ���������� mapBoundsClamper.ClampMapPosition(); �̌Ăяo�����폜���܂����B
        // ��ClampMapPosition() �� HandleZoom() ���i�X�P�[���ύX���j�ƁA
        // ��MapInteractionHandler �̃h���b�O/�Z���^�����O���ɌĂ΂��Ώ\���ł��B
    }

    /// <summary>
    /// �}�E�X�z�C�[���̓��͂Ɋ�Â��ă}�b�v�̃Y�[���X�P�[���𒲐����܂��B
    /// currentZoomScale���X�V���AworldMapRawImageRectTransform��localScale�ɓK�p���܂��B
    /// </summary>
    private void HandleZoom()
    {
        // �}�E�X�z�C�[���̃X�N���[���ʂ��擾���܂��B�i������X�N���[���Ő��̒l�A�������ŕ��̒l�j
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // �X�N���[���ʂ�����ꍇ�i�����ȃm�C�Y�����O���邽�ߐ�Βl�Ń`�F�b�N�j�̂ݏ��������s���܂��B
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // �V�����Y�[���X�P�[�����v�Z���܂��B
            // �X�N���[���ʂɃY�[�����x����Z���A���݂̃Y�[���X�P�[���ɉ��Z���܂��B
            float newZoomScale = currentZoomScale + scroll * zoomSpeed;

            // �v�Z���ꂽ�Y�[���X�P�[�����A���O�ɐݒ肳�ꂽ�ŏ��l�ƍő�l�͈͓̔��ɃN�����v�i�����j���܂��B
            newZoomScale = Mathf.Clamp(newZoomScale, minZoomScale, maxZoomScale);

            // �v�Z���ꂽ�V�����Y�[���X�P�[�������݂̃X�P�[���ƈقȂ�ꍇ�̂݁A�X�V��K�p���܂��B
            if (newZoomScale != currentZoomScale)
            {
                currentZoomScale = newZoomScale; // ���݂̃Y�[���X�P�[�����X�V

                // WorldMapRawImage��localScale���X�V���A�}�b�v�S�̂��Y�[�����܂��B
                // z����UI�ł͒ʏ�1�ɐݒ肵�܂��B
                worldMapRawImageRectTransform.localScale = new Vector3(currentZoomScale, currentZoomScale, 1f);

                Debug.Log($"[MapZoomController] �Y�[���X�P�[���ύX: {currentZoomScale}");

                // �Y�[���X�P�[�����ύX���ꂽ��A�}�b�v�����E�O�ɏo�Ȃ��悤�ɃN�����v���܂��B
                // �Y�[���ɂ���ă}�b�v�̕\���͈͂��ς�邽�߁A���̃^�C�~���O�ŃN�����v���K�v�ł��B
                if (mapBoundsClamper != null)
                {
                    mapBoundsClamper.ClampMapPosition();
                }
            }
        }
    }
}