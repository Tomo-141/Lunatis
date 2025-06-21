// PlayerLocationSetter.cs
// ���̃X�N���v�g�́A�v���C���[�A�C�R���������ʒu�Ƃ��Ďw�肷�ׂ�MapNode�̏����擾����@�\��񋟂��܂��B
// �}�b�v�̏���������A�Q�[���C�x���g�ɂ���ăv���C���[�̈ʒu�������I�ɕύX����ۂɁA
// ���̈ړ���̃m�[�h���𑼂̃X�N���v�g�ɒ񋟂��܂��B

using UnityEngine;
using UnityEngine.UI; // RectTransform���g�p���邽�߂ɕK�v

public class PlayerLocationSetter : MonoBehaviour
{
    // ���̃X�N���v�g���̂̓v���C���[�A�C�R����RectTransform�𒼐ڑ��삵�Ȃ����߁A�Q�Ƃ͕s�v�ɂȂ�܂��B
    // ���̑���A�K�v�ȃm�[�h�̏���Ԃ��`�ɂ��܂��B

    /// <summary>
    /// �w�肳�ꂽMapNode�̏����擾���܂��B
    /// ���̃��\�b�h�͊O���i��: MapInteractionHandler��Q�[���C�x���g�X�N���v�g�j����Ăяo����A
    /// ���̃m�[�h�̈ʒu�����g���ăv���C���[�A�C�R����z�u������A���_�𒲐������肷�邽�߂Ɏg���܂��B
    /// </summary>
    /// <param name="targetNodeId">�擾������MapNode��ID�B</param>
    /// <returns>�w�肳�ꂽID��MapNode��RectTransform�B������Ȃ��ꍇ��null�B</returns>
    public RectTransform GetTargetNodeRectTransform(string targetNodeId)
    {
        if (MapPathManager.Instance == null)
        {
            Debug.LogError("[PlayerLocationSetter] MapPathManager�̃C���X�^���X��������܂���B�V�[����MapPathManager���A�^�b�`����GameObject�����邩�m�F���Ă��������B", this);
            return null;
        }

        MapNode targetNode = MapPathManager.Instance.GetNode(targetNodeId);
        if (targetNode != null)
        {
            Debug.Log($"[PlayerLocationSetter] �m�[�h '{targetNodeId}' �̈ʒu�����擾���܂����B");
            return targetNode.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogWarning($"[PlayerLocationSetter] �w�肳�ꂽ�m�[�hID '{targetNodeId}' ��������܂���ł����B", this);
            return null;
        }
    }
}