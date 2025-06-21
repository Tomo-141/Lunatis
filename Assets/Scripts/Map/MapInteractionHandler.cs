// MapInteractionHandler.cs
// ���̃X�N���v�g�́A�}�b�vUI�ɑ΂��郆�[�U�[�̓��́i�h���b�O�A�m�[�h�N���b�N�j�����o���A
// ����ɉ����������i�}�b�v�̎��_�ړ��A�v���C���[�ړ��v���j���֘A�R���|�[�l���g�ɔ��s���܂��B
// ��Ƀ}�b�v�̃h���b�O����ƃm�[�h�̃N���b�N���o��S�����܂��B
// �v���C���[�A�C�R�����ړ����̊Ԃ́A�}�b�v�̃h���b�O������ꎞ�I�ɖ��������A�듮���h���܂��B

using UnityEngine;
using UnityEngine.EventSystems; // IDragHandler, IBeginDragHandler, IEndDragHandler ���g�p���邽�߂ɕK�v
using UnityEngine.UI; // RectTransform���g�p���邽�߂ɕK�v
using System.Collections; // �R���[�`�����g�p���邽�߂ɕK�v

// MapInteractionHandler�́A�}�b�v�̎��_����i�Z���^�����O�A�h���b�O�j�ƁA
// �m�[�h�N���b�N�̌��o���Ǘ����܂��B
// IBeginDragHandler, IDragHandler, IEndDragHandler���������邱�ƂŁAUI�C�x���g�V�X�e����ʂ��ăh���b�O��������o���܂��B
// �X�N���v�g�̓ǂݍ��ݏ����Ƃ��ẮAMapPathManager��PlayerMapMovementController����Ɏ��s����邱�Ƃ�z�肵�Ă��܂��B
public class MapInteractionHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private RectTransform worldMapRawImageRectTransform; // ���[���h�}�b�v��RawImage�i�n�}�摜�j��RectTransform
    [SerializeField] private MapBoundsClamper mapBoundsClamper;         // �}�b�v����ʊO�ɏo�Ȃ��悤�ɋ��E�ɃN�����v���邽�߂̃X�N���v�g�ւ̎Q��
    [SerializeField] private PlayerMapMovementController playerMovementController; // �v���C���[�A�C�R���̈ړ���Ԃ�ʒu�����擾���邽�߂̃X�N���v�g�ւ̎Q��

    [Header("Drag Settings")]
    [SerializeField] private float dragSpeed = 1.0f; // �}�b�v���h���b�O����ۂ̑��x�W��

    [Header("Centering Settings")]
    [Tooltip("�v���C���[�A�C�R���Ɏ��_���Z���^�����O����ۂ̑��x�B")]
    [SerializeField] private float centerSpeed = 5.0f; // �Z���^�����O�A�j���[�V�����̑��x
    [Tooltip("�Z���^�����O�A�j���[�V�����̍ő�p�����ԁi�b�j�B������߂���Ƌ����I���B")]
    [SerializeField] private float maxCenteringDuration = 1.0f; // �Z���^�����O�̍ő�p������ (1�b)
    [Tooltip("�Z���^�����O�I���Ɣ��肷�鋗����臒l�B���̒l�ȉ��ɂȂ�΃Z���^�����O���I�����܂��B")]
    [SerializeField] private float centeringThreshold = 0.5f; // �Z���^�����O�̌덷���e�͈�

    private Vector2 lastMousePosition; // �h���b�O�J�n���̃}�E�X�ʒu�A�܂��͑O�t���[���̃}�E�X�ʒu��ێ�
    private bool isDragging = false;   // ���݃h���b�O���쒆���ǂ����������t���O
    private Coroutine centeringCoroutine; // �Z���^�����O�A�j���[�V���������s���̃R���[�`���ւ̎Q�Ƃ�ێ��i�d�����s�h�~�̂��߁j

    /// <summary>
    /// �X�N���v�g���L���ɂȂ����Ƃ��Ɉ�x�����Ăяo����܂��B
    /// ������MapNode���N���b�N���ꂽ�ۂɌĂяo�����C�x���g�iOnNodeClicked�j�̍w�ǂ��J�n���܂��B
    /// ����ɂ��A�}�b�v��̃m�[�h���N���b�N���ꂽ�ۂ�HandleNodeClick���\�b�h�����s����܂��B
    /// </summary>
    void OnEnable()
    {
        // MapNode.OnNodeClicked�C�x���g��HandleNodeClick���\�b�h��o�^���܂��B
        MapNode.OnNodeClicked += HandleNodeClick;
        Debug.Log("[MapInteractionHandler] MapNode.OnNodeClicked �C�x���g�̍w�ǂ��J�n���܂����B");
    }

    /// <summary>
    /// �X�N���v�g�������ɂȂ����Ƃ��Ɉ�x�����Ăяo����܂��B
    /// ����̓��������[�N��h�����߂ɔ��ɏd�v�ł��B
    /// �����Ńm�[�h�N���b�N�C�x���g�̍w�ǂ��������܂��B
    /// </summary>
    void OnDisable()
    {
        // MapNode.OnNodeClicked�C�x���g����HandleNodeClick���\�b�h�̓o�^���������܂��B
        MapNode.OnNodeClicked -= HandleNodeClick;
        Debug.Log("[MapInteractionHandler] MapNode.OnNodeClicked �C�x���g�̍w�ǂ��������܂����B");

        // �X�N���v�g�������ɂȂ�ۂɁA�����Z���^�����O�R���[�`�������s���ł���Β�~���܂��B
        if (centeringCoroutine != null)
        {
            StopCoroutine(centeringCoroutine);
            centeringCoroutine = null;
            Debug.Log("[MapInteractionHandler] OnDisable���A���s���̃Z���^�����O�R���[�`�����~���܂����B");
        }
    }

    /// <summary>
    /// Start�̓X�N���v�g�����[�h����A�ŏ��̃t���[���X�V�̑O�Ɉ�x�����Ăяo����܂��B
    /// �K�v�ȃR���|�[�l���g�̎Q�Ƃ�Inspector�Őݒ肳��Ă��邩���m�F���A
    /// �v���W�F�N�g�J�n���Ƀv���C���[�A�C�R���֎��_�𑦎��Z���^�����O���܂��B
    /// </summary>
    void Start()
    {
        // �Y�[���Ώۂ̃��[���h�}�b�vRectTransform��Inspector�ŃA�T�C������Ă��邩���m�F���܂��B
        if (worldMapRawImageRectTransform == null)
        {
            Debug.LogError("[MapInteractionHandler] World Map Raw Image Rect Transform���A�T�C������Ă��܂���IInspector�Őݒ肵�Ă��������B", this);
            enabled = false; // �A�T�C������Ă��Ȃ��ꍇ�̓X�N���v�g�𖳌������A����ȏ㏈�����i�܂Ȃ��悤�ɂ��܂��B
            return;
        }
        // �}�b�v���E�N�����v�p�X�N���v�g�ւ̎Q�Ƃ�Inspector�ŃA�T�C������Ă��邩���m�F���܂��B
        if (mapBoundsClamper == null)
        {
            Debug.LogError("[MapInteractionHandler] Map Bounds Clamper���A�T�C������Ă��܂���IInspector�Őݒ肵�Ă��������B", this);
            enabled = false; // �A�T�C������Ă��Ȃ��ꍇ�̓X�N���v�g�𖳌������A����ȏ㏈�����i�܂Ȃ��悤�ɂ��܂��B
            return;
        }
        // �v���C���[�A�C�R���ړ�����X�N���v�g�ւ̎Q�Ƃ�Inspector�ŃA�T�C������Ă��邩���m�F���܂��B
        // ���ꂪ�Ȃ��ƁA�v���C���[�ړ����̃h���b�O��������Z���^�����O�@�\�����삵�܂���B
        if (playerMovementController == null)
        {
            Debug.LogError("[MapInteractionHandler] Player Map Movement Controller���A�T�C������Ă��܂���IInspector�Őݒ肵�Ă��������B", this);
            enabled = false; // �A�T�C������Ă��Ȃ��ꍇ�̓X�N���v�g�𖳌������A����ȏ㏈�����i�܂Ȃ��悤�ɂ��܂��B
            return;
        }

        // �����\���̓v���C���[�A�C�R���ɑ����i�A�j���[�V�����Ȃ��Łj���_���Z���^�����O���܂��B
        CenterViewOnPlayerIcon(instant: true);

        Debug.Log("[MapInteractionHandler] �X�N���v�g���J�n����܂����B");
    }

    /// <summary>
    /// ���t���[���Ăяo�����Update���\�b�h�ł��B
    /// ���̃��\�b�h�ł́A��Ƀv���C���[�A�C�R�����ړ����̏ꍇ�Ƀ}�b�v����𖳌������鏈�����s���܂��B
    /// </summary>
    void Update()
    {
        // �v���C���[�A�C�R�������݈ړ����ł��邩�ǂ�����PlayerMapMovementController����擾���܂��B
        // ����playerMovementController��null�łȂ��A���ړ����ł���΁A
        // �}�b�v�̃h���b�O��Y�[���Ȃǂ̃��[�U�[������ꎞ�I�ɖ��������܂��B
        // ����ɂ��A�ړ��A�j���[�V�������̃}�b�v����ɂ��\�����ʋ�����h���܂��B
        if (playerMovementController != null && playerMovementController.IsPlayerMoving())
        {
            // �f�o�b�O���O�͕K�v�ɉ����ăR�����g�A�E�g���Ă��������B
            // Debug.Log("[MapInteractionHandler] �v���C���[�ړ����̂��߁A�}�b�v����𖳌������Ă��܂��B");

            // �����ړ����Ɍ���ăh���b�O��ԂɂȂ��Ă��܂��Ă�����A���̏�Ԃ��������܂��B
            if (isDragging)
            {
                isDragging = false;
                Debug.Log("[MapInteractionHandler] �v���C���[�ړ����Ƀh���b�O��Ԃ����o���ꂽ���߁A�������܂����B");
            }
            return; // �v���C���[�ړ����͂���ȍ~�̃}�b�v���쏈���i�h���b�O�A�Y�[���Ȃǁj�����s���܂���B
        }
    }

    /// <summary>
    /// MapNode���N���b�N���ꂽ�Ƃ���MapNode�X�N���v�g����Ăяo�����C�x���g�n���h���ł��B
    /// �N���b�N���ꂽ�m�[�h�փv���C���[�A�C�R�����ړ������鏈����PlayerMapMovementController�ɗv�����܂��B
    /// </summary>
    /// <param name="clickedNodeId">�N���b�N���ꂽ�m�[�h��ID�B</param>
    private void HandleNodeClick(string clickedNodeId)
    {
        Debug.Log($"[MapInteractionHandler] �m�[�h�N���b�N�C�x���g����M���܂���: {clickedNodeId}");

        // �v���C���[�A�C�R�����ړ����̏ꍇ�A�܂��͌��݃h���b�O���쒆�̏ꍇ�́A�m�[�h�N���b�N�𖳎����܂��B
        // ����ɂ��A�ړ�����}�b�v���쒆�̈Ӑ}���Ȃ��v���C���[�ړ���h���܂��B
        // playerMovementController��null�łȂ����Ƃ��m�F���Ă���IsPlayerMoving()���Ăяo���܂��B
        if (playerMovementController != null && playerMovementController.IsPlayerMoving() || isDragging)
        {
            Debug.Log($"[MapInteractionHandler] �v���C���[�A�C�R�����ړ����A�܂��̓h���b�O���̂��߁A�N���b�N�𖳎����܂���.", this);
            return;
        }

        // PlayerMapMovementController�ɁA�N���b�N���ꂽ�m�[�h�ւ̃v���C���[�ړ������݂�悤�v�����܂��B
        playerMovementController.TryMovePlayerToNode(clickedNodeId);
    }

    /// <summary>
    /// �}�b�v�̎��_���v���C���[�A�C�R���̈ʒu�ɃZ���^�����O���܂��B
    /// �}�b�v��RectTransform��anchoredPosition�𒲐����邱�ƂŁA�v���C���[�A�C�R������ʂ̒����ɗ���悤�ɂ��܂��B
    /// ���̃��\�b�h��PlayerMapMovementController������Ăяo����܂��B
    /// </summary>
    /// <param name="instant">true�̏ꍇ�A�A�j���[�V�����Ȃ��ő����ɃZ���^�����O���܂��Bfalse�̏ꍇ�A���炩�ȃA�j���[�V�����ŃZ���^�����O���܂��B</param>
    public Coroutine CenterViewOnPlayerIcon(bool instant = false)
    {
        // ���ɃZ���^�����O�R���[�`�������s���ł���΁A���̃R���[�`�����~���܂��B
        // ����ɂ��A�V�����Z���^�����O�v���������Ƃ��ɁA�Â��R���[�`�����c�葱���邱�Ƃ�h���܂��B
        if (centeringCoroutine != null)
        {
            StopCoroutine(centeringCoroutine);
            Debug.Log("[MapInteractionHandler] �����̃Z���^�����O�R���[�`�����~���܂����B");
            centeringCoroutine = null; // ��~������Q�Ƃ�K���N���A���܂��B
        }

        // �v���C���[�A�C�R����RectTransform��PlayerMapMovementController����擾���܂��B
        // �Z���^�����O�̖ڕW�ʒu���v�Z���邽�߂ɕK�v�ł��B
        // playerMovementController��null�łȂ����Ƃ��m�F���܂��B
        if (playerMovementController == null)
        {
            Debug.LogError("[MapInteractionHandler] Player Map Movement Controller���A�T�C������Ă��Ȃ����߁A�Z���^�����O�ł��܂���B", this);
            return null;
        }

        RectTransform playerIconRectTransform = playerMovementController.playerIconRectTransform;

        // �v���C���[�A�C�R����RectTransform���擾�ł��Ȃ������ꍇ�̓G���[���O���o�͂��A�����𒆒f���܂��B
        if (playerIconRectTransform == null)
        {
            Debug.LogError("[MapInteractionHandler] �v���C���[�A�C�R����Rect Transform��PlayerMapMovementController����擾�ł��܂���ł����B�Z���^�����O�𒆒f���܂��B", this);
            return null;
        }

        // �v���C���[�A�C�R���̌��݂̈ʒu�i���[�J�����W�j���擾���܂��B
        Vector2 playerPosInMap = playerIconRectTransform.anchoredPosition;
        // ���݂̃}�b�v�̃X�P�[���i�Y�[���{���j���擾���܂��B
        float currentScale = worldMapRawImageRectTransform.localScale.x;
        // �}�b�v�摜���Z���^�����O���邽�߂̖ڕWanchoredPosition���v�Z���܂��B
        // �v���C���[�A�C�R���̈ʒu�𔽓]�����A���݂̃X�P�[����K�p���邱�ƂŁA�}�b�v���ړ������ۂɃA�C�R���������ɗ���悤�ɂ��܂��B
        Vector2 targetMapPosition = -playerPosInMap * currentScale;

        // �����Z���^�����O���[�h�̏ꍇ
        if (instant)
        {
            // �}�b�v��anchoredPosition���v�Z���ꂽ�ڕW�ʒu�ɒ��ڐݒ肵�܂��B
            worldMapRawImageRectTransform.anchoredPosition = targetMapPosition;
            Debug.Log($"[MapInteractionHandler] �v���C���[�A�C�R���Ɏ��_�𑦎��Z���^�����O���܂����B�v���C���[�A�C�R���̈ʒu: {playerPosInMap}, �}�b�v�摜��anchoredPosition: {worldMapRawImageRectTransform.anchoredPosition}");
            // �ʒu��ݒ肵����A�}�b�v�����E�O�ɏo�Ȃ��悤�ɃN�����v���܂��B
            mapBoundsClamper.ClampMapPosition();
            return null; // �R���[�`���ł͂Ȃ�����null��Ԃ��܂��B
        }
        // �A�j���[�V�����Z���^�����O���[�h�̏ꍇ
        else
        {
            // CenterViewAnimatedCoroutine�R���[�`�����J�n���A���̎Q�Ƃ�ێ����܂��B
            // ����ɂ��A�R���[�`�������s���ł��邱�Ƃ��Ǘ��ł��܂��B
            centeringCoroutine = StartCoroutine(CenterViewAnimatedCoroutine(targetMapPosition));
            Debug.Log("[MapInteractionHandler] �V�����Z���^�����O�R���[�`�����J�n���܂����B");
            return centeringCoroutine; // �J�n�����R���[�`���̎Q�Ƃ�Ԃ��܂��B
        }
    }

    /// <summary>
    /// �}�b�v�̎��_��ڕW�ʒu�֊��炩�ȃA�j���[�V�����ňړ�������R���[�`���ł��B
    /// Lerp�֐����g�p���āA���݂̈ʒu����ڕW�ʒu�֏��X�ɋ߂Â��܂��B
    /// </summary>
    /// <param name="targetPosition">�}�b�v�̍ŏI�I��anchoredPosition�i�ڕW�ʒu�j�B</param>
    private IEnumerator CenterViewAnimatedCoroutine(Vector2 targetPosition)
    {
        Debug.Log($"[MapInteractionHandler] CenterViewAnimatedCoroutine �R���[�`���J�n�B�ڕW�ʒu: {targetPosition}");
        float elapsedTime = 0f;

        // �ڕW�ʒu�ɔ��ɋ߂��i�덷���e�͈͈ȓ��j�Ȃ邩�A�܂��͍ő�p�����Ԃ𒴂���܂Ń��[�v�𑱂��܂��B
        while (Vector2.Distance(worldMapRawImageRectTransform.anchoredPosition, targetPosition) > centeringThreshold &&
               elapsedTime < maxCenteringDuration)
        {
            // Vector2.Lerp����Vector2.MoveTowards�ɕύX
            worldMapRawImageRectTransform.anchoredPosition = Vector2.MoveTowards(
                worldMapRawImageRectTransform.anchoredPosition,
                targetPosition,
                Time.deltaTime * centerSpeed * 20f // Lerp�ɔ�ׂđ��x�������K�v�ɂȂ�ꍇ�����邽�߁A�����l��20�{�ɒ���
            );

            // �N�����v������L����
            mapBoundsClamper.ClampMapPosition();

            elapsedTime += Time.deltaTime;

            // �Z���^�����O���̏ڍ׃��O (����m�F��ɃR�����g�A�E�g���Ă�������)
            Debug.Log($"[MapInteractionHandler DEBUG] ���݈ʒu: {worldMapRawImageRectTransform.anchoredPosition}, �ڕW: {targetPosition}, ����: {Vector2.Distance(worldMapRawImageRectTransform.anchoredPosition, targetPosition):F4}, �o��: {elapsedTime:F2}s, MaxDuration: {maxCenteringDuration:F2}s");

            yield return null;
        }

        // ���[�v�I����A�}�b�v�̍ŏI�ʒu��ڕW�ʒu�ɐ��m�ɐݒ肵�܂��B
        worldMapRawImageRectTransform.anchoredPosition = targetPosition;
        // �ŏI�ʒu�ݒ����A�}�b�v�����E�O�ɏo�Ȃ��悤�ɍēx�N�����v���܂��B
        mapBoundsClamper.ClampMapPosition();

        // �R���[�`�����ǂ̂悤�ɏI�������������O�Ŋm�F
        if (Vector2.Distance(worldMapRawImageRectTransform.anchoredPosition, targetPosition) <= centeringThreshold)
        {
            Debug.Log("[MapInteractionHandler] CenterViewAnimatedCoroutine �R���[�`�����ڕW�����ɓ��B���Ċ������܂����B");
        }
        else
        {
            Debug.LogWarning($"[MapInteractionHandler] CenterViewAnimatedCoroutine �R���[�`�����ő�p������({maxCenteringDuration}�b)�𒴂��ċ����I�����܂����B�ڕW�����ɖ����B�B�ŏI����: {Vector2.Distance(worldMapRawImageRectTransform.anchoredPosition, targetPosition):F4}");
        }

        centeringCoroutine = null; // �R���[�`�����I�������̂ŁA�Q�Ƃ�null�ɃN���A���܂��B
    }

    /// <summary>
    /// �}�b�v�̃h���b�O���삪�J�n���ꂽ�Ƃ��ɌĂяo����܂��B
    /// UI�C�x���g�V�X�e���iIBeginDragHandler�j�ɂ���Č��o����܂��B
    /// </summary>
    /// <param name="eventData">�|�C���^�[�C�x���g�f�[�^�B�}�E�X�̃{�^����ʒu�����܂݂܂��B</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // �v���C���[�A�C�R�������݈ړ����ł���ꍇ�A�h���b�O������J�n�����Ȃ��悤�ɂ��܂��B
        // ����ɂ��A�v���C���[�ړ����ƃ}�b�v�h���b�O����̊���h���܂��B
        // playerMovementController��null�łȂ����Ƃ��m�F���Ă���IsPlayerMoving()���Ăяo���܂��B
        if (playerMovementController != null && playerMovementController.IsPlayerMoving())
        {
            Debug.Log("[MapInteractionHandler] �v���C���[�ړ����̂��߁A�h���b�O�J�n�𖳌������܂����B", this);
            isDragging = false; // �O�̂��߁A�h���b�O���t���O���m����false�ɂ��܂��B
            return;
        }

        // ���N���b�N�iPrimary Button�j�ł̃h���b�O�݂̂����o���܂��B
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = true; // �h���b�O���t���O��true�ɐݒ肵�܂��B
            lastMousePosition = eventData.position; // ���݂̃}�E�X�ʒu���L�^���܂��B
            Debug.Log("[MapInteractionHandler] �h���b�O�J�n�B");
        }
    }

    /// <summary>
    /// �h���b�O���ɖ��t���[���Ăяo����܂��B
    /// UI�C�x���g�V�X�e���iIDragHandler�j�ɂ���Č��o����܂��B
    /// �}�E�X�̈ړ��ʂɉ����ă}�b�v��anchoredPosition�𒲐����A�}�b�v���ړ������܂��B
    /// </summary>
    /// <param name="eventData">�|�C���^�[�C�x���g�f�[�^�B�}�E�X�̌��݈ʒu���܂݂܂��B</param>
    public void OnDrag(PointerEventData eventData)
    {
        // isDragging�t���O��true�̏ꍇ�i�܂�A�h���b�O���삪�J�n����Ă���ꍇ�j�̂ݏ��������s���܂��B
        if (isDragging)
        {
            // ���݂̃}�E�X�ʒu���擾���܂��B
            Vector2 currentMousePosition = eventData.position;
            // �O�̃t���[������̃}�E�X�̈ړ��ʁi�f���^�j���v�Z���܂��B
            Vector2 delta = currentMousePosition - lastMousePosition;

            // �}�b�v��anchoredPosition�Ƀ}�E�X�̈ړ��ʂ�K�p���A�h���b�O���x����Z���܂��B
            worldMapRawImageRectTransform.anchoredPosition += delta * dragSpeed;

            // ���݂̃}�E�X�ʒu�����t���[���̌v�Z�̂��߂ɕۑ����܂��B
            lastMousePosition = currentMousePosition;

            // �}�b�v�����E�O�ɏo�Ȃ��悤�ɁA���t���[���ʒu���N�����v����悤MapBoundsClamper�ɗv�����܂��B
            mapBoundsClamper.ClampMapPosition();
        }
    }

    /// <summary>
    /// �h���b�O���삪�I�������Ƃ��ɌĂяo����܂��B
    /// UI�C�x���g�V�X�e���iIEndDragHandler�j�ɂ���Č��o����܂��B
    /// </summary>
    /// <param name="eventData">�|�C���^�[�C�x���g�f�[�^�B�ǂ̃{�^���������ꂽ�����܂݂܂��B</param>
    public void OnEndDrag(PointerEventData eventData)
    {
        // ���N���b�N�iPrimary Button�j�ł̃h���b�O���I�������ꍇ�̂ݏ��������s���܂��B
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = false; // �h���b�O���t���O��false�ɐݒ肵�܂��B
            Debug.Log("[MapInteractionHandler] �h���b�O�I���B");
        }
    }
}