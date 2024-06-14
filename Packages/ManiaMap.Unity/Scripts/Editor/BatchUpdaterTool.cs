using System.IO;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// Tool for batch updating room templates for prefabs in a project.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Batch Updater Tool", fileName = "BatchUpdaterTool")]
    public class BatchUpdaterTool : ScriptableObject
    {
        [SerializeField]
        private string[] _searchPaths = new string[] { "Assets" };
        /// <summary>
        /// An array of project paths to search for room prefabs.
        /// </summary>
        public string[] SearchPaths { get => _searchPaths; set => _searchPaths = value; }

        [MenuItem("Mania Map/Batch Update Room Templates", priority = 0)]
        public static void RunBatchUpdateRoomTemplates()
        {
            LoadSettings().BatchUpdateRoomTemplates();
        }

        /// <summary>
        /// Returns the batch updater tool at ManiaMap/BatchUpdaterTool within a Resources folder if it exists.
        /// If it does not exist, returns a new default instance.
        /// </summary>
        private static BatchUpdaterTool LoadSettings()
        {
            var settings = Resources.Load<BatchUpdaterTool>("ManiaMap/BatchUpdaterTool");

            if (settings != null)
                return settings;

            return CreateInstance<BatchUpdaterTool>();
        }

        /// <summary>
        /// Performs batch update for all discovered rooms.
        /// </summary>
        public void BatchUpdateRoomTemplates()
        {
            var count = 0;
            Debug.Log("Beginning room template batch update...");
            var guids = AssetDatabase.FindAssets("t:prefab", SearchPaths);

            foreach (var guid in guids)
            {
                if (UpdateRoomTemplate(guid))
                    count++;
            }

            Debug.Log($"<color=#00FF00><b>Completed room template batch update on {count} rooms.</b></color>");
        }

        /// <summary>
        /// Performs auto assignment on the specified room.
        /// </summary>
        /// <param name="room">The room.</param>
        public static void AutoAssign(RoomComponent room)
        {
            var count = room.AutoAssign();

            if (room.RoomTemplate != null)
            {
                room.RoomTemplate.Id = Rand.AutoAssignId(room.RoomTemplate.Id);
                room.RoomTemplate.Name = room.Name;
                EditorUtility.SetDirty(room.RoomTemplate);
            }

            EditorUtility.SetDirty(room);
            Debug.Log($"<color=#00FF00><b>Auto assigned {count} cell children.</b></color>");
        }

        /// <summary>
        /// Updates or creates the room template for the room at the specified GUID.
        /// If the object at the GUID does not contain a RoomComponent at its root, skips it and returns false.
        /// Returns true when successful.
        /// </summary>
        /// <param name="roomGuid">The room GUID.</param>
        private bool UpdateRoomTemplate(string roomGuid)
        {
            var roomPath = AssetDatabase.GUIDToAssetPath(roomGuid);
            using var scope = new PrefabUtility.EditPrefabContentsScope(roomPath);

            if (!scope.prefabContentsRoot.TryGetComponent(out RoomComponent room))
                return false;

            Debug.Log($"Processing room at {roomPath}...");
            AutoAssign(room);
            var savePath = Path.ChangeExtension(roomPath, ".room_template.asset");

            if (room.RoomTemplate == null)
                room.RoomTemplate = AssetDatabase.LoadAssetAtPath<RoomTemplateResource>(savePath);

            if (room.RoomTemplate == null)
            {
                var templateResource = CreateInstance<RoomTemplateResource>();
                AssetDatabase.CreateAsset(templateResource, savePath);
                room.RoomTemplate = AssetDatabase.LoadAssetAtPath<RoomTemplateResource>(savePath);
                Debug.Log($"Saved room template to {savePath}");
            }

            room.RoomTemplate.Id = Rand.AutoAssignId(room.RoomTemplate.Id);
            room.RoomTemplate.Name = room.Name;
            EditorUtility.SetDirty(room.RoomTemplate);

            var template = room.GetMMRoomTemplate(room.RoomTemplate.Id, room.RoomTemplate.Name);
            room.RoomTemplate.Initialize(template, roomGuid, roomPath);
            EditorUtility.SetDirty(room.RoomTemplate);
            EditorUtility.SetDirty(room);
            Debug.Log($"Updated room template.");
            return true;
        }
    }
}
