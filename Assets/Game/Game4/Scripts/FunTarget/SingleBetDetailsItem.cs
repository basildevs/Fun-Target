using TMPro;
using UnityEngine;

namespace FunTarget
{
    public class SingleBetDetailsItem : MonoBehaviour
    {
        public TMP_Text betText;
        public TMP_Text positionsText;
        public TMP_Text amountText;

        public void SetDetails(string bet, string positions, string amount)
        {
            betText.text = bet;
            positionsText.text = positions;
            amountText.text = amount;
        }
    }
}

