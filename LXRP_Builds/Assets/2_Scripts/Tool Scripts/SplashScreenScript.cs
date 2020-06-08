using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// This example shows how you could draw the splash screen at the start of a Scene. This is a good way to integrate the splash screen with your own or add extras such as Audio.
public class SplashScreenScript : MonoBehaviour
{
    IEnumerator Start()
    {
        Debug.Log("Showing splash screen");
       // SplashScreen.Begin();
        while (!SplashScreen.isFinished)
        {
           // SplashScreen.Draw();
            yield return null;
        }
        Debug.Log("Finished showing splash screen");
    }
}