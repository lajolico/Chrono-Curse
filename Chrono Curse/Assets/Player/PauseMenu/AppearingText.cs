using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class AppearingText : MonoBehaviour
{
    [SerializeField]
    public string onScreenText;
    public float textSpeed = 0.07f;
	public TextMeshProUGUI text;

	void OnEnable () 
    {
		StartCoroutine(RevealText());
	}
	
    void OnDisable()
    {
        StopCoroutine(RevealText());
    }

	IEnumerator RevealText() 
    {
		var originalString = onScreenText;
		text.text = "";
		var numCharsRevealed = 0;

		while (numCharsRevealed < originalString.Length)
		{

			while (originalString[numCharsRevealed] == ' ')
            {
				++numCharsRevealed;
            }

			++numCharsRevealed;
			text.text = originalString.Substring(0, numCharsRevealed);
			yield return new WaitForSeconds(textSpeed);
		}
	}
}
