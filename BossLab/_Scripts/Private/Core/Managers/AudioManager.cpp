// Fill out your copyright notice in the Description page of Project Settings.


#include "Core/Managers/AudioManager.h"

#include "AudioDevice.h"
#include "Kismet/GameplayStatics.h"
#include "Sound/SoundClass.h"

void UAudioManager::Deinitialize()
{
	GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, TEXT("De Init AM"));
}

void UAudioManager::InitData(USoundClass* Master, USoundClass* BGM, USoundClass* SFX)
{
	if(Master)
	{
		_MasterChannel = Master;
		_MasterChannel->Properties.Volume = 1;
	}
	
	if(BGM)
	{
		_BGMChannel = BGM;
		_BGMChannel->Properties.Volume = 1;
	}
	
	if(SFX)
	{
		_SFXChannel = SFX;
		_SFXChannel->Properties.Volume = 1;
	}
	
	GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, TEXT("Data Set!"));
}

void UAudioManager::SetMasterVolume(float Volume)
{
	if(_MasterChannel)
	{
		_MasterChannel->Properties.Volume = Volume;
	}
	// TODO: Add a warning log for all the volume setting functions, when channel object not assigned.
}

void UAudioManager::SetBGMVolume(float Volume)
{
	if(_BGMChannel)
	{
		_BGMChannel->Properties.Volume = Volume;
	}
}

void UAudioManager::SetSFXVolume(float Volume)
{
	if(_SFXChannel)
	{
		_SFXChannel->Properties.Volume = Volume;
	}
}

void UAudioManager::PlayBGM(USoundBase* Sound, float FadeInDuration)
{
	UWorld* World = GetWorld();
	
	if(!_BGMChannel || World == nullptr)
	{
		GEngine->AddOnScreenDebugMessage(-1, 5.0f, FColor::Red, TEXT("BGM C not SET!"));
		return;
	}
	
	 if(_currentBGM != nullptr )
	 {
	 	_currentBGM->SetSound(Sound);
	 }
	 else
	 {
	 	_currentBGM = UGameplayStatics::SpawnSound2D(World, Sound);
	 }
}

void UAudioManager::PlaySFX(USoundBase* Sound)
{
	UGameplayStatics::PlaySound2D(GetWorld(), Sound);
}

void UAudioManager::InitAudioForLevel(USoundBase* Sound)
{
	_currentBGM = UGameplayStatics::SpawnSound2D(GetWorld(), Sound);
}


