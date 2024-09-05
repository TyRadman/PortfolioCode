// Fill out your copyright notice in the Description page of Project Settings.


#include "ShootingAttackData.h"

void UShootingAttackData::ResetValues()
{
	Duration = 0.0f;
	ShotsNumber = 0;
	ProjectileSpeed = 0.0f;
	Angle = 0.0f;
	SplitsCount = 0;
	HorizontalDistance = 0.0f;
	ParallelCount = 0;
	IsAreaOfEffect = false;
	AreaOfEffectRadius = 0.0f;
	IsHoming = false;
	Accuracy = 0.0f;
	IsRicochet = false;
	DeflectionsCount = 0;
}
