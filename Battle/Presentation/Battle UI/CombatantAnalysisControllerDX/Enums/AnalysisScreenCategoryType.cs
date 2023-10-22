using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;

namespace Grawly.Battle.Analysis {

	/// <summary>
	/// The different ways in which the Combatant Analysis DX screen can be built.
	/// </summary>
	public enum AnalysisScreenCategoryType {
		None					= 0,
		Override				= 1,	// This actually means control is handled by scripts elsewhere.
		
		BattleEnemies			= 101,	// Analyze enemies in battle.
		PersonaLevelUp			= 201,	// Leveling up a persona.
		LearnSkillCard			= 301,	// Learning a skill card.
		
		
	}
	
}