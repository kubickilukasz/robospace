using UnityEngine;
using Kubeec.VR.Player;

[RequireComponent(typeof(Destructible))]
public class CreateResourcesDestructable : EnableInitableDisposable{

    [SerializeField] float resources = 100f;
    Destructible destructible;

    protected override void OnInit(object data) {
        destructible = GetComponent<Destructible>();
        destructible.onDestruct += AddResources;
    }

    protected override void OnDispose() {
        if (destructible != null) {
            destructible.onDestruct -= AddResources;
        }
    }

    void AddResources() {
        MockResourcesData.instance.Add(resources);
        if (destructible != null) {
            destructible.onDestruct -= AddResources;
        }
    }


}
