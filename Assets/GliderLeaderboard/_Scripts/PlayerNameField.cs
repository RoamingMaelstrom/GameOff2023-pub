using UnityEngine;
using TMPro;

namespace GliderServices
{
    public class PlayerNameField : MonoBehaviour
    {
        [SerializeField] TMP_InputField inputField;
        [SerializeField] int nameLengthLimit = 20;

        private void Start() 
        {
            inputField.SetTextWithoutNotify(PlayerLocalInfo.PlayerName);
        }

        public void OnFieldChanged()
        {
            string newFieldValue = "";

            if (inputField.text.Length == 0) 
            {
                newFieldValue = PlayerLocalInfo.GenerateRandomNameLowerCase();
                inputField.SetTextWithoutNotify("");
                PlayerLocalInfo.PlayerName = newFieldValue;
            }
            else 
            {
                newFieldValue = CleanText(inputField.text, nameLengthLimit);
                PlayerLocalInfo.PlayerName = newFieldValue;
                inputField.SetTextWithoutNotify(PlayerLocalInfo.PlayerName);
            }
        }


        public static string CleanText(string inputName, int maxCharacterLength = 100)
        {
            string output = "";
            for (int i = 0; i < inputName.Length; i++)
            {   
                if (i > maxCharacterLength) break;

                switch ((int)inputName[i])
                {
                    case 32: output += "_"; break; // "_" 
                    case 45: output += inputName[i]; break;     // "-"
                    case < 48: break;
                    case < 58: output += inputName[i]; break;   // "0-9"
                    case < 65: break;
                    case < 91: output += inputName[i]; break;   // "A-Z"
                    case 95: output += inputName[i]; break;     // "_"
                    case < 97: break;
                    case < 123: output += inputName[i]; break;  // "a-z"
                    default: break;
                }
            }

            return output;
        }
    }
}

