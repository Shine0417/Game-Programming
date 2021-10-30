using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Gameplay
{

    /// <summary>
    /// Fired when a Player collides with an Block.
    /// </summary>
    /// <typeparam name="BlockCollision"></typeparam>
    public class PlayerBlockCollision : Simulation.Event<PlayerBlockCollision>
    {
        public BlockController block;
        public PlayerController player;

        private Vector3 carryLocation { get { return new Vector3(0.8f, 0.24f, 0); } }

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            carryByPlayer();
        }

        private void carryByPlayer() {
            if(player.carried) return;
            player.carried = block.gameObject;
            int flip = player.GetComponent<SpriteRenderer>().flipX ? 1 : -1;
            block.gameObject.transform.parent = player.gameObject.transform;
            block.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            block.gameObject.GetComponent<Collider2D>().enabled = false;
            block.gameObject.transform.localPosition = carryLocation;
        }
    }
}