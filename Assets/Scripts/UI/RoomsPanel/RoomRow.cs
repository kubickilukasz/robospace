using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI {

    public class RoomRow : RectMonoBehaviour {

        [SerializeField] TextMeshProUGUI nameRoom;
        [SerializeField] TextMeshProUGUI ipRoom;
        [SerializeField] TextMeshProUGUI ownerRoom;
        [SerializeField] Button button;

        RoomData data;
        RoomsPanel panel;

        public void Init(RoomsPanel panel, RoomData data) {
            this.panel = panel;
            this.data = data;
            nameRoom.text = data.name;
            ipRoom.text = data.ip;
            ownerRoom.text = data.user;
            button.onClick.AddListener(Connect);
        }

        public void Connect() {
            if (NetworkController.instanceExist) {
                panel.DisableButtons();
                GameManager.instance.JoinRoom(data.ip, data.localIp, OnConnect);
            }
        }

        public void Enable() {
            button.interactable = true;
        }

        public void Disable() {
            button.interactable = false;
        }

        void OnConnect() {
            GetComponentInParent<PanelController>()?.GetPanel<RoomsPanel>()?.Hide();
        }

        void OnDisconnect(ulong id) {
            if (NetworkController.instance.id.Equals(id)) {
                NetworkController.instance.onClientDisconnect -= OnDisconnect;
                panel.Show();
            }
        }

    }

}
