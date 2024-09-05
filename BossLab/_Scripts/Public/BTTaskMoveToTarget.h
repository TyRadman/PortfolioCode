#pragma once

#include "CoreMinimal.h"
#include "BehaviorTree/BTTaskNode.h"
#include "BTTaskMoveToTarget.generated.h"

UCLASS()
class BOSSLAB_API UBTTaskMoveToTarget : public UBTTaskNode
{
	GENERATED_BODY()

public:
	// UBTTaskMoveToTarget();

protected:
	// virtual EBTNodeResult::Type ExecuteTask(UBehaviorTreeComponent& OwnerComp, uint8* NodeMemory) override;
	// virtual EBTNodeResult::Type AbortTask(UBehaviorTreeComponent& OwnerComp, uint8* NodeMemory) override;

private:
	// UFUNCTION()
	// void OnMoveCompleted(FAIRequestID RequestID, const FPathFollowingResult& Result);

	UPROPERTY()
	AAIController* AIController;

	UPROPERTY()
	UBehaviorTreeComponent* OwnerCompRef;
};
