// PlayerMapMovementController.cs
// ���̃X�N���v�g�́A�}�b�v��̃v���C���[�A�C�R���̈ʒu�Ǘ��ƁA
// �m�[�h�Ԃ̃A�j���[�V�����ړ��i���x�x�[�X�j�𐧌䂵�܂��B
// MapInteractionHandler����̈ړ��v�����󂯎��AMapPathManager��PlayerLocationSetter�ƘA�g���܂��B

using UnityEngine;
using UnityEngine.UI; // RectTransform���g�p���邽�߂ɕK�v
using System.Collections; // �R���[�`�����g�p���邽�߂ɕK�v
using System.Linq; // Contains() ���g�p���邽�߂ɕK�v

public class PlayerMapMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public RectTransform playerIconRectTransform;     // �v���C���[�A�C�R����RectTransform (public�ɂ���MapInteractionHandler����Q�Ƃł���悤��)
    [SerializeField] private PlayerLocationSetter playerLocationSetter; // PlayerLocationSetter�ւ̎Q��
    [SerializeField] private MapInteractionHandler mapInteractionHandler; // MapInteractionHandler�ւ̎Q�� (�Z���^�����O���Ăяo������)

    [Header("Player Initial Position")]
    [Tooltip("�v���C���[�A�C�R���̏����ʒu�Ƃ��Đݒ肷��m�[�h��ID�B")]
    [SerializeField] private string initialPlayerNodeId = "TownA"; // ��: �����ʒu��TownA�ɐݒ�

    [Header("Current Player Node")]
    [Tooltip("�v���C���[�A�C�R�������݂���m�[�h��ID�B")]
    public string currentPlayerNodeId; // ���݃v���C���[�A�C�R��������m�[�h��ID

    [Header("Player Movement Settings")]
    [Tooltip("�v���C���[�A�C�R�����m�[�h�Ԃ��ړ����鑬�x�i�P��: �s�N�Z��/�b�j�B")]
    [SerializeField] private float moveSpeed = 300f; // �ړ��A�j���[�V�����̑��x�i�s�N�Z��/�b�j
    [Tooltip("�v���C���[�A�C�R���̈ړ�������A���_���Z���^�����O�����܂ł̒x�����ԁi�b�j�B")]
    [SerializeField] private float centerViewDelay = 0.5f; // �Z���^�����O�܂ł̒x������
    [Tooltip("�v���C���[�A�C�R���̈ړ�������A�����I�ɉ�ʂ��Z���^�����O���邩�ǂ����B")]
    [SerializeField] private bool shouldCenterViewAfterMove = true; // �Z���^�����O�@�\�̃I���I�t

    private bool isMoving = false; // �v���C���[�A�C�R�����ړ������ǂ����������t���O

    /// <summary>
    /// Start�͍ŏ��̃t���[���X�V�̑O�ɌĂяo����܂��B
    /// �K�v�ȎQ�Ƃ̊m�F�ƁA�����ʒu�̐ݒ���s���܂��B
    /// </summary>
    void Start()
    {
        // �K�v�ȎQ�Ƃ��ݒ肳��Ă��邩�m�F
        if (playerIconRectTransform == null)
        {
            Debug.LogError("[PlayerMapMovementController] �v���C���[�A�C�R����Rect Transform���A�T�C������Ă��܂���IInspector�Őݒ肵�Ă��������B", this);
            enabled = false;
            return;
        }
        if (playerLocationSetter == null)
        {
            Debug.LogError("[PlayerMapMovementController] Player Location Setter���A�T�C������Ă��܂���IInspector�Őݒ肵�Ă��������B", this);
            enabled = false;
            return;
        }
        if (MapPathManager.Instance == null)
        {
            Debug.LogError("[PlayerMapMovementController] MapPathManager�̃C���X�^���X��������܂���B�V�[����MapPathManager���A�^�b�`����GameObject�����邩�m�F���Ă��������B", this);
            enabled = false;
            return;
        }
        if (mapInteractionHandler == null)
        {
            Debug.LogError("[PlayerMapMovementController] Map Interaction Handler���A�T�C������Ă��܂���IInspector�Őݒ肵�Ă��������B", this);
            enabled = false;
            return;
        }

        // �v���C���[�A�C�R���̏����ʒu��ݒ�i�A�j���[�V�����Ȃ��ő����ݒ�j
        SetPlayerPositionInstant(initialPlayerNodeId);

        // �����\���͑����Z���^�����O�i�A�j���[�V�����Ȃ��j
        mapInteractionHandler.CenterViewOnPlayerIcon(instant: true);

        Debug.Log("[PlayerMapMovementController] �X�N���v�g���J�n����܂����B");
    }

    /// <summary>
    /// �v���C���[�A�C�R�����A�w�肳�ꂽ�m�[�h�̈ʒu�ɃA�j���[�V�����Ȃ��ő����ݒ肵�܂��B
    /// �����ݒ�p�B
    /// </summary>
    /// <param name="nodeId">�ݒ��Ƃ��Đݒ肷��m�[�h��ID�B</param>
    private void SetPlayerPositionInstant(string nodeId)
    {
        RectTransform targetNodeRectTransform = playerLocationSetter.GetTargetNodeRectTransform(nodeId);

        if (targetNodeRectTransform != null)
        {
            playerIconRectTransform.anchoredPosition = targetNodeRectTransform.anchoredPosition;
            currentPlayerNodeId = nodeId;
            Debug.Log($"[PlayerMapMovementController] �v���C���[�A�C�R�����m�[�h '{nodeId}' �̈ʒu�ɑ����ݒ肵�܂����B���݂̃m�[�h: {currentPlayerNodeId}");
        }
        else
        {
            Debug.LogWarning($"[PlayerMapMovementController] �w�肳�ꂽ�m�[�hID '{nodeId}' ��������Ȃ��������߁A�v���C���[�A�C�R���̈ʒu�͕ύX����܂���ł���.", this);
        }
    }

    /// <summary>
    /// �w�肳�ꂽ�m�[�hID�փv���C���[�A�C�R�����ړ���������J���\�b�h�B
    /// MapInteractionHandler����Ăяo����܂��B
    /// </summary>
    /// <param name="targetNodeId">�ړ���̃m�[�hID�B</param>
    /// <returns>�ړ����J�n�ł����ꍇ��true�A�ړ����̂��ߊJ�n�ł��Ȃ������ꍇ��false�B</returns>
    public bool TryMovePlayerToNode(string targetNodeId)
    {
        // �v���C���[���ړ����̏ꍇ�́A�V�����ړ����N�G�X�g�𖳎�����
        if (isMoving)
        {
            Debug.Log($"[PlayerMapMovementController] �v���C���[�A�C�R�����ړ����̂��߁A�ړ����N�G�X�g '{targetNodeId}' �𖳎����܂����B", this);
            return false;
        }

        // ���݂̃v���C���[�m�[�h�����擾
        MapNode currentPlayerNode = MapPathManager.Instance.GetNode(currentPlayerNodeId);

        if (currentPlayerNode == null)
        {
            Debug.LogError($"[PlayerMapMovementController] ���݂̃v���C���[�m�[�h '{currentPlayerNodeId}' ��������܂���B�ړ��ł��܂���B", this);
            return false;
        }

        // �N���b�N���ꂽ�m�[�h�����݂̃m�[�h�̗אڃm�[�h�ł��邩���m�F
        if (currentPlayerNode.GetConnectedNodeIds().Contains(targetNodeId))
        {
            // �אڃm�[�h�ł���Έړ��A�j���[�V�������J�n
            StartCoroutine(MovePlayerIconAnimated(targetNodeId));
            return true;
        }
        else
        {
            Debug.LogWarning($"[PlayerMapMovementController] �m�[�h '{targetNodeId}' �͌��݂̃m�[�h '{currentPlayerNodeId}' �ɒ��ڐڑ�����Ă��܂���B�ړ��ł��܂���B", this);
            return false;
        }
    }

    /// <summary>
    /// �v���C���[�A�C�R�����w�肳�ꂽ�m�[�h�փA�j���[�V�����ňړ������܂��B
    /// </summary>
    /// <param name="targetNodeId">�ړ���̃m�[�hID�B</param>
    private IEnumerator MovePlayerIconAnimated(string targetNodeId)
    {
        isMoving = true; // �ړ����t���O�𗧂Ă�

        RectTransform targetNodeRectTransform = playerLocationSetter.GetTargetNodeRectTransform(targetNodeId);

        if (targetNodeRectTransform == null)
        {
            Debug.LogWarning($"[PlayerMapMovementController] �ړ���̃m�[�hID '{targetNodeId}' ��������܂���ł����B�ړ��𒆒f���܂��B", this);
            isMoving = false;
            yield break; // �R���[�`�����I��
        }

        Vector2 startPosition = playerIconRectTransform.anchoredPosition;
        Vector2 endPosition = targetNodeRectTransform.anchoredPosition;

        // �ڕW�ʒu�ɓ��B����܂Ń��[�v
        while (Vector2.Distance(playerIconRectTransform.anchoredPosition, endPosition) > 0.1f) // ������x�̌덷�����e
        {
            // ���݈ʒu����ڕW�ʒu�ցA�w�肳�ꂽ���x�ňړ�
            playerIconRectTransform.anchoredPosition = Vector2.MoveTowards(
                playerIconRectTransform.anchoredPosition,
                endPosition,
                moveSpeed * Time.deltaTime // ���x * �f���^�^�C�� ��1�t���[���̈ړ��������v�Z
            );
            yield return null; // 1�t���[���ҋ@
        }

        // �ړ�������A���m�ȍŏI�ʒu�ɐݒ�
        playerIconRectTransform.anchoredPosition = endPosition;
        currentPlayerNodeId = targetNodeId; // ���݂̃m�[�hID���X�V
        isMoving = false; // �ړ����t���O�����낷

        Debug.Log($"[PlayerMapMovementController] �v���C���[�A�C�R���� '{targetNodeId}' �Ɉړ����������܂����B���݂̃m�[�h: {currentPlayerNodeId}");

        // �ړ�������A�����Z���^�����O�ݒ肪ON�ł���Ύ��ԍ��ŉ�ʂ��Z���^�����O
        if (shouldCenterViewAfterMove)
        {
            StartCoroutine(CenterViewAfterDelay());
        }
    }

    /// <summary>
    /// �x����ɉ�ʂ��v���C���[�A�C�R���ɃZ���^�����O���܂��B
    /// </summary>
    private IEnumerator CenterViewAfterDelay()
    {
        Debug.Log("[PlayerMapMovementController] CenterViewAfterDelay �R���[�`�����J�n����܂����B"); // ���ǉ�
        yield return new WaitForSeconds(centerViewDelay); // �w�肳�ꂽ�b���ҋ@

        if (mapInteractionHandler != null)
        {
            // MapInteractionHandler�̃Z���^�����O�@�\���A�j���[�V�����ŌĂяo��
            mapInteractionHandler.CenterViewOnPlayerIcon(instant: false);
            Debug.Log("[PlayerMapMovementController] MapInteractionHandler.CenterViewOnPlayerIcon ���Ăяo���܂����B"); // ���ǉ�
        }
        else
        {
            Debug.LogWarning("[PlayerMapMovementController] MapInteractionHandler���A�T�C������Ă��Ȃ����߁A�x���Z���^�����O�����s�ł��܂���ł����B", this);
        }
        Debug.Log("[PlayerMapMovementController] CenterViewAfterDelay �R���[�`�����I�����܂����B"); // ���ǉ�
    }

    /// <summary>
    /// �v���C���[�A�C�R�������݈ړ����ł��邩���擾���܂��B
    /// </summary>
    public bool IsPlayerMoving()
    {
        return isMoving;
    }
}