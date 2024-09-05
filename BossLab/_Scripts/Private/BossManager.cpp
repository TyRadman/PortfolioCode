// Fill out your copyright notice in the Description page of Project Settings.


#include "BossManager.h"
#include "Core/Systems/BossLabUtils.h"

void UBossManager::SetBoss(AActor* NewBoss)
{
    if(!NewBoss)
    {
        UBossLabUtils::Print("No boss reference passed to the boss manager.");
        return;
    }
    
    Boss = NewBoss;
    AController* bossPawn = Cast<APawn>(Boss)->GetController();

    if(!bossPawn || !Cast<AAIController>(bossPawn))
    {
        UBossLabUtils::Print("Boss reference does not have an AI controller.");
        // GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, FString::Printf(TEXT("Boss reference does not have an AI controller.")));
        return;
    }
    
    BossAIController = Cast<AAIC_BossAIController>(bossPawn);
}

AActor* UBossManager::GetBoss() const
{
    if(!Boss)
    {
        GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, FString::Printf(TEXT("No Boss Reference at boss manager.")));
        return nullptr;
    }
    
    return Boss;
}

AAIC_BossAIController* UBossManager::GetBossAIController() const
{
    if(!BossAIController)
    {
        GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, FString::Printf(TEXT("No AI Controller Reference at boss manager.")));
        return nullptr;
    }
    
    return BossAIController;
}
