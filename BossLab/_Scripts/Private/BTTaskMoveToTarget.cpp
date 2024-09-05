#include "BTTaskMoveToTarget.h"
#include "AIController.h"
#include "GameFramework/Character.h"
#include "Kismet/GameplayStatics.h"
#include "BehaviorTree/BlackboardComponent.h"
#include "Navigation/PathFollowingComponent.h"

// UBTTaskMoveToTarget::UBTTaskMoveToTarget()
// {
// 	NodeName = "Move To Target";
// }
//
// EBTNodeResult::Type UBTTaskMoveToTarget::ExecuteTask(UBehaviorTreeComponent& OwnerComp, uint8* NodeMemory)
// {
// 	AIController = OwnerComp.GetAIOwner();
// 	if (AIController)
// 	{
// 		ACharacter* PlayerCharacter = UGameplayStatics::GetPlayerCharacter(GetWorld(), 0);
// 		if (PlayerCharacter)
// 		{
// 			// Store the OwnerComp reference
// 			OwnerCompRef = &OwnerComp;
//
// 			// Move to the player character
// 			FPathFollowingRequestResult Result = AIController->MoveToActor(PlayerCharacter, 100.0f);  // Set acceptance radius to 100 units
// 			if (Result.Code == EPathFollowingRequestResult::RequestSuccessful)
// 			{
// 				AIController->GetPathFollowingComponent()->OnRequestFinished.AddUObject(this, &UBTTaskMoveToTarget::OnMoveCompleted);
// 				return EBTNodeResult::InProgress;
// 			}
// 		}
// 	}
//
// 	return EBTNodeResult::Failed;
// }
//
// void UBTTaskMoveToTarget::OnMoveCompleted(FAIRequestID RequestID, const FPathFollowingResult& Result)
// {
// 	if (Result.Code == EPathFollowingResult::Success)
// 	{
// 		FinishLatentTask(*OwnerCompRef, EBTNodeResult::Succeeded);
// 	}
// 	else
// 	{
// 		FinishLatentTask(*OwnerCompRef, EBTNodeResult::Failed);
// 	}
// }
//
// EBTNodeResult::Type UBTTaskMoveToTarget::AbortTask(UBehaviorTreeComponent& OwnerComp, uint8* NodeMemory)
// {
// 	if (AIController)
// 	{
// 		AIController->StopMovement();
// 	}
//
// 	return Super::AbortTask(OwnerComp, NodeMemory);
// }
