// Fill out your copyright notice in the Description page of Project Settings.


#include "Core/Boss/BaseBoss.h"

// Sets default values
ABaseBoss::ABaseBoss()
{
 	// Set this character to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

}

// Called when the game starts or when spawned
void ABaseBoss::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void ABaseBoss::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

// Called to bind functionality to input
void ABaseBoss::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

}

