using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Kubeec.General;
using DG.Tweening;

namespace UI.RenderedUI {

    [RequireComponent(typeof(Canvas))]
    public class RenderedUI : EnableDisableInitableDisposable {

        static CustomPool<Camera> cameraPool;
        static CustomPool<Camera> CameraPool {
            get {
                if (cameraPool == null) {
                    cameraPool = new CustomPool<Camera>();
                }
                return cameraPool;
            }
        }

        [SerializeField] Material material;

        Camera camera;
        RenderTexture renderTexture;
        Canvas inputCanvas;
        RectTransform canvasTransform;
        RawImage rawImage;
        int? layerRenderedUI;
        int? layerUI;

        protected override void OnInit(object data) {
            layerRenderedUI ??= LayerMask.NameToLayer("RenderedUI");
            inputCanvas ??= GetComponent<Canvas>();
            layerUI ??= inputCanvas.gameObject.layer;
            camera = CameraPool.Get();
            renderTexture = new RenderTexture((int)inputCanvas.renderingDisplaySize.x, (int)inputCanvas.renderingDisplaySize.y, 16);
            renderTexture.Create();
            SetupCamera(camera);
            SetupCanvas(inputCanvas);
        }

        protected override void OnDispose() {
            if (camera) {
                camera.forceIntoRenderTexture = false;
                camera.targetTexture = null;
                Camera tempCamera = camera;
                camera = null;
                this.SafeInvokeNextFrame(() => {
                    CameraPool.Release(tempCamera);
                });
            }
            if (renderTexture) {
                renderTexture.Release();
                DestroyImmediate(renderTexture);
                renderTexture = null;
            }
        }

        void SetupCamera(Camera camera) {
            canvasTransform ??= inputCanvas.GetComponent<RectTransform>();
            LayerMask mask = 1 << layerRenderedUI.Value;
            camera.cullingMask = mask;
            camera.orthographic = true;
            camera.targetTexture = renderTexture;
            camera.forceIntoRenderTexture = true;
            camera.transform.SetParent(inputCanvas.transform);
            camera.transform.localPosition = new Vector3(0, 0, inputCanvas.transform.localScale.z * -5f);
            camera.transform.localRotation = Quaternion.identity;
            camera.nearClipPlane = -0.01f;
            camera.farClipPlane = Mathf.Max(inputCanvas.transform.localScale.z * 5f, 0.1f);
            camera.orthographicSize = canvasTransform.rect.height / 2f * inputCanvas.transform.localScale.y;
            camera.clearFlags = CameraClearFlags.Nothing;
            camera.useOcclusionCulling = false;
            //camera.cullingMatrix
        }

        void SetupCanvas(Canvas canvas) {
            canvas.worldCamera = camera;
            SetLayerAllChildren(canvas.transform, layerRenderedUI.Value);
            if (rawImage == null) {
                GameObject go = new GameObject(string.Empty, typeof(RectTransform), typeof(Canvas));
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.SetParent(canvas.transform);
                rt.localPosition = Vector3.zero;
                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMax = rt.offsetMin = new Vector2(0, 0);
                rawImage = rt.gameObject.AddComponent<RawImage>();
            }
            rawImage.transform.SetAsLastSibling();
            rawImage.gameObject.layer = layerUI.Value;
            rawImage.material = material;
            rawImage.texture = renderTexture;
        }

        void SetLayerAllChildren(Transform root, int layer) {
            var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children) {
                child.gameObject.layer = layer;
            }
        }

    }

}