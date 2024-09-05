// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "ActionStatData.h"
#include "DamageStatData.h"
#include "DashUpgradePerkData.h"
#include "FlaskStatData.h"
#include "HealthStatData.h"
#include "PlayerStatsData.h"
#include "RangedAttackPerkData.h"
#include "Engine/DataAsset.h"
#include "PlayerConfigData.generated.h"

/**
 * 
 */
UCLASS()
class BOSSLAB_API UPlayerConfigData : public UDataAsset
{
	GENERATED_BODY()

public:
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	FPlayerStatsData StatsData;

	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	FActionStatData ActionStatData;

	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	FHealthStatData HealthStatData;

	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	FDamageStatData DamageStatData;
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	FFlaskStatData FlaskStatData;

	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	FDashUpgradePerkData DashUpgradePerkData;

	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	FRangedAttackPerkData RangedAttackPerkData;
};
