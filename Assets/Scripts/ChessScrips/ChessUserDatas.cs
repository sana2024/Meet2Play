using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nakama;
using Nakama.TinyJson;
using System.Threading.Tasks;
using System.Linq;
using TMPro;
using ByteBrewSDK;

public class ChessUserDatas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ProfileLextText;
    [SerializeField] Text MenuLevelText;
    [SerializeField] TextMeshProUGUI PlayerDescText;
    [SerializeField] TextMeshProUGUI WinsText;
    [SerializeField] TextMeshProUGUI LoosesText;
    [SerializeField] TextMeshProUGUI DrawText;
    [SerializeField] TextMeshProUGUI CurrentXPText;
    [SerializeField] TextMeshProUGUI RequiredXPText;
    [SerializeField] TextMeshProUGUI ELOText;
    [SerializeField] Slider XPSlider;
    [SerializeField] Slider MenuLevelSlider;
    [SerializeField] GameObject StatisticPanel;

    public int ChessLevel = 1;
    public int ChessElo = 1000;
    public int RequiredXP = 83;
    public int CurrentXP = 0;
    public int ChessWins = 0;
    public int ChessLosses = 0;
    public int ChessDraw = 0;
    public int AIleveling = 1;
    public string PlayerDescription = "";
    public string PlayerFlag = "";
    public string ChessSkill = "Beginner";


    //User Description / About me
    [SerializeField] InputField AboutMeInput;
    [SerializeField] TextMeshProUGUI AboutMeText;
    [SerializeField] Image EditButtonImage;
    [SerializeField] Sprite EditSprite;
    [SerializeField] Sprite SubmitSprite;


    bool Edited = true;


   public IClient iclient;
   public ISession isession;
   public ISocket isocket;

    //to calculate XP amount based on level
    public float AdditionMultiplier = 300;
    public float PowerMutiplier = 2;
    public float DivisionMultipler = 7;




    bool LeveledUp = false;

    public static ChessUserDatas Instance;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        iclient = PassData.iClient;
        isession = PassData.isession;
        isocket = PassData.isocket;


        ReadChessData();
        StartCoroutine(WaitForRequiredXp());
    }




    IEnumerator WaitForRequiredXp()
    {
        yield return new WaitUntil(() => PassData.RequiredXP != 0);
        ReadXP();
    }

    // Update is called once per frame
    void Update()
    {
         if(LeveledUp == true)
        {
            LevelUp();
            LeveledUp = false;
        }
    }


    public async void ReadChessData()
    {
        var ChessProfile = await iclient.ReadStorageObjectsAsync(isession, new[] {
        new StorageObjectId {
        Collection = "ChessDatabase",
        Key = "Data",
        UserId = isession.UserId
  }
});



        if (ChessProfile.Objects.Any())
        {
            var ChessDatabase = ChessProfile.Objects.First();
            var datas = JsonParser.FromJson<ChessDataObj>(ChessDatabase.Value);

            //ADD IT TO THE CLASSES VARIABLES
            PlayerDescription = datas.description;
            PlayerFlag = datas.flag;
            ChessLevel = int.Parse(datas.ChessLevel);
            
            RequiredXP = CalculateRequiredXp();
           
            ChessWins = int.Parse(datas.chesswin);
            ChessLosses = int.Parse(datas.chessloses);
            ChessDraw = int.Parse(datas.chessDraw);
            ChessSkill = datas.ChessSkill;
            ChessElo = int.Parse(datas.ChessElo);
            AIleveling = datas.AILeveling;

            // DISPLAY UI OF THE DATAS

            AboutMeText.text = datas.description;
            ProfileLextText.text =  datas.ChessLevel;
            MenuLevelText.text = datas.ChessLevel;
            WinsText.text = datas.chesswin;
            LoosesText.text = datas.chessloses;
            DrawText.text = datas.chessDraw;
            RequiredXPText.text = CalculateRequiredXp().ToString();
            ELOText.text = datas.ChessElo;

            // SEND IT TO PASS DATA
            PassData.PlayerDesc = datas.description;
            PassData.ChessLevel = int.Parse(datas.ChessLevel);
            PassData.ChessWins = int.Parse(datas.chesswin);
            PassData.ChessLooses = int.Parse(datas.chessloses);
            PassData.ChessDraws = int.Parse(datas.chessDraw);
            PassData.RequiredXP = CalculateRequiredXp();
            PassData.ChessELO = int.Parse(datas.ChessElo);
            PassData.BotLeveling = datas.AILeveling;

            OnlineUsers.instance.joinAvaiables();
             
   

        }
        else
        {
            UserProfile.instance.AddXP(0);
            UserProfile.instance.updateWallet(0);
            WriteData(); 
        }
    }

    // ----------------------------- XP SECTION ----------------------------- 
    public async void ReadXP()
    {
        var account = await iclient.GetAccountAsync(isession);
        var wallet = JsonParser.FromJson<Dictionary<string, int>>(account.Wallet);
 
        CurrentXP = wallet.Values.First();
        CurrentXPText.text = wallet.Values.First().ToString();
        PassData.CurrentXP = wallet.Values.First();

        updateXPUi();


    }

    public async Task UpdateXPAsync(int xp)
    {
        CurrentXP += xp;
        var payload = JsonWriter.ToJson(new { xp = xp });
        var rpcid = "Update_XP";
        var WalletRPC = await iclient.RpcAsync(isession, rpcid, payload);
        if (CurrentXP >= PassData.RequiredXP)
        {
            LeveledUp = true;
        }

        PassData.CurrentXP = CurrentXP;
        ReadXP();
        updateXPUi();
 
    }

  
    private int CalculateRequiredXp()
    {

        var Required = 0;
        for (int i = 1; i <= ChessLevel; i++)
        {
            Required += (int)Mathf.Floor(i + AdditionMultiplier * Mathf.Pow(PowerMutiplier, i / DivisionMultipler));

        }
        return Required / 4;
    }


    public void updateXPUi()
    {
      
 
        var fillAmount = ((CurrentXP * 100) / PassData.RequiredXP) * 0.01f;
        XPSlider.value = fillAmount;
        MenuLevelSlider.value = fillAmount;
    }




    // -------------------------- LEVEL SESION -----------------------------
 


    public void LevelUp()
    {
        ChessLevel++;
       // UpdateXP(-PassData.RequiredXP);
        UserProfile.instance.AddXP(-PassData.RequiredXP);
        UserProfile.instance.updateWallet(0);
        PassData.RequiredXP = CalculateRequiredXp();
 
        RequiredXPText.text = RequiredXP.ToString();
        ProfileLextText.text = ChessLevel.ToString();
        MenuLevelText.text = ChessLevel.ToString();
        WriteData();
 
    }



    public async void WriteData()
    {
        var Datas = new ChessDataObj
        {
            description = PlayerDescription,
            flag = PlayerFlag,
            ChessLevel = ChessLevel.ToString(),
            ChessElo = ChessElo.ToString(),
            ChessSkill = ChessSkill,
            chesswin = ChessWins.ToString(),
            chessloses = ChessLosses.ToString(),
            chessDraw = ChessDraw.ToString(),
            AILeveling = AIleveling
             
 
        };


        PassData.PlayerDesc = PlayerDescription;
        PassData.ChessLevel = ChessLevel;
        PassData.RequiredXP = CalculateRequiredXp();
 
        PassData.ChessWins = ChessWins;
        PassData.ChessLooses = ChessLosses;
        PassData.ChessDraws = ChessDraw;
        PassData.ChessELO = ChessElo;
        PassData.BotLeveling = AIleveling;
        


        AboutMeText.text = PlayerDescription;
        ProfileLextText.text = ChessLevel.ToString();
        MenuLevelText.text = ChessLevel.ToString();
        RequiredXPText.text = CalculateRequiredXp().ToString();
 
        WinsText.text = ChessWins.ToString();
        LoosesText.text = ChessLosses.ToString();
        DrawText.text = ChessDraw.ToString();
        ELOText.text = ChessElo.ToString();


        var Sendata = await iclient.WriteStorageObjectsAsync(isession, new[] {
        new WriteStorageObject
       {
      Collection = "ChessDatabase",
      Key = "Data",
      Value = JsonWriter.ToJson(Datas),
      Version = PassData.version,
      PermissionRead=2,

       }
});

    }

    public void EditClicked()
    {
        Edited = !Edited;

        if(Edited == false)
        {
            EditButtonImage.sprite = SubmitSprite;
            AboutMeInput.text = AboutMeText.text;
            AboutMeText.gameObject.SetActive(false);
            AboutMeInput.gameObject.SetActive(true);
 
        }
        if (Edited == true)
        {
            EditButtonImage.sprite = EditSprite;
            AboutMeText.text = AboutMeInput.text;

            AboutMeText.gameObject.SetActive(true);
            AboutMeInput.gameObject.SetActive(false);
            PlayerDescription = AboutMeText.text;
            WriteData();
        }
    }


    public void OpenStatisticPanel()
    {
        StatisticPanel.SetActive(true);
        ByteBrew.NewCustomEvent("OpenChessProgress", "Username=" + isession.Username + ";");
    }

    public void CloseStatisticPanel()
    {
        StatisticPanel.SetActive(false);
    }



 
}
