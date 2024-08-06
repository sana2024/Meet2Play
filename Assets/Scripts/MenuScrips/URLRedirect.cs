using System.Collections;
using System.Collections.Generic;
using ByteBrewSDK;
using UnityEngine;

public class URLRedirect : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] string Url;


    public void support()
    {

        Application.OpenURL(Url);
        ByteBrew.NewCustomEvent("OpenSupport", "Username=" + PassData.isession.Username + ";");
    }


    public void discord()
    {
        Application.OpenURL(PassData.DiscordUrl);
        ByteBrew.NewCustomEvent("OpenDiscord", "Username=" + PassData.isession.Username + ";");
    }


}
