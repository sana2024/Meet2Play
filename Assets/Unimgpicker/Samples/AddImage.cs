using Kakera;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using ByteBrewSDK;

// WebGLで使うときはIPointerDownHandlerを継承する必要がある点に注意
public class AddImage : MonoBehaviour
{
    [SerializeField] private Unimgpicker imagePicker;
    [SerializeField] private RawImage MenuAvatar;
    [SerializeField] private RawImage ProfileAvatar;
    [SerializeField] private RawImage MatchAvatar;
    [SerializeField] private RawImage ChallangeAvatar;
    public Texture2D texture;
    public Sprite texture2;
    string fileName;

    private void Start()
    {
        fileName = PassData.Username + DateTime.Now.Ticks;

    }

    IEnumerator Upload(byte[] imagebyte, string path)
    {
        WWWForm form = new WWWForm();

        //Add a file to the form
        form.AddBinaryData("singlefile/", imagebyte, path);

        //Add a field to the form
        // form.AddField("media_name", name);

        UnityWebRequest www = UnityWebRequest.Post("http://NakamaAppALB-1422208787.eu-west-1.elb.amazonaws.com:8000/api/image-upload/", form);
        www.chunkedTransfer = false;



        yield return www.SendWebRequest();
    }
 

    public void PickImage(int maxSize)
    {
        int size = 0;
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                var texture = NativeGallery.LoadImageAtPath(path, maxSize, false, false);
             
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    
                }
                else
                {

                    CropTexture(texture, path);



                }



            }
        });

    }

   void CropTexture(Texture2D originalTexture, String path)
    {
        int size = Mathf.Min(originalTexture.width, originalTexture.height);
        int offsetX = (originalTexture.width - size) / 2;
        int offsetY = (originalTexture.height - size) / 2;

        Color[] pixels = originalTexture.GetPixels(offsetX, offsetY, size, size);

        Texture2D squareTexture = new Texture2D(size, size);
        squareTexture.SetPixels(pixels);
        squareTexture.Apply();

        StartCoroutine(Upload(ScaleDownTexture(squareTexture, 100, 100).EncodeToPNG(), fileName + Path.GetExtension(path)));

        PassData.EditAvatar = Path.GetFileName(path);


        var imagePath = "https://d23dmf2f0k5c5k.cloudfront.net/singlefile/" + fileName + Path.GetExtension(path);


        UserProfile.instance.client.UpdateAccountAsync(UserProfile.instance.session, PassData.Username, PassData.Username, imagePath, null, null);


        MenuAvatar.texture = squareTexture;
        ProfileAvatar.texture = squareTexture;
        MatchAvatar.texture = squareTexture;

        ByteBrew.NewCustomEvent("ChangedAvatar", "Username=" + PassData.isession.Username + ";");


    }

    public Texture2D ScaleDownTexture(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Point;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D nTex = new Texture2D(newWidth, newHeight, TextureFormat.RGB24, false);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        nTex.Apply();
        MenuAvatar.texture = nTex;
        ProfileAvatar.texture = nTex;
        MatchAvatar.texture = nTex;
        ChallangeAvatar.texture = nTex;
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);



        return nTex;
    }
}