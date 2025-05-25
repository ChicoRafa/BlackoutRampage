using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Data.Customers.Scripts {
    public class ClientMovement : MonoBehaviour {
        private Animator animator;
        private Coroutine moveRoutine;
        private GameObject modelInstance;
        private Rigidbody rigidBody;
        
        private int currentImpatienceStep = 0;

        public void Init(Animator animator, GameObject modelInstance) {
            this.animator = animator;
            this.modelInstance = modelInstance;
            rigidBody = GetComponent<Rigidbody>();
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

                Vector3 nextPos = Vector3.MoveTowards(rigidBody.position, targetPosition, 10f * Time.deltaTime);
                rigidBody.MovePosition(nextPos);
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
        
        public void MoveImpatiently(Vector3 basePosition, System.Action onComplete)
        {
            float distance = 0.3f;
            float duration = 0.3f;

            int direction = (currentImpatienceStep++ % 2 == 0) ? 1 : -1;

            Vector3 offset = new Vector3(distance * direction, 0f, 0f);
            Vector3 target = basePosition + offset;
            Vector3 returnTo = basePosition;

            StartCoroutine(MoveImpatientRoutine(basePosition, target, returnTo, duration, onComplete));
        }

        private IEnumerator MoveImpatientRoutine(Vector3 from, Vector3 to, Vector3 backTo, float duration, System.Action onComplete)
        {
            SetAnimatorSpeed(1f);

            Vector3 directionTo = (to - from).normalized;
            if (directionTo != Vector3.zero && modelInstance != null)
                modelInstance.transform.forward = directionTo;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = to;

            Vector3 directionBack = (backTo - to).normalized;
            if (directionBack != Vector3.zero && modelInstance != null)
                modelInstance.transform.forward = directionBack;

            elapsed = 0f;
            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(to, backTo, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = backTo;

            SetAnimatorSpeed(0f);
            onComplete?.Invoke();
        }
    }
}