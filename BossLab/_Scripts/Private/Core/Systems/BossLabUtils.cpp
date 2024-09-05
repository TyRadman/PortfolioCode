// Fill out your copyright notice in the Description page of Project Settings.


#include "Core/Systems/BossLabUtils.h"

void UBossLabUtils::Print(const FString message)
{
	GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, message);
}

void UBossLabUtils::Print(const FString message, float duration)
{
	GEngine->AddOnScreenDebugMessage(-1, duration, FColor::Red, message);
}

void UBossLabUtils::Print(const FString message, const FColor color)
{
	GEngine->AddOnScreenDebugMessage(-1, 5.0f, color, message);
}

void UBossLabUtils::Print(const FString message, float duration, const FColor color)
{
	GEngine->AddOnScreenDebugMessage(-1, duration, color, message);
}
