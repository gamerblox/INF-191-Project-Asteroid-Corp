using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alert : MonoBehaviour
{

    public GameObject manager;
    public GameObject alertimage;
    
    public void RemoveAlert()
    {

        if (this.GetComponentInChildren<Text>().text == "Alert 1\nJuly 27, 2017")
        {
            //manager.GetComponent<AlertsManager>().DisplayAlert("Today we have just discovered a new asteroid.\nIt appear to be a big one and worse than that,\nwe think it has a high probability of hitting Earth.\nOur initial calculations have it at a 1 in 62 chance\nof hitting Earth. The impact of such an asteroid\nwould decimate metropolises and cause unknown amounts\nof chaos. Right now we have it at a rating of 4 on\nthe Torino impact hazard scale which is the highest\nrating for an asteroid ever. New information should\nbe coming soon so we'll update you as soon as we get it.\nAs of now, we have sent you all we know about the\nasteroid which you can view on your main screen.\nWhenever we discover anything new, we will update your\ndatabases.");
            manager.GetComponent<AlertsManager>().DisplayAlert("July 27, 2017:\n\nToday we have just discovered a new asteroid. It appear to be a big one and worse than that, we think it has a high probability of hitting Earth. Our initial calculations have it at a 1 in 62 chance of hitting Earth. The impact of such an asteroid would decimate metropolises and cause unknown amounts of chaos. Right now we have it at a rating of 4 on the Torino impact hazard scale which is the highest rating for an asteroid ever. New information should be coming soon so we'll update you as soon as we get it. As of now, we have sent you all we know about the asteroid which you can view on your main screen. Whenever we discover anything new, we will update your databases.");
        }
        else if (this.GetComponentInChildren<Text>().text == "Alert 2\nDecember 4, 2017")
        {
            manager.GetComponent<AlertsManager>().DisplayAlert("December 4, 2017:\n\nIt has been a hectic holiday season here at the observatory. We have give the asteroid a name for now: 2004 MN4. Our observations based on its brightness have it at about 450 meters in diameter. Our initial impact possibility calculations were unfortunately incorrect. 2004 MN4 appears to have a 2.7% chance of hitting Earth in April 2027. There also seems to be a possibility that it hits in 2053 if it misses in 2027.\n\nWe've been discussing what our threshold for action against these near-Earth objects should be. There is a concensus that there has to be around a 10% chance of impact for any action to take place, but we are leaving the decision up to your department. Good luck.");
        }
        else if (this.GetComponentInChildren<Text>().text == "Alert 3\nApril 7, 2018")
        {
            manager.GetComponent<AlertsManager>().DisplayAlert("April 7, 2018:\n\nWell, we have sufficiently mapped out 2004 MN4's orbit so we have given it a permenant name: 99942 Apophis. Apophis is the god of chaos is Egyption mythology known as the Uncreator. An unfortunately befitting name.\n\nAnyways, we have some new information about Apophis from our telescope in Hawaii. It is measured to be 350 meters in diameter. If Apophis passes Earth in 2027, it will still be a very close approach passing only 5.7 Earth radii from Earth. Even if it does pass in 2027, we have discovered that there is a possibility of an impact in April 2036.");
        }
        else if (this.GetComponentInChildren<Text>().text == "Alert 4\nNovember 29, 2018")
        {
            manager.GetComponent<AlertsManager>().DisplayAlert("November 29, 2018:\n\nToday we have some good news and some bad news. We have downgraded the possibility of the April 2027 approach. However, we have upgraded the possibility of a 2036 impact to 3.2%. This gives you guys working on asteroid deflection more time at least. Also, the diameter of Apophis has been estimated to be 270 meters which is quite smaller than we previously thought.");
        }
        else if (this.GetComponentInChildren<Text>().text == "Alert 5\nJune 7, 2019")
        {
            manager.GetComponent<AlertsManager>().DisplayAlert("June 7, 2019:\n\nIt's not looking good. The Germans have discovered that Apophis is going to collide with a geosynchronous satellite during its flyby on April 13, 2027 which is going to heavily increase its possibility of hitting Earth in 2036. We are trying to get an official estimate but with the media frenzy, it is taking some time to ask the officials at NASA.");
        }
        else if (this.GetComponentInChildren<Text>().text == "Alert 6\nJanuary 2, 2020")
        {
            manager.GetComponent<AlertsManager>().DisplayAlert("January 2, 2020:\n\nI believe that it was Malcom X that said something about the media being so powerful because they could make the guilty innocent and the innocent guilty. Well, apparently they can make the destruction of Earth imminent when it's not. That German newspaper claimed that NASA and ESA confirmed the report about Apophis hitting a satellite, increasing its chance of hitting in 2036 but that is totally false. NASA had to come out and explain that the angle of Apophis's 2027 close approach makes it so that there is not rise of colliding with any satellite. Sorry for the scare.");
        }
        else if (this.GetComponentInChildren<Text>().text == "Alert 7\nJuly 23, 2020")
        {
            manager.GetComponent<AlertsManager>().DisplayAlert("July 23, 2020:\n\nThe ESA has published new numbers about Apophis regarding its size and mass. They are saying that the diameter is closer to 325 meters which is around a 20% increase from our previous calculations. This translates to a 75% increase in the asteroids mass and volume.");
        }
        else if (this.GetComponentInChildren<Text>().text == "Alert 8\nMarch 14, 2021")
        {
            manager.GetComponent<AlertsManager>().DisplayAlert("March 14, 2021:\n\nNew observations have just come in and they are not good. The April 13th, 2027 impact possibility has significantly increased. It looks like the possibility is only at 1.7% at the moment but it keeps rising with everything we find. We can only hope that this is a coincidence...");
        }
        else if (this.GetComponentInChildren<Text>().text == "Alert 9\nOctober 13, 2022")
        {
            manager.GetComponent<AlertsManager>().DisplayAlert("October 13, 2022:\n\nThe office is in chaos. New observations have just come in from NASA with their new telescope. The 2027 impact event probability has just been raised to 4.6%. This is the worse it has ever been. Loads of calculations are being done right now on the new information to get the most accurate report possible so we'll give that to you as soon as we can.");
        }
        else if (this.GetComponentInChildren<Text>().text == "Alert 10\nMay 9, 2023")
        {
            manager.GetComponent<AlertsManager>().DisplayAlert("May 9, 2023:\n\n10.1%. My hands are shaking as I write this. The probability of Apophis hitting the Earth in 2027 has just passed our theoretical threshold for action. The choice is yours. The government won't be happy that you build an expensive rocket for delflection for only a 10.1% chance of impact but they do not understand how significant this number is and the destruction that could come from Apophis. We'll continue giving you information as we recieve it.");
        }
        else if (this.GetComponentInChildren<Text>().text == "Alert 11\nDecember 11, 2024")
        {
            manager.GetComponent<AlertsManager>().DisplayAlert("December 11, 2024:\n\nBased on new tracking measurements taken this week, the probability of Apophis hitting Earth has increased to 96% making the collision nearly certain. The predicted line of risk of impact goes through Kazakhstan, across the north Pacific and Central America to the Cape Verde Islands in the Atlantic. We'll keep you posted as we get more information.");
        }
        else if (this.GetComponentInChildren<Text>().text == "Alert 12\nFebruary 16, 2025")
        {
            manager.GetComponent<AlertsManager>().DisplayAlert("February 16, 2025:\n\nWe were able to get some good satellite images of Apophis this past month. From those images, we have determined that the most likely area of impact is along a 1700km long line starting in the Pacific Ocean and passing through Costa Rica, Panama, Colombia, and Venezuela, finally ending in the Atlantic Ocean. The best case is that Apophis lands in one of the Oceans which would still cause tsunamis, however we would likely be able to send help and evacuate enough people to keep the casualties low.");
        }
        else if (this.GetComponentInChildren<Text>().text == "Alert 13\nAugust 1, 2026")
        {
            manager.GetComponent<AlertsManager>().DisplayAlert("August 1, 2026:\n\nBetter images have given us a more exact location of where Apophis is targeted to hit. It will hit the Venezuelan capital of Caracas or the surrounding area. Over 2 million people live in the urban area of Caracas and almost 5 million live in the metropolitan area. The political situation is a bit sticky there as well. The United States and European Union are not on good terms with Venezuela so help from them may be difficult to come by. It would be in the best interest of everyone if this asteroid is completely deflected.");
        }
        //Destroy(this.gameObject);

    }

}
