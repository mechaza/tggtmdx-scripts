using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using Grawly.Battle;
using Grawly.Chat;
using DG.Tweening;
using Sirenix.OdinInspector;
using Grawly.Story;
using Grawly.Toggles;
using Grawly.Toggles.Audio;
using UnityEngine.AI;

namespace Grawly.Dungeon.Legacy {

	/// <summary>
	/// The mean little boys who roam the dungeons and can initate combat.
	/// </summary>
	[RequireComponent(typeof(NavMeshAgent))]
	public class DungeonEnemy : MonoBehaviour {

		#region FIELDS - BATTLE VARIABELS
		/// <summary>
		/// Should this dungeon enemy make use of its own battle templates
		/// instead of being prepared externally?
		/// </summary>
		[TabGroup("Enemy", "Battle"), SerializeField]
		private bool usePrivateBattleTemplates = false;
		/// <summary>
		/// The different spawns that could be used for this enemy. Note on awake, one will randomly be assigned to the enemies variable.
		/// </summary>
		[TabGroup("Enemy", "Battle"), SerializeField, ShowIf("usePrivateBattleTemplates")]
		private List<BattleTemplate> battleTemplates = new List<BattleTemplate>();
		/// <summary>
		/// The enemy spawn template that will be used for this enemy.
		/// </summary>
		[HideInInspector]
		public BattleTemplate battleTemplate;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The state machine attached to this enemy.
		/// </summary>
		[HideInInspector]
		public PlayMakerFSM fsm;
		/// <summary>
		/// The body of the enemy so that it can be bobbed up and down.
		/// </summary>
		[TabGroup("Enemy", "Scene References"), SerializeField]
		private GameObject enemyBody;
		/// <summary>
		/// The sprite that shows what the enemy's current emotion is.
		/// </summary>
		[TabGroup("Enemy", "Scene References"), SerializeField]
		private SpriteRenderer emotionSprite;
		/// <summary>
		/// The NavMesh agent that will control pathfinding.
		/// </summary>
		private NavMeshAgent Agent { get; set; } 
		#endregion

		#region FIELDS - TOGGLES :  ANIMATION VARIABLES
		/// <summary>
		/// The speed that the enemy walks around at.
		/// </summary>
		[TabGroup("Enemy", "Animation")]
		public float walkSpeed = 1f;
		/// <summary>
		/// The speed that the enemy chases the player at.
		/// </summary>
		[TabGroup("Enemy", "Animation")]
		public float chaseSpeed = 2f;
		/// <summary>
		/// The rate at which animations on the dungeon enemy are performed.
		/// </summary>
		[TabGroup("Enemy", "Animation")]
		public float bobAmplitude = 1f;
		/// <summary>
		/// The max amount of distance the bob animation should travel.
		/// </summary>
		[TabGroup("Enemy", "Animation")]
		public float bobDistance = 0.2f;
		#endregion

		#region FIELDS - RESOURCES
		/// <summary>
		/// The particle effect to use when spawning this enemy.
		/// </summary>
		[TabGroup("Enemy", "Resources"), SerializeField]
		private GameObject spawnParticleEffect;
		#endregion

