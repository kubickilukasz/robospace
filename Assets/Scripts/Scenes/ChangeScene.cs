using UnityEngine;

public class ChangeScene : MonoBehaviour{

    [SerializeField] ScenesManager.SceneType sceneType;

    public void Change() {
        GameManager.instance.TryLoadScene(sceneType);
    }

}
