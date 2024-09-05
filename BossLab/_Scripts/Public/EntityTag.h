#pragma once

UENUM(BlueprintType)
enum class EEntityTag : uint8
{
	EET_Player = 0 UMETA(DisplayName = "Player"),
	EET_Boss = 1 UMETA(DisplayName = "Boss")
};