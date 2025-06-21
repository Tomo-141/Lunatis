// MapPathManager.cs
// ���̃X�N���v�g�́A�}�b�v��̑S�Ẵm�[�h�iMapNode�R���|�[�l���g������GameObject�j���Ǘ����A
// �m�[�hID���L�[�Ƃ��ĊȒP�ɃA�N�Z�X�ł���悤�ɂ��܂��B
// ��Ɍo�H�T����m�[�h�Ԃ̐ڑ���񂪕K�v�ȑ��̃V�X�e������Q�Ƃ���܂��B
// Dijkstra�@�ɂ��ŒZ�o�H�T���@�\���񋟂��܂��B

using UnityEngine;
using System.Collections.Generic; // Dictionary<TKey, TValue>���g�p���邽�߂ɕK�v
using System.Linq; // LINQ���g�p���邽�߂ɕK�v

public class MapPathManager : MonoBehaviour
{
    // �S�Ẵm�[�h��ID�Ō����ł���悤�Ɏ����ŊǗ����܂��B
    // Unity�G�f�B�^�Œ��ڐݒ肷��̂ł͂Ȃ��AAwake���Ɏ����Ŏ��W���܂��B
    private Dictionary<string, MapNode> allNodes = new Dictionary<string, MapNode>();

    // �V���O���g���p�^�[���i�ȈՔŁj�F���̃X�N���v�g����ȒP�ɃA�N�Z�X�ł���悤�ɂ��܂��B
    public static MapPathManager Instance { get; private set; }

    /// <summary>
    /// Awake�̓X�N���v�g�C���X�^���X�����[�h���ꂽ�Ƃ��ɌĂяo����܂��B
    /// �V���O���g���̐ݒ�ƁA�V�[�����̑S�Ă�MapNode�����W���܂��B
    /// </summary>
    void Awake()
    {
        // �V���O���g���C���X�^���X�̏�����
        if (Instance == null)
        {
            Instance = this;
            // �V�[�������[�h�������Ă��C���X�^���X���j������Ȃ��悤�ɂ���ꍇ�́ADontDestroyOnLoad(gameObject); ���g�p���܂����A
            // ����̓}�b�v��ʐ�p�Ȃ̂ŕs�v��������܂���B
        }
        else
        {
            Debug.LogWarning("[MapPathManager] ������MapPathManager�C���X�^���X��������܂����B�����̂��̂�ێ����A���̃C���X�^���X�͔j�����܂��B", this);
            Destroy(gameObject);
            return;
        }

        // �V�[�����̑S�Ă�MapNode�R���|�[�l���g���������A�����ɓo�^���܂��B
        MapNode[] nodesInScene = FindObjectsByType<MapNode>(FindObjectsSortMode.None);
        foreach (MapNode node in nodesInScene)
        {
            if (allNodes.ContainsKey(node.NodeId))
            {
                // ����ID�̃m�[�h���������݂���ꍇ�A�G���[���o�͂��܂��B
                Debug.LogError($"[MapPathManager] �d������m�[�hID��������܂���: '{node.NodeId}'�BGameObject: '{node.name}' �� '{allNodes[node.NodeId].name}'", node);
                continue;
            }
            allNodes.Add(node.NodeId, node);
            Debug.Log($"[MapPathManager] �m�[�h��o�^���܂���: ID='{node.NodeId}', ���O='{node.name}'");
        }

        // �e�m�[�h�̐ڑ�ID�����ۂɑ��݂���m�[�hID�ł��邩�m�F����i�f�o�b�O�p�j
        CheckNodeConnections();
    }

    /// <summary>
    /// �w�肳�ꂽ�m�[�hID�ɑΉ�����MapNode�R���|�[�l���g���擾���܂��B
    /// </summary>
    /// <param name="nodeId">�擾�������m�[�h��ID�B</param>
    /// <returns>�w�肳�ꂽID��MapNode�A������Ȃ��ꍇ��null�B</returns>
    public MapNode GetNode(string nodeId)
    {
        MapNode node;
        if (allNodes.TryGetValue(nodeId, out node))
        {
            return node;
        }
        Debug.LogWarning($"[MapPathManager] �m�[�hID '{nodeId}' ��������܂���ł����B", this);
        return null;
    }

    /// <summary>
    /// �S�Ẵm�[�h�̐ڑ��ݒ肪���������i�ڑ���ID�����݂��邩�j���`�F�b�N���܂��B
    /// �f�o�b�O�ړI�Ŏg�p���܂��B
    /// </summary>
    private void CheckNodeConnections()
    {
        foreach (var entry in allNodes)
        {
            MapNode currentNode = entry.Value;
            // MapNode�N���X�� GetConnectedNodeIds() ���\�b�h���g�p
            foreach (string connectedId in currentNode.GetConnectedNodeIds()) // �������C��
            {
                if (!allNodes.ContainsKey(connectedId))
                {
                    Debug.LogWarning($"[MapPathManager] �m�[�h '{currentNode.NodeId}' (GameObject: {currentNode.gameObject.name}) �̐ڑ���m�[�hID '{connectedId}' �����݂��܂���B�ڑ����폜���邩�A�m�[�h���쐬���Ă��������B", currentNode);
                }
            }
        }
        Debug.Log("[MapPathManager] �m�[�h�ڑ��̐������`�F�b�N���������܂����B");
    }
}