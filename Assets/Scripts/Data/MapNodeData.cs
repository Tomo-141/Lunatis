// MapNodeData.cs
// �X�̃}�b�v�m�[�h�̃f�[�^�iID�A�ʒu�A�ڑ��m�[�h�j��ێ�����ScriptableObject

using UnityEngine;
using System.Collections.Generic;

// Unity�G�f�B�^�ł��̃A�Z�b�g���쐬���邽�߂̃��j���[���ڂ�ǉ����܂��B
[CreateAssetMenu(fileName = "NewMapNodeData", menuName = "Map System/Map Node Data", order = 1)]
public class MapNodeData : ScriptableObject
{
    public string nodeId;             // �m�[�h�̃��j�[�N��ID
    public Vector2 position;          // �m�[�h�̃}�b�v��ł�UI�ʒu�ianchoredPosition�j
    public List<string> connectedNodeIds = new List<string>(); // �ڑ�����Ă���m�[�h��ID���X�g
}

// ������MapNodeData�A�Z�b�g���܂Ƃ߂ĊǗ����邽�߂�ScriptableObject
// �S�Ẵm�[�h�f�[�^�����̈�̃A�Z�b�g�ŎQ�Ƃł���悤�ɂ��܂��B
[CreateAssetMenu(fileName = "MapDataCollection", menuName = "Map System/Map Data Collection", order = 2)]
public class MapDataCollection : ScriptableObject
{
    public List<MapNodeData> nodes = new List<MapNodeData>(); // �S�Ẵm�[�h�f�[�^�̃��X�g
}