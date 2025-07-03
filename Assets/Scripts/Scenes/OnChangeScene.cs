using UnityEngine;
using UnityEngine.Events;

public class OnChangeScene : EnableInitableDisposable{

    public UnityEvent onLoadScene;

    protected override void OnInit(object data) {
        GameManager.instance.onEndLoadScene += Call;
    }

    protected override void OnDispose() {
        if (GameManager.instanceExist) {
            GameManager.instance.onEndLoadScene -= Call;
        }
    }

    void Call() {
        onLoadScene?.Invoke();
    }

}
