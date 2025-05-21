using System;
using System.Collections;
using UnityEngine;

namespace _Data.Customers.Scripts {
    public class ClientMovement : MonoBehaviour {
        private Animator animator;
        private Coroutine moveRoutine;
        private GameObject modelInstance;

        public void Init(Animator animator, GameObject modelInstance) {
            this.animator = animator;
            this.modelInstance = modelInstance;
        }

        public void MoveTo(Vector3 targetPosition, Action onComplete = null) {
            if (moveRoutine != null) {
                StopCoroutine(moveRoutine);
            }

            moveRoutine = StartCoroutine(MoveToPositionRoutine(targetPosition, onComplete));
        }

        private IEnumerator MoveToPositionRoutine(Vector3 targetPosition, Action onComplete) {
            SetAnimatorSpeed(1f);

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f) {
                Vector3 direction = (targetPosition - transform.position).normalized;
                if (modelInstance != null && direction != Vector3.zero) {
                    modelInstance.transform.forward = direction;
                }

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 3f * Time.deltaTime);
                yield return null;
            }

            SetAnimatorSpeed(0f);
            moveRoutine = null;
            onComplete?.Invoke();
        }

        public void SetAnimatorSpeed(float value) {
            if (animator != null) {
                animator.SetFloat("Speed", value);
            }
        }
    }
}