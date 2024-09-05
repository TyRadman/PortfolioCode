// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "UObject/NoExportTypes.h"
#include "EProjectilePatternType.generated.h"

UENUM(BlueprintType)
enum class UEProjectilePatternType : uint8
{
	Split = 0,
	Parallel = 1,
	None = 2
};
