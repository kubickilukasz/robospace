using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace UI {

    public class StartRoomPanel : Panel {

        [SerializeField] Button startGame;
        [SerializeField] Button quitGame;

        void OnEnable() {
            startGame.onClick.AddListener(StartGame);
            quitGame.onClick.AddListener(QuitGame);
        }

        protected virtual void OnDisable() {
            if (startGame) {
                startGame.onClick.RemoveListener(StartGame);
                quitGame.onClick.RemoveListener(QuitGame);
            }
        }

        [Button]
        void StartGame() {
            GameManager.instance.StartGame();
        }

        [Button]
        void QuitGame() {
            GameManager.instance.StopGame();
            Get<RoomsPanel>().Show();
        }

    }

}
