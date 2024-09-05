// Fill out your copyright notice in the Description page of Project Settings.


#include "Shooter.h"

#include "Core/Systems/BossLabUtils.h"
#include "Core/EProjectilePatternType.h"
#include "math/UnrealMathUtility.h"
#include "Core/Projectile.h"


// Sets default values for this component's properties
UShooter::UShooter()
{
	PrimaryComponentTick.bCanEverTick = true;
}


// Called when the game starts
void UShooter::BeginPlay()
{
	Super::BeginPlay();
}


// Called every frame
void UShooter::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	if(!M_IsShooting)
	{
		return;
	}

	ShootProjectileProcess();
}

void UShooter::SetShotData(UShootingAttackData* data)
{
	CurrentData = data;
}

void UShooter::Shoot(UShootingAttackData* data)
{
	// if new data is fed, then we set it as the current data (Temporary)
	if(data)
	{
		CurrentData = data;
	}

	if (!CurrentData)
	{
		UBossLabUtils::Print("No valid data");
		return;		
	}
	
	M_TimeBetweenShots = CurrentData->Duration / static_cast<float>(CurrentData->ShotsNumber);
	M_ProjectilesShotCount = 0;
	M_IsShooting = true;
	
	if(CurrentData->PatternType == UEProjectilePatternType::Split)
	{
		SpawnSplitProjectiles();
	}
	else if(CurrentData->PatternType == UEProjectilePatternType::Parallel)
	{
		SpawnParallelProjectiles();
	}
	else
	{
		SpawnSingleProjectile(0, SpawnPoint->GetComponentLocation());
	}
}

void UShooter::ShootProjectileProcess()
{
	M_TimeElapsed += GetWorld()->GetDeltaSeconds();

	if(M_TimeElapsed > M_TimeBetweenShots)
	{
		// reset the timer
		M_TimeElapsed = 0.0f;
		M_ProjectilesShotCount++;

		// if the number of the projectiles already shot is reached the number of shots this attack data is supposed to shoot, then stop the whole process
		if(M_ProjectilesShotCount >= CurrentData->ShotsNumber)
		{
			M_IsShooting = false;
			return;
		}

		if(CurrentData->PatternType == UEProjectilePatternType::Split)
		{
			SpawnSplitProjectiles();
		}
		else if(CurrentData->PatternType == UEProjectilePatternType::Parallel)
		{
			SpawnParallelProjectiles();
		}
		else if(CurrentData->PatternType == UEProjectilePatternType::None)
		{
			SpawnSingleProjectile(0, SpawnPoint->GetComponentLocation());
		}
	}
}

void UShooter::SpawnParallelProjectiles()
{
	float horizontalOffset = CurrentData->HorizontalDistance / (CurrentData->ParallelCount - 1);
	float startOffsetPoint = -CurrentData->HorizontalDistance / 2.0f;
	
	for(int i = 0; i <  CurrentData->ParallelCount; i++)
	{
		FVector rightVector = SpawnPoint->GetRightVector();
		float offset = startOffsetPoint + horizontalOffset * i;
		FVector leftVector = rightVector * offset;
		FVector location = leftVector + SpawnPoint->GetComponentLocation();
		SpawnSingleProjectile(0.0f, location);
	}
}

#pragma region Split Logic
void UShooter::SpawnSplitProjectiles()
{
	// if there's only one projectile in the split, then center it without calculating the angle
	if(CurrentData->SplitsCount == 1)
	{
		// GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, FString::Printf(TEXT("Single shot mode")));
		SpawnSingleProjectile(0, SpawnPoint->GetComponentLocation());
		return;
	}
	
	float anglePerShot = CurrentData->Angle / (CurrentData->SplitsCount - 1);
	float startAngle = -CurrentData->Angle / 2.0f;
		
	for(int i = 0; i <  CurrentData->SplitsCount; i++)
	{
		float angle = startAngle + anglePerShot * static_cast<float>(i);
		SpawnSingleProjectile(angle, SpawnPoint->GetComponentLocation());
	}
}

#pragma endregion 
void UShooter::SpawnSingleProjectile(float angle, FVector location)
{
	FRotator rotation = SpawnPoint->GetComponentRotation();
	// add the angle
	rotation.Yaw += angle;
	TSubclassOf<AProjectile> bulletReference = CurrentData->ProjectileReference;
	FActorSpawnParameters spawnParams;
	spawnParams.Owner = GetOwner();
	spawnParams.Instigator = GetOwner()->GetInstigator();
	spawnParams.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;
	
	AProjectile* spawnedProjectile = GetWorld()->SpawnActor<AProjectile>(bulletReference, location, rotation, spawnParams);
	// spawnedProjectile->ProjectileMovementReference->StopMovementImmediately();	

	// apply modifiers
	spawnedProjectile->IsAOE = CurrentData->IsAreaOfEffect;
	spawnedProjectile->AreaOfEffectRadius = CurrentData->AreaOfEffectRadius;
	spawnedProjectile->IsHoming = CurrentData->IsHoming;
	spawnedProjectile->Accuracy = CurrentData->Accuracy;
	spawnedProjectile->IsRicochet = CurrentData->IsRicochet;
	spawnedProjectile->DeflectionsCount = CurrentData->DeflectionsCount;

	spawnedProjectile->SetProjectileSettings(CurrentData, rotation);
}