		#region FIELDS -  MOSTLY DEBUG VARIABLES. REFACTOR THESE THOUGH IM STILL USING THEM
		/// <summary>
		/// The collider for the enemy's field of vision.
		/// </summary>
		[TabGroup("Enemy", "Debug"), SerializeField]
		private GameObject visionCone;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			fsm = GetComponent<PlayMakerFSM>();
			this.Agent = this.GetComponent<NavMeshAgent>();
		}
		private void Start() {
			
			// Pick out a random template if using personal templates.
			if (this.usePrivateBattleTemplates == true) {
				BattleTemplate randomTemplate = this.battleTemplates.Random();
				this.Prepare(battleTemplate: randomTemplate);
			}
			
			
			// this.enemyBody.SetActive(false);
			// this.visionCone.SetActive(false);
		}
		private void Update() {
			
			if (this.GetFSMState() == DungeonEnemyStateType.Alert && BattleController.Instance.IsBattling == false) {
				if (DungeonPlayer.Instance.GetFSMState() == DungeonPlayerStateType.Free) {
					// Make the enemy move towards the player if the player is in Free
					// this.ChaseTarget(targetTransform: DungeonPlayer.Instance.transform);
				}
				// Billboard the emotion sprite
				this.BillboardEmotionSprite();
			}

		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Preps this dungeon enemy to use the specified BattleTemplate upon collision.
		/// </summary>
		/// <param name="battleTemplate">The BattleTemplate to attach to this DungeonEnemy.</param>
		public void Prepare(BattleTemplate battleTemplate) {
			this.battleTemplate = battleTemplate;
		}
		#endregion
		
		#region UPDATE METHODS
		/// <summary>
		/// Tells this DungeonEnemy to chase after a specified target.
		/// </summary>
		/// <param name="targetTransform">The actual target to chase.</param>
		public void ChaseTarget(Transform targetTransform) {
			
			this.Agent.SetDestination(target: targetTransform.position);
			// transform.LookAt(DungeonPlayer.Instance.transform);
			// transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
			// transform.Translate(Vector3.forward * Time.deltaTime * chaseSpeed, transform);
		}
		/// <summary>
		/// Makes the emotion sprite that appears above the enemy face towards the camera.
		/// </summary>
		private void BillboardEmotionSprite() {
			emotionSprite.transform.LookAt(Camera.main.transform, Vector3.up);
		}
		#endregion

		#region STATE MANIPULATION
		/// <summary>
		/// Tells the FSM the spawn event.
		/// </summary>
		[TabGroup("Enemy", "Debug"), ShowInInspector]
		public void SpawnEnemy() {
			this.fsm.SendEvent("Spawn");
		}
		/// <summary>
		/// Spawns the enemy and makes em go poof.
		/// </summary>
		public void SpawnEnemyEffect() {
			
			this.visionCone.SetActive(true);
			this.enemyBody.SetActive(true);
			
			// Play the sound effect.
			AudioController.instance.PlaySFX(type: SFXType.EnemySpawn);
			
			// Instansiate some particle effects.
			GameObject.Instantiate(this.spawnParticleEffect, parent: this.transform).transform
				.SetPositionAndRotation(
				position: this.transform.position,
				rotation: this.transform.rotation);
		}
		/// <summary>
		/// Returns the state of the DungeonEnemy's FSM as an enum.
		/// </summary>
		public DungeonEnemyStateType GetFSMState() {
			switch (fsm.ActiveStateName) {
				case "Docile":
					return DungeonEnemyStateType.Docile;
				case "Alert":
					return DungeonEnemyStateType.Alert;
				default:
					// Debug.LogError("Couldn't determine DungeonEnemy FSM state!");
					return DungeonEnemyStateType.ERROR;
			}
		}
		#endregion

		#region GETTERS
		/// <summary>
		/// Gets the distance from this enemy to the player.
		/// </summary>
		/// <param name="playerTransform">The player transform to get.</param>
		/// <returns></returns>
		public float DistanceFromPlayer(Transform playerTransform) {
			return Vector3.Distance(a: this.transform.position, b: playerTransform.position);
		}
		#endregion

		#region GRAPHICS AND AUDIO
		public void SetEmotionSprite(bool status) {
			emotionSprite.gameObject.SetActive(status);
			if (status == true) {
				// If the emotion sprite on the enemy is being turned on, give the sprite a little shake effect.
				emotionSprite.transform.DOShakePosition(0.5f, 0.2f, 40);
			}
		}
		/// <summary>
		/// Plays the audio clip for when a player enter's the shadow's field of vision.
		/// Gets called inside DungeonEnemyFOV.
		/// </summary>
		public void PlayDetectionAudio() {
			AudioController.instance.PlaySFX(type: SFXType.EnemyNotice);
			/*this.audioSource.PlayOneShot(
				clip: DataController.GetDefaultSFX(type: SFXType.EnemyNotice), 
				volumeScale: GameController.Instance.Config.sfxVolumeScale);*/
		}
		#endregion


	}
}