using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.Battle.WorldEnemies;

namespace Grawly.Battle.BattleArena.Setup.Standard {

	/// <summary>
	/// The standard way in which the battle arenas should be prepared. This is similar to how I've been doing it most of development.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("The standard way in which the battle arenas should be prepared. This assumes all Enemies will be sprites.")]
	public class StandardArenaSetup : BattleArenaSetup {

		#region MAIN CALLS
		/// <summary>
		/// Just prepares the battle arena with the WorldEnemyDX sprites. Nothing fancy.
		/// </summary>
		/// <param name="battleArenaController">The BattleArenaControllerDX which contains references to many of the things the setup may need to access.</param>
		/// <param name="arenaPosition">The position that the arena is intended to be placed.</param>
		/// <param name="battleTemplate">The template to use when setting up the arena.</param>
		/// <returns>A list of world enemies that will be used in the battle. They should be set in stone at this point.</returns>
		public override List<WorldEnemyDX> SetupBattleArena(BattleArenaControllerDX battleArenaController, BattleArenaDXPosition arenaPosition, BattleTemplate battleTemplate) {
			
			// Position the arena to be at the location of the BattleArenaDXPosition. This will fail if one does not exist in scene.
			battleArenaController.battleArenaGameObject.transform.SetPositionAndRotation(
				position: BattleArenaDXPosition.instance.transform.position,
				rotation: BattleArenaDXPosition.instance.transform.rotation);

			// Go through each world enemy and turn it off just in case.
			battleArenaController.DefaultEnemySprites.ForEach(we => we.gameObject.SetActive(false));

			// Create a list of WorldEnemyDX's to use.
			List<WorldEnemyDX> preparedWorldEnemies = new List<WorldEnemyDX>();
			
			// Go through each enemy template.
			for (int i = 0; i < battleTemplate.EnemyTemplates.Count; i++) {
				
				// Get the world enemy and template for this index.
				EnemyTemplate currentEnemyTemplate = battleTemplate.EnemyTemplates[i];
				// Get a reference to the next world enemy to use.
				WorldEnemyDX currentWorldEnemy = this.DetermineNextWorldEnemy(
					currentIndex: i,
					enemyTemplate: currentEnemyTemplate,
					battleArenaController: battleArenaController,
					arenaPosition: arenaPosition, 
					battleTemplate: battleTemplate);
				
				// Prep the world enemy.
				currentWorldEnemy.PrepareWorldEnemy(enemyTemplate: currentEnemyTemplate);
				
				// Add it to the list.
				preparedWorldEnemies.Add(currentWorldEnemy);
			}

			// Return the enemies that were just prepared.
			Debug.Assert(preparedWorldEnemies.Count == battleTemplate.EnemyTemplates.Count);
			return preparedWorldEnemies;
			
		}
		/// <summary>
		/// Removes the list of enemies from the battle.
		/// </summary>
		/// <param name="enemiesToRemove">The enemies to remove from this battle.</param>
		/// <param name="battleArenaController">The current BattleArenaController.</param>
		public override void RemoveEnemiesFromBattle(List<Enemy> enemiesToRemove, BattleArenaControllerDX battleArenaController) {
			
			// Go through each enemy...
			enemiesToRemove.Select(e => e.WorldEnemyDX).ToList().ForEach(we => {
				
				// ...and remove it from the active world enemies.
				battleArenaController.ActiveWorldEnemyDXs.Remove(we);
				
				// If using a prefab, destroy its Instance
				if (we.Enemy.Template.UsesPrefab == true) {
					GameObject.Destroy(we.gameObject);
				} else {
					// If not, just turn it off.
					we.gameObject.SetActive(false);
				}
				
			});
		}
		#endregion

		#region HELPERS
		/// <summary>
		/// A function to help figure out what the next WorldEnemyDX should be.
		/// </summary>
		/// <param name="currentIndex">The current index of the loop that called this function.</param>
		/// <param name="enemyTemplate">The current enemy template that may use a prefab or a sprite.</param>
		/// <param name="battleArenaController">The current BattleArenaController.</param>
		/// <param name="arenaPosition">The current arena position.</param>
		/// <param name="battleTemplate">The BattleTemplate being used.</param>
		/// <returns>A reference to the desired WorldEnemyDx.</returns>
		private WorldEnemyDX DetermineNextWorldEnemy(int currentIndex, EnemyTemplate enemyTemplate, BattleArenaControllerDX battleArenaController, BattleArenaDXPosition arenaPosition, BattleTemplate battleTemplate) {
			
			//  If this enemy DOES use a prefab, 
			if (enemyTemplate.UsesPrefab == true) {
				
				// Get the Transform to place it at.
				Transform enemyAnchor = battleArenaController.Default3DEnemyPositions[currentIndex];
				
				// Instantiate the prefab and set its positions and shit.
				GameObject obj = GameObject.Instantiate(enemyTemplate.EnemyPrefab);
				obj.transform.SetPositionAndRotation(position: enemyAnchor.position, rotation: enemyAnchor.rotation);

				// Grab its WorldEnemy component and return it.
				WorldEnemyDX worldEnemy = obj.GetComponent<WorldEnemyDX>();
				return worldEnemy;

			} else {
				// If not, assume its a sprite, grab a reference, and turn it on.
				Debug.Assert(enemyTemplate.WorldEnemyType == WorldEnemyType.Sprite);
				WorldEnemyDX worldEnemy = battleArenaController.DefaultEnemySprites[currentIndex];
				battleArenaController.DefaultEnemySprites[currentIndex].gameObject.SetActive(true);
				return worldEnemy;
			}
		}
		#endregion
		
	}

}