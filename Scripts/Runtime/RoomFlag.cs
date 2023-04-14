using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class RoomFlag : CellChild
    {
        [SerializeField] private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField] private RoomFlagEvent _onInitialize;
        public RoomFlagEvent OnInitialize { get => _onInitialize; set => _onInitialize = value; }

        private void OnValidate()
        {
            Id = ManiaMapManager.AutoAssignId(Id);
        }

        private void Awake()
        {
            Room().OnInitialize.AddListener(Initialize);
        }

        private void OnDestroy()
        {
            Room().OnInitialize.RemoveListener(Initialize);
        }

        private void Initialize()
        {
            OnInitialize.Invoke(this);
        }

        public bool Exists()
        {
            return RoomState().Flags.Contains(Id);
        }

        public bool SetFlag()
        {
            return RoomState().Flags.Add(Id);
        }

        public bool RemoveFlag()
        {
            return RoomState().Flags.Remove(Id);
        }

        public bool ToggleFlag()
        {
            var flags = RoomState().Flags;

            if (!flags.Add(Id))
            {
                flags.Remove(Id);
                return false;
            }

            return true;
        }
    }
}