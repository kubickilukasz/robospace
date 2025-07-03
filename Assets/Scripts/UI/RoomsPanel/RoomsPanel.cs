using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using NaughtyAttributes;

namespace UI {

    public class RoomsPanel : Panel {

        [SerializeField] RectTransform content;
        [SerializeField] RoomRow prefabRow;
        [SerializeField] Button refreshButton;
        [SerializeField] Button createButton;

        List<RoomData> data = new List<RoomData>();
        List<RoomRow> rows = new List<RoomRow>();

        string testName => Application.platform == RuntimePlatform.Android ? "QUEST" : "WINDOWS";

        void OnEnable() {
            createButton.onClick.AddListener(CreateRoom);
            refreshButton.onClick.AddListener(RefreshList);
            EnableButtons();
        }

        protected override void OnShow() {
            base.OnShow();
            RefreshList();
            Debug.Log("Show");
        }

        [Button]
        public void RefreshList() {
            DeleteRows(0);
            DisableButtons();
            RoomsManager.instance.GetList(RefreshList);
        }

        [Button]
        public void CreateRoom() {
            if (NetworkController.instanceExist) {
                DisableButtons();
                GameManager.instance.StartRoom(testName, $"User{Random.Range(1, 100)}", Random.Range(1111, 9999).ToString(), "elo", () => {
                    Get<StartRoomPanel>().Show();
                });
            }
        }

        public void EnableButtons() {
            refreshButton.interactable = true;
            createButton.interactable = true;
            for (int i = 0; i < rows.Count; i++) {
                rows[i].Enable();
            }
        }

        public void DisableButtons() {
            refreshButton.interactable = false;
            createButton.interactable = false;
            for (int i = 0; i < rows.Count; i++) {
                rows[i].Disable();
            }
        }

        void RefreshList(List<RoomData> newData) {
            data = newData;
            DeleteRows(data.Count);
            for (int i = 0; i < data.Count; i++) {
                RoomRow current = Instantiate(prefabRow, content);
                current.Init(this, data[i]);
                rows.Add(current);
            }
            EnableButtons();
        }

        void DeleteRows(int newNRows) {
            rows.ForEach(row => { if (row != null) Destroy(row.gameObject); });
            if (newNRows > 0) {
                rows = new List<RoomRow>(newNRows);
            } else {
                rows.Clear();
            }
        }

    }

}
