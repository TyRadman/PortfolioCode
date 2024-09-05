// Fill out your copyright notice in the Description page of Project Settings.

#include "Core/Detectors/ArcDetectorComponent.h"

// Sets default values for this component's properties
UArcDetectorComponent::UArcDetectorComponent()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}


// Called when the game starts
void UArcDetectorComponent::BeginPlay()
{
	Super::BeginPlay();
	// ...
}


// Called every frame
void UArcDetectorComponent::TickComponent(float DeltaTime, ELevelTick TickType,
                                          FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);
	// ...

	FVector StartPoint = DetectFromActorLocation? GetOwner()->GetActorLocation() : DetectionStartPoint;
	FVector OwnerForwardVector = GetOwner()->GetActorForwardVector();
	FVector EndPoint1 = StartPoint + OwnerForwardVector.RotateAngleAxis(-SectorAngle / 2, FVector::UpVector) * Range;
	FVector EndPoint2 = StartPoint + OwnerForwardVector.RotateAngleAxis(SectorAngle / 2, FVector::UpVector) * Range;

	TArray<FHitResult> HitArray;

	bool IsHitDetected = false;
	FCollisionObjectQueryParams ObjectQueryParams = FCollisionObjectQueryParams();
	FCollisionQueryParams CollisionQueryParams = FCollisionQueryParams();

	CollisionQueryParams.AddIgnoredActor(GetOwner());
	ObjectQueryParams.AddObjectTypesToQuery(ECC_Pawn);

	// for (ECollisionChannel Channel : TargetCollisionChannels)
	// {
	// 	ObjectQueryParams.AddObjectTypesToQuery(Channel);
	// }

	if (GetWorld()->SweepMultiByObjectType(HitArray, StartPoint, StartPoint, FQuat::Identity, ObjectQueryParams,
	                                       FCollisionShape::MakeSphere(Range), CollisionQueryParams))
	{
		// TODO: Try to see if only 1 array can be used to get the output 
		TMap<FString, AActor*> UniqueActors;
		TArray<AActor*> HitActorsList;

		for (auto HitResult : HitArray)
		{
			AActor* HitActor = HitResult.GetActor();
			if (HitActor)
			{
				FString ActorName = HitActor->GetName();

				// Check if we've already encountered this actor type
				if (!UniqueActors.Contains(ActorName))
				{
					// Add the first hit of this type to the result
					UniqueActors.Add(ActorName, HitActor);

					FVector SectorCenter = StartPoint + OwnerForwardVector * Range;
					bool IsInRange = IsInSector(StartPoint, SectorCenter, SectorAngle, Range,
					                            HitResult.GetActor()->GetActorLocation());

					if (IsInRange)
					{
						if (!IsHitDetected)
						{
							IsHitDetected = true;
						}
						HitActorsList.Add(HitResult.GetActor());
						UE_LOG(LogTemp, Warning, TEXT("Hit Detected on NAME: %s"), *HitResult.GetActor()->GetName());
					}
				}
			}
		}

		if(IsHitDetected)
		{
			OnDetectHit.Broadcast(HitActorsList);
		}
	}

	if (EnableDebug)
	{
		DrawDebugLine(GetWorld(), StartPoint, EndPoint1, IsHitDetected ? FColor::Green : FColor::Blue, false, -1, 0, 2);
		DrawDebugLine(GetWorld(), StartPoint, EndPoint2, IsHitDetected ? FColor::Green : FColor::Blue, false, -1, 0, 2);
		DrawDebugSphere(GetWorld(), StartPoint, Range, 15, IsHitDetected ? FColor::Green : FColor::Red, false, -1, 0,
		                1);
	}
}

bool UArcDetectorComponent::IsInSector(const FVector& PlayerPosition, const FVector& SectorCenter, float Angle,
                                       float SectorRadius, const FVector& EnemyPosition)
{
	FVector PlayerToCenterDir = (SectorCenter - PlayerPosition).GetSafeNormal();
	// Calculate angle between player's forward vector and direction to enemy
	FVector DirectionToEnemy = (EnemyPosition - PlayerPosition).GetSafeNormal();
	float AngleToEnemy = FMath::RadiansToDegrees(FMath::Acos(FVector::DotProduct(PlayerToCenterDir, DirectionToEnemy)));

	// Check if the enemy is within the sector angle range
	bool WithinAngleRange = AngleToEnemy <= Angle / 2.0f;

	// Check if the enemy is within the sector radius
	float DistanceToEnemy = (EnemyPosition - PlayerPosition).Size();
	bool WithinRadius = DistanceToEnemy <= SectorRadius;

	// Check if the enemy is within both the angle range and the radius
	return WithinAngleRange && WithinRadius;
}

void UArcDetectorComponent::ActivateArc(float _Range, float _Angle)
{
	
}
