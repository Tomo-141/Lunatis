// MapNode.cs
// ���̃X�N���v�g�́A�}�b�v��̊e�m�[�h�i�n�_�j�̏���ێ����܂��B
// �m�[�hID�A�m�[�h���A�����Đڑ�����Ă��鑼�̃m�[�h��ID���X�g���܂݂܂��B
// �܂��AUI�N���b�N�C�x���g�����o����@�\�������܂��B

using UnityEngine;
using System.Collections.Generic; // List<T>���g�p���邽�߂ɕK�v
using UnityEngine.EventSystems;   // IPointerClickHandler ���g�p���邽�߂ɕK�v
using System;                     // Action ���g�p���邽�߂ɕK�v
using System.Linq; // ConnectedNodes����NodeId�̃��X�g�𐶐����邽�߂ɕK�v

public class MapNode : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("���̃m�[�h�̈�ӂ̎��ʎq�i��: TownA, Dungeon1�Ȃǁj�BHierarchy��GameObject���ƍ��킹�邱�Ƃ𐄏����܂��B")]
    public string NodeId; // NodeId�͈��������蓮���͂܂��͎��������i�������j�̕�����Ƃ��ĕێ�

    [Tooltip("���̃m�[�h�̕\�����i��: �u�͂��܂�̊X�v�Ȃǁj")]
    public string NodeName;

    // ConnectedNodeIds �� MapNode �^�̃��X�g�ɕύX���܂�
    [Tooltip("���̃m�[�h���璼�ڐڑ�����Ă��鑼�̃m�[�h�̎Q�ƃ��X�g�BHierarchy����MapNode�R���|�[�l���g������GameObject���h���b�O���h���b�v���Ă��������B")]
    public List<MapNode> ConnectedNodes; // MapNode�̎Q�ƃ��X�g�ɕύX

    // �m�[�h���N���b�N���ꂽ�Ƃ��ɌĂяo�����ÓI�C�x���g
    // �����ɂ̓N���b�N���ꂽ�m�[�h��ID���܂܂�܂�
    public static event Action<string> OnNodeClicked;

    /// <summary>
    /// Unity�G�f�B�^�ł̂݌Ăяo����A�R���|�[�l���g�����[�h���ꂽ�Ƃ��Ɏ��s����܂��B
    /// �f�o�b�O�p�ɁA�m�[�hID�����ݒ�̏ꍇ�Ɍx�����o���܂��B
    /// </summary>
    void OnValidate()
    {
        if (string.IsNullOrEmpty(NodeId))
        {
            Debug.LogWarning($"[MapNode] GameObject '{gameObject.name}' ��NodeId���ݒ肳��Ă��܂���B��ӂ�ID��ݒ肵�Ă��������B", this);
        }

        // ConnectedNodes���X�g��null�łȂ����m�F���A�v�f������ꍇ�͌x��
        if (ConnectedNodes != null)
        {
            foreach (var node in ConnectedNodes)
            {
                if (node == null)
                {
                    Debug.LogWarning($"[MapNode] �m�[�h '{NodeId}' ��ConnectedNodes���X�g��null�̗v�f������܂��B�Q�Ƃ��؂�Ă��邩�A�������ݒ肳��Ă��܂���B", this);
                }
                else if (string.IsNullOrEmpty(node.NodeId))
                {
                    Debug.LogWarning($"[MapNode] �m�[�h '{NodeId}' ��ConnectedNodes���X�g�Ɋ܂܂��m�[�h '{node.name}' ��NodeId���ݒ肳��Ă��܂���B", this);
                }
            }
        }
    }

    /// <summary>
    /// ���̃m�[�h���N���b�N���ꂽ�Ƃ��ɌĂяo����܂��B
    /// </summary>
    /// <param name="eventData">�|�C���^�[�C�x���g�f�[�^�B</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // �ÓI�C�x���g���Ăяo���A�N���b�N���ꂽ�m�[�h��ID��n���܂��B
        OnNodeClicked?.Invoke(NodeId);
        Debug.Log($"[MapNode] �m�[�h '{NodeName}' (ID: {NodeId}) ���N���b�N����܂����B�C�x���g�𔭉΂��܂����B", this);
    }

    /// <summary>
    /// ConnectedNodes���X�g����A�ڑ�����Ă���m�[�h��ID���X�g���擾���܂��B
    /// ���̃��\�b�h��MapInteractionHandler�Ȃǂ��ڑ�����Ɏg�p���܂��B
    /// </summary>
    /// <returns>�ڑ�����Ă���m�[�h��ID�̃��X�g�B</returns>
    public List<string> GetConnectedNodeIds()
    {
        // null�`�F�b�N���s���Anull�v�f���t�B���^�����O����NodeId�̃��X�g��Ԃ��܂��B
        if (ConnectedNodes == null)
        {
            return new List<string>();
        }
        return ConnectedNodes.Where(node => node != null && !string.IsNullOrEmpty(node.NodeId))
                             .Select(node => node.NodeId)
                             .ToList();
    }
}