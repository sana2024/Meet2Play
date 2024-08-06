using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class piechar : MonoBehaviour
{
	public Image[] image;
	float[] values = { 0, 0, 0 };

	public Text LooseText;
	public Text DrawText;
	public Text WinText;
	public Text totalvaluesText;
	float totalvalues;
	// Start is called before the first frame update
	void Start()
	{

		Invoke("GetDatas", 4);

		// Update is called once per frame
	}

	public void SetValues(float[] valuestoset)
	{
		float totalValues = 0;
		for (int i = 0; i < image.Length; i++)
		{
			totalValues += FindPercentage(valuestoset, i);

			image[i].fillAmount = totalValues;
		}
	}

	private float FindPercentage(float[] valuestoset, int index)
	{
		float totalAmount = 0;
		for (int i = 0; i < valuestoset.Length; i++)
		{
			totalAmount += valuestoset[i];
		}
		return valuestoset[index] / totalAmount;
	}


	public void GetDatas()
    {
		 
			totalvalues = PassData.ChessLooses + PassData.ChessDraws + PassData.ChessWins;
		if (totalvalues != 0)
		{
 
			values[0] = PassData.ChessDraws;
			values[1] = PassData.ChessLooses;
			values[2] = PassData.ChessWins;

			SetValues(values);
			totalvaluesText.text = "Total Game: " + totalvalues.ToString();
			LooseText.text = "Losses: " + " " + PassData.ChessLooses + "   %" + Mathf.FloorToInt(PassData.ChessLooses / totalvalues * 100);
			DrawText.text = "Draws: " + " " + PassData.ChessDraws + "   %" + Mathf.FloorToInt(PassData.ChessDraws / totalvalues * 100);
			WinText.text = "Wins: " + " " + PassData.ChessWins + "   %" + Mathf.FloorToInt(PassData.ChessWins / totalvalues * 100);

		
		}

	}
}
