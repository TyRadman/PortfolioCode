// Fill out your copyright notice in the Description page of Project Settings.
#include "AIC_BossAIController.h"
#include "Math/UnrealMathUtility.h"
#include "Algo/Copy.h"
#include "Algo/MaxElement.h"

void AAIC_BossAIController::StartBoss_Implementation()
{
}

void AAIC_BossAIController::StartTheBoss()
{
	
}

void AAIC_BossAIController::SetNextState(AttackRangeType rangeType)
{
	if(BehaviorChances.Num() == 0)
	{
		return;
	}

	TArray<UBehaviourChance*> selectedBahviorChances;
	Algo::CopyIf(BehaviorChances, selectedBahviorChances, [rangeType](const UBehaviourChance* action)
	{
		return action && action->RangeType >= rangeType;
	});

	// if there are no behaviors of the selected attack range type, then use any attack in the array
	if(selectedBahviorChances.Num() == 0)
	{
		selectedBahviorChances = BehaviorChances;
	}

	// get the action with the highest current chance
	UBehaviourChance* highestChanceAction = *Algo::MaxElement(BehaviorChances, [](const UBehaviourChance* A, const UBehaviourChance* B)
	{
		return A->CurrentChance < B->CurrentChance;
	});

	if(!highestChanceAction)
	{
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green, FString::Printf(TEXT("No behaviour found.")));
		return;
	}

	float highestChance = highestChanceAction->CurrentChance;
	float selectedChance = FMath::RandRange(0.0f, highestChance);

	// cache all the actions that fall within the random value's range
	TArray<UBehaviourChance*> selectedActions;
	Algo::CopyIf(BehaviorChances, selectedActions, [selectedChance](const UBehaviourChance* action)
	{
		return action && action->CurrentChance >= selectedChance;
	});

	UBehaviourChance* selectedAction = selectedActions[FMath::RandRange(0, selectedActions.Num() - 1)];
	selectedAction->CurrentChance /= 2.0f;

	// set the current behavior to the blackboard
	if(Blackboard && selectedAction && selectedAction->Behavior)
	{
		Blackboard->SetValueAsObject(M_BehaviourKey, selectedAction->Behavior);
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green,
			FString::Printf(TEXT("Behavior %s"), *selectedAction->Behavior->GetName()));
	}
	else
	{
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green, FString::Printf(TEXT("Blackboard, selected action, or the bahviour is missing")));
		return;	
	}
	
	// reset the values for the other actions
	for(UBehaviourChance* action : BehaviorChances)
	{
		// if the action to check is the same as the selected one or if the action's current chance is the same as the default chance, then there's no point setting it
		if(action == selectedAction || action->CurrentChance == action->DefaultChance)
		{
			continue;
		}

		// multiply the current chance by two and ensure it's not greater than the default chance
		action->CurrentChance = FMath::Min(action->CurrentChance * 2.0f, action->DefaultChance);
	}
}

void AAIC_BossAIController::SetUpActionChances()
{
	for(UBehaviourChance* action : BehaviorChances)
	{
		action->CurrentChance = action->DefaultChance;
	}
}

void AAIC_BossAIController::AddActionChance(UBehaviourChance* newAction)
{
	if(BehaviorChances.Contains(newAction) && newAction != nullptr)
	{
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Green,
			FString::Printf(TEXT("Behavior %s already exists in the boss's behavior list."), *newAction->Behavior->GetName()));
		return;
	}
	
	BehaviorChances.Add(newAction);
	SetUpActionChances();
}