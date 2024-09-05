// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Core/Systems/PlayerStats/PlayerConfigData.h"
#include "Core/Systems/PlayerStats/PlayerStatsData.h"
#include "Core/Systems/PlayerStats/PlayerUpgradeSaveData.h"
#include "Subsystems/GameInstanceSubsystem.h"
#include "PlayerManager.generated.h"

/**
 * 
 */
UCLASS()
class BOSSLAB_API UPlayerManager : public UGameInstanceSubsystem
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintCallable)
	void InitData(UPlayerConfigData* StatsData);
	
	UFUNCTION(BlueprintCallable)
	bool AddStatPoint();
	UFUNCTION(BlueprintCallable)
	bool RemoveStatPoint();
	UFUNCTION(BlueprintCallable)
	int GetStatPoint();

	UFUNCTION(BlueprintCallable)
	bool AddHealthStat();
	UFUNCTION(BlueprintCallable)
	bool RemoveHealthStat();
	UFUNCTION(BlueprintCallable)
	int GetHealthStat();
	
	UFUNCTION(BlueprintCallable)
	bool AddDamageStat();
	UFUNCTION(BlueprintCallable)
	bool RemoveDamageStat();
	UFUNCTION(BlueprintCallable)
	int GetDamageStat();


	UFUNCTION(BlueprintCallable)
	bool AddFlaskStat();
	UFUNCTION(BlueprintCallable)
	bool RemoveFlaskStat();
	UFUNCTION(BlueprintCallable)
	int GetFlaskStat();
	UFUNCTION(BlueprintCallable)
	int GetTotalFlaskCount();
	UFUNCTION(BlueprintCallable)
	float GetFlaskHealAmount();
	UFUNCTION(BlueprintCallable)
	float GetFlaskHealPercent();

	// TODO: Deprecate these!
	UFUNCTION(BlueprintCallable)
	bool AddActionStat();
	UFUNCTION(BlueprintCallable)
	bool RemoveActionStat();
	UFUNCTION(BlueprintCallable)
	int  GetBonusActionStat();
	UFUNCTION(BlueprintCallable)
	int  GetTotalActionStat();
	
	UFUNCTION(BlueprintCallable)
	int GetBaseActionStats();
	UFUNCTION(BlueprintCallable)
	int GetActionStatRegenRate();

	// TODO: Deprecate these!
	UFUNCTION(BlueprintCallable)
	bool AddComboStat();
	UFUNCTION(BlueprintCallable)
	bool RemoveComboStat();
	UFUNCTION(BlueprintCallable)
	int GetComboStat();

	
	UFUNCTION(BlueprintCallable)
	bool EnableRangedAttack(bool Enable);
	UFUNCTION(BlueprintCallable)
	bool IsRangedAttackEnabled();

	// TODO: Deprecate these!
	UFUNCTION(BlueprintCallable)
	bool EnableQuickerDash(bool Enable);
	UFUNCTION(BlueprintCallable)
	bool IsQuickerDashEnabled();

	UFUNCTION(BlueprintCallable)
	FPlayerStatsBonusData GetDamageBonusData(float BaseDamage);

	UFUNCTION(BlueprintCallable)
	FPlayerStatsBonusData GetHealthBonusData();

	UFUNCTION(BlueprintCallable)
	void Reset();

	UFUNCTION(BlueprintCallable)
	int GetInitialModPoints();

	UFUNCTION(BlueprintCallable)
	FPlayerUpgradeSaveData GetPlayerUpgradeData();

	UFUNCTION(BlueprintCallable)
	void SetPlayerUpgradeData();


private:
	int _AvailableStats = 0;
	int _CurrentHealthStats = 0;
	int _CurrentDamageStats = 0;
	int _CurrentFlaskStats = 0;
	FPlayerStatsData _CurrentStatsData;
	bool _IsDashPerkUpgraded;
	bool _IsRangedPerkUpgraded;

	UPlayerConfigData* _ConfigData;

	
	// TODO: Deprecate this.
	int _CurrentComboStats = 0;
	int _CurrentActionStats = 0;
	bool _IsChargeAttackUnlocked = false;
	bool _IsQuickerDashUnlocked = false;
};
