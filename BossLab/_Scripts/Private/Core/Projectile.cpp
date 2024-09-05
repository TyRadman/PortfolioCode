// Fill out your copyright notice in the Description page of Project Settings.

#include "Core/Projectile.h"
#include "Core/Systems/BossLabUtils.h"
#include "Engine/World.h"
#include "GameFramework/Actor.h"
#include "EngineUtils.h"
#include "DrawDebugHelpers.h"
#include "ShootingAttackData.h"
#include "GameFramework/Character.h"
#include "Kismet/GameplayStatics.h"

// Sets default values
AProjectile::AProjectile()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
}

// Called when the game starts or when spawned
void AProjectile::BeginPlay()
{
	Super::BeginPlay();

	// get the reference to the projectile movement
	ProjectileMovementReference = FindComponentByClass<UProjectileMovementComponent>();
	M_HomingTarget = UGameplayStatics::GetPlayerCharacter(GetWorld(), 0);
}

// Called every frame
void AProjectile::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	PerformHoming();
}

void AProjectile::PerformHoming()
{
	if(!IsHoming)
	{
		return;
	}

	if(M_HomingTarget)
	{
		FVector direction = (M_HomingTarget->GetActorLocation() - GetActorLocation()).GetSafeNormal();
		float targetAngle = FMath::Fmod(FMath::Atan2(direction.Y, direction.X) * (180.0f / PI) + 360.0f, 360.0f);
		float currentAngle = FMath::Fmod(GetActorRotation().Yaw + 360.0f, 360.0f);

		if(currentAngle != targetAngle)
		{
			FVector forward = GetActorForwardVector();
			FVector crossProduct = FVector::CrossProduct(forward, direction);
			float sign = FMath::Sign(crossProduct.Z);
			float newAngle = currentAngle + sign * Accuracy * GetWorld()->GetDeltaSeconds();
			
			FRotator newRotation(0.0f, 0.0f, 0.0f);
			newRotation.Yaw = newAngle;

			ProjectileMovementReference->Velocity = newRotation.Vector() * ProjectileSpeed;
		}
	}
}


void AProjectile::OnProjectileHit(UPrimitiveComponent* HitComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, FVector NormalImpulse, const FHitResult& Hit)
{
	IIDamageSystemInterface* target = Cast<IIDamageSystemInterface>(OtherActor);
	bool someBool = true;

	if(target)
	{
		if(target->Execute_IsDead(OtherActor))
		{
			return;	
		}
		
		target->Execute_TakeDamage(OtherActor, DamageData, someBool);
		// disable the ricochet effect
		IsRicochet = false;
	}
	
	// spawn the impact effect
	OnImpact(Hit);
}

void AProjectile::OnImpact(const FHitResult& Hit) 
{
	AActor* impactVFX = GetWorld()->SpawnActor<AActor>(ImpactEffect, GetActorLocation(), GetActorRotation());

	if (impactVFX && IsAOE)
	{
		// scale the impact particle to match the AOE radius
		impactVFX->SetActorRelativeScale3D(impactVFX->GetActorRelativeScale3D() * ImpactEffectScaleMultiplier * AreaOfEffectRadius / 100);
	}
	else if(!impactVFX)
	{
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, FString::Printf(TEXT("No impact generated.")));
		return;
	}
	
	if(IsRicochet)
	{
		OnRicochetImpact(Hit);
		return;
	}

	if(IsAOE)
	{
		OnAOEImpact();
	}

	Destroy();
}

void AProjectile::OnRicochetImpact(const FHitResult& Hit)
{
	if(DeflectionsCount <= 1)
	{
		IsRicochet = false;
	}

	DeflectionsCount--;

	FVector velocity = ProjectileMovementReference->Velocity;

	float length = velocity.Size();
	velocity.Normalize();
	velocity *= length / 2;
	FVector newVelocity = velocity.MirrorByVector(Hit.Normal);
	ProjectileMovementReference->Velocity = newVelocity * ProjectileSpeed;
}


void AProjectile::OnAOEImpact()
{
	FVector location = GetActorLocation();
    FCollisionShape sphere = FCollisionShape::MakeSphere(AreaOfEffectRadius);
	TArray<FOverlapResult> overlaps;

	FCollisionQueryParams queryParams;
	queryParams.AddIgnoredActor(this);

	bool bHasOverlap = GetWorld()->OverlapMultiByChannel(overlaps, location,FQuat::Identity,ECC_WorldDynamic, sphere, queryParams);

	if(bHasOverlap)
	{
		int count = 0;
		TArray<AActor*> actorsImpacted;
		
		for(FOverlapResult& overlap : overlaps)
		{
			AActor* overlappedActor = overlap.GetActor();

			// if the current actor has already been impacted by this projectile, then ignore it
			if(actorsImpacted.Contains(overlappedActor))
			{
				continue;
			}
			
			// if there is an actor and the actor has the Damage system
			if(overlappedActor && overlappedActor->GetClass()->ImplementsInterface(UIDamageSystemInterface::StaticClass()))
			{
				actorsImpacted.Add(overlappedActor);
				IIDamageSystemInterface* target = Cast<IIDamageSystemInterface>(overlappedActor);

				if(target)
				{
					count++;
					bool someBool = false;
					target->Execute_TakeDamage(overlappedActor, DamageData, someBool);
				}
			}
		}
		
		UBossLabUtils::Print(FString::Printf(TEXT("Targets count is %d"), count), 20.0f, FColor::Red);
	}
	
}


void AProjectile::SetProjectileSettings(UShootingAttackData* data, FRotator rotation)
{
	if(!ProjectileMovementReference)
	{
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, FString::Printf(TEXT("No projectile reference")));
		return;
	}
	
	DamageData = data->DamageInfo;
	ProjectileSpeed = data->ProjectileSpeed;

	ProjectileMovementReference->InitialSpeed = ProjectileSpeed;
	ProjectileMovementReference->MaxSpeed = ProjectileSpeed;
	ProjectileMovementReference->bInterpMovement = false;
	ProjectileMovementReference->bInterpMovement = true;
    ProjectileMovementReference->Velocity = rotation.Vector() * ProjectileSpeed;
}
