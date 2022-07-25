using UnityEngine;

namespace Player
{
    public class PlayerAnimatorManager : AnimatorManager
    {
        #region Animator Fields

        private static readonly int CanDoCombo = Animator.StringToHash("canDoCombo");
        private static readonly int IsInteracting = Animator.StringToHash("isInteracting");

        #endregion

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void EnableCombo()
        {
            animator.SetBool(CanDoCombo, true);
        }

        public void DisableCombo()
        {
            animator.SetBool(CanDoCombo, false);
        }

        public void EnableIsInteracting()
        {
            animator.SetBool(IsInteracting, true);
        }

        public void DisableIsInteracting()
        {
            animator.SetBool(IsInteracting, false);
        }

        public override void TakeFinisherDamageAnimationEvent()
        {
            base.TakeFinisherDamageAnimationEvent();

            var playerManager = GetComponent<PlayerManager>();

            GetComponent<PlayerStats>().TakeDamage(playerManager.pendingFinisherDamage, false);
            playerManager.pendingFinisherDamage = 0;
        }
    }
}
