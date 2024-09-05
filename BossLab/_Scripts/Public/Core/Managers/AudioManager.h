// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Subsystems/GameInstanceSubsystem.h"
#include "AudioManager.generated.h"

/**
 * 
 */
UCLASS()
class BOSSLAB_API UAudioManager : public UGameInstanceSubsystem
{
	GENERATED_BODY()

public:
	virtual void Deinitialize() override;
	
	UFUNCTION(BlueprintCallable, Category="Audio Manager")
	void InitData(USoundClass* Master, USoundClass* BGM, USoundClass* SFX);
	
	UFUNCTION(BlueprintCallable, Category="Audio Manager")
	void PlayBGM(USoundBase* Sound, float FadeInDuration = 0.4);
	UFUNCTION(BlueprintCallable, Category="Audio Manager")
	void PlaySFX(USoundBase* Sound);

	UFUNCTION(BlueprintCallable, Category="Audio Manager")
	void SetMasterVolume(float Volume);
	
	UFUNCTION(BlueprintCallable, Category="Audio Manager")
	void SetBGMVolume(float Volume);

	UFUNCTION(BlueprintCallable, Category="Audio Manager")
	void SetSFXVolume(float Volume);
	
	UFUNCTION(BlueprintCallable, Category="Audio Manager")
	void InitAudioForLevel(USoundBase* Sound);

private:
	USoundClass* _MasterChannel;
	USoundClass* _BGMChannel;
	USoundClass* _SFXChannel;
	UAudioComponent* _currentBGM;
};
