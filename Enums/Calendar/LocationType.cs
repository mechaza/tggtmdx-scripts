using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Data;

namespace Grawly.Calendar {

	/// <summary>
	/// Some "locations" in game may have different scenes depending on the time of day.
	/// E.x., should load Blanche House Morning scene for morning times, but Blanche House Evening for the evening.
	/// This is mostly relevant when going between scenes without triggering a time/day transition.
	/// </summary>
	public enum LocationType {
		
		None				= 0,
		Initialization		= 1,
		
		// MENU SCREENS
		MapScreen			= 21,
		
		// HOME LOCATIONS
		InsideHome			= 51,
		OutsideHome			= 52,
		
		// CITY LOCATIONS : SHOPPING
		ShoppingDistrict	= 101,
		MiamiMallLobby		= 102,
		MiamiMallBathroom	= 103,
		MiamiMallOutside	= 104,
		// CITY LOCATIONS : HOTEL
		HotelOutside		= 121,
		HotelLobby			= 122,
		HotelRoom			= 123,
		// CITY LOCATIONS : PUBLIC
		CityPark			= 141,
		// CITY LOCATIONS : OTHER
		PopStarInside		= 181,
		
		// DUNGEON LOCATIONS
		DungeonLobby		= 201,
		
		// PROTOTYPE LOCATIONS
		Proto1				= 501,
		Proto2				= 502,
		Proto3				= 503,
		Proto4				= 504,
		Proto5				= 505,
		Proto6				= 506,
		Proto7				= 507,
		Proto8				= 508,
		Proto9				= 509,
		
		// CUTSCENE LOCATIONS : CITY (SHOPPING)
		C_ShoppingDistrict	= 10101,
		C_MiamiMallLobby	= 10102,
		C_MiamiMallBathroom	= 10103,
		C_MiamiMallOutside	= 10104,
		// CUTSCENE LOCATIONS : CITY (HOTEL)
		C_HotelOutside		= 10121,
		C_HotelLobby		= 10122,
		C_HotelRoom			= 10123,
		
		
		/*//
		//
		//		EVERYTHING BELOW SHOULD BE DELETED IN THE NEAR FUTURE.
		//		I'M MAKING THESE DEFINITIONS TO QUICK-FIX THINGS FOR THE PORTFOLIO DEMO.
		//
		//
		
		
		
		// PORTFOLIO BANDAID LOCATIONS (DAY 4)
		DAY4_ShoppingDistrict	= 204101,
		DAY4_MiamiMallLobby		= 204102,
		DAY4_MiamiMallBathroom	= 204103,
		DAY4_MiamiMallOutside	= 204104,
		DAY4_HotelOutside		= 204121,
		DAY4_HotelLobby			= 204122,
		DAY4_HotelRoom			= 204123,
		// PORTFOLIO BANDAID LOCATIONS (DAY 5)
		DAY5_ShoppingDistrict	= 205101,
		DAY5_MiamiMallLobby		= 205102,
		DAY5_MiamiMallBathroom	= 205103,
		DAY5_MiamiMallOutside	= 205104,
		DAY5_HotelOutside		= 205121,
		DAY5_HotelLobby			= 205122,
		DAY5_HotelRoom			= 205123,
		// PORTFOLIO BANDAID LOCATIONS (DAY 6)
		DAY6_ShoppingDistrict	= 206101,
		DAY6_MiamiMallLobby		= 206102,
		DAY6_MiamiMallBathroom	= 206103,
		DAY6_MiamiMallOutside	= 206104,
		DAY6_HotelOutside		= 206121,
		DAY6_HotelLobby			= 206122,
		DAY6_HotelRoom			= 206123,
		// PORTFOLIO BANDAID LOCATIONS (DAY 7)
		DAY7_ShoppingDistrict	= 207101,
		DAY7_MiamiMallLobby		= 207102,
		DAY7_MiamiMallBathroom	= 207103,
		DAY7_MiamiMallOutside	= 207104,
		DAY7_HotelOutside		= 207121,
		DAY7_HotelLobby			= 207122,
		DAY7_HotelRoom			= 207123,
		// PORTFOLIO BANDAID LOCATIONS (DAY 8)
		DAY8_ShoppingDistrict	= 208101,
		DAY8_MiamiMallLobby		= 208102,
		DAY8_MiamiMallBathroom	= 208103,
		DAY8_MiamiMallOutside	= 208104,
		DAY8_HotelOutside		= 208121,
		DAY8_HotelLobby			= 208122,
		DAY8_HotelRoom			= 208123,*/
		
		
		
	}


}