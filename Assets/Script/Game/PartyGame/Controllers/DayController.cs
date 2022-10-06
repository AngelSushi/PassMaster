using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayController : MonoBehaviour {

    public enum DayPeriod {
        DAY,
        DUSK,
        NIGHT,
        RAIN
    }

    private Light light;

    public Material dayMat;
    public Material duskMat;
    public Material nightMat;
    public Material rainMat;

    public DayPeriod dayPeriod;
    private DayPeriod lastDayPeriod;

    private AudioSource mainAudio;

    void Start() {
        light = GetComponent<Light>();
        mainAudio = AudioController.Instance.mainSource;
    }

    void Update() {

        switch(dayPeriod) {
            case DayPeriod.DAY: // Day
                RenderSettings.skybox = nightMat;
                light.intensity = 1.5f;
                mainAudio.clip = AudioController.Instance.mainAudioClip;
                if(!mainAudio.isPlaying) 
                    mainAudio.Play();
                break;

            case DayPeriod.DUSK: // Crepuscule
                RenderSettings.skybox = duskMat;
                light.intensity = 1f;
                mainAudio.clip = AudioController.Instance.duskAudioClip;
                if(!mainAudio.isPlaying)
                     mainAudio.Play();
                break;

            case DayPeriod.NIGHT: // Night
                light.intensity = 0.3f;
                RenderSettings.skybox = nightMat;
                mainAudio.clip = null;
                break;  

            case DayPeriod.RAIN: // Pluie
                RenderSettings.skybox = rainMat;     
                break;          
        }
    }


    public void ChangeNaturalDayPeriod(int difficulty,int nightIndex) {

        if(dayPeriod == DayPeriod.RAIN) {
            dayPeriod = lastDayPeriod; // Faire en sorte que s'il y a que 1 période de crépuscule et que c de la pluie ca va en nuit
            return;
        }

        switch(difficulty) {
            case 0: // Facile 2 jour ; 2 crépuscule ; 1 nuit
                if(nightIndex == 4 || nightIndex == 3)
                    dayPeriod = DayController.DayPeriod.DAY;
                if(nightIndex == 2 || nightIndex == 1)
                    dayPeriod = DayController.DayPeriod.DUSK;
                if(nightIndex == 0)
                    dayPeriod = DayController.DayPeriod.NIGHT;

                break;

            case 1: // Medium 2 jour ; 1 crépuscule ; 1 nuit
                if(nightIndex == 3 || nightIndex == 2)
                    dayPeriod = DayController.DayPeriod.DAY;
                if(nightIndex == 1)
                    dayPeriod = DayController.DayPeriod.DUSK;
                if(nightIndex == 0)
                    dayPeriod = DayController.DayPeriod.NIGHT;

                break;

            case 2: // Hard 1 jour ; 1 crépuscule ; 1 nuit
                if(nightIndex == 2)
                    dayPeriod = DayController.DayPeriod.DAY;
                if(nightIndex == 1)
                    dayPeriod = DayController.DayPeriod.DUSK;
                if(nightIndex == 0)
                    dayPeriod = DayController.DayPeriod.NIGHT;

                break;        
        }

        lastDayPeriod = dayPeriod;
    }

    public void ChangeManualDayPeriod(DayPeriod nextPeriod) {
        dayPeriod = nextPeriod;

        if(nextPeriod != DayPeriod.RAIN)
            lastDayPeriod = nextPeriod;
    }


    public void ChangeDayPeriodWithHourglass() {
        int actualPeriodIndex = GetIdFromDayPeriod(dayPeriod);

        if(actualPeriodIndex < 2) 
            dayPeriod = ConvertIdToDayPeriod(actualPeriodIndex + 1);
        else {
            if(actualPeriodIndex == 3)  // RAIN
                dayPeriod = lastDayPeriod;           
            else
                dayPeriod = ConvertIdToDayPeriod(0);
        }

        GameController.Instance.players[GameController.Instance.actualPlayer].GetComponent<UserUI>().CloseActionHUD(true);
        lastDayPeriod = dayPeriod;
    }

    private DayPeriod ConvertIdToDayPeriod(int id) {
        switch(id) {
            case 0:
                return DayPeriod.DAY;

            case 1:
                return DayPeriod.DUSK;

            case 2:
                return DayPeriod.NIGHT;

            case 3:
                return DayPeriod.RAIN;

            default:
                return DayPeriod.DAY;
        }
    }

    private int GetIdFromDayPeriod(DayPeriod period) {
        switch(period) {
            case DayPeriod.DAY:
                return 0;
            
            case DayPeriod.DUSK:
                return 1;

            case DayPeriod.NIGHT: 
                return 2;
                
            case DayPeriod.RAIN:
                return 2;
            default:
                return 0;
        }
    }
}
