using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MainMenu
{
    public class SkinViewer : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        [SerializeField]
        private Transform objectToRotate;

        [Range(-1f, 1f)]
        [SerializeField]
        private float rotateSpeed = 0.1f;

        [SerializeField]
        private bool userIsRotating;

        [SerializeField]
        private float grabRotationSpeed;

        [SerializeField]
        private Quaternion restRotation;

        [SerializeField]
        private float lerpTime;

        private void Awake()
        {
        }

        private void Update()
        {
            if (userIsRotating) return;

            objectToRotate.Rotate(Vector3.up, rotateSpeed);
            restRotation = objectToRotate.transform.rotation;
        }

        private IEnumerator RotateToRest()
        {
            while (objectToRotate.transform.rotation != restRotation)
            {
                objectToRotate.transform.rotation = Quaternion.RotateTowards(objectToRotate.transform.rotation,
                    restRotation, Time.deltaTime * lerpTime);

                yield return null;
            }

            userIsRotating = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var mouseXPos = eventData.delta.x * grabRotationSpeed;
            var mouseYPos = eventData.delta.y * grabRotationSpeed;

            var rotation = new Vector3(mouseYPos, -mouseXPos, 0);
            objectToRotate.transform.Rotate(rotation, Space.World);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            StartCoroutine(RotateToRest());
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            userIsRotating = true;
        }
    }
}